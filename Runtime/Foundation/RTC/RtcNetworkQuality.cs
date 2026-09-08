namespace Xmax.SDK
{
    internal enum RtcQualityLevel { Unknown, Excellent, Good, Poor, Bad, VeryBad, Down }

    internal readonly struct RtcNetworkQuality
    {
        internal readonly RtcQualityLevel Uplink;
        internal readonly RtcQualityLevel Downlink;
        internal readonly double PacketLossRatio;
        internal readonly int RoundTripMilliseconds;
        internal readonly int BandwidthBitsPerSecond;
        internal RtcNetworkQuality(RtcQualityLevel uplink, RtcQualityLevel downlink, double packetLossRatio, int rtt, int bandwidth)
        { Uplink = uplink; Downlink = downlink; PacketLossRatio = packetLossRatio; RoundTripMilliseconds = rtt; BandwidthBitsPerSecond = bandwidth; }
    }
}
