using System;

namespace Xmax.SDK
{
    internal sealed class QualityController
    {
        internal event Action<RealtimeNetworkQuality> NetworkQualityChanged;
        internal QualityController(IRtcManager rtc) { rtc.NetworkQualityChanged += Receive; }
        private void Receive(RtcNetworkQuality quality)
        {
            var converted = new RealtimeNetworkQuality(Level(quality.Uplink), Level(quality.Downlink),
                quality.PacketLossRatio, quality.RoundTripMilliseconds, quality.BandwidthBitsPerSecond);
            XmaxLogger.Rtc.Info(() => $"uplink={converted.Uplink} downlink={converted.Downlink} loss={converted.PacketLossRatio} rtt={converted.RoundTripMilliseconds}ms bandwidth={converted.BandwidthBitsPerSecond}bps", XmaxLoggerOption.Performance);
            EventDispatch.Raise(NetworkQualityChanged, converted);
        }
        private static RealtimeNetworkQualityLevel Level(RtcQualityLevel level) => Enum.IsDefined(typeof(RtcQualityLevel), level)
            ? (RealtimeNetworkQualityLevel)(int)level : RealtimeNetworkQualityLevel.Unknown;
    }
}
