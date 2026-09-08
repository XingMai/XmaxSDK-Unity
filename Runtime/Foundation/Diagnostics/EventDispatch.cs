using System;

namespace Xmax.SDK
{
    internal static class EventDispatch
    {
        // Application callbacks must not interrupt SDK state transitions or resource cleanup.
        internal static void Raise<T>(Action<T> handlers, T value, Func<bool> isCurrent = null)
        {
            if (handlers == null) return;
            foreach (Action<T> handler in handlers.GetInvocationList())
            {
                if (isCurrent != null && !isCurrent()) break;
                try { handler(value); }
                catch (Exception exception) { UnityEngine.Debug.LogException(exception); }
            }
        }
    }
}
