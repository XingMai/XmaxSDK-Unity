using bytertc;

namespace Xmax.SDK
{
    internal static class RtcQualityConverter
    {
        internal static RtcNetworkQuality Convert(NetworkQualityStats local) => new RtcNetworkQuality(
            Level(local.tx_quality), Level(local.rx_quality), local.fraction_lost, local.rtt, local.total_bandwidth);
        private static RtcQualityLevel Level(NetworkQuality quality)
        {
            switch (quality)
            {
                case NetworkQuality.kNetworkQualityExcellent: return RtcQualityLevel.Excellent;
                case NetworkQuality.kNetworkQualityGood: return RtcQualityLevel.Good;
                case NetworkQuality.kNetworkQualityPoor: return RtcQualityLevel.Poor;
                case NetworkQuality.kNetworkQualityBad: return RtcQualityLevel.Bad;
                case NetworkQuality.kNetworkQualityVbad: return RtcQualityLevel.VeryBad;
                default: return RtcQualityLevel.Unknown;
            }
        }
    }
}
