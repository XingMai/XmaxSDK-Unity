namespace Xmax.SDK
{
    /// <summary>
    /// RTC 适配层统一的网络质量等级。
    /// </summary>
    internal enum RtcQualityLevel
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
        Down
    }

    /// <summary>
    /// RTC 适配层的本地网络质量快照。
    /// </summary>
    internal readonly struct RtcNetworkQuality
    {
        /// <summary>
        /// 本地上行网络质量等级。
        /// </summary>
        internal readonly RtcQualityLevel Uplink;

        /// <summary>
        /// 本地下行网络质量等级。
        /// </summary>
        internal readonly RtcQualityLevel Downlink;

        /// <summary>
        /// 本地网络丢包比例，沿用 RTC 统计值。
        /// </summary>
        internal readonly double PacketLossRatio;

        /// <summary>
        /// 往返时延，单位为毫秒。
        /// </summary>
        internal readonly int RoundTripMilliseconds;

        /// <summary>
        /// 估算可用带宽，单位为比特每秒。
        /// </summary>
        internal readonly int BandwidthBitsPerSecond;

        /// <summary>
        /// 保存上下行等级及本地网络统计。
        /// </summary>
        /// <param name="uplink">本地上行网络质量等级。</param>
        /// <param name="downlink">本地下行网络质量等级。</param>
        /// <param name="packetLossRatio">本地网络丢包比例，沿用 RTC 统计值。</param>
        /// <param name="rtt">本地网络往返时延，单位为毫秒。</param>
        /// <param name="bandwidth">估算可用带宽，单位为比特每秒。</param>
        internal RtcNetworkQuality(
            RtcQualityLevel uplink,
            RtcQualityLevel downlink,
            double packetLossRatio,
            int rtt,
            int bandwidth)
        {
            Uplink = uplink;
            Downlink = downlink;
            PacketLossRatio = packetLossRatio;
            RoundTripMilliseconds = rtt;
            BandwidthBitsPerSecond = bandwidth;
        }
    }
}
