namespace Xmax.SDK
{
    /// <summary>
    /// 实时视频的上传编码策略偏好。
    /// </summary>
    public enum RealtimeVideoEncoderPreference
    {
        /// <summary>
        /// 平衡帧率和分辨率。
        /// </summary>
        Auto,

        /// <summary>
        /// 优先保障帧率。
        /// </summary>
        MaintainFramerate,

        /// <summary>
        /// 优先保障分辨率。
        /// </summary>
        MaintainQuality
    }

    /// <summary>
    /// 本地视频的编码尺寸、帧率和上传编码配置；在创建流或连接时校验。
    /// </summary>
    public readonly struct RealtimeVideoFormat
    {
        /// <summary>
        /// 视频宽度，单位为像素，使用时须为正偶数。
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// 视频高度，单位为像素，使用时须为正偶数。
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// 目标编码帧率，单位为帧每秒，使用时须大于零。
        /// </summary>
        public int Fps { get; }

        /// <summary>
        /// 最低上传码率，单位为 kbps；null 使用 SDK 默认值，0 表示不设最低码率。
        /// </summary>
        public int? MinimumBitrate { get; }

        /// <summary>
        /// 最高上传码率，单位为 kbps；null 使用 SDK 默认值，指定时必须大于零。
        /// </summary>
        public int? MaximumBitrate { get; }

        /// <summary>
        /// 上传编码策略偏好，默认平衡帧率和分辨率。
        /// </summary>
        public RealtimeVideoEncoderPreference EncoderPreference { get; }

        /// <summary>
        /// 保存视频编码尺寸和帧率，码率范围由 SDK 计算。
        /// </summary>
        /// <param name="width">视频宽度，单位为像素，使用时须为正偶数。</param>
        /// <param name="height">视频高度，单位为像素，使用时须为正偶数。</param>
        /// <param name="fps">目标编码帧率，必须大于零。</param>
        public RealtimeVideoFormat(int width, int height, int fps)
            : this(width, height, fps, null, null, RealtimeVideoEncoderPreference.Auto)
        {
        }

        /// <summary>
        /// 保存视频格式及可选上传编码配置。
        /// </summary>
        /// <param name="width">视频宽度，单位为像素，使用时须为正偶数。</param>
        /// <param name="height">视频高度，单位为像素，使用时须为正偶数。</param>
        /// <param name="fps">目标编码帧率，必须大于零。</param>
        /// <param name="minimumBitrate">最低上传码率，单位为 kbps；null 按编码尺寸和帧率计算，0 不设下限。</param>
        /// <param name="maximumBitrate">最高上传码率，单位为 kbps；null 按编码尺寸和帧率计算，指定时须大于零。</param>
        /// <param name="encoderPreference">上传编码策略偏好，默认平衡帧率和分辨率。</param>
        public RealtimeVideoFormat(
            int width,
            int height,
            int fps,
            int? minimumBitrate = null,
            int? maximumBitrate = null,
            RealtimeVideoEncoderPreference encoderPreference = RealtimeVideoEncoderPreference.Auto)
        {
            Width = width;
            Height = height;
            Fps = fps;

            MinimumBitrate = minimumBitrate;
            MaximumBitrate = maximumBitrate;
            EncoderPreference = encoderPreference;
        }

        /// <summary>
        /// 校验尺寸、帧率、显式码率范围和编码策略。
        /// </summary>
        /// <exception cref="XmaxException">尺寸、帧率、码率或编码策略无效。</exception>
        internal void Validate()
        {
            if (Width <= 0 || Height <= 0 || Fps <= 0 || Width % 2 != 0 || Height % 2 != 0)
            {
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Realtime video width and height must be positive even numbers, and fps must be greater than zero.");
            }

            if (MinimumBitrate < 0)
            {
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Minimum bitrate must not be negative.");
            }

            if (MaximumBitrate <= 0)
            {
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Maximum bitrate must be greater than zero.");
            }

            if (MinimumBitrate > MaximumBitrate)
            {
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Minimum bitrate must not exceed maximum bitrate.");
            }

            if (EncoderPreference != RealtimeVideoEncoderPreference.Auto &&
                EncoderPreference != RealtimeVideoEncoderPreference.MaintainFramerate &&
                EncoderPreference != RealtimeVideoEncoderPreference.MaintainQuality)
            {
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Unknown video encoder preference.");
            }
        }
    }
}
