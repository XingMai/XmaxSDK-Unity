using System;

namespace Xmax.SDK
{
    internal static class RealtimeErrorHandler
    {
        internal static XmaxException Wrap(Exception exception, XmaxErrorSeverity severity = XmaxErrorSeverity.Recoverable)
        {
            var known = exception as XmaxException;
            if (known != null && known.Severity == severity) return known;
            return new XmaxException(known?.Code ?? XmaxErrorCode.RtcError, exception.Message,
                known?.ApiCode, known?.HttpStatus, exception, severity);
        }
    }
}
