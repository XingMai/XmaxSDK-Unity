using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    public sealed class XmaxRealtimeManager : IXmaxRealtimeManager
    {
        private readonly XmaxConfiguration _configuration;
        private readonly RealtimeCoordinator _coordinator = new RealtimeCoordinator();
        private readonly IStreamController _stream;
        private readonly XmaxRealtimeConnectionManager _connection;
        private readonly XmaxRealtimeGenerationManager _generation;
        private readonly MediaController _media;
        private readonly InteractionController _interaction;
        private readonly RenderController _render;
        private RealtimeVideoFormat _videoFormat;
        private Task _failureCleanup = Task.CompletedTask;

        public RealtimeConfiguration Options { get; }
        public RealtimeState CurrentState { get; private set; } = new RealtimeState(RealtimeConnectionState.Idle);
        public RealtimeMediaStream LocalStream => _media.LocalStream;
        public event Action<RealtimeState> StateChanged;
        public event Action<XmaxVideoFrame> RemoteFrameReceived
        {
            add { _render.FrameReceived += value; }
            remove { _render.FrameReceived -= value; }
        }
        /// <summary>Fatal asynchronous errors are reported after cleanup. Awaited operation failures are thrown.</summary>
        public event Action<XmaxException> ErrorOccurred;
        /// <summary>Best-effort server cleanup failures; native resources have still been released.</summary>
        public event Action<XmaxException> CleanupWarning;
        public event Action<RealtimeNetworkQuality> NetworkQualityChanged;

        public XmaxRealtimeManager(string apiKey, RealtimeModel model = RealtimeModel.X2_0,
            string baseUrl = XmaxConfiguration.DefaultBaseUrl)
            : this(new XmaxConfiguration(apiKey, baseUrl), new RealtimeConfiguration(Models.Realtime(model))) { }

        internal XmaxRealtimeManager(XmaxConfiguration configuration, RealtimeConfiguration options)
            : this(configuration, options, new RealtimeSessionService(new ApiService(configuration)),
                new StreamController(new RtcManager())) { }

        internal XmaxRealtimeManager(XmaxConfiguration configuration, RealtimeConfiguration options,
            IRealtimeSessionService sessions, IStreamController stream)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            configuration.Validate();
            Options = options ?? throw new ArgumentNullException(nameof(options));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            var render = _render = new RenderController();
            _connection = new XmaxRealtimeConnectionManager(sessions, stream, render);
            _generation = new XmaxRealtimeGenerationManager(stream, render);
            _media = new MediaController(_coordinator.RequireThread, PublishLocalFrame);
            _interaction = new InteractionController(stream);
            stream.FrameReceived += render.Receive;
            stream.FatalError += HandleFatalError;
            stream.NetworkQualityChanged += quality =>
            {
                if (!_coordinator.IsTerminating && _connection.ActiveSession != null) EventDispatch.Raise(NetworkQualityChanged, quality);
            };
            _connection.FatalError += HandleFatalError;
            _connection.CleanupError += error => EventDispatch.Raise(CleanupWarning, error);
        }
        public RealtimeMediaStream CreateLocalExternalStream(RealtimeVideoFormat format)
        {
            _coordinator.RequireAvailable();
            if (_coordinator.HasOperation || _connection.ActiveSession != null)
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Disconnect before replacing the local source.");
            return _media.CreateExternalStream(format);
        }
        public Task<RealtimeMediaStream> ConnectAsync(RealtimeMediaStream localStream, CancellationToken cancellationToken = default)
        {
            _coordinator.RequireAvailable();
            return ConnectCoreAsync(_media.RequireOwned(localStream), false, cancellationToken);
        }
        public Task<RealtimeMediaStream> ConnectAsync(RealtimeVideoFormat videoFormat, CancellationToken cancellationToken = default)
            => ConnectCoreAsync(videoFormat, true, cancellationToken);
        public Task<RealtimeMediaStream> ConnectAsync(int width, int height, int fps, CancellationToken cancellationToken = default)
            => ConnectAsync(new RealtimeVideoFormat(width, height, fps), cancellationToken);

        private async Task<RealtimeMediaStream> ConnectCoreAsync(RealtimeVideoFormat format, bool createSource, CancellationToken cancellationToken)
        {
            _coordinator.RequireAvailable();
            if (_connection.ActiveSession != null) throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Realtime connection is already open.");
            format.Validate();
            _configuration.Validate();
            using (var operation = _coordinator.Begin(RealtimeOperation.Connection, cancellationToken))
            {
                try
                {
                    _stream.ValidatePlatform();
                    if (createSource) _media.CreateExternalStream(format);
                    _videoFormat = format;
                    _generation.ResetContext();
                    EmitState(new RealtimeState(RealtimeConnectionState.Connecting));
                    operation.EnsureCurrent();
                    var remote = await _connection.ConnectAsync(Options.Model.Name, format, operation.Token);
                    operation.EnsureCurrent();
                    EmitState(new RealtimeState(RealtimeConnectionState.Connected, _connection.ActiveSession.SessionUid));
                    operation.EnsureCurrent();
                    return remote;
                }
                catch (Exception exception)
                {
                    // A state listener may cancel immediately after the connection commits.
                    if (_connection.ActiveSession != null) await _connection.DisconnectAsync();
                    if (!_coordinator.IsTerminating)
                        EmitState(new RealtimeState(exception is OperationCanceledException ? RealtimeConnectionState.Disconnected : RealtimeConnectionState.Error));
                    if (exception is OperationCanceledException) throw;
                    throw RealtimeErrorHandler.Wrap(exception);
                }
            }
        }
        public void PushVideoFrame(XmaxVideoFrame frame)
        {
            _coordinator.RequireAvailable();
            RequireConnection();
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            frame.Validate();
            _stream.PushFrame(frame);
        }
        private void PublishLocalFrame(XmaxVideoFrame frame)
        {
            if (!_coordinator.IsTerminating && _connection.ActiveSession != null) _stream.PushFrame(frame);
        }
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


        public void SendTracks(IReadOnlyList<XmaxTrackPoint> tracks)
        {
            _coordinator.RequireAvailable();
            _interaction.Send(tracks, _videoFormat,
                CurrentState.ConnectionState == RealtimeConnectionState.Generating ? _generation.TaskId : null);
        }
        public async Task StartGenerationAsync(RealtimeContext context = null, CancellationToken cancellationToken = default)
        {
            _coordinator.RequireAvailable();
            RequireConnection();
            using (var operation = _coordinator.Begin(RealtimeOperation.Generation, cancellationToken))
            {
                try
                {
                    var taskId = await _generation.StartAsync(context, _videoFormat, operation.Token);
                    operation.EnsureCurrent();
                    EmitState(new RealtimeState(RealtimeConnectionState.Generating, _connection.ActiveSession.SessionUid, taskId));
                    operation.EnsureCurrent();
                }
                catch (OperationCanceledException)
                {
                    await _generation.StopAsync();
                    if (!_coordinator.IsTerminating && _connection.ActiveSession != null)
                        EmitState(new RealtimeState(RealtimeConnectionState.Connected, _connection.ActiveSession.SessionUid));
                    throw;
                }
                catch (Exception exception) { throw RealtimeErrorHandler.Wrap(exception); }
            }
        }
        public Task StartGenerationAsync(string prompt, string referencePath = null, CancellationToken cancellationToken = default)
            => StartGenerationAsync(new RealtimeContext(prompt, referencePath), cancellationToken);
        public Task StopGenerationAsync()
        {
            _coordinator.RequireThread();
            // Stopping generation while connecting must not cancel the connection operation.
            if (_coordinator.IsConnecting && !_coordinator.IsTerminating) return Task.CompletedTask;
            return TerminateAsync(TerminationScope.Generation);
        }
        public Task DisconnectAsync() => TerminateAsync(TerminationScope.Connection);
        /// <summary>Disconnect and invalidate the local source. The manager can be reused.</summary>
        public Task CloseAsync() => TerminateAsync(TerminationScope.All);
        private Task TerminateAsync(TerminationScope scope, XmaxException error = null)
        {
            var fatalReported = false;
            return _coordinator.TerminateAsync(scope, error,
                () => { if (scope >= TerminationScope.Connection) EmitState(new RealtimeState(RealtimeConnectionState.Disconnecting, CurrentState.SessionId)); },
                async target =>
                {
                    await _generation.StopAsync();
                    if (target >= TerminationScope.Connection)
                    {
                        await _connection.DisconnectAsync();
                        _generation.ResetContext();
                    }
                    if (target == TerminationScope.All) _media.Close();
                },
                (target, fatal) =>
                {
                    if (target >= TerminationScope.Connection)
                        EmitState(new RealtimeState(fatal == null ? RealtimeConnectionState.Disconnected : RealtimeConnectionState.Error));
                    else if (_connection.ActiveSession != null)
                        EmitState(new RealtimeState(RealtimeConnectionState.Connected, _connection.ActiveSession.SessionUid));
                    if (fatal != null && !fatalReported)
                    {
                        fatalReported = true;
                        EventDispatch.Raise(ErrorOccurred, RealtimeErrorHandler.Wrap(fatal, XmaxErrorSeverity.Fatal));
                    }
                });
        }
        private void HandleFatalError(XmaxException exception)
        {
            if (!_coordinator.HasOperation && _connection.ActiveSession == null) return;
            _failureCleanup = ObserveFailureCleanupAsync(TerminateAsync(TerminationScope.Connection, exception));
        }
        private static async Task ObserveFailureCleanupAsync(Task cleanup)
        {
            try { await cleanup; }
            catch (Exception exception) { UnityEngine.Debug.LogException(exception); }
        }
        private void RequireConnection()
        {
            if (_connection.ActiveSession == null ||
                (CurrentState.ConnectionState != RealtimeConnectionState.Connected && CurrentState.ConnectionState != RealtimeConnectionState.Generating))
                throw new XmaxException(XmaxErrorCode.RtcError, "Realtime connection is not open.");
        }
        private void EmitState(RealtimeState state)
        {
            if (CurrentState.ConnectionState == state.ConnectionState && CurrentState.SessionId == state.SessionId && CurrentState.TaskId == state.TaskId) return;
            CurrentState = state;
            EventDispatch.Raise(StateChanged, state, () => ReferenceEquals(CurrentState, state));
        }
    }
}
