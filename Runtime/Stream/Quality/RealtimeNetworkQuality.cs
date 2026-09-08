namespace Xmax.SDK
{
    public sealed class RealtimeNetworkQuality
    {
        public double PacketLossRatio { get; }
        public int RoundTripMilliseconds { get; }
        public int BandwidthBitsPerSecond { get; }
        internal RealtimeNetworkQuality(double packetLossRatio, int roundTripMilliseconds, int bandwidthBitsPerSecond)
        { PacketLossRatio = packetLossRatio; RoundTripMilliseconds = roundTripMilliseconds; BandwidthBitsPerSecond = bandwidthBitsPerSecond; }
    }
}
