using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 以全局独占租约保护厂商 RTC 引擎的单实例约束。
    /// </summary>
    internal static class RtcEngineManager
    {
        private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 可取消地等待全局引擎使用权。
        /// </summary>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>必须在引擎释放后 Dispose 的独占租约。</returns>
        internal static async Task<IDisposable> AcquireAsync(CancellationToken cancellationToken)
        {
            await Gate.WaitAsync(cancellationToken);

            return new Lease();
        }

        /// <summary>
        /// 持有全局 RTC 引擎使用权的一次性租约。
        /// </summary>
        private sealed class Lease : IDisposable
        {
            private int _released;

            /// <summary>
            /// 仅在首次释放时归还全局引擎使用权。
            /// </summary>
            public void Dispose()
            {
                if (Interlocked.Exchange(ref _released, 1) == 0)
                    Gate.Release();
            }
        }
    }
}
