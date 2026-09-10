using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 仅记录 HTTP 方法、脱敏路由和数值统计，避免输出请求与响应正文。
    /// </summary>
    internal static class ApiLogger
    {
        /// <summary>
        /// 记录请求完成后的状态码、响应字节数和耗时。
        /// </summary>
        /// <param name="method">HTTP 方法，例如 POST、PUT 或 DELETE。</param>
        /// <param name="path">相对于 API 根地址的请求路径。</param>
        /// <param name="status">HTTP 响应状态码。</param>
        /// <param name="bytes">收到的响应字节数。</param>
        /// <param name="milliseconds">等待时限，单位为毫秒。</param>
        internal static void Response(string method, string path, long status, long bytes, long milliseconds)
            => XmaxLogger.Api.Info(() => $"{method} {Route(path)} status={status} duration={milliseconds}ms bytes={bytes}");

        /// <summary>
        /// 记录请求失败的脱敏路由、异常分类和耗时。
        /// </summary>
        /// <param name="method">HTTP 方法，例如 POST、PUT 或 DELETE。</param>
        /// <param name="path">相对于 API 根地址的请求路径。</param>
        /// <param name="exception">需要处理的原始异常。</param>
        /// <param name="milliseconds">等待时限，单位为毫秒。</param>
        internal static void Failure(string method, string path, Exception exception, long milliseconds)
            => XmaxLogger.Api.Error(() => $"{method} {Route(path)} failed={XmaxLogger.ErrorCode(exception)} duration={milliseconds}ms");

        /// <summary>
        /// 移除查询参数并将会话标识替换为路由占位符。
        /// </summary>
        /// <param name="path">相对于 API 根地址的请求路径。</param>
        /// <returns>固定或脱敏后的路由名称，未知路由返回占位文本。</returns>
        private static string Route(string path)
        {
            // 只输出已知路由模板，避免带出会话标识或查询参数。
            var route = (path ?? string.Empty).Split('?')[0];
            if (route == "/session")
                return route;

            if (route.StartsWith("/session/", StringComparison.Ordinal))
                return route.EndsWith("/heartbeat", StringComparison.Ordinal) ? "/session/{id}/heartbeat" : "/session/{id}";

            return "<route>";
        }
    }
}
