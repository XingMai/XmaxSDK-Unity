using System;

namespace Xmax.SDK
{
    /// <summary>
    /// SDK 的服务端环境。
    /// </summary>
    public enum XmaxEnvironment
    {
        /// <summary>
        /// 中国区服务环境。
        /// </summary>
        China,

        /// <summary>
        /// 全球服务环境。
        /// </summary>
        Global
    }

    /// <summary>
    /// 客户端的 API 地址、凭证和日志配置；构造时规范化，在线请求前校验。
    /// </summary>
    public sealed class XmaxConfiguration
    {
        /// <summary>
        /// 默认中国区 API 根地址。
        /// </summary>
        public const string DefaultBaseUrl = "https://cloud.xmax.22duck.cn/open/api/v1";

        /// <summary>
        /// 全球服务 API 根地址。
        /// </summary>
        public const string GlobalBaseUrl = "https://api.xmax.cloud/open/api/v1";

        /// <summary>
        /// 去除首尾空白后的 API key，仅用于在线请求鉴权。
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// 去除首尾空白和末尾斜杠后的 API 根地址。
        /// </summary>
        public string BaseUrl { get; }

        /// <summary>
        /// 创建客户端时应用的进程级日志过滤选项。
        /// </summary>
        public XmaxLoggerOption LoggerOptions { get; }

        /// <summary>
        /// 保存并规范化客户端配置；不在构造时校验在线凭证。
        /// </summary>
        /// <param name="apiKey">在线请求使用的 API key；仅本地预览时可为空。</param>
        /// <param name="environment">服务端区域，用于选择对应的 API 根地址。</param>
        /// <param name="loggerOptions">全局日志过滤选项，默认关闭，由创建客户端时应用。</param>
        public XmaxConfiguration(
            string apiKey,
            XmaxEnvironment environment,
            XmaxLoggerOption loggerOptions = XmaxLoggerOption.None)
            : this(
                apiKey,
                environment == XmaxEnvironment.China ? DefaultBaseUrl :
                environment == XmaxEnvironment.Global ? GlobalBaseUrl :
                throw new ArgumentOutOfRangeException(nameof(environment)),
                loggerOptions)
        {
        }

        /// <summary>
        /// 保存并规范化客户端配置；不在构造时校验在线凭证。
        /// </summary>
        /// <param name="apiKey">在线请求使用的 API key；仅本地预览时可为空。</param>
        /// <param name="baseUrl">HTTP API 根地址，在线请求时校验为绝对 HTTP 或 HTTPS URL。</param>
        /// <param name="loggerOptions">全局日志过滤选项，默认关闭，由创建客户端时应用。</param>
        public XmaxConfiguration(
            string apiKey,
            string baseUrl = DefaultBaseUrl,
            XmaxLoggerOption loggerOptions = XmaxLoggerOption.None)
        {
            ApiKey = (apiKey ?? string.Empty).Trim();
            BaseUrl = (baseUrl ?? string.Empty).Trim().TrimEnd('/');
            LoggerOptions = loggerOptions;
        }

        /// <summary>
        /// 校验 API key 非空且服务地址为绝对 HTTP 或 HTTPS URL。
        /// </summary>
        internal void Validate()
        {
            if (ApiKey.Length == 0)
            {
                throw new XmaxException(XmaxErrorCode.InvalidApiKey, "API key cannot be empty.");
            }

            if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
            {
                throw new XmaxException(XmaxErrorCode.InvalidBaseUrl, $"Invalid base URL: {BaseUrl}");
            }
        }
    }
}
