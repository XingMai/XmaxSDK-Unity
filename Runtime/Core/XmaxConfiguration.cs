using System;

namespace Xmax.SDK
{
    public enum XmaxEnvironment { China, Global }

    public sealed class XmaxConfiguration
    {
        public const string DefaultBaseUrl = "https://cloud.xmax.22duck.cn/open/api/v1";
        public const string GlobalBaseUrl = "https://api.xmax.cloud/open/api/v1";

        public string ApiKey { get; }
        public string BaseUrl { get; }
        public XmaxLoggerOption LoggerOptions { get; }

        public XmaxConfiguration(string apiKey, XmaxEnvironment environment, XmaxLoggerOption loggerOptions = XmaxLoggerOption.None)
            : this(apiKey, environment == XmaxEnvironment.China ? DefaultBaseUrl :
                environment == XmaxEnvironment.Global ? GlobalBaseUrl : throw new ArgumentOutOfRangeException(nameof(environment)), loggerOptions) { }

        public XmaxConfiguration(string apiKey, string baseUrl = DefaultBaseUrl, XmaxLoggerOption loggerOptions = XmaxLoggerOption.None)
        {
            ApiKey = (apiKey ?? string.Empty).Trim();
            BaseUrl = (baseUrl ?? string.Empty).Trim().TrimEnd('/');
            LoggerOptions = loggerOptions;
        }

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
