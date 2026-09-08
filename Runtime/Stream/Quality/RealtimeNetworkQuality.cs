namespace Xmax.SDK
{
    public enum RealtimeNetworkQualityLevel { Unknown, Excellent, Good, Poor, Bad, VeryBad, Down }

    public sealed class RealtimeNetworkQuality
    {
        public RealtimeNetworkQualityLevel Uplink { get; }
        public RealtimeNetworkQualityLevel Downlink { get; }
        public double PacketLossRatio { get; }
        public int RoundTripMilliseconds { get; }
        public int BandwidthBitsPerSecond { get; }
        internal RealtimeNetworkQuality(double packetLossRatio, int roundTripMilliseconds, int bandwidthBitsPerSecond)
            : this(RealtimeNetworkQualityLevel.Unknown, RealtimeNetworkQualityLevel.Unknown, packetLossRatio, roundTripMilliseconds, bandwidthBitsPerSecond) { }
        public RealtimeNetworkQuality(RealtimeNetworkQualityLevel uplink, RealtimeNetworkQualityLevel downlink,
            double packetLossRatio = 0, int roundTripMilliseconds = 0, int bandwidthBitsPerSecond = 0)
        {
            Uplink = uplink; Downlink = downlink;
            PacketLossRatio = packetLossRatio; RoundTripMilliseconds = roundTripMilliseconds; BandwidthBitsPerSecond = bandwidthBitsPerSecond;
        }
    }
}
