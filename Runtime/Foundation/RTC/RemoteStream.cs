using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 以房间和用户标识远端主视频流，不向上层暴露厂商流索引。
    /// </summary>
    internal readonly struct RemoteStream : IEquatable<RemoteStream>
    {
        /// <summary>
        /// 远端流所属的 RTC 房间标识。
        /// </summary>
        internal readonly string RoomId;

        /// <summary>
        /// 发布远端流的 RTC 用户标识。
        /// </summary>
        internal readonly string UserId;

        /// <summary>
        /// 以冒号连接房间和用户的展示标识；相等性仍按两个原始字段判断。
        /// </summary>
        internal string Key => $"{RoomId}:{UserId}";

        /// <summary>
        /// 保存远端主流的房间和用户标识。
        /// </summary>
        /// <param name="roomId">RTC 房间标识，用于过滤过期或其他房间的回调。</param>
        /// <param name="userId">RTC 用户标识。</param>
        internal RemoteStream(string roomId, string userId)
        {
            RoomId = roomId;
            UserId = userId;
        }

        /// <summary>
        /// 比较房间和用户标识是否同时相等。
        /// </summary>
        /// <param name="other">要比较的对象或远端流标识。</param>
        /// <returns>对方为相同房间、相同用户的远端流时为 true。</returns>
        public bool Equals(RemoteStream other) => RoomId == other.RoomId && UserId == other.UserId;

        /// <summary>
        /// 比较房间和用户标识是否同时相等。
        /// </summary>
        /// <param name="other">要比较的对象或远端流标识。</param>
        /// <returns>对方为相同房间、相同用户的远端流时为 true。</returns>
        public override bool Equals(object other) => other is RemoteStream stream && Equals(stream);

        /// <summary>
        /// 组合房间和用户标识的哈希值。
        /// </summary>
        /// <returns>与字段相等性一致的哈希码。</returns>
        public override int GetHashCode() => (RoomId ?? "").GetHashCode() ^ (UserId ?? "").GetHashCode();
    }
}
