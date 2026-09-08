using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Xmax.SDK
{
    internal interface IApiService
    {
        Task<JsonValue> RequestAsync(string method, string path, string body, CancellationToken cancellationToken);
    }
    internal sealed class ApiService : IApiService
    {
        private const int TimeoutSeconds = 15;
        private readonly XmaxConfiguration _configuration;
        internal ApiService(XmaxConfiguration configuration) { _configuration = configuration; }
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
                    ApiLogger.Response(method, path, request.responseCode, (long)request.downloadedBytes, elapsed.ElapsedMilliseconds);
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

        internal static JsonValue ParseEnvelope(string text, long status, bool connectionError, string error, bool allowEmpty = false)
        {
            if (connectionError) throw new XmaxException(XmaxErrorCode.NetworkError, error ?? "HTTP request failed.", httpStatus: status);
            if (allowEmpty && status >= 200 && status < 300 && string.IsNullOrWhiteSpace(text)) return null;
            JsonValue envelope;
            try { envelope = JsonCodec.Decode(text); }
            catch (Exception exception) { throw new XmaxException(XmaxErrorCode.ApiError, "Server returned invalid JSON.", httpStatus: status, innerException: exception); }
            if (envelope == null || !envelope.IsObject)
                throw new XmaxException(XmaxErrorCode.ApiError, "Invalid API response envelope.", httpStatus: status);
            var success = envelope.ContainsKey("success") && envelope["success"] != null && envelope["success"].IsBoolean && (bool)envelope["success"];
            if (status >= 200 && status < 300 && success && envelope.ContainsKey("data")) return envelope["data"];
            int? code = envelope.ContainsKey("code") && envelope["code"] != null && envelope["code"].IsInt ? (int)envelope["code"] : (int?)null;
            var message = envelope.ContainsKey("message") && envelope["message"] != null && envelope["message"].IsString ? (string)envelope["message"] : error;
            throw new XmaxException(XmaxErrorCode.ApiError, message ?? "Xmax API request failed.", code, status);
        }
    }
}
