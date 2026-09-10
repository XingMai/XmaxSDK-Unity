using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 由协调器串行接纳的异步操作类型。
    /// </summary>
    internal enum RealtimeOperation
    {
        /// <summary>
        /// 建立实时连接。
        /// </summary>
        Connection,

        /// <summary>
        /// 启动或更新生成，可包含一步式连接。
        /// </summary>
        Generation
    }

    /// <summary>
    /// 清理范围，按生成、连接、全部资源的顺序逐级扩大。
    /// </summary>
    internal enum TerminationScope
    {
        /// <summary>
        /// 停止生成并保留连接和本地流。
        /// </summary>
        Generation,

        /// <summary>
        /// 停止生成并断开连接，保留本地流。
        /// </summary>
        Connection,

        /// <summary>
        /// 清理连接并使本地流失效。
        /// </summary>
        All
    }

    /// <summary>
    /// 在 Unity 主线程串行管理操作租约，合并清理请求并阻止清理期间的新操作。
    /// </summary>
    internal sealed class RealtimeCoordinator
    {
        // 调用线程约束。
        private readonly int _threadId = Thread.CurrentThread.ManagedThreadId;

        // 当前独占操作。
        private Operation _active;

        // 可合并、可扩大范围的清理屏障。
        private TaskCompletionSource<bool> _termination;
        private TerminationScope _scope;
        private XmaxException _fatalError;

        /// <summary>
        /// 是否有尚未完成的清理任务。
        /// </summary>
        internal bool IsTerminating => _termination != null;

        /// <summary>
        /// 是否存在尚未释放的操作租约。
        /// </summary>
        internal bool HasOperation => _active != null;

        /// <summary>
        /// 当前租约是否属于独立的连接操作。
        /// </summary>
        internal bool IsConnecting => _active?.Kind == RealtimeOperation.Connection;

        /// <summary>
        /// 校验调用线程与创建协调器的线程一致。
        /// </summary>
        internal void RequireThread()
        {
            if (Thread.CurrentThread.ManagedThreadId != _threadId)
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Use realtime APIs on the Unity main thread where the manager was created.");
        }

        /// <summary>
        /// 校验调用线程，并拒绝在清理期间开始新工作。
        /// </summary>
        internal void RequireAvailable()
        {
            RequireThread();
            if (IsTerminating)
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Realtime cleanup is in progress. Await it before starting new work.");
        }

        /// <summary>
        /// 接纳一个可取消的操作；已有操作或正在清理时拒绝接纳。
        /// </summary>
        /// <param name="kind">本次接纳的连接或生成操作类型。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>需要由调用方释放的独占操作租约。</returns>
        internal Operation Begin(RealtimeOperation kind, CancellationToken cancellationToken)
        {
            RequireAvailable();
            cancellationToken.ThrowIfCancellationRequested();
            if (_active != null)
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Another realtime operation is in progress.");

            return _active = new Operation(this, kind, cancellationToken);
        }

        /// <summary>
        /// 取消当前操作并合并清理请求；后续请求可扩大同一清理任务的范围。
        /// </summary>
        /// <param name="scope">本次请求需要的清理范围，后续请求可以扩大该范围。</param>
        /// <param name="fatalError">需要在清理后报告的致命错误，没有时为 null。</param>
        /// <param name="begin">安装清理屏障或扩大范围时执行的状态通知。</param>
        /// <param name="cleanup">按给定范围清理资源的异步回调。</param>
        /// <param name="complete">资源清理后的终态通知，可重入请求更大的清理范围。</param>
        /// <returns>涵盖当前操作退出、资源清理及终态通知的共享任务。</returns>
        internal Task TerminateAsync(
            TerminationScope scope,
            XmaxException fatalError,
            Action begin,
            Func<TerminationScope, Task> cleanup,
            Action<TerminationScope, XmaxException> complete)
        {
            RequireThread();
            if (_termination != null)
            {
                if (scope > _scope)
                {
                    _scope = scope;
                    begin();
                }

                if (fatalError != null)
                    _fatalError = fatalError;

                return _termination.Task;
            }

            _scope = scope;
            _fatalError = fatalError;
            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _termination = completion; // 在取消或宿主回调前先安装清理屏障。
            var active = _active;
            active?.Cancel();
            begin();
            _ = FinishTerminationAsync(active, completion, cleanup, complete);

            return completion.Task;
        }

        /// <summary>
        /// 等待操作退出，反复处理扩大的清理范围，最后解除清理屏障。
        /// </summary>
        /// <param name="active">清理开始时仍在执行的操作租约，没有时为 null。</param>
        /// <param name="completion">向所有等待方报告清理结果的共享完成源。</param>
        /// <param name="cleanup">按给定范围清理资源的异步回调。</param>
        /// <param name="complete">资源清理后的终态通知，可重入请求更大的清理范围。</param>
        /// <returns>清理协调任务；结果或异常写入共享完成源。</returns>
        private async Task FinishTerminationAsync(
            Operation active,
            TaskCompletionSource<bool> completion,
            Func<TerminationScope, Task> cleanup,
            Action<TerminationScope, XmaxException> complete)
        {
            try
            {
                if (active != null)
                    await active.Done.Task;

                TerminationScope cleaned;
                do
                {
                    do
                    {
                        cleaned = _scope;
                        await cleanup(cleaned);
                    } while (_scope > cleaned);

                    // 终态通知期间保留屏障，监听器可能请求更大的清理范围。
                    // 扩大的清理也必须完成，才能结束共享任务。
                    complete(cleaned, _fatalError);
                } while (_scope > cleaned);

                completion.TrySetResult(true);
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
            finally
            {
                _termination = null;
                _fatalError = null;
            }
        }

        /// <summary>
        /// 持有单次操作的取消令牌和完成信号，释放时归还协调器占用。
        /// </summary>
        internal sealed class Operation : IDisposable
        {
            private readonly RealtimeCoordinator _owner;
            private readonly CancellationTokenSource _cancellation;

            /// <summary>
            /// 操作租约已释放的完成信号，供清理屏障等待。
            /// </summary>
            internal readonly TaskCompletionSource<bool> Done =
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            /// <summary>
            /// 该租约对应的操作类型。
            /// </summary>
            internal RealtimeOperation Kind { get; }

            /// <summary>
            /// 与调用方取消请求关联的操作令牌。
            /// </summary>
            internal CancellationToken Token => _cancellation.Token;

            /// <summary>
            /// 创建关联调用方取消令牌的操作租约。
            /// </summary>
            /// <param name="owner">负责接纳和回收此操作的协调器。</param>
            /// <param name="kind">本次接纳的连接或生成操作类型。</param>
            /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
            internal Operation(RealtimeCoordinator owner, RealtimeOperation kind, CancellationToken cancellationToken)
            {
                _owner = owner;
                Kind = kind;
                _cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            }

            /// <summary>
            /// 检查操作是否已取消，防止等待或回调后继续提交过期结果。
            /// </summary>
            internal void EnsureCurrent() => Token.ThrowIfCancellationRequested();

            /// <summary>
            /// 向当前操作发出取消请求。
            /// </summary>
            internal void Cancel() => _cancellation.Cancel();

            /// <summary>
            /// 释放取消源和协调器占用，并完成操作退出信号；重复释放无效果。
            /// </summary>
            public void Dispose()
            {
                if (!ReferenceEquals(_owner._active, this))
                    return;

                _owner._active = null;
                _cancellation.Dispose();
                Done.TrySetResult(true);
            }
        }
    }
}
