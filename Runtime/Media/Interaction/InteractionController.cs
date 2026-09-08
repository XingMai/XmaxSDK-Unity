using System;
using System.Collections.Generic;

namespace Xmax.SDK
{
    internal sealed class InteractionController
    {
        private readonly IStreamController _stream;
        internal InteractionController(IStreamController stream) { _stream = stream; }
        internal void Send(IReadOnlyList<XmaxTrackPoint> tracks, RealtimeVideoFormat format, string taskId)
        {
            if (tracks == null) throw new ArgumentNullException(nameof(tracks));
            if (tracks.Count == 0) return;
            if (string.IsNullOrEmpty(taskId)) throw new XmaxException(XmaxErrorCode.RtcError, "Tracks require active realtime generation.");
            for (var i = 0; i < tracks.Count; i++)
            {
                var point = tracks[i];
                if (point.X < 0 || point.X >= format.Width || point.Y < 0 || point.Y >= format.Height)
                    throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Track point is outside the encoder video bounds.");
            }
            _stream.SendTracks(tracks, taskId);
        }
    }
}
