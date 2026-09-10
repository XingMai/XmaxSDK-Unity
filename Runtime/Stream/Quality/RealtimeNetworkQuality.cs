namespace Xmax.SDK
{
    /// <summary>
    /// 对外暴露的上下行网络质量等级。
    /// </summary>
    public enum RealtimeNetworkQualityLevel
    {
        /// <summary>
        /// 未知或无法识别的网络质量。
        /// </summary>
        Unknown,

        /// <summary>
        /// 网络质量极佳。
        /// </summary>
        Excellent,

        /// <summary>
        /// 网络质量良好。
        /// </summary>
        Good,

        /// <summary>
        /// 网络质量一般。
        /// </summary>
        Poor,

        /// <summary>
        /// 网络质量较差。
        /// </summary>
        Bad,

        /// <summary>
        /// 网络质量很差。
        /// </summary>
        VeryBad,

        /// <summary>
        /// 网络连接不可用。
        /// </summary>
        /// <remarks>
        /// 保留与跨平台模型一致的等级；当前厂商适配未产生该等级。
        /// </remarks>
        Down
    }

    /// <summary>
    /// 本地上下行质量及丢包、往返时延、带宽统计的不可变快照。
    /// </summary>
    public sealed class RealtimeNetworkQuality
    {
        /// <summary>
        /// 本地上行网络质量等级。
        /// </summary>
        public RealtimeNetworkQualityLevel Uplink { get; }

        /// <summary>
        /// 本地下行网络质量等级。
        /// </summary>
        public RealtimeNetworkQualityLevel Downlink { get; }

        /// <summary>
        /// 本地网络丢包比例，沿用 RTC 统计值。
        /// </summary>
        public double PacketLossRatio { get; }

        /// <summary>
        /// 往返时延，单位为毫秒。
        /// </summary>
        public int RoundTripMilliseconds { get; }

        /// <summary>
        /// 估算可用带宽，单位为比特每秒。
        /// </summary>
        public int BandwidthBitsPerSecond { get; }

        /// <summary>
        /// 保存网络质量等级和本地统计；仅数值的兼容构造将等级设为 Unknown。
        /// </summary>
        /// <param name="packetLossRatio">本地网络丢包比例，沿用 RTC 统计值。</param>
        /// <param name="roundTripMilliseconds">本地网络往返时延，单位为毫秒。</param>
        /// <param name="bandwidthBitsPerSecond">估算可用带宽，单位为比特每秒。</param>
        internal RealtimeNetworkQuality(double packetLossRatio, int roundTripMilliseconds, int bandwidthBitsPerSecond)
            : this(
            RealtimeNetworkQualityLevel.Unknown,
            RealtimeNetworkQualityLevel.Unknown,
            packetLossRatio,
            roundTripMilliseconds,
            bandwidthBitsPerSecond)
        {
        }

        /// <summary>
        /// 保存网络质量等级和本地统计；仅数值的兼容构造将等级设为 Unknown。
        /// </summary>
        /// <param name="uplink">本地上行网络质量等级。</param>
        /// <param name="downlink">本地下行网络质量等级。</param>
        /// <param name="packetLossRatio">本地网络丢包比例，沿用 RTC 统计值。</param>
        /// <param name="roundTripMilliseconds">本地网络往返时延，单位为毫秒。</param>
        /// <param name="bandwidthBitsPerSecond">估算可用带宽，单位为比特每秒。</param>
        public RealtimeNetworkQuality(
            RealtimeNetworkQualityLevel uplink,
            RealtimeNetworkQualityLevel downlink,
            double packetLossRatio = 0,
            int roundTripMilliseconds = 0,
            int bandwidthBitsPerSecond = 0)
        {
            Uplink = uplink;
            Downlink = downlink;
            PacketLossRatio = packetLossRatio;
            RoundTripMilliseconds = roundTripMilliseconds;
            BandwidthBitsPerSecond = bandwidthBitsPerSecond;
        }
    }
}
