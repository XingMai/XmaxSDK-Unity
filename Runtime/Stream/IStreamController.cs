using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    internal interface IStreamController
    {
        event Action<XmaxException> FatalError;
        event Action<RealtimeNetworkQuality> NetworkQualityChanged;
        event Action<XmaxVideoFrame> FrameReceived;
        void ValidatePlatform();
        Task ConnectAsync(RtcJoinInfo info, RealtimeVideoFormat format, CancellationToken cancellationToken);
        Task DisconnectAsync();
        Task<string> StartGenerationAsync(RealtimeContext context, RealtimeVideoFormat format, CancellationToken cancellationToken);
        void ChangeGenerationCondition(RealtimeContext context, RealtimeVideoFormat format, string taskId);
        Task StopGenerationAsync();
        void PushFrame(XmaxVideoFrame frame);
        void SendTracks(IReadOnlyList<XmaxTrackPoint> tracks, string taskId);
    }
}
