using bytertc;

namespace Xmax.SDK
{
    /// <summary>
    /// 将厂商网络统计映射到 RTC 适配层，未知枚举保留为 Unknown。
    /// </summary>
    internal static class RtcQualityConverter
    {
        /// <summary>
        /// 提取厂商本地统计中的上下行等级及网络指标。
        /// </summary>
        /// <param name="local">厂商提供的本地上下行网络统计。</param>
        /// <returns>与厂商类型隔离的网络质量快照。</returns>
        internal static RtcNetworkQuality Convert(NetworkQualityStats local) => new RtcNetworkQuality(
            Level(local.tx_quality), Level(local.rx_quality), local.fraction_lost, local.rtt, local.total_bandwidth);

        /// <summary>
        /// 映射已知厂商网络等级，无法识别的值回退为 Unknown。
        /// </summary>
        /// <param name="quality">厂商定义的网络质量枚举值。</param>
        /// <returns>RTC 适配层网络等级。</returns>
        private static RtcQualityLevel Level(NetworkQuality quality)
        {
            switch (quality)
            {
                case NetworkQuality.kNetworkQualityExcellent:
                    return RtcQualityLevel.Excellent;
                case NetworkQuality.kNetworkQualityGood:
                    return RtcQualityLevel.Good;
                case NetworkQuality.kNetworkQualityPoor:
                    return RtcQualityLevel.Poor;
                case NetworkQuality.kNetworkQualityBad:
                    return RtcQualityLevel.Bad;
                case NetworkQuality.kNetworkQualityVbad:
                    return RtcQualityLevel.VeryBad;
                default:
                    return RtcQualityLevel.Unknown;
            }
        }
    }
}
