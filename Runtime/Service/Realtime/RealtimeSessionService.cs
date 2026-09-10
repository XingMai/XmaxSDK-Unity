using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 实时会话创建、心跳和关闭的服务层抽象。
    /// </summary>
    internal interface IRealtimeSessionService
    {
        /// <summary>
        /// 向服务端创建指定模型的实时会话。
        /// </summary>
        /// <param name="model">服务端实时模型名称，例如 x2.0。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>包含会话标识及可选 RTC 入房扩展数据的会话。</returns>
        Task<XmaxSession> CreateSessionAsync(string model, CancellationToken cancellationToken);

        /// <summary>
        /// 续期指定实时会话并读取其最新状态。
        /// </summary>
        /// <param name="id">服务端会话唯一标识。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>服务端返回的会话状态。</returns>
        Task<XmaxSession> HeartbeatSessionAsync(string id, CancellationToken cancellationToken);

        /// <summary>
        /// 请求关闭指定服务端会话，HTTP 失败通过任务异常返回。
        /// </summary>
        /// <param name="id">服务端会话唯一标识。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>关闭请求完成的任务。</returns>
        Task CloseSessionAsync(string id, CancellationToken cancellationToken);
    }

    /// <summary>
    /// 将实时会话 API 请求与服务端响应转换为内部会话模型。
    /// </summary>
    internal sealed class RealtimeSessionService : IRealtimeSessionService
    {
        private readonly IApiService _api;

        /// <summary>
        /// 注入共享 HTTP 服务。
        /// </summary>
        /// <param name="api">共享的 HTTP 请求和响应解析服务。</param>
        internal RealtimeSessionService(IApiService api)
        {
            _api = api;
        }

        /// <summary>
        /// 向服务端创建指定模型的实时会话。
        /// </summary>
        /// <param name="model">服务端实时模型名称，例如 x2.0。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>包含会话标识及可选 RTC 入房扩展数据的会话。</returns>
        public async Task<XmaxSession> CreateSessionAsync(string model, CancellationToken cancellationToken)
        {
            var body = JsonCodec.Encode(new System.Collections.Generic.Dictionary<string, object> { ["model"] = model });

            return ParseSession(await _api.RequestAsync("POST", "/session", body, cancellationToken));
        }

        /// <summary>
        /// 续期指定实时会话并读取其最新状态。
        /// </summary>
        /// <param name="id">服务端会话唯一标识。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>服务端返回的会话状态。</returns>
        public async Task<XmaxSession> HeartbeatSessionAsync(string id, CancellationToken cancellationToken)
        {
            return ParseSession(await _api.RequestAsync(
                    "PUT",
                    "/session/" + Uri.EscapeDataString(id) + "/heartbeat",
                    null,
                    cancellationToken));
        }

        /// <summary>
        /// 请求关闭指定服务端会话，HTTP 失败通过任务异常返回。
        /// </summary>
        /// <param name="id">服务端会话唯一标识。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>关闭请求完成的任务。</returns>
        public async Task CloseSessionAsync(string id, CancellationToken cancellationToken)
        {
            await _api.RequestAsync("DELETE", "/session/" + Uri.EscapeDataString(id), null, cancellationToken);
        }

        /// <summary>
        /// 解析会话及字符串或对象形式的 modelExtra 扩展数据。
        /// </summary>
        /// <param name="data">待解析或转换的 SDK 内部 JSON 值。</param>
        /// <returns>具有非空会话标识和可选 RTC 入房信息的会话。</returns>
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

        /// <summary>
        /// 校验会话包含完整的应用、房间、用户和令牌信息。
        /// </summary>
        /// <param name="session">服务端实时会话及其 RTC 入房信息。</param>
        /// <returns>可用于 RTC 入房的完整信息。</returns>
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

        /// <summary>
        /// 安全读取对象中的字符串，去除首尾空白并将空串视为缺失。
        /// </summary>
        /// <param name="data">待解析或转换的 SDK 内部 JSON 值。</param>
        /// <param name="key">需要读取或检查的 JSON 对象成员名称。</param>
        /// <returns>规范化的字符串；键缺失、类型不符或内容为空时为 null。</returns>
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
