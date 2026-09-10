using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 将 RTC 网络统计转换为公开质量模型，记录性能日志并通知订阅者。
    /// </summary>
    internal sealed class QualityController
    {
        /// <summary>
        /// 转换后的本地上下行网络质量更新。
        /// </summary>
        internal event Action<RealtimeNetworkQuality> NetworkQualityChanged;

        /// <summary>
        /// 订阅 RTC 网络质量回调。
        /// </summary>
        /// <param name="rtc">隔离厂商实现的 RTC 传输入口。</param>
        internal QualityController(IRtcManager rtc)
        {
            rtc.NetworkQualityChanged += Receive;
        }

        /// <summary>
        /// 转换本地网络质量、按性能配置记录数值并分发事件。
        /// </summary>
        /// <param name="quality">RTC 适配层的本地网络质量快照。</param>
        private void Receive(RtcNetworkQuality quality)
        {
            var converted = new RealtimeNetworkQuality(Level(quality.Uplink), Level(quality.Downlink),
                quality.PacketLossRatio, quality.RoundTripMilliseconds, quality.BandwidthBitsPerSecond);
            XmaxLogger.Rtc.Info(
                () => $"uplink={converted.Uplink} downlink={converted.Downlink} loss={converted.PacketLossRatio} rtt={converted.RoundTripMilliseconds}ms bandwidth={converted.BandwidthBitsPerSecond}bps",
                XmaxLoggerOption.Performance);
            EventDispatch.Raise(NetworkQualityChanged, converted);
        }

        /// <summary>
        /// 将 RTC 质量等级映射为公开等级，未定义值回退为 Unknown。
        /// </summary>
        /// <param name="level">RTC 适配层定义的网络质量等级。</param>
        /// <returns>对外网络质量等级。</returns>
        private static RealtimeNetworkQualityLevel Level(RtcQualityLevel level) => Enum.IsDefined(
            typeof(RtcQualityLevel),
            level)
            ? (RealtimeNetworkQualityLevel)(int)level : RealtimeNetworkQualityLevel.Unknown;
    }
}
