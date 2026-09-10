using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 视频在交互视口中的等比布局方式。
    /// </summary>
    public enum RealtimeContentMode
    {
        /// <summary>
        /// 完整显示画面，允许上下或左右留边。
        /// </summary>
        Fit,

        /// <summary>
        /// 填满视口，允许裁剪超出视口的画面。
        /// </summary>
        Fill
    }

    /// <summary>
    /// 将左上角为原点的视口坐标映射到编码画面，处理留边和居中裁剪。
    /// </summary>
    public static class InteractionCoordinateMapper
    {
        /// <summary>
        /// 尝试映射交互坐标，拒绝无效输入、视口外点和 Fit 模式的留边区域。
        /// </summary>
        /// <param name="x">视口内从左向右的坐标。</param>
        /// <param name="y">视口内从上向下的坐标。</param>
        /// <param name="viewportWidth">视口宽度，须与 x、y 使用相同单位且大于零。</param>
        /// <param name="viewportHeight">视口高度，须与 x、y 使用相同单位且大于零。</param>
        /// <param name="video">编码画面的尺寸，用于等比布局和坐标映射。</param>
        /// <param name="mode">完整显示或填满视口的等比布局模式。</param>
        /// <param name="point">映射成功时输出编码画面的像素坐标，失败时输出默认值。</param>
        /// <returns>映射成功时为 true；失败时 point 为默认值。</returns>
        public static bool TryMap(
            double x,
            double y,
            double viewportWidth,
            double viewportHeight,
            RealtimeVideoFormat video,
            RealtimeContentMode mode,
            out XmaxTrackPoint point)
        {
            point = default;
            if (!Finite(x) || !Finite(y) || !Finite(viewportWidth) || !Finite(viewportHeight) ||
                viewportWidth <= 0 || viewportHeight <= 0 || video.Width <= 0 || video.Height <= 0 ||
                x < 0 || y < 0 || x > viewportWidth || y > viewportHeight)
                return false;

            if (mode != RealtimeContentMode.Fit && mode != RealtimeContentMode.Fill)
                return false;

            var scale = mode == RealtimeContentMode.Fill
                ? Math.Max(viewportWidth / video.Width, viewportHeight / video.Height)
                : Math.Min(viewportWidth / video.Width, viewportHeight / video.Height);
            var left = (viewportWidth - video.Width * scale) / 2;
            var top = (viewportHeight - video.Height * scale) / 2;
            if (mode == RealtimeContentMode.Fit && (x < left || y < top || x > viewportWidth - left || y > viewportHeight - top))
                return false;

            point = new XmaxTrackPoint(
                (int)Math.Max(
                    0,
                    Math.Min(
                        video.Width - 1,
                        Math.Round((x - left) / scale, MidpointRounding.AwayFromZero))),
                (int)Math.Max(
                    0,
                    Math.Min(
                        video.Height - 1,
                        Math.Round((y - top) / scale, MidpointRounding.AwayFromZero))));

            return true;
        }

        /// <summary>
        /// 检查数值不是 NaN 或无穷大。
        /// </summary>
        /// <param name="value">需要检查是否有限的数值。</param>
        /// <returns>有限数值为 true。</returns>
        private static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
