using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    // The bundled VolcEngine wrapper has a process-wide singleton. A lease prevents
    // one manager from replacing or releasing another manager's native engine.
    internal static class RtcEngineManager
    {
        private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1, 1);
        internal static async Task<IDisposable> AcquireAsync(CancellationToken cancellationToken)
        {
            await Gate.WaitAsync(cancellationToken);
            return new Lease();
        }
        private sealed class Lease : IDisposable
        {
            private int _released;
            public void Dispose()
            {
                if (Interlocked.Exchange(ref _released, 1) == 0) Gate.Release();
            }
        }
    }
}
