using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    /// <summary>
    /// 管理远端轨道、首帧就绪信号和回调版本，避免分发过期画面。
    /// </summary>
    internal sealed class RenderController
    {
        // 当前绑定的远端轨道。
        private RealtimeVideoTrack _track;

        // 本轮生成的首帧信号与通知版本。
        private TaskCompletionSource<bool> _ready;
        private long _revision;

        /// <summary>
        /// 当前生成最近收到的帧引用；未就绪或已重置时为 null，长期保留需 Clone。
        /// </summary>
        internal XmaxVideoFrame LatestFrame { get; private set; }

        /// <summary>
        /// 当前生成收到有效远端帧时触发。
        /// </summary>
        internal event Action<XmaxVideoFrame> FrameReceived;

        /// <summary>
        /// 重置旧渲染状态并绑定新的远端轨道。
        /// </summary>
        /// <param name="track">需要绑定的视频轨道；null 表示解除轨道输入。</param>
        internal void Bind(RealtimeVideoTrack track)
        {
            Reset();
            _track = track;
        }

        /// <summary>
        /// 清除上一轮画面并建立本轮首帧等待信号。
        /// </summary>
        internal void BeginGeneration()
        {
            ResetGeneration();
            _ready = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>
        /// 等待本轮生成的第一帧，支持超时和调用方取消。
        /// </summary>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <param name="timeout">等待首帧的时限，单位为毫秒。</param>
        /// <returns>收到有效首帧后完成的任务。</returns>
        internal async Task WaitUntilRemoteFrameReadyAsync(CancellationToken cancellationToken, int timeout = 10000)
        {
            var ready = _ready ?? throw new XmaxException(XmaxErrorCode.RtcError, "Remote rendering is not active.");
            await AsyncDeadline.WaitAsync(ready.Task, timeout, cancellationToken);
        }

        /// <summary>
        /// 校验当前帧并完成首帧信号，再向仍有效的轨道和订阅者分发。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        internal void Receive(XmaxVideoFrame frame)
        {
            if (_track == null || _ready == null)
                return;

            frame.Validate();
            var revision = _revision;
            LatestFrame = frame;
            _ready.TrySetResult(true);
            _track.RaiseFrame(frame, () => revision == _revision);
            if (revision == _revision)
                EventDispatch.Raise(FrameReceived, frame, () => revision == _revision);
        }

        /// <summary>
        /// 使当前帧通知失效，取消首帧等待并清除最近帧。
        /// </summary>
        internal void ResetGeneration()
        {
            _revision++;
            _ready?.TrySetCanceled();
            _ready = null;
            LatestFrame = null;
        }

        /// <summary>
        /// 重置生成渲染状态并解除轨道绑定。
        /// </summary>
        internal void Reset()
        {
            ResetGeneration();
            _track = null;
        }
    }
}
