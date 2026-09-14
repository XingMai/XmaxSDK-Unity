using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 提供无需在线请求的模型能力查询和视频编码尺寸建议。
    /// </summary>
    public sealed class MediaService
    {
        /// <summary>
        /// 查询内置实时模型的分辨率约束和默认 Camera 编码规格。
        /// </summary>
        /// <param name="model">目标模型定义。</param>
        /// <returns>模型固定分辨率、像素范围、尺寸对齐和默认 Camera 规格。</returns>
        /// <exception cref="XmaxException">模型为空或不是受支持的内置模型。</exception>
        public RealtimeModelCapabilities GetCapabilities(ModelDefinition model)
        {
            return model?.Capabilities ?? throw new XmaxException(
                XmaxErrorCode.NotSupported,
                "Media recommendations require a built-in realtime model. Custom models can use explicit formats.");
        }

        /// <summary>
        /// 按模型配置解析编码尺寸；固定分辨率须精确匹配，其他模型按像素范围和对齐规则推荐。
        /// </summary>
        /// <param name="model">目标模型定义。</param>
        /// <param name="sourceWidth">原始 Camera 画面的宽度，必须大于零。</param>
        /// <param name="sourceHeight">原始 Camera 画面的高度，必须大于零。</param>
        /// <param name="fps">期望帧率，0 表示使用模型默认帧率，负数无效。</param>
        /// <returns>符合模型尺寸约束的编码格式，保留显式帧率或采用模型默认帧率。</returns>
        /// <exception cref="XmaxException">模型不受支持，或源尺寸、帧率不符合要求。</exception>
        public RealtimeVideoFormat RecommendVideoFormat(
            ModelDefinition model,
            int sourceWidth,
            int sourceHeight,
            int fps = 0)
        {
            if (sourceWidth <= 0 || sourceHeight <= 0 || fps < 0)
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Source dimensions must be positive and fps cannot be negative.");

            var capability = GetCapabilities(model);
            if (capability.ResolutionBuckets.Count > 0)
            {
                capability.ValidateResolution(sourceWidth, sourceHeight);

                return new RealtimeVideoFormat(sourceWidth, sourceHeight, fps == 0 ? capability.DefaultFps : fps);
            }

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
                if (minH > maxH)
                    continue;

                var h = (int)Math.Max(minH, Math.Min(maxH, Math.Round(targetHeight, MidpointRounding.AwayFromZero)));
                var dx = (w - targetWidth) / targetWidth;
                var dy = (h - targetHeight) / targetHeight;
                var score = dx * dx + dy * dy;
                if (score >= best)
                    continue;

                best = score;
                width = w * alignment;
                height = h * alignment;
            }

            return new RealtimeVideoFormat(width, height, fps == 0 ? capability.DefaultFps : fps);
        }
    }
}
