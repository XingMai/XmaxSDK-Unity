using System;

namespace Xmax.SDK
{
    internal sealed class MediaController
    {
        private readonly Action _requireThread;
        private readonly Action<XmaxVideoFrame> _publish;
        private long _revision;
        internal RealtimeMediaStream LocalStream { get; private set; }
        internal MediaController(Action requireThread, Action<XmaxVideoFrame> publish)
        { _requireThread = requireThread; _publish = publish; }
        internal RealtimeMediaStream CreateExternalStream(RealtimeVideoFormat format)
        {
            format.Validate();
            var revision = ++_revision;
            var track = new RealtimeVideoTrack("video-local");
            LocalStream = new RealtimeMediaStream(RealtimeStreamId.Local, track, format, frame =>
            {
                _requireThread();
                if (revision != _revision) throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "This local stream has been closed or replaced.");
                if (frame == null) throw new ArgumentNullException(nameof(frame));
                frame.Validate();
                track.RaiseFrame(frame, () => revision == _revision);
                if (revision == _revision) _publish(frame);
            });
            return LocalStream;
        }
        internal RealtimeVideoFormat RequireOwned(RealtimeMediaStream stream)
        {
            if (stream == null || !ReferenceEquals(stream, LocalStream))
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Use a local stream created by this realtime manager.");
            return stream.VideoFormat.Value;
        }
        internal void Close() { _revision++; LocalStream = null; }
    }
}
