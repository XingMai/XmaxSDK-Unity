using System;

namespace Xmax.SDK
{
    /// <summary>Identifies a remote main stream in an RTC room, matching iOS RemoteStream.</summary>
    internal readonly struct RemoteStream : IEquatable<RemoteStream>
    {
        internal readonly string RoomId;
        internal readonly string UserId;
        internal string Key => $"{RoomId}:{UserId}";

        internal RemoteStream(string roomId, string userId)
        {
            RoomId = roomId;
            UserId = userId;
        }

        public bool Equals(RemoteStream other) => RoomId == other.RoomId && UserId == other.UserId;
        public override bool Equals(object other) => other is RemoteStream stream && Equals(stream);
        public override int GetHashCode() => (RoomId ?? "").GetHashCode() ^ (UserId ?? "").GetHashCode();
    }
}
