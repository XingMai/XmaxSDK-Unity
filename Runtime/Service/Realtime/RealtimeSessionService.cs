using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    internal interface IRealtimeSessionService
    {
        Task<XmaxSession> CreateSessionAsync(string model, CancellationToken cancellationToken);
        Task<XmaxSession> HeartbeatSessionAsync(string id, CancellationToken cancellationToken);
        Task CloseSessionAsync(string id, CancellationToken cancellationToken);
    }

    internal sealed class RealtimeSessionService : IRealtimeSessionService
    {
        private readonly IApiService _api;
        internal RealtimeSessionService(IApiService api) { _api = api; }
        public async Task<XmaxSession> CreateSessionAsync(string model, CancellationToken cancellationToken)
        {
            var body = JsonCodec.Encode(new System.Collections.Generic.Dictionary<string, object> { ["model"] = model });
            return ParseSession(await _api.RequestAsync("POST", "/session", body, cancellationToken));
        }
        public async Task<XmaxSession> HeartbeatSessionAsync(string id, CancellationToken cancellationToken)
        {
            return ParseSession(await _api.RequestAsync("PUT", "/session/" + Uri.EscapeDataString(id) + "/heartbeat", null, cancellationToken));
        }
        public async Task CloseSessionAsync(string id, CancellationToken cancellationToken)
        {
            await _api.RequestAsync("DELETE", "/session/" + Uri.EscapeDataString(id), null, cancellationToken);
        }
        internal static XmaxSession ParseSession(JsonValue data)
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

            JsonValue modelExtra = null;
            if (data.ContainsKey("modelExtra"))
            {
                var raw = data["modelExtra"];
                if (raw != null && raw.IsString)
                {
                    try
                    {
                        modelExtra = JsonCodec.Decode((string)raw);
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
            var info = session?.JoinInfo;
            if (info == null || string.IsNullOrEmpty(info.AppId) || string.IsNullOrEmpty(info.RoomId) ||
                string.IsNullOrEmpty(info.UserId) || string.IsNullOrEmpty(info.Token))
            {
                throw new XmaxException(
                    XmaxErrorCode.SessionError,
                    "Session does not contain complete RTC join information.");
            }
            return info;
        }

        private static string ReadString(JsonValue data, string key)
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

    }
}
