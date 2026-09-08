using System;

namespace Xmax.SDK
{
    public sealed class RealtimeModelCapabilities
    {
        public int MinimumPixels { get; } = 600000;
        public int MaximumPixels { get; } = 1280000;
        public int DimensionAlignment { get; } = 32;
        public int DefaultFps { get; } = 24;
        internal RealtimeModelCapabilities() { }
    }

    public sealed class MediaService
    {
        public RealtimeModelCapabilities GetCapabilities(ModelDefinition model)
        {
            if (model == null || model.Name != "x2.0")
                throw new XmaxException(XmaxErrorCode.NotSupported, "Media recommendations are available for x2.0 only. Custom models can use explicit formats.");
            return new RealtimeModelCapabilities();
        }
        /// <summary>Recommend an aligned encoder size using the iOS x2.0 pixel limits.</summary>
        public RealtimeVideoFormat RecommendVideoFormat(ModelDefinition model, int sourceWidth, int sourceHeight, int fps = 0)
        {
            if (sourceWidth <= 0 || sourceHeight <= 0 || fps < 0)
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Source dimensions must be positive and fps cannot be negative.");
            var capability = GetCapabilities(model);
            var area = (double)sourceWidth * sourceHeight;
            var scale = area < capability.MinimumPixels ? Math.Sqrt(capability.MinimumPixels / area) :
                area > capability.MaximumPixels ? Math.Sqrt(capability.MaximumPixels / area) : 1;
            var alignment = capability.DimensionAlignment;
            var targetWidth = sourceWidth * scale / alignment;
            var targetHeight = sourceHeight * scale / alignment;
            Func<double, double> round = area < capability.MinimumPixels ? Math.Ceiling :
                area > capability.MaximumPixels ? Math.Floor : value => Math.Round(value, MidpointRounding.AwayFromZero);
            var alignedWidth = Math.Max(alignment, (int)round(targetWidth) * alignment);
            var alignedHeight = Math.Max(alignment, (int)round(targetHeight) * alignment);
            var alignedPixels = (long)alignedWidth * alignedHeight;
            if (alignedPixels >= capability.MinimumPixels && alignedPixels <= capability.MaximumPixels)
                return new RealtimeVideoFormat(alignedWidth, alignedHeight, fps == 0 ? capability.DefaultFps : fps);
            var minUnits = (int)Math.Ceiling((double)capability.MinimumPixels / (alignment * alignment));
            var maxUnits = capability.MaximumPixels / (alignment * alignment);
            var best = double.PositiveInfinity;
            var width = 0;
            var height = 0;
            for (var w = 1; w <= maxUnits; w++)
            {
                var minH = (minUnits + w - 1) / w;
                var maxH = maxUnits / w;
                if (minH > maxH) continue;
                var h = (int)Math.Max(minH, Math.Min(maxH, Math.Round(targetHeight, MidpointRounding.AwayFromZero)));
                var dx = (w - targetWidth) / targetWidth;
                var dy = (h - targetHeight) / targetHeight;
                var score = dx * dx + dy * dy;
                if (score >= best) continue;
                best = score; width = w * alignment; height = h * alignment;
            }
            return new RealtimeVideoFormat(width, height, fps == 0 ? capability.DefaultFps : fps);
        }
    }
}
