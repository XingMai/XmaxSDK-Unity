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

    internal readonly struct XmaxLogger
    {
        internal static readonly XmaxLogger Realtime = new XmaxLogger("Realtime");
        internal static readonly XmaxLogger Rtc = new XmaxLogger("RTC");
        internal static readonly XmaxLogger Media = new XmaxLogger("Media");
        internal static readonly XmaxLogger Api = new XmaxLogger("API");
        internal static readonly XmaxLogger Storage = new XmaxLogger("Storage");
        internal static readonly XmaxLogger Room = new XmaxLogger("Room");
        internal static readonly XmaxLogger Stream = new XmaxLogger("Stream");
        internal static readonly XmaxLogger Render = new XmaxLogger("Render");
        internal static readonly XmaxLogger Interaction = new XmaxLogger("Interaction");
        internal static readonly XmaxLogger Permission = new XmaxLogger("Permission");
        internal static readonly XmaxLogger Timing = new XmaxLogger("Timing");

        private readonly string _category;
        private static int _options;
        private XmaxLogger(string category) { _category = category; }
        internal static void Configure(XmaxLoggerOption options) => Volatile.Write(ref _options, (int)options);
        internal static bool IsEnabled(XmaxLoggerOption option) => option != XmaxLoggerOption.None &&
            (Volatile.Read(ref _options) & (int)option) == (int)option;

        // Call sites log fixed descriptions and numeric diagnostics, never credentials,
        // prompts, response bodies, or arbitrary exception messages.
        internal void Debug(Func<string> message, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Write(LogType.Log, message, option);
        internal void Info(Func<string> message, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Write(LogType.Log, message, option);
        internal void Warn(Func<string> message, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Write(LogType.Warning, message, option);
        internal void Error(Func<string> message, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Write(LogType.Error, message, option);
        internal void Failure(Exception exception, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Error(() => "Operation failed: " + ErrorCode(exception), option);
        internal static string ErrorCode(Exception exception) => exception is XmaxException known ? known.Code.ToString() :
            exception is OperationCanceledException ? "Cancelled" : exception.GetType().Name;
        internal string FormattedMessage(string message)
        {
            var prefix = "[Xmax][" + _category + "] ";
            return prefix + message.Replace("\n", "\n" + prefix);
        }

        private void Write(LogType level, Func<string> message, XmaxLoggerOption option)
        {
            if (!IsEnabled(option)) return;
            var formatted = FormattedMessage(message());
            // Diagnostics must not alter lifecycle behavior if a host log handler throws.
            try
            {
                if (level == LogType.Error) UnityEngine.Debug.LogError(formatted);
                else if (level == LogType.Warning) UnityEngine.Debug.LogWarning(formatted);
                else UnityEngine.Debug.Log(formatted);
            }
            catch { }
        }
    }
}
