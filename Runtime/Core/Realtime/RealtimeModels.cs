using System;

namespace Xmax.SDK
{
    /// <summary>
    /// SDK 内置的实时模型标识。
    /// </summary>
    public enum RealtimeModel
    {
        /// <summary>
        /// x2.0 实时生成模型。
        /// </summary>
        X2_0,

        /// <summary>
        /// x2.0-pro 实时生成模型。
        /// </summary>
        X2_0_Pro
    }

    /// <summary>
    /// 服务端模型名称的不可变定义，也可用于自定义模型。
    /// </summary>
    public sealed class ModelDefinition
    {
        /// <summary>
        /// 去除首尾空白后的非空模型名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 创建并校验服务端模型名称。
        /// </summary>
        /// <param name="name">非空服务端模型名称，首尾空白会被移除。</param>
        /// <exception cref="XmaxException">规范化后的模型名称为空。</exception>
        public ModelDefinition(string name)
        {
            Name = (name ?? string.Empty).Trim();
            if (Name.Length == 0)
            {
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Model name cannot be empty.");
            }
        }
    }

    /// <summary>
    /// 将内置模型标识转换为服务端模型定义。
    /// </summary>
    public static class Models
    {
        /// <summary>
        /// 解析内置实时模型对应的服务端名称。
        /// </summary>
        /// <param name="model">SDK 内置实时模型标识。</param>
        /// <returns>指定实时模型的定义。</returns>
        /// <exception cref="ArgumentOutOfRangeException">model 不是已定义的内置模型。</exception>
        public static ModelDefinition Realtime(RealtimeModel model)
        {
            switch (model)
            {
                case RealtimeModel.X2_0:
                    return new ModelDefinition("x2.0");
                case RealtimeModel.X2_0_Pro:
                    return new ModelDefinition("x2.0-pro");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model), model, null);
            }
        }
    }

    /// <summary>
    /// 单个实时管理器使用的不可变模型配置。
    /// </summary>
    public sealed class RealtimeConfiguration
    {
        /// <summary>
        /// 本管理器创建会话时使用的模型。
        /// </summary>
        public ModelDefinition Model { get; }

        /// <summary>
        /// 创建实时配置，模型不能为空。
        /// </summary>
        /// <param name="model">目标模型定义。</param>
        /// <exception cref="XmaxException">model 为 null。</exception>
        public RealtimeConfiguration(ModelDefinition model)
        {
            Model = model ?? throw new XmaxException(
                XmaxErrorCode.InvalidConfiguration,
                "A realtime model is required.");
        }
    }

    /// <summary>
    /// 本地流的编码宽高和帧率；在创建流或连接时校验。
    /// </summary>
    public readonly struct RealtimeVideoFormat
    {
        /// <summary>
        /// 视频宽度，单位为像素，使用时须为正偶数。
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// 视频高度，单位为像素，使用时须为正偶数。
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// 目标编码帧率，单位为帧每秒，使用时须大于零。
        /// </summary>
        public int Fps { get; }

        /// <summary>
        /// 保存视频编码尺寸和帧率。
        /// </summary>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <param name="fps">目标编码帧率，必须大于零。</param>
        public RealtimeVideoFormat(int width, int height, int fps)
        {
            Width = width;
            Height = height;
            Fps = fps;
        }

        /// <summary>
        /// 校验宽高为正偶数、帧率大于零。
        /// </summary>
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

    /// <summary>
    /// 实时生成条件，包含提示词和可选的服务端参考资源路径。
    /// </summary>
    public sealed class RealtimeContext
    {
        /// <summary>
        /// 去除首尾空白后的提示词；空输入保存为空字符串。
        /// </summary>
        public string Prompt { get; }

        /// <summary>
        /// 去除首尾空白后的参考资源路径；未设置时为 null。
        /// </summary>
        public string ReferencePath { get; }

        /// <summary>
        /// 规范化提示词和参考资源路径。
        /// </summary>
        /// <param name="prompt">生成提示词；null 会规范化为空字符串。</param>
        /// <param name="referencePath">可选的服务端参考资源路径；null 或空白表示不指定。</param>
        public RealtimeContext(string prompt, string referencePath = null)
        {
            Prompt = (prompt ?? string.Empty).Trim();
            var normalized = (referencePath ?? string.Empty).Trim();
            ReferencePath = normalized.Length == 0 ? null : normalized;
        }
    }

    /// <summary>
    /// 编码画面中的整数交互坐标，以左上角为原点。
    /// </summary>
    public readonly struct XmaxTrackPoint
    {
        /// <summary>
        /// 从画面左侧向右计算的像素坐标。
        /// </summary>
        public int X { get; }

        /// <summary>
        /// 从画面顶部向下计算的像素坐标。
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// 保存编码画面中的交互点，发送时校验其范围。
        /// </summary>
        /// <param name="x">从编码画面左侧向右的像素坐标。</param>
        /// <param name="y">从编码画面顶部向下的像素坐标。</param>
        public XmaxTrackPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// 实时管理器的连接和生成阶段。
    /// </summary>
    public enum RealtimeConnectionState
    {
        /// <summary>
        /// 尚未开始连接。
        /// </summary>
        Idle,

        /// <summary>
        /// 正在创建会话或加入 RTC 房间。
        /// </summary>
        Connecting,

        /// <summary>
        /// 已连接，可开始生成。
        /// </summary>
        Connected,

        /// <summary>
        /// 生成任务已就绪，可接收远端视频帧。
        /// </summary>
        Generating,

        /// <summary>
        /// 连接已清理，本地流可继续用于预览。
        /// </summary>
        Disconnected,

        /// <summary>
        /// 连接失败或致命错误清理后的状态。
        /// </summary>
        Error,

        /// <summary>
        /// 正在停止生成并清理连接。
        /// </summary>
        Disconnecting
    }

    /// <summary>
    /// 实时管理器状态的不可变快照。
    /// </summary>
    public sealed class RealtimeState
    {
        /// <summary>
        /// 当前连接或生成阶段。
        /// </summary>
        public RealtimeConnectionState ConnectionState { get; }

        /// <summary>
        /// 当前服务端会话标识；没有会话时为 null。
        /// </summary>
        public string SessionId { get; }

        /// <summary>
        /// 当前生成任务标识；尚未生成或已停止时为 null。
        /// </summary>
        public string TaskId { get; }

        /// <summary>
        /// 创建连接状态及关联会话、任务标识的快照。
        /// </summary>
        /// <param name="connectionState">需要保存的连接或生成阶段。</param>
        /// <param name="sessionId">需要处理的服务端会话标识。</param>
        /// <param name="taskId">本次生成任务的唯一标识，用于校验任务归属。</param>
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

    /// <summary>
    /// 区分本地输入流和远端生成流。
    /// </summary>
    public enum RealtimeStreamId
    {
        /// <summary>
        /// 远端生成视频流。
        /// </summary>
        Remote,

        /// <summary>
        /// 由宿主提供 Camera 帧的本地流。
        /// </summary>
        Local
    }

    /// <summary>
    /// 通过事件分发视频帧的轨道。
    /// </summary>
    public sealed class RealtimeVideoTrack
    {
        /// <summary>
        /// 轨道标识，用于区分本地轨道和远端轨道。
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 收到帧时触发；需要在回调结束后保留帧时应先调用 Clone。
        /// </summary>
        public event Action<XmaxVideoFrame> FrameReceived;

        /// <summary>
        /// 创建轨道，空标识回退为 video-remote。
        /// </summary>
        /// <param name="id">轨道标识，为空时使用默认远端轨道标识。</param>
        internal RealtimeVideoTrack(string id)
        {
            Id = string.IsNullOrWhiteSpace(id) ? "video-remote" : id;
        }

        /// <summary>
        /// 逐个分发视频帧；轨道已失效时停止后续回调。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        /// <param name="isCurrent">可选有效性检查，每次回调前执行；返回 false 时停止后续分发。</param>
        internal void RaiseFrame(XmaxVideoFrame frame, Func<bool> isCurrent = null)
        {
            EventDispatch.Raise(FrameReceived, frame, isCurrent);
        }
    }

    /// <summary>
    /// SDK 管理的媒体流，本地流可接收宿主输入，远端流用于接收生成画面。
    /// </summary>
    public sealed class RealtimeMediaStream
    {
        /// <summary>
        /// 当前流属于本地输入还是远端输出。
        /// </summary>
        public RealtimeStreamId Id { get; }

        /// <summary>
        /// 该流的视频轨道。
        /// </summary>
        public RealtimeVideoTrack VideoTrack { get; }

        /// <summary>
        /// 本地流创建时指定的编码格式；远端流未指定时为 null。
        /// </summary>
        public RealtimeVideoFormat? VideoFormat { get; }
        private readonly Action<XmaxVideoFrame> _push;

        /// <summary>
        /// 在 Unity 主线程向有效的本地流推送 Camera 帧，同时触发预览；连接后再发布至 RTC。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        /// <exception cref="XmaxException">流不是有效的本地输入流、线程不符合要求，或帧数据无效。</exception>
        /// <exception cref="ArgumentNullException">有效本地流收到的 frame 为 null。</exception>
        public void PushVideoFrame(XmaxVideoFrame frame)
        {
            if (_push == null)
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Only a local external stream accepts input frames.");

            _push(frame);
        }

        /// <summary>
        /// 组装流标识、视频轨道和可选的本地输入回调。
        /// </summary>
        /// <param name="id">流类型，区分本地输入和远端输出。</param>
        /// <param name="videoTrack">当前流的视频轨道。</param>
        /// <param name="videoFormat">可选的本地编码格式；远端流可不指定。</param>
        /// <param name="push">本地流的输入回调；远端只读流传 null。</param>
        internal RealtimeMediaStream(
            RealtimeStreamId id,
            RealtimeVideoTrack videoTrack,
            RealtimeVideoFormat? videoFormat = null,
            Action<XmaxVideoFrame> push = null)
        {
            Id = id;
            VideoTrack = videoTrack;
            VideoFormat = videoFormat;
            _push = push;
        }
    }
}
