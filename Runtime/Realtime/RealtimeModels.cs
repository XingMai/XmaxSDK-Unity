using System;

namespace Xmax.SDK
{
    public enum RealtimeModel
    {
        X2_0
    }

    public sealed class ModelDefinition
    {
        public string Name { get; }

        public ModelDefinition(string name)
        {
            Name = (name ?? string.Empty).Trim();
            if (Name.Length == 0)
            {
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Model name cannot be empty.");
            }
        }
    }

    public static class Models
    {
        public static ModelDefinition Realtime(RealtimeModel model)
        {
            switch (model)
            {
                case RealtimeModel.X2_0:
                    return new ModelDefinition("x2.0");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model), model, null);
            }
        }
    }

    public sealed class RealtimeConfiguration
    {
        public ModelDefinition Model { get; }

        public RealtimeConfiguration(ModelDefinition model)
        {
            Model = model ?? throw new XmaxException(
                XmaxErrorCode.InvalidConfiguration,
                "A realtime model is required.");
        }
    }

    public readonly struct RealtimeVideoFormat
    {
        public int Width { get; }
        public int Height { get; }
        public int Fps { get; }

        public RealtimeVideoFormat(int width, int height, int fps)
        {
            Width = width;
            Height = height;
            Fps = fps;
        }

        internal void Validate()
        {
            if (Width <= 0 || Height <= 0 || Fps <= 0 || Width % 2 != 0 || Height % 2 != 0)
            {
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Realtime video width and height must be positive even numbers, and fps must be greater than zero.");
            }
        }
    }

    public sealed class RealtimeContext
    {
        public string Prompt { get; }
        public string ReferencePath { get; }

        public RealtimeContext(string prompt, string referencePath = null)
        {
            Prompt = (prompt ?? string.Empty).Trim();
            var normalized = (referencePath ?? string.Empty).Trim();
            ReferencePath = normalized.Length == 0 ? null : normalized;
        }
    }

    public enum RealtimeConnectionState
    {
        Idle,
        Connecting,
        Connected,
        Generating,
        Disconnected,
        Error
    }

    public sealed class RealtimeState
    {
        public RealtimeConnectionState ConnectionState { get; }
        public string SessionId { get; }
        public string TaskId { get; }

        public RealtimeState(
            RealtimeConnectionState connectionState,
            string sessionId = null,
            string taskId = null)
        {
            ConnectionState = connectionState;
            SessionId = sessionId;
            TaskId = taskId;
        }
    }

    public enum RealtimeStreamId
    {
        Remote
    }

    public sealed class RealtimeVideoTrack
    {
        public string Id { get; }
        public event Action<XmaxVideoFrame> FrameReceived;

        internal RealtimeVideoTrack(string id)
        {
            Id = string.IsNullOrWhiteSpace(id) ? "video-remote" : id;
        }

        internal void RaiseFrame(XmaxVideoFrame frame)
        {
            FrameReceived?.Invoke(frame);
        }
    }

    public sealed class RealtimeMediaStream
    {
        public RealtimeStreamId Id { get; }
        public RealtimeVideoTrack VideoTrack { get; }

        internal RealtimeMediaStream(RealtimeStreamId id, RealtimeVideoTrack videoTrack)
        {
            Id = id;
            VideoTrack = videoTrack;
        }
    }
}
