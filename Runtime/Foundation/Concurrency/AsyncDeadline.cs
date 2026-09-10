using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 为不可直接取消的任务提供超时和调用方取消等待。
    /// </summary>
    internal static class AsyncDeadline
    {
        /// <summary>
        /// 等待任务完成、超时或取消；结束等待不会自动停止底层任务。
        /// </summary>
        /// <param name="task">需要等待的底层任务；超时或取消等待不会自动终止该任务。</param>
        /// <param name="milliseconds">等待时限，单位为毫秒。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>原任务的完成结果；超时或取消通过异常结束等待。</returns>
        internal static async Task WaitAsync(Task task, int milliseconds, CancellationToken cancellationToken)
        {
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                var delay = Task.Delay(milliseconds, timeout.Token);
                var completed = await Task.WhenAny(task, delay);
                timeout.Cancel();
                cancellationToken.ThrowIfCancellationRequested();
                if (completed != task)
                    throw new XmaxException(XmaxErrorCode.Timeout, "Realtime operation timed out.");

                await task;
            }
        }
    }
}
