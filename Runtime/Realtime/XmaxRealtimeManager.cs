using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    public sealed class XmaxRealtimeManager
    {
        private const int SessionHeartbeatMilliseconds = 10000;

        private readonly XmaxConfiguration _configuration;
        private readonly XmaxSessionClient _sessionClient;
        private readonly VolcRtcController _rtcController;
        private CancellationTokenSource _connectCancellation;
        private CancellationTokenSource _connectionCancellation;
        private XmaxSession _activeSession;
        private RealtimeVideoFormat _videoFormat;
        private RealtimeContext _currentContext;
        private RealtimeVideoTrack _remoteTrack;
        private bool _connecting;
        private bool _generationStarting;

        public RealtimeConfiguration Options { get; }
        public RealtimeState CurrentState { get; private set; } =
            new RealtimeState(RealtimeConnectionState.Idle);

        public event Action<RealtimeState> StateChanged;
        public event Action<XmaxVideoFrame> RemoteFrameReceived;

        public XmaxRealtimeManager(
            string apiKey,
            RealtimeModel model = RealtimeModel.X2_0,
            string baseUrl = XmaxConfiguration.DefaultBaseUrl)
            : this(
                new XmaxConfiguration(apiKey, baseUrl),
                new RealtimeConfiguration(Models.Realtime(model)))
        {
            _configuration.Validate();
        }

        internal XmaxRealtimeManager(XmaxConfiguration configuration, RealtimeConfiguration options)
        {
            _configuration = configuration;
            Options = options ?? throw new XmaxException(
                XmaxErrorCode.InvalidConfiguration,
                "Realtime options are required.");
            _sessionClient = new XmaxSessionClient(configuration);
            _rtcController = new VolcRtcController();
            _rtcController.FatalError += HandleRtcFatalError;
        }

        public async Task<RealtimeMediaStream> ConnectAsync(
            RealtimeVideoFormat videoFormat,
            CancellationToken cancellationToken = default)
        {
            if (_activeSession != null || _connecting)
            {
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Realtime connection is already open.");
            }

            videoFormat.Validate();
            _configuration.Validate();
            _connecting = true;
            _currentContext = null;
            EmitState(new RealtimeState(RealtimeConnectionState.Connecting));
            XmaxSession session = null;
            var connectCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _connectCancellation = connectCancellation;
            try
            {
                session = await _sessionClient.CreateSessionAsync(Options.Model.Name, connectCancellation.Token);
                var joinInfo = XmaxSessionClient.RequireJoinInfo(session);
                var remoteTrack = new RealtimeVideoTrack(joinInfo.BotName);
                remoteTrack.FrameReceived += HandleRemoteFrame;
                await _rtcController.JoinAsync(joinInfo, videoFormat, remoteTrack, connectCancellation.Token);
                connectCancellation.Token.ThrowIfCancellationRequested();

                _connectionCancellation = new CancellationTokenSource();
                _activeSession = session;
                _videoFormat = videoFormat;
                _remoteTrack = remoteTrack;
                StartSessionHeartbeat(session.SessionUid, _connectionCancellation.Token);
                EmitState(new RealtimeState(RealtimeConnectionState.Connected, session.SessionUid));
                return new RealtimeMediaStream(RealtimeStreamId.Remote, remoteTrack);
            }
            catch (OperationCanceledException)
            {
                await _rtcController.LeaveAsync();
                await TryCloseSessionAsync(session);
                EmitState(new RealtimeState(RealtimeConnectionState.Disconnected));
                throw;
            }
            catch (Exception exception)
            {
                await _rtcController.LeaveAsync();
                await TryCloseSessionAsync(session);
                EmitState(new RealtimeState(RealtimeConnectionState.Error));
                throw AsXmaxException(exception);
            }
            finally
            {
                if (ReferenceEquals(_connectCancellation, connectCancellation))
                {
                    _connectCancellation = null;
                }
                connectCancellation.Dispose();
                _connecting = false;
            }
        }

        public Task<RealtimeMediaStream> ConnectAsync(
            int width,
            int height,
            int fps,
            CancellationToken cancellationToken = default)
        {
            return ConnectAsync(new RealtimeVideoFormat(width, height, fps), cancellationToken);
        }

        public void PushVideoFrame(XmaxVideoFrame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }
            if (_activeSession == null ||
                (CurrentState.ConnectionState != RealtimeConnectionState.Connected &&
                 CurrentState.ConnectionState != RealtimeConnectionState.Generating))
            {
                throw new XmaxException(XmaxErrorCode.RtcError, "Realtime connection is not open.");
            }
            _rtcController.PushFrame(frame);
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
            if (tracks == null)
            {
                throw new ArgumentNullException(nameof(tracks));
            }
            if (tracks.Count == 0)
            {
                return;
            }
            if (_activeSession == null ||
                CurrentState.ConnectionState != RealtimeConnectionState.Generating ||
                string.IsNullOrEmpty(CurrentState.TaskId))
            {
                throw new XmaxException(
                    XmaxErrorCode.RtcError,
                    "Tracks can only be sent while realtime generation is active.");
            }

            for (var index = 0; index < tracks.Count; index++)
            {
                var point = tracks[index];
                if (point.X < 0 || point.X >= _videoFormat.Width ||
                    point.Y < 0 || point.Y >= _videoFormat.Height)
                {
                    throw new XmaxException(
                        XmaxErrorCode.InvalidConfiguration,
                        $"Track point ({point.X}, {point.Y}) is outside the video bounds " +
                        $"0..{_videoFormat.Width - 1}, 0..{_videoFormat.Height - 1}.");
                }
            }

            _rtcController.SendTracks(tracks, CurrentState.TaskId);
        }

        public async Task StartGenerationAsync(
            RealtimeContext context = null,
            CancellationToken cancellationToken = default)
        {
            var session = _activeSession;
            if (session == null ||
                (CurrentState.ConnectionState != RealtimeConnectionState.Connected &&
                 CurrentState.ConnectionState != RealtimeConnectionState.Generating))
            {
                throw new XmaxException(XmaxErrorCode.RtcError, "Realtime connection is not open.");
            }
            if (_generationStarting)
            {
                throw new XmaxException(XmaxErrorCode.RtcError, "Realtime generation is starting.");
            }

            var resolvedContext = context ?? _currentContext;
            if (resolvedContext == null)
            {
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "A realtime context is required for the first generation.");
            }

            if (context != null && CurrentState.ConnectionState == RealtimeConnectionState.Generating)
            {
                _rtcController.ChangeGenerationCondition(context, _videoFormat, CurrentState.TaskId);
                _currentContext = context;
                return;
            }

            if (CurrentState.ConnectionState == RealtimeConnectionState.Generating)
            {
                _rtcController.StopGeneration();
                EmitState(new RealtimeState(RealtimeConnectionState.Connected, session.SessionUid));
            }

            _generationStarting = true;
            try
            {
                using (var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                           cancellationToken,
                           _connectionCancellation?.Token ?? CancellationToken.None))
                {
                    var taskId = await _rtcController.StartGenerationAsync(
                        resolvedContext,
                        _videoFormat,
                        linkedCancellation.Token);
                    if (_activeSession != session)
                    {
                        throw new XmaxException(XmaxErrorCode.RtcError, "Realtime connection was cancelled.");
                    }
                    _currentContext = resolvedContext;
                    EmitState(new RealtimeState(
                        RealtimeConnectionState.Generating,
                        session.SessionUid,
                        taskId));
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw AsXmaxException(exception);
            }
            finally
            {
                _generationStarting = false;
            }
        }

        public Task StartGenerationAsync(
            string prompt,
            string referencePath = null,
            CancellationToken cancellationToken = default)
        {
            return StartGenerationAsync(
                new RealtimeContext(prompt, referencePath),
                cancellationToken);
        }

        public Task StopGenerationAsync()
        {
            var session = _activeSession;
            if (session == null ||
                (CurrentState.ConnectionState != RealtimeConnectionState.Connected &&
                 CurrentState.ConnectionState != RealtimeConnectionState.Generating))
            {
                return Task.CompletedTask;
            }
            _rtcController.StopGeneration();
            EmitState(new RealtimeState(RealtimeConnectionState.Connected, session.SessionUid));
            return Task.CompletedTask;
        }

        public async Task DisconnectAsync()
        {
            _connectCancellation?.Cancel();
            _connectionCancellation?.Cancel();
            _connectionCancellation?.Dispose();
            _connectionCancellation = null;
            _generationStarting = false;
            var session = _activeSession;
            _activeSession = null;
            if (_remoteTrack != null)
            {
                _remoteTrack.FrameReceived -= HandleRemoteFrame;
            }
            _remoteTrack = null;
            _currentContext = null;
            await _rtcController.LeaveAsync();
            await TryCloseSessionAsync(session);
            EmitState(new RealtimeState(RealtimeConnectionState.Disconnected));
        }

        private async void StartSessionHeartbeat(string sessionUid, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(SessionHeartbeatMilliseconds, cancellationToken);
                    var session = await _sessionClient.HeartbeatSessionAsync(sessionUid, cancellationToken);
                    if (!string.IsNullOrEmpty(session.Status) && session.Status != "ACTIVE")
                    {
                        throw new XmaxException(
                            XmaxErrorCode.SessionError,
                            $"Session is no longer active: {session.Status}.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                await FailConnectionAsync(sessionUid, AsXmaxException(exception));
            }
        }

        private async void HandleRtcFatalError(XmaxException exception)
        {
            var sessionId = _activeSession?.SessionUid;
            if (!string.IsNullOrEmpty(sessionId))
            {
                await FailConnectionAsync(sessionId, exception);
            }
        }

        private async Task FailConnectionAsync(string sessionUid, XmaxException exception)
        {
            if (_activeSession?.SessionUid != sessionUid)
            {
                return;
            }
            _connectionCancellation?.Cancel();
            _connectionCancellation?.Dispose();
            _connectionCancellation = null;
            var session = _activeSession;
            _activeSession = null;
            _currentContext = null;
            if (_remoteTrack != null)
            {
                _remoteTrack.FrameReceived -= HandleRemoteFrame;
            }
            _remoteTrack = null;
            await _rtcController.LeaveAsync();
            await TryCloseSessionAsync(session);
            EmitState(new RealtimeState(RealtimeConnectionState.Error, sessionUid));
            UnityEngine.Debug.LogException(exception);
        }

        private async Task TryCloseSessionAsync(XmaxSession session)
        {
            if (session == null || string.IsNullOrEmpty(session.SessionUid))
            {
                return;
            }
            try
            {
                await _sessionClient.CloseSessionAsync(session.SessionUid, CancellationToken.None);
            }
            catch
            {
            }
        }

        private void EmitState(RealtimeState state)
        {
            CurrentState = state;
            StateChanged?.Invoke(state);
        }

        private void HandleRemoteFrame(XmaxVideoFrame frame)
        {
            RemoteFrameReceived?.Invoke(frame);
        }

        private static XmaxException AsXmaxException(Exception exception)
        {
            if (exception is XmaxException xmaxException)
            {
                return xmaxException;
            }
            return new XmaxException(XmaxErrorCode.RtcError, exception.Message, null, null, exception);
        }
    }
}
