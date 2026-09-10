using System;
using System.Threading;
using UnityEngine;

namespace Xmax.SDK
{
    /// <summary>
    /// 进程级日志过滤选项，可按位组合。
    /// </summary>
    [Flags]
    public enum XmaxLoggerOption
    {
        /// <summary>
        /// 关闭全部 SDK 日志。
        /// </summary>
        None = 0,

        /// <summary>
        /// 启用业务流程和错误诊断日志。
        /// </summary>
        Business = 1 << 0,

        /// <summary>
        /// 启用网络质量和启动耗时等性能日志。
        /// </summary>
        Performance = 1 << 1,

        /// <summary>
        /// 启用业务和性能日志。
        /// </summary>
        All = Business | Performance
    }

    /// <summary>
    /// 带固定分类的惰性日志入口，所有分类共享线程安全的全局过滤配置。
    /// </summary>
    internal readonly struct XmaxLogger
    {
        /// <summary>
        /// 实时生命周期分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Realtime = new XmaxLogger("Realtime");

        /// <summary>
        /// RTC 传输分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Rtc = new XmaxLogger("RTC");

        /// <summary>
        /// 媒体输入分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Media = new XmaxLogger("Media");

        /// <summary>
        /// HTTP API 分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Api = new XmaxLogger("API");

        /// <summary>
        /// 存储分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Storage = new XmaxLogger("Storage");

        /// <summary>
        /// 房间信令分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Room = new XmaxLogger("Room");

        /// <summary>
        /// 流控制分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Stream = new XmaxLogger("Stream");

        /// <summary>
        /// 渲染分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Render = new XmaxLogger("Render");

        /// <summary>
        /// 交互分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Interaction = new XmaxLogger("Interaction");

        /// <summary>
        /// 权限分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Permission = new XmaxLogger("Permission");

        /// <summary>
        /// 启动耗时分类的共享日志入口。
        /// </summary>
        internal static readonly XmaxLogger Timing = new XmaxLogger("Timing");

        // 实例的固定分类。
        private readonly string _category;

        // 全部分类共享的进程级过滤位。
        private static int _options;

        /// <summary>
        /// 创建具有固定分类前缀的日志入口。
        /// </summary>
        /// <param name="category">固定日志分类名称，用于每行日志的前缀。</param>
        private XmaxLogger(string category)
        {
            _category = category;
        }

        /// <summary>
        /// 原子替换全部分类共享的日志过滤选项。
        /// </summary>
        /// <param name="options">新的进程级日志过滤位，None 表示关闭全部日志。</param>
        internal static void Configure(XmaxLoggerOption options) => Volatile.Write(ref _options, (int)options);

        /// <summary>
        /// 检查请求的非空日志选项是否全部启用。
        /// </summary>
        /// <param name="option">本条日志所需的过滤位，所有位均启用时才计算并输出消息。</param>
        /// <returns>选项非 None 且所有位均已启用时为 true。</returns>
        internal static bool IsEnabled(XmaxLoggerOption option) => option != XmaxLoggerOption.None &&
            (Volatile.Read(ref _options) & (int)option) == (int)option;

        /// <summary>
        /// 按过滤选项惰性输出调试日志，映射到 Unity 普通日志级别。
        /// </summary>
        /// <param name="message">惰性消息工厂，仅在日志启用时调用；不得包含凭证、提示词或响应正文。</param>
        /// <param name="option">本条日志所需的过滤位，所有位均启用时才计算并输出消息。</param>
        internal void Debug(Func<string> message, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Write(LogType.Log, message, option);

        /// <summary>
        /// 按过滤选项惰性输出普通信息日志。
        /// </summary>
        /// <param name="message">惰性消息工厂，仅在日志启用时调用；不得包含凭证、提示词或响应正文。</param>
        /// <param name="option">本条日志所需的过滤位，所有位均启用时才计算并输出消息。</param>
        internal void Info(Func<string> message, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Write(LogType.Log, message, option);

        /// <summary>
        /// 按过滤选项惰性输出 Unity 警告日志。
        /// </summary>
        /// <param name="message">惰性消息工厂，仅在日志启用时调用；不得包含凭证、提示词或响应正文。</param>
        /// <param name="option">本条日志所需的过滤位，所有位均启用时才计算并输出消息。</param>
        internal void Warn(Func<string> message, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Write(LogType.Warning, message, option);

        /// <summary>
        /// 按过滤选项惰性输出 Unity 错误日志。
        /// </summary>
        /// <param name="message">惰性消息工厂，仅在日志启用时调用；不得包含凭证、提示词或响应正文。</param>
        /// <param name="option">本条日志所需的过滤位，所有位均启用时才计算并输出消息。</param>
        internal void Error(Func<string> message, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Write(LogType.Error, message, option);

        /// <summary>
        /// 只记录异常分类，避免输出异常消息中的凭证或业务内容。
        /// </summary>
        /// <param name="exception">需要处理的原始异常。</param>
        /// <param name="option">本条日志所需的过滤位，所有位均启用时才计算并输出消息。</param>
        internal void Failure(Exception exception, XmaxLoggerOption option = XmaxLoggerOption.Business)
            => Error(() => "Operation failed: " + ErrorCode(exception), option);

        /// <summary>
        /// 提取可安全记录的 SDK 错误码、取消标识或异常类型名称。
        /// </summary>
        /// <param name="exception">需要处理的原始异常。</param>
        /// <returns>不包含原始异常消息的诊断标识。</returns>
        internal static string ErrorCode(Exception exception) => exception is XmaxException known ? known.Code.ToString() :
            exception is OperationCanceledException ? "Cancelled" : exception.GetType().Name;

        /// <summary>
        /// 为消息的每一行补齐 SDK 名称和分类前缀。
        /// </summary>
        /// <param name="message">已经脱敏的日志文本。</param>
        /// <returns>每行均带有分类前缀的日志文本。</returns>
        internal string FormattedMessage(string message)
        {
            var prefix = "[Xmax][" + _category + "] ";

            return prefix + message.Replace("\n", "\n" + prefix);
        }

        /// <summary>
        /// 过滤日志后才计算消息，并隔离宿主日志处理器抛出的异常。
        /// </summary>
        /// <param name="level">映射到 Unity 日志系统的输出级别。</param>
        /// <param name="message">惰性消息工厂，仅在日志启用时调用；不得包含凭证、提示词或响应正文。</param>
        /// <param name="option">本条日志所需的过滤位，所有位均启用时才计算并输出消息。</param>
        private void Write(LogType level, Func<string> message, XmaxLoggerOption option)
        {
            if (!IsEnabled(option))
                return;

            var formatted = FormattedMessage(message());

            // 宿主日志处理器的异常不能中断 SDK 的状态推进或清理。
            try
            {
                if (level == LogType.Error)
                    UnityEngine.Debug.LogError(formatted);
                else if (level == LogType.Warning)
                    UnityEngine.Debug.LogWarning(formatted);
                else
                    UnityEngine.Debug.Log(formatted);
            }
            catch
            {
            }
        }
    }
}
