using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 在 Unity 主线程协调本地 Camera 输入、实时连接、生成和清理的 SDK 门面。
    /// </summary>
    /// <remarks>
    /// 应在 Unity 主线程创建管理器并调用其 API。异步操作使用 Unity 同步上下文恢复，清理期间不接纳新操作。
    /// </remarks>
    public sealed class XmaxRealtimeManager : IXmaxRealtimeManager
    {
        // 配置与操作协调。
        private readonly XmaxConfiguration _configuration;
        private readonly RealtimeCoordinator _coordinator = new RealtimeCoordinator();

        // 连接、生成与媒体依赖。
        private readonly IStreamController _stream;
        private readonly XmaxRealtimeConnectionManager _connection;
        private readonly XmaxRealtimeGenerationManager _generation;
        private readonly MediaController _media;
        private readonly InteractionController _interaction;
        private readonly RenderController _render;

        // 启动诊断。
        private readonly RealtimeTiming _timing;

        // 当前编码状态。
        private RealtimeVideoFormat _videoFormat;

        // 需要持续观察的后台错误清理。
        private Task _failureCleanup = Task.CompletedTask;

        /// <summary>
        /// 当前管理器使用的模型配置。
        /// </summary>
        public RealtimeConfiguration Options { get; }

        /// <summary>
        /// 最近提交的连接和生成状态快照。
        /// </summary>
        public RealtimeState CurrentState { get; private set; } = new RealtimeState(RealtimeConnectionState.Idle);

        /// <summary>
        /// 当前管理器有效的本地流；尚未创建或已关闭时为 null。
        /// </summary>
        public RealtimeMediaStream LocalStream => _media.LocalStream;

        /// <summary>
        /// 连接或生成状态变化时触发；订阅者异常不会中断 SDK 流程。
        /// </summary>
        public event Action<RealtimeState> StateChanged;

        /// <summary>
        /// 当前生成收到有效远端帧时触发；跨回调保留帧需先 Clone。
        /// </summary>
        public event Action<XmaxVideoFrame> RemoteFrameReceived
        {
            add
            {
                _render.FrameReceived += value;
            }
            remove
            {
                _render.FrameReceived -= value;
            }
        }

        /// <summary>
        /// 异步致命错误完成清理后触发；被 await 的操作失败直接通过异常返回。
        /// </summary>
        public event Action<XmaxException> ErrorOccurred;

        /// <summary>
        /// 服务端会话关闭失败时触发的可恢复警告，本地连接资源仍已清理。
        /// </summary>
        public event Action<XmaxException> CleanupWarning;

        /// <summary>
        /// 有效连接中的本地上下行网络质量更新。
        /// </summary>
        public event Action<RealtimeNetworkQuality> NetworkQualityChanged;

        /// <summary>
        /// 创建实时管理器并组装其依赖；构造本身不建立在线会话。
        /// </summary>
        /// <param name="apiKey">在线请求使用的 API key；仅本地预览时可为空。</param>
        /// <param name="model">SDK 内置实时模型标识。</param>
        /// <param name="baseUrl">HTTP API 根地址，在线请求时校验为绝对 HTTP 或 HTTPS URL。</param>
        /// <param name="loggerOptions">全局日志过滤选项，默认关闭，由创建客户端时应用。</param>
        public XmaxRealtimeManager(
            string apiKey,
            RealtimeModel model = RealtimeModel.X2_0,
            string baseUrl = XmaxConfiguration.DefaultBaseUrl,
            XmaxLoggerOption loggerOptions = XmaxLoggerOption.None)
            : this(
                new XmaxClient(new XmaxConfiguration(apiKey, baseUrl, loggerOptions)),
                new RealtimeConfiguration(Models.Realtime(model)))
        {
        }

        /// <summary>
        /// 创建实时管理器并组装其依赖；构造本身不建立在线会话。
        /// </summary>
        /// <param name="client">用于集中应用全局配置的客户端。</param>
        /// <param name="options">当前管理器使用的实时模型配置。</param>
        private XmaxRealtimeManager(XmaxClient client, RealtimeConfiguration options)
            : this(client.Configuration, options)
        {
        }

        /// <summary>
        /// 创建实时管理器并组装其依赖；构造本身不建立在线会话。
        /// </summary>
        /// <param name="configuration">客户端的不可变服务地址、凭证和日志配置。</param>
        /// <param name="options">当前管理器使用的实时模型配置。</param>
        internal XmaxRealtimeManager(XmaxConfiguration configuration, RealtimeConfiguration options)
            : this(configuration, options, new RealtimeSessionService(new ApiService(configuration)),
                null)
        {
        }

        /// <summary>
        /// 创建实时管理器并组装其依赖；构造本身不建立在线会话。
        /// </summary>
        /// <param name="configuration">客户端的不可变服务地址、凭证和日志配置。</param>
        /// <param name="options">当前管理器使用的实时模型配置。</param>
        /// <param name="sessions">实时会话创建、续期和关闭服务。</param>
        /// <param name="stream">房间信令和视频传输入口；为 null 时创建默认 RTC 流控制器。</param>
        /// <param name="timing">共享启动计时器；为 null 时创建使用单调时钟的计时器。</param>
        internal XmaxRealtimeManager(
            XmaxConfiguration configuration,
            RealtimeConfiguration options,
            IRealtimeSessionService sessions,
            IStreamController stream,
            RealtimeTiming timing = null)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Options = options ?? throw new ArgumentNullException(nameof(options));

            _timing = timing ?? new RealtimeTiming();
            _stream = stream = stream ?? new StreamController(new RtcManager(), timing: _timing);
            var render = _render = new RenderController();
            _connection = new XmaxRealtimeConnectionManager(sessions, stream, render, _timing);
            _generation = new XmaxRealtimeGenerationManager(stream, render);
            _media = new MediaController(_coordinator.RequireThread, PublishLocalFrame);
            _interaction = new InteractionController(stream);

            stream.FrameReceived += render.Receive;
            stream.FatalError += HandleFatalError;
            stream.NetworkQualityChanged += quality =>
            {
                if (!_coordinator.IsTerminating && _connection.ActiveSession != null)
                    EventDispatch.Raise(NetworkQualityChanged, quality);
            };
            _connection.FatalError += HandleFatalError;
            _connection.CleanupError += error => EventDispatch.Raise(CleanupWarning, error);
        }

        /// <summary>
        /// 创建或替换本地 Camera 输入流，可在未连接时预览；连接或异步操作期间不可替换。
        /// </summary>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <returns>属于此管理器的新本地流，旧本地流随即失效。</returns>
        /// <exception cref="XmaxException">编码格式无效，或当前线程、连接、操作状态不允许替换本地流。</exception>
        public RealtimeMediaStream CreateLocalExternalStream(RealtimeVideoFormat format)
        {
            _coordinator.RequireAvailable();
            if (_coordinator.HasOperation || _connection.ActiveSession != null)
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Disconnect before replacing the local source.");

            return _media.CreateExternalStream(format);
        }

        /// <summary>
        /// 创建服务端会话并加入 RTC 房间；失败或取消时回滚连接资源。
        /// </summary>
        /// <param name="localStream">由当前管理器创建且仍有效的本地外部视频流。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>可接收生成视频的远端流，此时尚未启动生成。</returns>
        /// <exception cref="XmaxException">配置、调用状态或平台无效，或会话与 RTC 连接失败。</exception>
        /// <exception cref="OperationCanceledException">调用方取消或清理操作撤销了本次连接。</exception>
        public Task<RealtimeMediaStream> ConnectAsync(
            RealtimeMediaStream localStream,
            CancellationToken cancellationToken = default)
        {
            _coordinator.RequireAvailable();

            return ConnectCoreAsync(_media.RequireOwned(localStream), false, cancellationToken);
        }

        /// <summary>
        /// 创建服务端会话并加入 RTC 房间；失败或取消时回滚连接资源。
        /// </summary>
        /// <param name="videoFormat">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>可接收生成视频的远端流，此时尚未启动生成。</returns>
        /// <remarks>
        /// 此重载会创建或替换管理器的本地流；已有连接时不能重复连接。
        /// </remarks>
        /// <exception cref="XmaxException">配置、调用状态或平台无效，或会话与 RTC 连接失败。</exception>
        /// <exception cref="OperationCanceledException">调用方取消或清理操作撤销了本次连接。</exception>
        public Task<RealtimeMediaStream> ConnectAsync(
            RealtimeVideoFormat videoFormat,
            CancellationToken cancellationToken = default)
            => ConnectCoreAsync(videoFormat, true, cancellationToken);

        /// <summary>
        /// 创建服务端会话并加入 RTC 房间；失败或取消时回滚连接资源。
        /// </summary>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <param name="fps">目标编码帧率，必须大于零。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>可接收生成视频的远端流，此时尚未启动生成。</returns>
        /// <remarks>
        /// 此重载会创建或替换管理器的本地流；已有连接时不能重复连接。
        /// </remarks>
        /// <exception cref="XmaxException">配置、调用状态或平台无效，或会话与 RTC 连接失败。</exception>
        /// <exception cref="OperationCanceledException">调用方取消或清理操作撤销了本次连接。</exception>
        public Task<RealtimeMediaStream> ConnectAsync(
            int width,
            int height,
            int fps,
            CancellationToken cancellationToken = default)
            => ConnectAsync(new RealtimeVideoFormat(width, height, fps), cancellationToken);

        /// <summary>
        /// 校验配置并获取独立连接租约，串行执行连接。
        /// </summary>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="createSource">是否为此次连接创建或替换本地外部视频流。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>连接完成后的远端媒体流。</returns>
        private async Task<RealtimeMediaStream> ConnectCoreAsync(
            RealtimeVideoFormat format,
            bool createSource,
            CancellationToken cancellationToken)
        {
            _coordinator.RequireAvailable();
            if (_connection.ActiveSession != null)
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Realtime connection is already open.");

            format.Validate();
            _configuration.Validate();

            using (var operation = _coordinator.Begin(RealtimeOperation.Connection, cancellationToken))
            {
                return await PerformConnectAsync(format, createSource, operation);
            }
        }

        /// <summary>
        /// 在现有租约中连接并发布状态，失败或取消时回滚连接。
        /// </summary>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="createSource">是否为此次连接创建或替换本地外部视频流。</param>
        /// <param name="operation">调用方持有的操作租约，用于跨等待和回调检查取消。</param>
        /// <returns>本次连接创建的远端媒体流。</returns>
        private async Task<RealtimeMediaStream> PerformConnectAsync(
            RealtimeVideoFormat format,
            bool createSource,
            RealtimeCoordinator.Operation operation)
        {
            try
            {
                _configuration.Validate();
                _stream.ValidatePlatform();
                if (createSource)
                    _media.CreateExternalStream(format);

                _videoFormat = format;
                _generation.ResetContext();

                _timing.Mark("connection-start");
                EmitState(new RealtimeState(RealtimeConnectionState.Connecting));
                operation.EnsureCurrent();

                var remote = await _connection.ConnectAsync(Options.Model.Name, format, operation.Token);
                operation.EnsureCurrent();

                _timing.Mark("connection-end");
                EmitState(new RealtimeState(RealtimeConnectionState.Connected, _connection.ActiveSession.SessionUid));
                operation.EnsureCurrent();

                return remote;
            }
            catch (Exception exception)
            {
                // 状态监听器可能在连接提交后立即取消，此时仍需回滚。
                if (_connection.ActiveSession != null)
                    await _connection.DisconnectAsync();

                if (!_coordinator.IsTerminating)
                    EmitState(new RealtimeState(exception is OperationCanceledException
                        ? RealtimeConnectionState.Disconnected
                        : RealtimeConnectionState.Error));

                XmaxLogger.Realtime.Failure(exception);
                if (exception is OperationCanceledException)
                    throw;

                throw RealtimeErrorHandler.Wrap(exception);
            }
        }

        /// <summary>
        /// 在已连接状态下直接向 RTC 推送一帧 Camera 数据。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        /// <remarks>
        /// 此入口直接发布至 RTC；需要本地预览时，通过 LocalStream.PushVideoFrame 输入。
        /// </remarks>
        /// <exception cref="XmaxException">未连接、正在清理、线程不符合要求，或帧数据无效。</exception>
        /// <exception cref="ArgumentNullException">允许推帧时传入的 frame 为 null。</exception>
        public void PushVideoFrame(XmaxVideoFrame frame)
        {
            _coordinator.RequireAvailable();
            RequireConnection();
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            frame.Validate();
            _stream.PushFrame(frame);
        }

        /// <summary>
        /// 仅在连接有效且未开始清理时，将本地预览帧发布至 RTC。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        private void PublishLocalFrame(XmaxVideoFrame frame)
        {
            if (!_coordinator.IsTerminating && _connection.ActiveSession != null)
                _stream.PushFrame(frame);
        }

        /// <summary>
        /// 在已连接状态下推送 RGBA Camera 帧，使用指定行步长、时间戳和旋转信息。
        /// </summary>
        /// <param name="rgba">RGBA 像素数组，调用期间不得修改。</param>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <param name="timestampMicroseconds">传递给 RTC 的微秒时间戳，默认原样传递 0。</param>
        /// <param name="stride">每行字节数；小于等于零时使用宽度乘以 4 的紧凑布局。</param>
        /// <param name="rotation">视频旋转元数据，支持 0、90、180 和 270 度，不改变像素数组。</param>
        public void PushRgbaFrame(
            byte[] rgba,
            int width,
            int height,
            long timestampMicroseconds = 0,
            int stride = 0,
            XmaxVideoRotation rotation = XmaxVideoRotation.Rotation0)
        {
            PushVideoFrame(XmaxVideoFrame.CreateRgba(
                    rgba,
                    width,
                    height,
                    stride,
                    timestampMicroseconds,
                    rotation));
        }

        /// <summary>
        /// 在已连接状态下推送 I420 Camera 帧，按 Y、U、V 平面提供数据。
        /// </summary>
        /// <param name="y">Y 亮度平面，数组不会被复制。</param>
        /// <param name="u">U 色度平面，宽高为亮度平面的一半，数组不会被复制。</param>
        /// <param name="v">V 色度平面，宽高为亮度平面的一半，数组不会被复制。</param>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <param name="timestampMicroseconds">传递给 RTC 的微秒时间戳，默认原样传递 0。</param>
        /// <param name="strideY">Y 平面的每行字节数；小于等于零时使用视频宽度。</param>
        /// <param name="strideU">U 平面的每行字节数；小于等于零时使用视频宽度的一半。</param>
        /// <param name="strideV">V 平面的每行字节数；小于等于零时使用视频宽度的一半。</param>
        /// <param name="rotation">视频旋转元数据，支持 0、90、180 和 270 度，不改变像素数组。</param>
        public void PushI420Frame(
            byte[] y,
            byte[] u,
            byte[] v,
            int width,
            int height,
            long timestampMicroseconds = 0,
            int strideY = 0,
            int strideU = 0,
            int strideV = 0,
            XmaxVideoRotation rotation = XmaxVideoRotation.Rotation0)
        {
            PushVideoFrame(XmaxVideoFrame.CreateI420(
                    y,
                    u,
                    v,
                    width,
                    height,
                    strideY,
                    strideU,
                    strideV,
                    timestampMicroseconds,
                    rotation));
        }


        /// <summary>
        /// 向当前生成任务发送编码画面内的交互轨迹，非生成状态不能发送有效轨迹。
        /// </summary>
        /// <param name="tracks">以编码画面左上角为原点的交互点序列，坐标须位于画面内。</param>
        public void SendTracks(IReadOnlyList<XmaxTrackPoint> tracks)
        {
            _coordinator.RequireAvailable();
            _interaction.Send(tracks, _videoFormat,
                CurrentState.ConnectionState == RealtimeConnectionState.Generating ? _generation.TaskId : null);
        }

        /// <summary>
        /// 启动或更新生成；首次启动等待任务确认和远端首帧，已有生成任务时更新条件。
        /// </summary>
        /// <param name="context">生成条件；首次生成必须提供，后续可传 null 复用最近一次成功条件。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>首次生成的首帧已就绪，或当前任务条件已更新的任务。</returns>
        /// <exception cref="XmaxException">没有可复用的首次生成条件、调用状态无效，或生成确认、首帧等待失败。</exception>
        /// <exception cref="OperationCanceledException">调用方取消或清理操作撤销了本次启动。</exception>
        public async Task StartGenerationAsync(
            RealtimeContext context = null,
            CancellationToken cancellationToken = default)
        {
            _coordinator.RequireAvailable();
            RequireConnection();

            using (var operation = _coordinator.Begin(RealtimeOperation.Generation, cancellationToken))
            {
                if (_generation.TaskId == null)
                    _timing.Begin();

                await PerformGenerationAsync(context, operation);
            }
        }

        /// <summary>
        /// 使用本管理器的本地流按需连接，再启动或更新生成；整个调用占用同一个操作租约。
        /// </summary>
        /// <param name="localStream">由当前管理器创建且仍有效的本地外部视频流。</param>
        /// <param name="context">生成条件；首次生成必须提供，后续可传 null 复用最近一次成功条件。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>当前连接的远端流；首次启动在任务确认及首帧就绪后返回。</returns>
        /// <remarks>
        /// 连接阶段失败会回滚连接；连接成功后的生成阶段失败保留连接，便于重新生成。
        /// </remarks>
        /// <exception cref="XmaxException">没有可复用的首次生成条件、调用状态无效，或生成确认、首帧等待失败。</exception>
        /// <exception cref="OperationCanceledException">调用方取消或清理操作撤销了本次启动。</exception>
        public async Task<RealtimeMediaStream> StartGenerationAsync(
            RealtimeMediaStream localStream,
            RealtimeContext context,
            CancellationToken cancellationToken = default)
        {
            _coordinator.RequireAvailable();
            var format = _media.RequireOwned(localStream);

            using (var operation = _coordinator.Begin(RealtimeOperation.Generation, cancellationToken))
            {
                if (_generation.TaskId == null)
                    _timing.Begin();

                try
                {
                    var remote = _connection.CurrentRemoteStream;
                    if (_connection.ActiveSession == null)
                        remote = await PerformConnectAsync(format, false, operation);

                    operation.EnsureCurrent();
                    if (remote == null)
                        throw new XmaxException(XmaxErrorCode.RtcError, "Realtime connection has no remote stream.");

                    await PerformGenerationAsync(context, operation);

                    return remote;
                }
                catch (Exception exception)
                {
                    _timing.Fail(exception);

                    throw;
                }
            }
        }

        /// <summary>
        /// 在操作租约中启动或更新生成，首帧就绪后提交状态；取消时停止生成。
        /// </summary>
        /// <param name="context">生成条件；首次生成必须提供，后续可传 null 复用最近一次成功条件。</param>
        /// <param name="operation">调用方持有的操作租约，用于跨等待和回调检查取消。</param>
        /// <returns>本次生成操作及其状态提交完成的任务。</returns>
        private async Task PerformGenerationAsync(RealtimeContext context, RealtimeCoordinator.Operation operation)
        {
            try
            {
                var taskId = await _generation.StartAsync(context, _videoFormat, operation.Token);
                operation.EnsureCurrent();
                EmitState(new RealtimeState(
                        RealtimeConnectionState.Generating,
                        _connection.ActiveSession.SessionUid,
                        taskId));
                operation.EnsureCurrent();
                _timing.Finish();
            }
            catch (OperationCanceledException exception)
            {
                await _generation.StopAsync();
                if (!_coordinator.IsTerminating && _connection.ActiveSession != null)
                    EmitState(new RealtimeState(RealtimeConnectionState.Connected, _connection.ActiveSession.SessionUid));

                _timing.Fail(exception);

                throw;
            }
            catch (Exception exception)
            {
                _timing.Fail(exception);
                XmaxLogger.Realtime.Failure(exception);

                throw RealtimeErrorHandler.Wrap(exception);
            }
        }

        /// <summary>
        /// 启动或更新生成；首次启动等待任务确认和远端首帧，已有生成任务时更新条件。
        /// </summary>
        /// <param name="prompt">生成提示词；null 会规范化为空字符串。</param>
        /// <param name="referencePath">可选的服务端参考资源路径；null 或空白表示不指定。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>首次生成的首帧已就绪，或当前任务条件已更新的任务。</returns>
        /// <exception cref="XmaxException">没有可复用的首次生成条件、调用状态无效，或生成确认、首帧等待失败。</exception>
        /// <exception cref="OperationCanceledException">调用方取消或清理操作撤销了本次启动。</exception>
        public Task StartGenerationAsync(
            string prompt,
            string referencePath = null,
            CancellationToken cancellationToken = default)
            => StartGenerationAsync(new RealtimeContext(prompt, referencePath), cancellationToken);

        /// <summary>
        /// 停止生成并保留已有连接和成功条件；独立连接操作进行中时不取消该连接。
        /// </summary>
        /// <returns>生成清理已完成的任务；一步式启动仍在连接阶段时同时清理该连接。</returns>
        public Task StopGenerationAsync()
        {
            _coordinator.RequireThread();

            // 停止生成不能撤销独立发起的连接操作。
            if (_coordinator.IsConnecting && !_coordinator.IsTerminating)
                return Task.CompletedTask;

            if (CurrentState.ConnectionState == RealtimeConnectionState.Connecting)
                return TerminateAsync(TerminationScope.Connection);

            return TerminateAsync(TerminationScope.Generation);
        }

        /// <summary>
        /// 停止生成并清理 RTC 和服务端会话，保留本地流用于预览或重新连接。
        /// </summary>
        /// <returns>连接清理结束的任务，重复请求合并处理。</returns>
        public Task DisconnectAsync() => TerminateAsync(TerminationScope.Connection);

        /// <summary>
        /// 断开连接并使本地流失效；管理器可重新创建本地流后继续使用。
        /// </summary>
        /// <returns>全部清理结束的任务，重复请求合并处理。</returns>
        public Task CloseAsync() => TerminateAsync(TerminationScope.All);

        /// <summary>
        /// 按指定范围协调停止生成、断开连接及本地流失效，并发送终态通知。
        /// </summary>
        /// <param name="scope">本次请求需要的清理范围，后续请求可以扩大该范围。</param>
        /// <param name="error">清理后需要报告的致命错误，主动清理时为 null。</param>
        /// <returns>合并后的清理任务，完成前不会接纳新操作。</returns>
        private Task TerminateAsync(TerminationScope scope, XmaxException error = null)
        {
            var fatalReported = false;

            return _coordinator.TerminateAsync(scope, error,
                () =>
                {
                    if (scope >= TerminationScope.Connection)
                        EmitState(new RealtimeState(
                            RealtimeConnectionState.Disconnecting,
                            CurrentState.SessionId));
                },
                async target =>
                {
                    await _generation.StopAsync();
                    if (target >= TerminationScope.Connection)
                    {
                        await _connection.DisconnectAsync();
                        _generation.ResetContext();
                    }

                    if (target == TerminationScope.All)
                        _media.Close();
                },
                (target, fatal) =>
                {
                    if (target >= TerminationScope.Connection)
                        EmitState(new RealtimeState(fatal == null
                            ? RealtimeConnectionState.Disconnected
                            : RealtimeConnectionState.Error));
                    else if (_connection.ActiveSession != null)
                        EmitState(new RealtimeState(
                            RealtimeConnectionState.Connected,
                            _connection.ActiveSession.SessionUid));

                    if (fatal != null && !fatalReported)
                    {
                        fatalReported = true;
                        EventDispatch.Raise(ErrorOccurred, RealtimeErrorHandler.Wrap(fatal, XmaxErrorSeverity.Fatal));
                    }
                });
        }

        /// <summary>
        /// 为仍有效的连接或操作启动致命错误清理，并观察其结果。
        /// </summary>
        /// <param name="exception">需要处理的原始异常。</param>
        private void HandleFatalError(XmaxException exception)
        {
            if (!_coordinator.HasOperation && _connection.ActiveSession == null)
                return;

            XmaxLogger.Realtime.Failure(exception);
            _failureCleanup = ObserveFailureCleanupAsync(TerminateAsync(TerminationScope.Connection, exception));
        }

        /// <summary>
        /// 等待后台错误清理，并记录清理过程中的异常。
        /// </summary>
        /// <param name="cleanup">已经启动的后台清理任务。</param>
        /// <returns>清理结果已被观察的任务。</returns>
        private static async Task ObserveFailureCleanupAsync(Task cleanup)
        {
            try
            {
                await cleanup;
            }
            catch (Exception exception)
            {
                XmaxLogger.Realtime.Failure(exception);
            }
        }

        /// <summary>
        /// 校验存在会话且状态为已连接或生成中。
        /// </summary>
        private void RequireConnection()
        {
            if (_connection.ActiveSession == null ||
                (CurrentState.ConnectionState != RealtimeConnectionState.Connected &&
                 CurrentState.ConnectionState != RealtimeConnectionState.Generating))
                throw new XmaxException(XmaxErrorCode.RtcError, "Realtime connection is not open.");
        }

        /// <summary>
        /// 提交有变化的状态快照并通知订阅者，回调重入导致状态变化时停止旧通知。
        /// </summary>
        /// <param name="state">即将提交的不可变连接和生成状态快照。</param>
        private void EmitState(RealtimeState state)
        {
            if (CurrentState.ConnectionState == state.ConnectionState &&
                CurrentState.SessionId == state.SessionId &&
                CurrentState.TaskId == state.TaskId)
                return;

            CurrentState = state;
            XmaxLogger.Realtime.Info(() => "State=" + state.ConnectionState);
            EventDispatch.Raise(StateChanged, state, () => ReferenceEquals(CurrentState, state));
        }
    }
}
