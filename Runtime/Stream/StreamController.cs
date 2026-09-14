using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 管理房间业务信令、任务 SEI 确认、心跳及匹配后的视频转发。
    /// </summary>
    internal sealed class StreamController : IStreamController
    {
        // 传输、生成配置与诊断依赖。
        private readonly IRtcManager _rtc;
        private readonly int _generationTimeout;
        private readonly Func<string> _taskIdFactory;
        private readonly RealtimeTiming _timing;
        private readonly QualityController _quality;

        // 当前房间身份。
        private RtcJoinInfo _info;

        // 后台循环及其取消源。
        private CancellationTokenSource _roomCancellation;
        private CancellationTokenSource _generationCancellation;
        private Task _heartbeat = Task.CompletedTask;
        private Task _sei = Task.CompletedTask;

        // 当前生成任务及远端流匹配状态。
        private TaskCompletionSource<bool> _confirmation;
        private string _taskId;
        private RemoteStream? _matchedStream;

        /// <summary>
        /// 有效连接中的传输、任务确认或后台循环发生致命失败时触发。
        /// </summary>
        public event Action<XmaxException> FatalError;

        /// <summary>
        /// 当前有效连接的公开网络质量统计更新。
        /// </summary>
        public event Action<RealtimeNetworkQuality> NetworkQualityChanged;

        /// <summary>
        /// 收到已通过当前任务 SEI 确认的远端视频帧时触发。
        /// </summary>
        public event Action<XmaxVideoFrame> FrameReceived;

        /// <summary>
        /// 注入 RTC 传输及生成确认配置，并绑定传输和网络质量事件。
        /// </summary>
        /// <param name="rtc">隔离厂商实现的 RTC 传输入口。</param>
        /// <param name="generationTimeout">等待远端任务 SEI 确认的时限，单位为毫秒。</param>
        /// <param name="taskIdFactory">可选任务标识工厂；为空时使用带 Unity 前缀的 GUID。</param>
        /// <param name="timing">共享启动计时器；为 null 时不记录信令和确认阶段耗时。</param>
        internal StreamController(
            IRtcManager rtc,
            int generationTimeout = 30000,
            Func<string> taskIdFactory = null,
            RealtimeTiming timing = null)
        {
            _rtc = rtc;
            _timing = timing;
            _quality = new QualityController(rtc);
            _generationTimeout = generationTimeout;
            _taskIdFactory = taskIdFactory ?? (() => "task-unity-" + Guid.NewGuid().ToString("N"));
            rtc.FatalError += OnFatalError;
            _quality.NetworkQualityChanged += quality =>
{
    if (_info != null)
        EventDispatch.Raise(
        NetworkQualityChanged,
        quality);
};
            rtc.VideoPublished += OnPublished;
            rtc.VideoUnpublished += OnUnpublished;
            rtc.SeiReceived += OnSei;
            rtc.FrameReceived += OnFrame;
        }

        /// <summary>
        /// 在创建在线会话前校验底层 RTC 的运行平台。
        /// </summary>
        public void ValidatePlatform() => _rtc.ValidatePlatform();

        /// <summary>
        /// 加入 RTC 房间并启动房间心跳，失败或取消时清理本次连接。
        /// </summary>
        /// <param name="info">经过校验的 RTC 应用、房间、用户及入房令牌。</param>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>入房成功且心跳已启动的任务。</returns>
        public async Task ConnectAsync(
            RtcJoinInfo info,
            RealtimeVideoFormat format,
            CancellationToken cancellationToken)
        {
            var encoding = EncodingController.Resolve(format);
            _info = info;

            try
            {
                await _rtc.JoinAsync(info, encoding, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                _roomCancellation = new CancellationTokenSource();
                _heartbeat = RunHeartbeatAsync(_roomCancellation.Token);
            }
            catch
            {
                await DisconnectAsync();

                throw;
            }
        }

        /// <summary>
        /// 停止房间心跳和生成后台循环，再退出 RTC 房间。
        /// </summary>
        /// <returns>全部房间循环和 RTC 清理已结束的任务。</returns>
        public async Task DisconnectAsync()
        {
            _roomCancellation?.Cancel();
            await StopGenerationAsync();
            await _heartbeat;
            _roomCancellation?.Dispose();
            _roomCancellation = null;
            _info = null;
            await _rtc.LeaveAsync();
        }

        /// <summary>
        /// 发送启动信令并周期发送任务 SEI，等待远端匹配确认，失败时停止本次生成。
        /// </summary>
        /// <param name="context">已经由上层解析的非空生成条件。</param>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>远端 SEI 已确认的任务标识；首帧就绪由渲染层另行等待。</returns>
        public async Task<string> StartGenerationAsync(
            RealtimeContext context,
            RealtimeVideoFormat format,
            CancellationToken cancellationToken)
        {
            if (_info == null || _roomCancellation == null || _taskId != null)
                throw new XmaxException(
                    XmaxErrorCode.RtcError,
                    "RTC connection is unavailable or generation is already active.");

            cancellationToken.ThrowIfCancellationRequested();
            var taskId = _taskIdFactory();
            _taskId = taskId;
            _matchedStream = null;
            _confirmation = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _generationCancellation = CancellationTokenSource.CreateLinkedTokenSource(_roomCancellation.Token);

            try
            {
                _timing?.BeginSignal(taskId);
                _rtc.SendRoomMessage(RtcRoomEvent.Start(_info.UserId, taskId, format, context));
                _timing?.SignalSent(taskId);
                XmaxLogger.Room.Info(() => "Generation start signal sent.");
                _sei = RunSeiAsync(taskId, _generationCancellation.Token);
                await AsyncDeadline.WaitAsync(_confirmation.Task, _generationTimeout, cancellationToken);

                return taskId;
            }
            catch
            {
                await StopGenerationAsync();

                throw;
            }
        }

        /// <summary>
        /// 校验任务归属后发送生成条件更新信令。
        /// </summary>
        /// <param name="context">已经由上层解析的非空生成条件。</param>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        public void ChangeGenerationCondition(RealtimeContext context, RealtimeVideoFormat format, string taskId)
        {
            RequireTask(taskId);
            _rtc.SendRoomMessage(RtcRoomEvent.ChangeCondition(_info.UserId, format, context));
        }

        /// <summary>
        /// 校验当前任务标识后发送交互轨迹消息。
        /// </summary>
        /// <param name="tracks">以编码画面左上角为原点的交互点序列，坐标须位于画面内。</param>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        public void SendTracks(IReadOnlyList<XmaxTrackPoint> tracks, string taskId)
        {
            RequireTask(taskId);
            _rtc.SendRoomMessage(RtcRoomEvent.Tracks(_info.UserId, taskId, tracks));
        }

        /// <summary>
        /// 将本地外部帧交给 RTC 传输层。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        public void PushFrame(XmaxVideoFrame frame) => _rtc.PushFrame(frame);

        /// <summary>
        /// 使任务和匹配流失效，停止 SEI 循环并尽力发送停止信令。
        /// </summary>
        /// <returns>生成后台循环和停止信令处理已结束的任务。</returns>
        public async Task StopGenerationAsync()
        {
            var taskId = _taskId;
            _taskId = null;
            _matchedStream = null;
            _confirmation?.TrySetCanceled();
            _confirmation = null;

            _generationCancellation?.Cancel();
            await _sei;
            _generationCancellation?.Dispose();
            _generationCancellation = null;

            if (taskId != null && _info != null)
            {
                try
                {
                    _rtc.SendRoomMessage(RtcRoomEvent.Stop(_info.UserId, taskId));
                }
                catch (Exception exception)
                {
                    XmaxLogger.Room.Failure(exception);
                }
            }
        }

        /// <summary>
        /// 检查远端流属于当前房间且符合可选机器人用户过滤。
        /// </summary>
        /// <param name="key">由房间和用户共同标识的远端主流。</param>
        /// <returns>该流可以参与当前任务确认时为 true。</returns>
        private bool Accept(RemoteStream key) => _info != null && key.RoomId == _info.RoomId &&
            (string.IsNullOrEmpty(_info.BotName) || key.UserId == _info.BotName);

        /// <summary>
        /// 发现可接受的视频发布后订阅远端主流，订阅失败报告致命错误。
        /// </summary>
        /// <param name="key">由房间和用户共同标识的远端主流。</param>
        private void OnPublished(RemoteStream key)
        {
            if (!Accept(key))
                return;

            try
            {
                _rtc.SubscribeVideo(key);
            }
            catch (Exception exception)
            {
                OnFatalError(RealtimeErrorHandler.Wrap(exception));
            }
        }

        /// <summary>
        /// 使已取消发布的匹配流失效，等待再次发布和 SEI 确认。
        /// </summary>
        /// <param name="key">由房间和用户共同标识的远端主流。</param>
        private void OnUnpublished(RemoteStream key)
        {
            if (_matchedStream.HasValue && _matchedStream.Value.Equals(key))
                _matchedStream = null;
        }

        /// <summary>
        /// 仅接受当前任务的 SEI 确认，完成等待并允许匹配流的帧通过。
        /// </summary>
        /// <param name="key">由房间和用户共同标识的远端主流。</param>
        /// <param name="data">用于传输或匹配的 SEI 字节数据。</param>
        private void OnSei(RemoteStream key, byte[] data)
        {
            if (_taskId == null || !Accept(key))
                return;

            var message = Encoding.UTF8.GetString(data ?? Array.Empty<byte>()).Trim('\0', ' ', '\r', '\n', '\t');
            if (message != _taskId)
                return;

            _timing?.MatchSei(_taskId);
            // 重新发布后仍需按 SEI 重新绑定，不能只依赖首次确认。
            _matchedStream = key;
            _confirmation?.TrySetResult(true);
        }

        /// <summary>
        /// 只分发当前生成任务已通过 SEI 匹配的远端流视频帧。
        /// </summary>
        /// <param name="key">由房间和用户共同标识的远端主流。</param>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        private void OnFrame(RemoteStream key, XmaxVideoFrame frame)
        {
            if (_taskId == null || !_matchedStream.HasValue || !_matchedStream.Value.Equals(key))
                return;

            EventDispatch.Raise(FrameReceived, frame);
        }

        /// <summary>
        /// 对有效连接结束确认等待，并向上层报告致命错误。
        /// </summary>
        /// <param name="exception">需要处理的原始异常。</param>
        private void OnFatalError(XmaxException exception)
        {
            if (_info == null)
                return;

            _confirmation?.TrySetException(exception);
            EventDispatch.Raise(FatalError, exception);
        }

        /// <summary>
        /// 校验当前房间有效且给定标识与活动生成任务一致。
        /// </summary>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        private void RequireTask(string taskId)
        {
            if (_info == null || string.IsNullOrEmpty(taskId) || taskId != _taskId)
                throw new XmaxException(XmaxErrorCode.RtcError, "Realtime generation task does not match.");
        }

        /// <summary>
        /// 约每 66 毫秒发送当前任务的 SEI，取消或任务失效后退出。
        /// </summary>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>SEI 循环退出后完成的任务；传输失败通过致命事件报告。</returns>
        private async Task RunSeiAsync(string taskId, CancellationToken cancellationToken)
        {
            try
            {
                // 先交还调度，确保清理回调重入前已保存循环任务。
                await Task.Yield();
                var data = Encoding.UTF8.GetBytes(taskId);
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (_taskId != taskId)
                        return;

                    _rtc.SendSei(data);
                    await Task.Delay(66, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                if (!cancellationToken.IsCancellationRequested)
                    OnFatalError(RealtimeErrorHandler.Wrap(exception));
            }
        }

        /// <summary>
        /// 约每 10 秒发送房间心跳，取消后退出。
        /// </summary>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>房间心跳循环退出后完成的任务。</returns>
        private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    await Task.Delay(10000, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    _rtc.SendRoomMessage(RtcRoomEvent.Heartbeat(_info.UserId));
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                if (!cancellationToken.IsCancellationRequested)
                    OnFatalError(RealtimeErrorHandler.Wrap(exception));
            }
        }
    }
}
