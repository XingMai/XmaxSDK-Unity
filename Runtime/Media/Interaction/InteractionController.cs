using System;
using System.Collections.Generic;

namespace Xmax.SDK
{
    /// <summary>
    /// 校验生成任务和编码画面内的交互点后发送轨迹。
    /// </summary>
    internal sealed class InteractionController
    {
        private readonly IStreamController _stream;

        /// <summary>
        /// 注入房间交互消息的发送入口。
        /// </summary>
        /// <param name="stream">当前管理器的房间信令和视频传输入口。</param>
        internal InteractionController(IStreamController stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// 校验轨迹点位于编码画面内，并发送给当前生成任务。
        /// </summary>
        /// <param name="tracks">以编码画面左上角为原点的交互点序列，坐标须位于画面内。</param>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        internal void Send(IReadOnlyList<XmaxTrackPoint> tracks, RealtimeVideoFormat format, string taskId)
        {
            if (tracks == null)
                throw new ArgumentNullException(nameof(tracks));

            if (tracks.Count == 0)
                return;

            if (string.IsNullOrEmpty(taskId))
                throw new XmaxException(XmaxErrorCode.RtcError, "Tracks require active realtime generation.");

            for (var i = 0; i < tracks.Count; i++)
            {
                var point = tracks[i];
                if (point.X < 0 || point.X >= format.Width || point.Y < 0 || point.Y >= format.Height)
                    throw new XmaxException(
                        XmaxErrorCode.InvalidConfiguration,
                        "Track point is outside the encoder video bounds.");
            }

            _stream.SendTracks(tracks, taskId);
        }
    }
}
