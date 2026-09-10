using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    internal sealed class XmaxRealtimeConnectionManager
    {
        private readonly IRealtimeSessionService _sessions;
        private readonly IStreamController _stream;
        private readonly RenderController _render;
        private readonly SessionHeartbeat _heartbeat;
        private readonly RealtimeTiming _timing;
        internal XmaxSession ActiveSession { get; private set; }
        internal RealtimeMediaStream CurrentRemoteStream { get; private set; }
        internal event Action<XmaxException> FatalError;
        internal event Action<XmaxException> CleanupError;
        internal XmaxRealtimeConnectionManager(IRealtimeSessionService sessions, IStreamController stream, RenderController render, RealtimeTiming timing = null)
        {
            _sessions = sessions; _stream = stream; _render = render;
            _timing = timing;
            _heartbeat = new SessionHeartbeat(sessions);
            _heartbeat.Failed += exception => EventDispatch.Raise(FatalError, exception);
        }
        internal async Task<RealtimeMediaStream> ConnectAsync(string model, RealtimeVideoFormat format, CancellationToken token)
        {
            _stream.ValidatePlatform(); // Reject unsupported players before creating a billable session.
            XmaxSession session = null;
            try
            {
                _timing?.Mark("session-start");
                session = await _sessions.CreateSessionAsync(model, token);
                _timing?.Mark("session-end");
                token.ThrowIfCancellationRequested();
                var info = RealtimeSessionService.RequireJoinInfo(session);
                var track = new RealtimeVideoTrack(info.BotName);
                _render.Bind(track);
                _timing?.Mark("room-start");
                await _stream.ConnectAsync(info, format, token);
                _timing?.Mark("room-end");
                token.ThrowIfCancellationRequested();
                ActiveSession = session;
                _heartbeat.Start(session.SessionUid);
                return CurrentRemoteStream = new RealtimeMediaStream(RealtimeStreamId.Remote, track);
            }
            catch
            {
                _render.Reset();
                try { await _stream.DisconnectAsync(); }
                finally { await CloseSessionAsync(session); }
                throw;
            }
        }
        internal async Task DisconnectAsync()
        {
            var session = ActiveSession;
            ActiveSession = null;
            CurrentRemoteStream = null;
            _render.Reset();
            try
            {
                await _heartbeat.StopAsync();
                await _stream.DisconnectAsync();
            }
            finally { await CloseSessionAsync(session); }
        }
        private async Task CloseSessionAsync(XmaxSession session)
        {
            if (session == null || string.IsNullOrEmpty(session.SessionUid)) return;
            try { await _sessions.CloseSessionAsync(session.SessionUid, CancellationToken.None); }
            catch (Exception exception)
            {
                XmaxLogger.Realtime.Failure(exception);
                EventDispatch.Raise(CleanupError, RealtimeErrorHandler.Wrap(exception));
            }
        }
    }
}
