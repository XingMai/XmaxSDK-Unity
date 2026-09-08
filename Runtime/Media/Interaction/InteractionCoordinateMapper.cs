using System;

namespace Xmax.SDK
{
    public enum RealtimeContentMode { Fit, Fill }

    public static class InteractionCoordinateMapper
    {
        /// <summary>Map a viewport point to encoder pixels. Both coordinate systems use a top-left origin.</summary>
        public static bool TryMap(double x, double y, double viewportWidth, double viewportHeight,
            RealtimeVideoFormat video, RealtimeContentMode mode, out XmaxTrackPoint point)
        {
            point = default;
            if (!Finite(x) || !Finite(y) || !Finite(viewportWidth) || !Finite(viewportHeight) ||
                viewportWidth <= 0 || viewportHeight <= 0 || video.Width <= 0 || video.Height <= 0 ||
                x < 0 || y < 0 || x > viewportWidth || y > viewportHeight) return false;
            if (mode != RealtimeContentMode.Fit && mode != RealtimeContentMode.Fill) return false;
            var scale = mode == RealtimeContentMode.Fill
                ? Math.Max(viewportWidth / video.Width, viewportHeight / video.Height)
                : Math.Min(viewportWidth / video.Width, viewportHeight / video.Height);
            var left = (viewportWidth - video.Width * scale) / 2;
            var top = (viewportHeight - video.Height * scale) / 2;
            if (mode == RealtimeContentMode.Fit && (x < left || y < top || x > viewportWidth - left || y > viewportHeight - top)) return false;
            point = new XmaxTrackPoint(
                (int)Math.Max(0, Math.Min(video.Width - 1, Math.Round((x - left) / scale, MidpointRounding.AwayFromZero))),
                (int)Math.Max(0, Math.Min(video.Height - 1, Math.Round((y - top) / scale, MidpointRounding.AwayFromZero))));
            return true;
        }
        private static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
