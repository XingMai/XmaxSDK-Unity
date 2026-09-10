using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 错误对当前实时连接的影响程度。
    /// </summary>
    public enum XmaxErrorSeverity
    {
        /// <summary>
        /// 操作失败或清理警告，是否重试由调用方决定。
        /// </summary>
        Recoverable,

        /// <summary>
        /// 当前连接需要清理后重新建立。
        /// </summary>
        Fatal
    }

    /// <summary>
    /// 跨平台 SDK 错误分类。
    /// </summary>
    public enum XmaxErrorCode
    {
        /// <summary>
        /// 在线请求缺少有效 API key。
        /// </summary>
        InvalidApiKey,

        /// <summary>
        /// API 根地址格式无效。
        /// </summary>
        InvalidBaseUrl,

        /// <summary>
        /// 参数、调用顺序或线程不符合约定。
        /// </summary>
        InvalidConfiguration,

        /// <summary>
        /// HTTP 传输层连接失败。
        /// </summary>
        NetworkError,

        /// <summary>
        /// HTTP API 返回错误或响应格式无效。
        /// </summary>
        ApiError,

        /// <summary>
        /// 会话状态或 RTC 入房信息无效。
        /// </summary>
        SessionError,

        /// <summary>
        /// RTC 传输或实时生成操作失败。
        /// </summary>
        RtcError,

        /// <summary>
        /// 等待操作超过规定时限。
        /// </summary>
        Timeout,

        /// <summary>
        /// 当前平台、格式或模型暂不支持该操作。
        /// </summary>
        NotSupported
    }

    /// <summary>
    /// 保留 SDK 分类、服务端错误码、HTTP 状态和原始异常的统一错误。
    /// </summary>
    public sealed class XmaxException : Exception
    {
        /// <summary>
        /// SDK 错误分类。
        /// </summary>
        public XmaxErrorCode Code { get; }

        /// <summary>
        /// 服务端业务错误码；不可用时为 null。
        /// </summary>
        public int? ApiCode { get; }

        /// <summary>
        /// HTTP 响应状态码；未关联 HTTP 响应时为 null。
        /// </summary>
        public long? HttpStatus { get; }

        /// <summary>
        /// 错误是否要求清理当前连接。
        /// </summary>
        public XmaxErrorSeverity Severity { get; }

        /// <summary>
        /// 创建保留服务端诊断信息和原始原因的 SDK 异常。
        /// </summary>
        /// <param name="code">SDK 错误分类。</param>
        /// <param name="message">面向调用方的错误说明。</param>
        /// <param name="apiCode">服务端业务错误码，没有时为 null。</param>
        /// <param name="httpStatus">HTTP 状态码，未关联响应时为 null。</param>
        /// <param name="innerException">造成当前错误的原始异常，可为空。</param>
        /// <param name="severity">对外报告的错误严重程度。</param>
        public XmaxException(
            XmaxErrorCode code,
            string message,
            int? apiCode = null,
            long? httpStatus = null,
            Exception innerException = null,
            XmaxErrorSeverity severity = XmaxErrorSeverity.Recoverable)
            : base(message, innerException)
        {
            Code = code;
            ApiCode = apiCode;
            HttpStatus = httpStatus;
            Severity = severity;
        }
    }
}
