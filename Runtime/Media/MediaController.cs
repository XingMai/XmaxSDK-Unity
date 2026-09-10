using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 管理本地外部视频流的归属和版本，防止关闭或替换后的流继续推帧。
    /// </summary>
    internal sealed class MediaController
    {
        // 线程检查与帧发布依赖。
        private readonly Action _requireThread;
        private readonly Action<XmaxVideoFrame> _publish;

        // 本地流版本，用于拒绝已替换或关闭的输入。
        private long _revision;

        /// <summary>
        /// 当前有效的本地流；尚未创建或已关闭时为 null。
        /// </summary>
        internal RealtimeMediaStream LocalStream { get; private set; }

        /// <summary>
        /// 注入线程校验及连接有效时的帧发布回调。
        /// </summary>
        /// <param name="requireThread">在推帧时校验 Unity 主线程的回调。</param>
        /// <param name="publish">本地预览回调结束后尝试向有效连接发布帧的回调。</param>
        internal MediaController(Action requireThread, Action<XmaxVideoFrame> publish)
        {
            _requireThread = requireThread;
            _publish = publish;
        }

        /// <summary>
        /// 创建本地轨道及输入回调，使此前创建的本地流失效。
        /// </summary>
        /// <param name="format">本地视频编码格式，宽高须为正偶数且帧率大于零。</param>
        /// <returns>可推送 Camera 帧并触发本地预览的新流。</returns>
        internal RealtimeMediaStream CreateExternalStream(RealtimeVideoFormat format)
        {
            format.Validate();
            var revision = ++_revision;
            var track = new RealtimeVideoTrack("video-local");
            LocalStream = new RealtimeMediaStream(RealtimeStreamId.Local, track, format, frame =>
            {
                _requireThread();
                if (revision != _revision)
                    throw new XmaxException(
                        XmaxErrorCode.InvalidConfiguration,
                        "This local stream has been closed or replaced.");

                if (frame == null)
                    throw new ArgumentNullException(nameof(frame));

                frame.Validate();
                track.RaiseFrame(frame, () => revision == _revision);
                if (revision == _revision)
                    _publish(frame);
            });

            return LocalStream;
        }

        /// <summary>
        /// 校验流是当前管理器仍有效的本地流。
        /// </summary>
        /// <param name="stream">需要验证归属的本地流。</param>
        /// <returns>该本地流创建时指定的编码格式。</returns>
        internal RealtimeVideoFormat RequireOwned(RealtimeMediaStream stream)
        {
            if (stream == null || !ReferenceEquals(stream, LocalStream))
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Use a local stream created by this realtime manager.");

            return stream.VideoFormat.Value;
        }

        /// <summary>
        /// 清除本地流并递增版本，使旧流及正在重入的帧通知失效。
        /// </summary>
        internal void Close()
        {
            _revision++;
            LocalStream = null;
        }
    }
}
