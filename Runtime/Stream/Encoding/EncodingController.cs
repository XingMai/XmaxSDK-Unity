using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 按上传像素面积和帧率计算默认码率范围，并合并调用方的编码配置。
    /// </summary>
    internal static class EncodingController
    {
        /// <summary>
        /// 15 fps 流畅优先参考码率，按像素面积升序排列，码率单位为 kbps。
        /// </summary>
        private static readonly (double Value, double Bitrate)[] ReferenceBitratesAt15Fps =
        {
            (120 * 120, 50),
            (160 * 120, 65),
            (180 * 180, 100),
            (240 * 180, 120),
            (320 * 180, 140),
            (320 * 240, 200),
            (424 * 240, 220),
            (360 * 360, 260),
            (480 * 360, 320),
            (640 * 360, 400),
            (640 * 480, 500),
            (848 * 480, 610),
            (960 * 720, 910),
            (1280 * 720, 1130),
            (1920 * 1080, 2080)
        };

        /// <summary>
        /// 30 fps 流畅优先参考码率，按像素面积升序排列，码率单位为 kbps。
        /// </summary>
        private static readonly (double Value, double Bitrate)[] ReferenceBitratesAt30Fps =
        {
            (360 * 360, 400),
            (480 * 360, 490),
            (640 * 360, 600),
            (640 * 480, 750),
            (848 * 480, 930),
            (960 * 720, 1380),
            (1280 * 720, 1710),
            (1920 * 1080, 3150)
        };

        /// <summary>
        /// 校验视频格式并解析最终上传编码参数，不访问 RTC 或创建在线会话。
        /// </summary>
        /// <param name="format">上传编码格式；未指定的码率按尺寸和帧率计算。</param>
        /// <returns>可以交给 RTC 传输层的完整编码配置。</returns>
        /// <exception cref="XmaxException">格式无效、默认码率溢出或合并后的最低码率超过最高码率。</exception>
        internal static VideoEncodingConfiguration Resolve(RealtimeVideoFormat format)
        {
            format.Validate();

            int minimum;
            int maximum;
            if (format.MinimumBitrate.HasValue && format.MaximumBitrate.HasValue)
            {
                minimum = format.MinimumBitrate.Value;
                maximum = format.MaximumBitrate.Value;
            }
            else
            {
                var bitrates = ResolveBitrates((double)format.Width * format.Height, format.Fps);
                var roundedMinimum = Math.Round(bitrates.Minimum, MidpointRounding.AwayFromZero);
                var roundedMaximum = Math.Round(bitrates.Maximum, MidpointRounding.AwayFromZero);
                if (double.IsNaN(roundedMaximum) || double.IsInfinity(roundedMaximum) || roundedMaximum > int.MaxValue)
                {
                    throw new XmaxException(
                        XmaxErrorCode.InvalidConfiguration,
                        "Realtime video format exceeds the supported bitrate range.");
                }

                var defaultMinimum = Math.Max(1, (int)roundedMinimum);
                var defaultMaximum = Math.Max(defaultMinimum + 1, (int)roundedMaximum);
                minimum = format.MinimumBitrate ?? defaultMinimum;
                maximum = format.MaximumBitrate ?? defaultMaximum;
            }

            if (minimum > maximum)
            {
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Minimum bitrate must not exceed maximum bitrate after applying SDK defaults.");
            }

            return new VideoEncodingConfiguration(format, minimum, maximum);
        }

        /// <summary>
        /// 先按像素面积插值，再按帧率插值，生成流畅优先下限和画质优先上限。
        /// </summary>
        /// <param name="pixels">编码宽高的乘积，必须大于零。</param>
        /// <param name="fps">编码帧率，必须大于零。</param>
        /// <returns>尚未取整的最低和最高码率，单位为 kbps。</returns>
        private static (double Minimum, double Maximum) ResolveBitrates(double pixels, double fps)
        {
            var bitrate15 = Interpolate(pixels, ReferenceBitratesAt15Fps);
            var first30 = ReferenceBitratesAt30Fps[0];
            // 小尺寸沿用 15 fps 曲线，按比例衔接 30 fps 的首个参考点。
            var bitrate30 = pixels < first30.Value
                ? bitrate15 * (first30.Bitrate / Interpolate(first30.Value, ReferenceBitratesAt15Fps))
                : Interpolate(pixels, ReferenceBitratesAt30Fps);

            // 10 fps 以 640 × 480 的 400 kbps 为参考；60 fps 匹配 1080p 的两档码率。
            var bitrate10 = bitrate15 * (400.0 / 500);
            var minimum60 = bitrate30 * (4780.0 / 3150);
            var maximum60 = bitrate30 * (6500.0 / 3150);

            return (
                Interpolate(fps, new[]
                {
                    (10.0, bitrate10),
                    (15.0, bitrate15),
                    (30.0, bitrate30),
                    (60.0, minimum60)
                }),
                Interpolate(fps, new[]
                {
                    (10.0, bitrate10 * 2),
                    (15.0, bitrate15 * 2),
                    (30.0, bitrate30 * 2),
                    (60.0, maximum60)
                }));
        }

        /// <summary>
        /// 在相邻参考点间线性插值，表外按最近端点的比例外推。
        /// </summary>
        /// <param name="value">要查询的正像素面积或帧率。</param>
        /// <param name="points">按横坐标升序排列的非空参考点，横坐标须为正数。</param>
        /// <returns>插值或外推后的码率，单位为 kbps。</returns>
        private static double Interpolate(double value, (double Value, double Bitrate)[] points)
        {
            var first = points[0];
            if (value <= first.Value)
            {
                return first.Bitrate * (value / first.Value);
            }

            for (var index = 1; index < points.Length; index++)
            {
                var upper = points[index];
                if (value <= upper.Value)
                {
                    var lower = points[index - 1];
                    var ratio = (value - lower.Value) / (upper.Value - lower.Value);

                    return lower.Bitrate + (upper.Bitrate - lower.Bitrate) * ratio;
                }
            }

            var last = points[points.Length - 1];

            return last.Bitrate * (value / last.Value);
        }
    }
}
