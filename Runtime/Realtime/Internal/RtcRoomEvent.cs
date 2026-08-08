using System.Collections.Generic;
using UNBridgeLib.LitJson;

namespace Xmax.SDK
{
    internal static class RtcRoomEvent
    {
        internal static string Start(
            string userId,
            string taskId,
            RealtimeVideoFormat videoFormat,
            RealtimeContext context)
        {
            return JsonMapper.ToJson(new Dictionary<string, object>
            {
                ["event"] = "start",
                ["params"] = Parameters(videoFormat, context, true),
                ["user_id"] = userId,
                ["uid"] = taskId,
                ["session_uid"] = taskId
            });
        }

        internal static string ChangeCondition(
            string userId,
            RealtimeVideoFormat videoFormat,
            RealtimeContext context)
        {
            return JsonMapper.ToJson(new Dictionary<string, object>
            {
                ["event"] = "change_condition",
                ["params"] = Parameters(videoFormat, context, false),
                ["user_id"] = userId
            });
        }

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
            return JsonMapper.ToJson(message);
        }

        internal static string Heartbeat(string userId)
        {
            return JsonMapper.ToJson(new Dictionary<string, object>
            {
                ["event"] = "heartbeat",
                ["user_id"] = userId
            });
        }

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

            return JsonMapper.ToJson(new Dictionary<string, object>
            {
                ["event"] = "tracks",
                ["user_id"] = userId,
                ["uid"] = taskId,
                ["session_uid"] = taskId,
                ["tracks"] = serializedTracks
            });
        }

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
