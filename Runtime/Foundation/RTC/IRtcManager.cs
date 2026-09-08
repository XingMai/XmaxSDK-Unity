using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    // No vendor types cross this boundary. Call on the Unity main thread.
    internal interface IRtcManager
    {
        event Action<XmaxException> FatalError;
        event Action<RealtimeNetworkQuality> NetworkQualityChanged;
        event Action<RemoteStream> VideoPublished;
        event Action<RemoteStream> VideoUnpublished;
        event Action<RemoteStream, byte[]> SeiReceived;
        event Action<RemoteStream, XmaxVideoFrame> FrameReceived;
        void ValidatePlatform();
        Task JoinAsync(RtcJoinInfo info, RealtimeVideoFormat format, CancellationToken cancellationToken);
        Task LeaveAsync();
        void SubscribeVideo(RemoteStream key);
        void PushFrame(XmaxVideoFrame frame);
        void SendRoomMessage(string message);
        void SendSei(byte[] data);
    }
}
