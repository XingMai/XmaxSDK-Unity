using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    internal sealed class SessionHeartbeat
    {
        private readonly IRealtimeSessionService _sessions;
        private readonly Func<CancellationToken, Task> _delay;
        private CancellationTokenSource _cancellation;
        private Task _loop = Task.CompletedTask;
        internal event Action<XmaxException> Failed;
        internal SessionHeartbeat(IRealtimeSessionService sessions, Func<CancellationToken, Task> delay = null)
        { _sessions = sessions; _delay = delay ?? (token => Task.Delay(10000, token)); }
        internal void Start(string sessionId)
        {
            if (_cancellation != null) throw new InvalidOperationException("Stop the previous heartbeat first.");
            _cancellation = new CancellationTokenSource();
            _loop = RunAsync(sessionId, _cancellation.Token);
        }
        internal async Task StopAsync()
        {
            var cancellation = _cancellation;
            if (cancellation == null) return;
            cancellation.Cancel();
            await _loop;
            cancellation.Dispose();
            if (ReferenceEquals(_cancellation, cancellation)) _cancellation = null;
        }
        private async Task RunAsync(string sessionId, CancellationToken token)
        {
            await Task.Yield();
            try
            {
                while (true)
                {
                    await _delay(token);
                    token.ThrowIfCancellationRequested();
                    var session = await _sessions.HeartbeatSessionAsync(sessionId, token);
                    token.ThrowIfCancellationRequested(); // A late response cannot fail a replacement connection.
                    if (!string.IsNullOrEmpty(session.Status) && session.Status != "ACTIVE")
                        throw new XmaxException(XmaxErrorCode.SessionError, "Session is no longer active: " + session.Status);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
            catch (Exception exception)
            {
                if (!token.IsCancellationRequested) EventDispatch.Raise(Failed, RealtimeErrorHandler.Wrap(exception, XmaxErrorSeverity.Fatal));
            }
        }
    }
}
