using System;
using System.Threading;
using UnityEngine;

namespace Xmax.SDK
{
    [Flags]
    public enum XmaxLoggerOption
    {
        None = 0,
        Business = 1 << 0,
        Performance = 1 << 1,
        All = Business | Performance
    }

    internal static class XmaxLogger
    {
        private static int _options;
        internal static void Configure(XmaxLoggerOption options) => Volatile.Write(ref _options, (int)options);
        internal static bool IsEnabled(XmaxLoggerOption option) => option != XmaxLoggerOption.None &&
            (Volatile.Read(ref _options) & (int)option) == (int)option;

        // Call sites log fixed descriptions and numeric diagnostics, never credentials,
        // prompts, response bodies, or arbitrary exception messages.
        internal static void Info(string category, Func<string> message, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Write(LogType.Log, category, message, option);
        internal static void Warning(string category, string message)
            => Write(LogType.Warning, category, () => message, XmaxLoggerOption.Business);
        internal static void Error(string category, Func<string> message)
            => Write(LogType.Error, category, message, XmaxLoggerOption.Business);
        internal static void Failure(string category, Exception exception)
            => Error(category, () => "Operation failed: " + ErrorCode(exception));
        internal static string ErrorCode(Exception exception) => exception is XmaxException known ? known.Code.ToString() :
            exception is OperationCanceledException ? "Cancelled" : exception.GetType().Name;
        private static void Write(LogType level, string category, Func<string> message, XmaxLoggerOption option)
        {
            if (!IsEnabled(option)) return;
            var prefix = "[Xmax][" + category + "] ";
            var formatted = prefix + message().Replace("\n", "\n" + prefix);
            // Diagnostics must not alter lifecycle behavior if a host log handler throws.
            try
            {
                if (level == LogType.Error) Debug.LogError(formatted);
                else if (level == LogType.Warning) Debug.LogWarning(formatted);
                else Debug.Log(formatted);
            }
            catch { }
        }
    }
}
