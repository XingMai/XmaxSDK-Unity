using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    internal sealed class RenderController
    {
        private RealtimeVideoTrack _track;
        private TaskCompletionSource<bool> _ready;
        private long _revision;
        internal XmaxVideoFrame LatestFrame { get; private set; }
        internal event Action<XmaxVideoFrame> FrameReceived;
        internal void Bind(RealtimeVideoTrack track) { Reset(); _track = track; }
        internal void BeginGeneration()
        {
            ResetGeneration();
            _ready = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
        internal async Task WaitUntilRemoteFrameReadyAsync(CancellationToken cancellationToken, int timeout = 10000)
        {
            var ready = _ready ?? throw new XmaxException(XmaxErrorCode.RtcError, "Remote rendering is not active.");
            await AsyncDeadline.WaitAsync(ready.Task, timeout, cancellationToken);
        }
        internal void Receive(XmaxVideoFrame frame)
        {
            if (_track == null || _ready == null) return;
            frame.Validate();
            var revision = _revision;
            LatestFrame = frame;
            _ready.TrySetResult(true);
            _track.RaiseFrame(frame, () => revision == _revision);
            if (revision == _revision) EventDispatch.Raise(FrameReceived, frame, () => revision == _revision);
        }
        internal void ResetGeneration()
        {
            _revision++;
            _ready?.TrySetCanceled();
            _ready = null;
            LatestFrame = null;
        }
        internal void Reset() { ResetGeneration(); _track = null; }
    }
}
