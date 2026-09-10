namespace Xmax.SDK
{
    /// <summary>
    /// 服务端实时会话响应及解析后的 RTC 入房信息。
    /// </summary>
    internal sealed class XmaxSession
    {
        /// <summary>
        /// 服务端会话唯一标识。
        /// </summary>
        internal string SessionUid;

        /// <summary>
        /// 服务端用户标识，可作为 RTC 用户标识的回退值。
        /// </summary>
        internal string UserUid;

        /// <summary>
        /// 服务端会话状态，心跳用 ACTIVE 判断会话仍可用。
        /// </summary>
        internal string Status;

        /// <summary>
        /// 解析后的 RTC 入房信息；响应未包含有效扩展数据时可为 null。
        /// </summary>
        internal RtcJoinInfo JoinInfo;
    }

    /// <summary>
    /// 服务端下发的 RTC 入房凭证和远端机器人标识。
    /// </summary>
    internal sealed class RtcJoinInfo
    {
        /// <summary>
        /// RTC 应用标识。
        /// </summary>
        internal string AppId;

        /// <summary>
        /// RTC 房间标识。
        /// </summary>
        internal string RoomId;

        /// <summary>
        /// 本地入房用户标识。
        /// </summary>
        internal string UserId;

        /// <summary>
        /// RTC 入房令牌，不应写入日志。
        /// </summary>
        internal string Token;

        /// <summary>
        /// 预期远端机器人用户标识；为空时不按用户过滤。
        /// </summary>
        internal string BotName;
    }

}
