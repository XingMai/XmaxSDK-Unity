namespace Xmax.SDK
{
    /// <summary>
    /// 已解析的 RTC 主视频流编码参数，不包含厂商类型或缺省码率。
    /// </summary>
    internal readonly struct VideoEncodingConfiguration
    {
        /// <summary>
        /// 编码输出宽度，单位为像素。
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// 编码输出高度，单位为像素。
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// 编码输出帧率，单位为帧每秒。
        /// </summary>
        public int FrameRate { get; }

        /// <summary>
        /// 最低上传码率，单位为 kbps；0 表示不设最低码率。
        /// </summary>
        public int MinimumBitrate { get; }

        /// <summary>
        /// 最高上传码率，单位为 kbps。
        /// </summary>
        public int MaximumBitrate { get; }

        /// <summary>
        /// 上传编码策略偏好。
        /// </summary>
        public RealtimeVideoEncoderPreference EncoderPreference { get; }

        /// <summary>
        /// 保存已校验的视频格式和计算完成的码率范围。
        /// </summary>
        /// <param name="format">已校验尺寸、帧率和编码偏好的视频格式。</param>
        /// <param name="minimumBitrate">非负最低码率，单位为 kbps，不得超过最高码率。</param>
        /// <param name="maximumBitrate">大于零的最高码率，单位为 kbps。</param>
        internal VideoEncodingConfiguration(RealtimeVideoFormat format, int minimumBitrate, int maximumBitrate)
        {
            Width = format.Width;
            Height = format.Height;
            FrameRate = format.Fps;

            MinimumBitrate = minimumBitrate;
            MaximumBitrate = maximumBitrate;
            EncoderPreference = format.EncoderPreference;
        }
    }
}
