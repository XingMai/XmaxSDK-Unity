using System;

namespace Xmax.SDK
{
    public sealed class XmaxConfiguration
    {
        public const string DefaultBaseUrl = "https://cloud.xmax.22duck.cn/open/api/v1";

        public string ApiKey { get; }
        public string BaseUrl { get; }

        public XmaxConfiguration(string apiKey, string baseUrl = DefaultBaseUrl)
        {
            ApiKey = (apiKey ?? string.Empty).Trim();
            BaseUrl = (baseUrl ?? string.Empty).Trim().TrimEnd('/');
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
