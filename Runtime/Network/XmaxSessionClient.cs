using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UNBridgeLib.LitJson;
using UnityEngine.Networking;

namespace Xmax.SDK
{
    internal sealed class XmaxSession
    {
        internal string SessionUid;
        internal string UserUid;
        internal string Status;
        internal RtcJoinInfo JoinInfo;
    }

    internal sealed class RtcJoinInfo
    {
        internal string AppId;
        internal string RoomId;
        internal string UserId;
        internal string Token;
        internal string BotName;
    }

    internal sealed class XmaxSessionClient
    {
        private const int TimeoutSeconds = 15;
        private readonly XmaxConfiguration _configuration;

        internal XmaxSessionClient(XmaxConfiguration configuration)
        {
            _configuration = configuration;
        }

        internal async Task<XmaxSession> CreateSessionAsync(string model, CancellationToken cancellationToken)
        {
            var body = JsonMapper.ToJson(new System.Collections.Generic.Dictionary<string, object>
            {
                ["model"] = model
            });
            var data = await RequestAsync("POST", "/session", body, cancellationToken);
            return ParseSession(data);
        }

        internal async Task<XmaxSession> HeartbeatSessionAsync(string sessionUid, CancellationToken cancellationToken)
        {
            var data = await RequestAsync(
                "PUT",
                $"/session/{UnityWebRequest.EscapeURL(sessionUid)}/heartbeat",
                null,
                cancellationToken);
            return ParseSession(data);
        }

        internal async Task CloseSessionAsync(string sessionUid, CancellationToken cancellationToken)
        {
            await RequestAsync(
                "DELETE",
                $"/session/{UnityWebRequest.EscapeURL(sessionUid)}",
                null,
                cancellationToken);
        }

        private async Task<JsonData> RequestAsync(
            string method,
            string path,
            string body,
            CancellationToken cancellationToken)
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
                var text = request.downloadHandler?.text ?? string.Empty;
                JsonData envelope;
                try
                {
                    envelope = JsonMapper.ToObject(text);
                }
                catch (Exception exception)
                {
                    if (request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        throw new XmaxException(
                            XmaxErrorCode.NetworkError,
                            $"HTTP request failed: {request.error}",
                            null,
                            request.responseCode,
                            exception);
                    }
                    throw new XmaxException(
                        XmaxErrorCode.ApiError,
                        "Server returned invalid JSON.",
                        null,
                        request.responseCode,
                        exception);
                }

                var success = ReadBoolean(envelope, "success") == true;
                if (request.responseCode >= 200 && request.responseCode < 300 &&
                    success && envelope.IsObject && envelope.ContainsKey("data"))
                {
                    return envelope["data"];
                }

                var errorCode = ReadInteger(envelope, "code");
                var message = ReadString(envelope, "message") ?? request.error ?? "Xmax API request failed.";
                var code = request.result == UnityWebRequest.Result.ConnectionError
                    ? XmaxErrorCode.NetworkError
                    : XmaxErrorCode.ApiError;
                throw new XmaxException(code, message, errorCode, request.responseCode);
            }
        }

        private static XmaxSession ParseSession(JsonData data)
        {
            if (data == null || !data.IsObject)
            {
                throw new XmaxException(XmaxErrorCode.SessionError, "Invalid session response.");
            }

            var sessionUid = ReadString(data, "sessionUid");
            if (string.IsNullOrEmpty(sessionUid))
            {
                throw new XmaxException(XmaxErrorCode.SessionError, "Session response does not contain sessionUid.");
            }

            var session = new XmaxSession
            {
                SessionUid = sessionUid,
                UserUid = ReadString(data, "userUid"),
                Status = ReadString(data, "status")
            };

            JsonData modelExtra = null;
            if (data.ContainsKey("modelExtra"))
            {
                var raw = data["modelExtra"];
                if (raw != null && raw.IsString)
                {
                    try
                    {
                        modelExtra = JsonMapper.ToObject((string)raw);
                    }
                    catch
                    {
                        modelExtra = null;
                    }
                }
                else
                {
                    modelExtra = raw;
                }
            }

            if (modelExtra != null && modelExtra.IsObject)
            {
                session.JoinInfo = new RtcJoinInfo
                {
                    AppId = ReadString(modelExtra, "rtc_app_id"),
                    RoomId = ReadString(modelExtra, "room_id"),
                    UserId = ReadString(modelExtra, "user_id") ?? session.UserUid,
                    Token = ReadString(modelExtra, "room_token"),
                    BotName = ReadString(modelExtra, "bot_name")
                };
            }
            return session;
        }

        internal static RtcJoinInfo RequireJoinInfo(XmaxSession session)
        {
            var info = session.JoinInfo;
            if (info == null || string.IsNullOrEmpty(info.AppId) || string.IsNullOrEmpty(info.RoomId) ||
                string.IsNullOrEmpty(info.UserId) || string.IsNullOrEmpty(info.Token))
            {
                throw new XmaxException(
                    XmaxErrorCode.SessionError,
                    "Session does not contain complete RTC join information.");
            }
            return info;
        }

        private static string ReadString(JsonData data, string key)
        {
            if (data == null || !data.IsObject || !data.ContainsKey(key))
            {
                return null;
            }
            var value = data[key];
            if (value == null || !value.IsString)
            {
                return null;
            }
            var text = ((string)value)?.Trim();
            return string.IsNullOrEmpty(text) ? null : text;
        }

        private static bool? ReadBoolean(JsonData data, string key)
        {
            if (data == null || !data.IsObject || !data.ContainsKey(key) || !data[key].IsBoolean)
            {
                return null;
            }
            return (bool)data[key];
        }

        private static int? ReadInteger(JsonData data, string key)
        {
            if (data == null || !data.IsObject || !data.ContainsKey(key))
            {
                return null;
            }
            var value = data[key];
            if (value.IsInt)
            {
                return (int)value;
            }
            if (value.IsLong)
            {
                var number = (long)value;
                return number >= int.MinValue && number <= int.MaxValue ? (int)number : null;
            }
            return null;
        }
    }
}
