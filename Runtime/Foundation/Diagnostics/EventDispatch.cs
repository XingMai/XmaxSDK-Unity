using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 隔离宿主订阅者异常，并支持在事件重入后中止过期通知。
    /// </summary>
    internal static class EventDispatch
    {
        /// <summary>
        /// 依次通知订阅者；单个订阅者抛出的异常只记录日志，不阻断其他有效回调。
        /// </summary>
        /// <typeparam name="T">事件数据的类型。</typeparam>
        /// <param name="handlers">待通知的多播委托；为 null 时不执行回调。</param>
        /// <param name="value">向每个有效订阅者传递的事件数据。</param>
        /// <param name="isCurrent">可选有效性检查，每次回调前执行；返回 false 时停止后续分发。</param>
        internal static void Raise<T>(Action<T> handlers, T value, Func<bool> isCurrent = null)
        {
            if (handlers == null)
                return;

            foreach (Action<T> handler in handlers.GetInvocationList())
            {
                if (isCurrent != null && !isCurrent())
                    break;

                try
                {
                    handler(value);
                }
                catch (Exception exception)
                {
                    XmaxLogger.Realtime.Failure(exception);
                }
            }
        }
    }
}
