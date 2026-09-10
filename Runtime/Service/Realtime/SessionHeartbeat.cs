using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 管理服务端会话心跳循环，停止时等待循环退出，避免过期响应影响新连接。
    /// </summary>
    internal sealed class SessionHeartbeat
    {
        // 会话服务与心跳调度依赖。
        private readonly IRealtimeSessionService _sessions;
        private readonly Func<CancellationToken, Task> _delay;

        // 当前心跳循环及其取消源。
        private CancellationTokenSource _cancellation;
        private Task _loop = Task.CompletedTask;

        /// <summary>
        /// 未取消的心跳请求失败或会话不再 ACTIVE 时触发。
        /// </summary>
        internal event Action<XmaxException> Failed;

        /// <summary>
        /// 注入会话服务及可选的心跳间隔等待函数。
        /// </summary>
        /// <param name="sessions">实时会话创建、续期和关闭服务。</param>
        /// <param name="delay">心跳间隔等待函数；为空时按约 10 秒间隔执行。</param>
        internal SessionHeartbeat(IRealtimeSessionService sessions, Func<CancellationToken, Task> delay = null)
        {
            _sessions = sessions;
            _delay = delay ?? (token => Task.Delay(10000, token));
        }

        /// <summary>
        /// 启动指定会话的心跳；上一轮未停止时拒绝启动。
        /// </summary>
        /// <param name="sessionId">需要处理的服务端会话标识。</param>
        internal void Start(string sessionId)
        {
            if (_cancellation != null)
                throw new InvalidOperationException("Stop the previous heartbeat first.");

            _cancellation = new CancellationTokenSource();
            _loop = RunAsync(sessionId, _cancellation.Token);
        }

        /// <summary>
        /// 取消心跳并等待当前循环退出后释放取消源。
        /// </summary>
        /// <returns>心跳循环已停止的任务。</returns>
        internal async Task StopAsync()
        {
            var cancellation = _cancellation;
            if (cancellation == null)
                return;

            cancellation.Cancel();
            await _loop;
            cancellation.Dispose();
            if (ReferenceEquals(_cancellation, cancellation))
                _cancellation = null;
        }

        /// <summary>
        /// 周期续期会话并检查活动状态，取消后忽略迟到的响应和错误。
        /// </summary>
        /// <param name="sessionId">需要处理的服务端会话标识。</param>
        /// <param name="token">当前操作或循环的取消令牌。</param>
        /// <returns>心跳循环退出后完成的任务。</returns>
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
                    token.ThrowIfCancellationRequested(); // 取消后不再用迟到的心跳响应影响新连接。
                    if (!string.IsNullOrEmpty(session.Status) && session.Status != "ACTIVE")
                        throw new XmaxException(
                            XmaxErrorCode.SessionError,
                            "Session is no longer active: " + session.Status);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                if (!token.IsCancellationRequested)
                    EventDispatch.Raise(Failed, RealtimeErrorHandler.Wrap(exception, XmaxErrorSeverity.Fatal));
            }
        }
    }
}
