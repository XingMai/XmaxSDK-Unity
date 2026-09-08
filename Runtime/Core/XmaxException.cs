using System;

namespace Xmax.SDK
{
    public enum XmaxErrorSeverity { Recoverable, Fatal }

    public enum XmaxErrorCode
    {
        InvalidApiKey,
        InvalidBaseUrl,
        InvalidConfiguration,
        NetworkError,
        ApiError,
        SessionError,
        RtcError,
        Timeout,
        NotSupported
    }

    public sealed class XmaxException : Exception
    {
        public XmaxErrorCode Code { get; }
        public int? ApiCode { get; }
        public long? HttpStatus { get; }
        public XmaxErrorSeverity Severity { get; }

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
