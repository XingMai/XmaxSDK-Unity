using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 统一异步失败的 SDK 错误类型和严重程度。
    /// </summary>
    internal static class RealtimeErrorHandler
    {
        /// <summary>
        /// 保留已有 SDK 错误的详细信息，并按指定严重程度封装异常。
        /// </summary>
        /// <param name="exception">需要处理的原始异常。</param>
        /// <param name="severity">对外报告的错误严重程度。</param>
        /// <returns>可向上层传递的 SDK 异常。</returns>
        internal static XmaxException Wrap(
            Exception exception,
            XmaxErrorSeverity severity = XmaxErrorSeverity.Recoverable)
        {
            var known = exception as XmaxException;
            if (known != null && known.Severity == severity)
                return known;

            return new XmaxException(known?.Code ?? XmaxErrorCode.RtcError, exception.Message,
                known?.ApiCode, known?.HttpStatus, exception, severity);
        }
    }
}
