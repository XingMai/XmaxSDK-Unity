using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    internal static class AsyncDeadline
    {
        internal static async Task WaitAsync(Task task, int milliseconds, CancellationToken cancellationToken)
        {
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                var delay = Task.Delay(milliseconds, timeout.Token);
                var completed = await Task.WhenAny(task, delay);
                timeout.Cancel();
                cancellationToken.ThrowIfCancellationRequested();
                if (completed != task) throw new XmaxException(XmaxErrorCode.Timeout, "Realtime operation timed out.");
                await task;
            }
        }
    }
}
