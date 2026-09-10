using System.Collections.Generic;

namespace Xmax.SDK
{
    /// <summary>
    /// 按 Xmax 房间协议编码 JSON 消息，统一字段名称和兼容参数。
    /// </summary>
    internal static class RtcRoomEvent
    {
        /// <summary>
        /// 编码包含任务标识和生成条件的启动消息。
        /// </summary>
        /// <param name="userId">RTC 用户标识。</param>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        /// <param name="videoFormat">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="context">已经由上层解析的非空生成条件。</param>
        /// <returns>start 事件的 JSON 文本。</returns>
        internal static string Start(
            string userId,
            string taskId,
            RealtimeVideoFormat videoFormat,
            RealtimeContext context)
        {
            return JsonCodec.Encode(new Dictionary<string, object>
            {
                ["event"] = "start",
                ["params"] = Parameters(videoFormat, context, true),
                ["user_id"] = userId,
                ["uid"] = taskId,
                ["session_uid"] = taskId
            });
        }

        /// <summary>
        /// 编码更新当前生成条件的消息。
        /// </summary>
        /// <param name="userId">RTC 用户标识。</param>
        /// <param name="videoFormat">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="context">已经由上层解析的非空生成条件。</param>
        /// <returns>change_condition 事件的 JSON 文本。</returns>
        internal static string ChangeCondition(
            string userId,
            RealtimeVideoFormat videoFormat,
            RealtimeContext context)
        {
            return JsonCodec.Encode(new Dictionary<string, object>
            {
                ["event"] = "change_condition",
                ["params"] = Parameters(videoFormat, context, false),
                ["user_id"] = userId
            });
        }

        /// <summary>
        /// 编码停止生成消息，有任务标识时附带 session_uid。
        /// </summary>
        /// <param name="userId">RTC 用户标识。</param>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        /// <returns>stop 事件的 JSON 文本。</returns>
        internal static string Stop(string userId, string taskId)
        {
            var message = new Dictionary<string, object>
            {
                ["event"] = "stop",
                ["user_id"] = userId
            };
            if (!string.IsNullOrEmpty(taskId))
            {
                message["session_uid"] = taskId;
            }

            return JsonCodec.Encode(message);
        }

        /// <summary>
        /// 编码维持房间活动状态的消息。
        /// </summary>
        /// <param name="userId">RTC 用户标识。</param>
        /// <returns>heartbeat 事件的 JSON 文本。</returns>
        internal static string Heartbeat(string userId)
        {
            return JsonCodec.Encode(new Dictionary<string, object>
            {
                ["event"] = "heartbeat",
                ["user_id"] = userId
            });
        }

        /// <summary>
        /// 将编码画面坐标序列化为整型点数组并附带当前任务标识。
        /// </summary>
        /// <param name="userId">RTC 用户标识。</param>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
        /// <param name="tracks">以编码画面左上角为原点的交互点序列，坐标须位于画面内。</param>
        /// <returns>tracks 事件的 JSON 文本。</returns>
        internal static string Tracks(
            string userId,
            string taskId,
            IReadOnlyList<XmaxTrackPoint> tracks)
        {
            var serializedTracks = new List<int[]>(tracks.Count);

            for (var index = 0; index < tracks.Count; index++)
            {
                serializedTracks.Add(new[] { tracks[index].X, tracks[index].Y });
            }

            return JsonCodec.Encode(new Dictionary<string, object>
            {
                ["event"] = "tracks",
                ["user_id"] = userId,
                ["uid"] = taskId,
                ["session_uid"] = taskId,
                ["tracks"] = serializedTracks
            });
        }

        /// <summary>
        /// 组装生成尺寸、提示词和可选参考路径，按需保留旧参考字段。
        /// </summary>
        /// <param name="videoFormat">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <param name="context">已经由上层解析的非空生成条件。</param>
        /// <param name="includeLegacyReference">是否同时写入旧协议的 ref_image 字段。</param>
        /// <returns>用于房间消息 params 字段的参数字典。</returns>
        private static Dictionary<string, object> Parameters(
            RealtimeVideoFormat videoFormat,
            RealtimeContext context,
            bool includeLegacyReference)
        {
            var parameters = new Dictionary<string, object>
            {
                ["model"] = "default",
                ["size"] = new[] { videoFormat.Width, videoFormat.Height },
                ["prompt"] = context.Prompt
            };
            if (!string.IsNullOrEmpty(context.ReferencePath))
            {
                parameters["ref_image_path"] = context.ReferencePath;
                if (includeLegacyReference)
                {
                    parameters["ref_image"] = context.ReferencePath;
                }
            }

            return parameters;
        }
    }
}
