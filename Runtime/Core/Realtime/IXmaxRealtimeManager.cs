using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    public interface IXmaxRealtimeManager
    {
        RealtimeConfiguration Options { get; }
        RealtimeState CurrentState { get; }
        RealtimeMediaStream LocalStream { get; }
        event Action<RealtimeState> StateChanged;
        event Action<XmaxVideoFrame> RemoteFrameReceived;
        event Action<XmaxException> ErrorOccurred;
        event Action<XmaxException> CleanupWarning;
        event Action<RealtimeNetworkQuality> NetworkQualityChanged;
        RealtimeMediaStream CreateLocalExternalStream(RealtimeVideoFormat format);
        Task<RealtimeMediaStream> ConnectAsync(RealtimeMediaStream localStream, CancellationToken cancellationToken = default);
        Task<RealtimeMediaStream> ConnectAsync(RealtimeVideoFormat format, CancellationToken cancellationToken = default);
        void PushVideoFrame(XmaxVideoFrame frame);
        void SendTracks(IReadOnlyList<XmaxTrackPoint> tracks);
        Task StartGenerationAsync(RealtimeContext context = null, CancellationToken cancellationToken = default);
        Task<RealtimeMediaStream> StartGenerationAsync(RealtimeMediaStream localStream, RealtimeContext context,
            CancellationToken cancellationToken = default);
        Task StopGenerationAsync();
        Task DisconnectAsync();
        Task CloseAsync();
    }
}
