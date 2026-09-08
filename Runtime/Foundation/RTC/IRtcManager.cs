using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    internal readonly struct RtcStreamKey : IEquatable<RtcStreamKey>
    {
        internal readonly string RoomId;
        internal readonly string UserId;
        internal readonly int Index;
        internal RtcStreamKey(string roomId, string userId, int index = 0)
        { RoomId = roomId; UserId = userId; Index = index; }
        public bool Equals(RtcStreamKey other) => RoomId == other.RoomId && UserId == other.UserId && Index == other.Index;
        public override bool Equals(object other) => other is RtcStreamKey key && Equals(key);
        public override int GetHashCode() => (RoomId ?? "").GetHashCode() ^ (UserId ?? "").GetHashCode() ^ Index;
    }

    // No vendor types cross this boundary. Call on the Unity main thread.
    internal interface IRtcManager
    {
        event Action<XmaxException> FatalError;
        event Action<RealtimeNetworkQuality> NetworkQualityChanged;
        event Action<RtcStreamKey> VideoPublished;
        event Action<RtcStreamKey> VideoUnpublished;
        event Action<RtcStreamKey, byte[]> SeiReceived;
        event Action<RtcStreamKey, XmaxVideoFrame> FrameReceived;
        void ValidatePlatform();
        Task JoinAsync(RtcJoinInfo info, RealtimeVideoFormat format, CancellationToken cancellationToken);
        Task LeaveAsync();
        void SubscribeVideo(RtcStreamKey key);
        void PushFrame(XmaxVideoFrame frame);
        void SendRoomMessage(string message);
        void SendSei(byte[] data);
    }
}
