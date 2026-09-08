using System;

namespace Xmax.SDK
{
    internal static class ApiLogger
    {
        internal static void Response(string method, string path, long status, long bytes, long milliseconds)
            => XmaxLogger.Info("API", () => $"{method} {Route(path)} status={status} duration={milliseconds}ms bytes={bytes}");
        internal static void Failure(string method, string path, Exception exception, long milliseconds)
            => XmaxLogger.Error("API", () => $"{method} {Route(path)} failed={XmaxLogger.ErrorCode(exception)} duration={milliseconds}ms");
        private static string Route(string path)
        {
            // Only emit known route templates, never session IDs or URL query parameters.
            var route = (path ?? string.Empty).Split('?')[0];
            if (route == "/session") return route;
            if (route.StartsWith("/session/", StringComparison.Ordinal))
                return route.EndsWith("/heartbeat", StringComparison.Ordinal) ? "/session/{id}/heartbeat" : "/session/{id}";
            return "<route>";
        }
    }
}
