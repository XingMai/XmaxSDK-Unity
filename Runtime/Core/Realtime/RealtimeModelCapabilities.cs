using System;
using System.Collections.Generic;

namespace Xmax.SDK
{
    /// <summary>
    /// 模型支持的视频分辨率，仅描述宽高，不限制帧率。
    /// </summary>
    public readonly struct RealtimeVideoSize
    {
        /// <summary>
        /// 视频宽度，单位为像素。
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// 视频高度，单位为像素。
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// 保存视频分辨率，实际使用时由模型和视频格式校验尺寸。
        /// </summary>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        public RealtimeVideoSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    /// <summary>
    /// 内置实时模型的不可变分辨率约束及默认 Camera 编码规格。
    /// </summary>
    public sealed class RealtimeModelCapabilities
    {
        // 与 iOS 一致的内置模型配置，所有调用方共享只读实例。
        private static readonly RealtimeModelCapabilities Standard = new RealtimeModelCapabilities(
            1280000, new RealtimeVideoFormat(832, 1472, 30));

        private static readonly RealtimeModelCapabilities Pro = new RealtimeModelCapabilities(
            2100000, new RealtimeVideoFormat(1024, 1920, 30),
            new RealtimeVideoSize(1024, 1920), new RealtimeVideoSize(1920, 1024));

        /// <summary>
        /// 支持的固定分辨率，只读；非空时宽高必须精确匹配，不自动缩放或取整。
        /// 空列表表示尺寸推荐使用像素上下界和对齐规则。
        /// </summary>
        public IReadOnlyList<RealtimeVideoSize> ResolutionBuckets { get; }

        /// <summary>
        /// 最小像素总数，仅在固定分辨率列表为空时参与尺寸推荐。
        /// </summary>
        public int MinimumPixels { get; } = 600000;

        /// <summary>
        /// 最大像素总数，仅在固定分辨率列表为空时参与尺寸推荐。
        /// </summary>
        public int MaximumPixels { get; }

        /// <summary>
        /// 推荐宽高的对齐倍数，仅在固定分辨率列表为空时参与尺寸推荐。
        /// </summary>
        public int DimensionAlignment { get; } = 32;

        /// <summary>
        /// 未指定帧率时使用的每秒帧数，与默认 Camera 规格保持一致。
        /// </summary>
        public int DefaultFps => DefaultCameraVideoFormat.Fps;

        /// <summary>
        /// 未指定 Camera 编码格式时使用的宽高和帧率；宿主仍负责采集并推送相机帧。
        /// </summary>
        public RealtimeVideoFormat DefaultCameraVideoFormat { get; }

        /// <summary>
        /// 创建内置模型配置，固定分辨率以只读集合对外提供。
        /// </summary>
        /// <param name="maximumPixels">无固定分辨率时用于尺寸推荐的像素上界。</param>
        /// <param name="defaultCameraVideoFormat">默认 Camera 编码宽高和帧率。</param>
        /// <param name="resolutionBuckets">固定分辨率列表；为空时采用像素上下界和对齐规则。</param>
        private RealtimeModelCapabilities(
            int maximumPixels,
            RealtimeVideoFormat defaultCameraVideoFormat,
            params RealtimeVideoSize[] resolutionBuckets)
        {
            MaximumPixels = maximumPixels;
            DefaultCameraVideoFormat = defaultCameraVideoFormat;
            ResolutionBuckets = Array.AsReadOnly(resolutionBuckets);
        }

        /// <summary>
        /// 按服务端模型名称查找内置配置。
        /// </summary>
        /// <param name="name">服务端模型名称。</param>
        /// <returns>内置模型的共享配置；自定义或未知模型返回 null。</returns>
        internal static RealtimeModelCapabilities ForName(string name)
        {
            switch (name)
            {
                case "x2.0":
                    return Standard;
                case "x2.0-pro":
                    return Pro;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 校验模型的固定分辨率约束；没有固定列表的模型不在此处进行缩放或面积校验。
        /// </summary>
        /// <param name="width">请求的视频宽度，单位为像素。</param>
        /// <param name="height">请求的视频高度，单位为像素。</param>
        /// <exception cref="XmaxException">请求尺寸未精确匹配任一固定分辨率。</exception>
        internal void ValidateResolution(int width, int height)
        {
            if (ResolutionBuckets.Count == 0)
            {
                return;
            }

            foreach (var size in ResolutionBuckets)
            {
                if (size.Width == width && size.Height == height)
                {
                    return;
                }
            }

            var supported = new List<string>();
            foreach (var size in ResolutionBuckets)
            {
                supported.Add($"{size.Width}x{size.Height}");
            }

            throw new XmaxException(
                XmaxErrorCode.InvalidConfiguration,
                $"Unsupported model input resolution {width}x{height}. Supported resolutions: " +
                string.Join(", ", supported));
        }
    }
}
