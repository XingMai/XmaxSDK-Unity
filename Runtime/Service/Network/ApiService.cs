using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Xmax.SDK
{
    /// <summary>
    /// 提供统一 HTTP 请求和 API 响应信封解析的内部抽象。
    /// </summary>
    internal interface IApiService
    {
        /// <summary>
        /// 校验配置并发送 JSON 请求，支持取消和响应信封校验。
        /// </summary>
        /// <param name="method">HTTP 方法，例如 POST、PUT 或 DELETE。</param>
        /// <param name="path">相对于 API 根地址的请求路径。</param>
        /// <param name="body">JSON 请求正文；null 表示没有请求正文。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>成功响应中的 data；允许空正文的成功 DELETE 请求返回 null。</returns>
        Task<JsonValue> RequestAsync(string method, string path, string body, CancellationToken cancellationToken);
    }

    /// <summary>
    /// 使用 UnityWebRequest 发送已鉴权的 JSON 请求，并统一取消和错误处理。
    /// </summary>
    internal sealed class ApiService : IApiService
    {
        // HTTP 请求时限。
        private const int TimeoutSeconds = 15;

        // 共享客户端配置。
        private readonly XmaxConfiguration _configuration;

        /// <summary>
        /// 保存 HTTP 地址、凭证及客户端配置。
        /// </summary>
        /// <param name="configuration">客户端的不可变服务地址、凭证和日志配置。</param>
        internal ApiService(XmaxConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 校验配置并发送 JSON 请求，支持取消和响应信封校验。
        /// </summary>
        /// <param name="method">HTTP 方法，例如 POST、PUT 或 DELETE。</param>
        /// <param name="path">相对于 API 根地址的请求路径。</param>
        /// <param name="body">JSON 请求正文；null 表示没有请求正文。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>成功响应中的 data；允许空正文的成功 DELETE 请求返回 null。</returns>
        public async Task<JsonValue> RequestAsync(
            string method,
            string path,
            string body,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _configuration.Validate();
            var elapsed = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using (var request = new UnityWebRequest(_configuration.BaseUrl + path, method))
                {
                    request.downloadHandler = new DownloadHandlerBuffer();
                    if (body != null)
                    {
                        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
                    }

                    request.timeout = TimeoutSeconds;
                    request.SetRequestHeader("Accept", "application/json");
                    request.SetRequestHeader("Content-Type", "application/json");
                    request.SetRequestHeader("X-Api-Key", _configuration.ApiKey);

                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            request.Abort();
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        await Task.Yield();
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    ApiLogger.Response(
                        method,
                        path,
                        request.responseCode,
                        (long)request.downloadedBytes,
                        elapsed.ElapsedMilliseconds);

                    return ParseEnvelope(request.downloadHandler?.text ?? string.Empty,
                        request.responseCode, request.result == UnityWebRequest.Result.ConnectionError,
                        request.error, method == "DELETE");
                }
            }
            catch (Exception exception)
            {
                ApiLogger.Failure(method, path, exception, elapsed.ElapsedMilliseconds);

                throw;
            }
        }

        /// <summary>
        /// 解析 API 响应信封，将网络失败、业务失败及无效 JSON 转换为 SDK 错误。
        /// </summary>
        /// <param name="text">需要解析的 JSON 文本。</param>
        /// <param name="status">HTTP 响应状态码。</param>
        /// <param name="connectionError">是否发生 HTTP 传输层连接失败。</param>
        /// <param name="error">HTTP 传输提供的错误说明，可为空。</param>
        /// <param name="allowEmpty">是否接受成功 HTTP 状态下的空响应正文。</param>
        /// <returns>成功信封中的 data，或允许空成功响应时的 null。</returns>
        internal static JsonValue ParseEnvelope(
            string text,
            long status,
            bool connectionError,
            string error,
            bool allowEmpty = false)
        {
            if (connectionError)
                throw new XmaxException(XmaxErrorCode.NetworkError, error ?? "HTTP request failed.", httpStatus: status);

            if (allowEmpty && status >= 200 && status < 300 && string.IsNullOrWhiteSpace(text))
                return null;

            JsonValue envelope;

            try
            {
                envelope = JsonCodec.Decode(text);
            }
            catch (Exception exception)
            {
                throw new XmaxException(
                    XmaxErrorCode.ApiError,
                    "Server returned invalid JSON.",
                    httpStatus: status,
                    innerException: exception);
            }

            if (envelope == null || !envelope.IsObject)
                throw new XmaxException(XmaxErrorCode.ApiError, "Invalid API response envelope.", httpStatus: status);

            var success = envelope.ContainsKey("success") &&
                envelope["success"] != null &&
                envelope["success"].IsBoolean &&
                (bool)envelope["success"];
            if (status >= 200 && status < 300 && success && envelope.ContainsKey("data"))
                return envelope["data"];

            int? code = envelope.ContainsKey("code") && envelope["code"] != null && envelope["code"].IsInt
                ? (int)envelope["code"]
                : (int?)null;
            var message = envelope.ContainsKey("message") && envelope["message"] != null && envelope["message"].IsString
                ? (string)envelope["message"]
                : error;

            throw new XmaxException(XmaxErrorCode.ApiError, message ?? "Xmax API request failed.", code, status);
        }
    }
}
