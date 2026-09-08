using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    internal enum RealtimeOperation { Connection, Generation }
    internal enum TerminationScope { Generation, Connection, All }

    // Unity main-thread equivalent of the iOS actor: synchronous admission, cancellable
    // operation leases, and a single cleanup barrier. No new work enters during cleanup.
    internal sealed class RealtimeCoordinator
    {
        private readonly int _threadId = Thread.CurrentThread.ManagedThreadId;
        private Operation _active;
        private TaskCompletionSource<bool> _termination;
        private TerminationScope _scope;
        private XmaxException _fatalError;
        internal bool IsTerminating => _termination != null;
        internal bool HasOperation => _active != null;
        internal bool IsConnecting => _active?.Kind == RealtimeOperation.Connection;
        internal void RequireThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != _threadId)
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Use realtime APIs on the Unity main thread where the manager was created.");
        }
        internal void RequireAvailable()
        {
            RequireThread();
            if (IsTerminating) throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Realtime cleanup is in progress. Await it before starting new work.");
        }
        internal Operation Begin(RealtimeOperation kind, CancellationToken cancellationToken)
        {
            RequireAvailable();
            cancellationToken.ThrowIfCancellationRequested();
            if (_active != null) throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Another realtime operation is in progress.");
            return _active = new Operation(this, kind, cancellationToken);
        }
        internal Task TerminateAsync(TerminationScope scope, XmaxException fatalError,
            Action begin, Func<TerminationScope, Task> cleanup, Action<TerminationScope, XmaxException> complete)
        {
            RequireThread();
            if (_termination != null)
            {
                if (scope > _scope) { _scope = scope; begin(); }
                if (fatalError != null) _fatalError = fatalError;
                return _termination.Task;
            }
            _scope = scope;
            _fatalError = fatalError;
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _termination = completion; // Install the barrier before any user callback or cancellation.
            var active = _active;
            active?.Cancel();
            begin();
            _ = FinishTerminationAsync(active, completion, cleanup, complete);
            return completion.Task;
        }
        private async Task FinishTerminationAsync(Operation active, TaskCompletionSource<bool> completion,
            Func<TerminationScope, Task> cleanup, Action<TerminationScope, XmaxException> complete)
        {
            try
            {
                if (active != null) await active.Done.Task;
                TerminationScope cleaned;
                do
                {
                    do { cleaned = _scope; await cleanup(cleaned); } while (_scope > cleaned);
                    // Keep the barrier through terminal notifications. Listeners can request
                    // a wider cleanup, which must finish before releasing the shared task.
                    complete(cleaned, _fatalError);
                } while (_scope > cleaned);
                completion.TrySetResult(true);
            }
            catch (Exception exception) { completion.TrySetException(exception); }
            finally { _termination = null; _fatalError = null; }
        }
        internal sealed class Operation : IDisposable
        {
            private readonly RealtimeCoordinator _owner;
            private readonly CancellationTokenSource _cancellation;
            internal readonly TaskCompletionSource<bool> Done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            internal RealtimeOperation Kind { get; }
            internal CancellationToken Token => _cancellation.Token;
            internal Operation(RealtimeCoordinator owner, RealtimeOperation kind, CancellationToken cancellationToken)
            { _owner = owner; Kind = kind; _cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken); }
            internal void EnsureCurrent() => Token.ThrowIfCancellationRequested();
            internal void Cancel() => _cancellation.Cancel();
            public void Dispose()
            {
                if (!ReferenceEquals(_owner._active, this)) return;
                _owner._active = null;
                _cancellation.Dispose();
                Done.TrySetResult(true);
            }
        }
    }
}
