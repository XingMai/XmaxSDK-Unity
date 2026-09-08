using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xmax.SDK
{
    // Owns the Xmax room protocol; the RTC adapter only owns native transport.
    internal sealed class StreamController : IStreamController
    {
        private readonly IRtcManager _rtc;
        private readonly int _generationTimeout;
        private readonly Func<string> _taskIdFactory;
        private readonly RealtimeTiming _timing;
        private readonly QualityController _quality;
        private RtcJoinInfo _info;
        private CancellationTokenSource _roomCancellation;
        private CancellationTokenSource _generationCancellation;
        private Task _heartbeat = Task.CompletedTask;
        private Task _sei = Task.CompletedTask;
        private TaskCompletionSource<bool> _confirmation;
        private string _taskId;
        private RemoteStream? _matchedStream;
        public event Action<XmaxException> FatalError;
        public event Action<RealtimeNetworkQuality> NetworkQualityChanged;
        public event Action<XmaxVideoFrame> FrameReceived;

        internal StreamController(IRtcManager rtc, int generationTimeout = 30000, Func<string> taskIdFactory = null, RealtimeTiming timing = null)
        {
            _rtc = rtc;
            _timing = timing;
            _quality = new QualityController(rtc);
            _generationTimeout = generationTimeout;
            _taskIdFactory = taskIdFactory ?? (() => "task-unity-" + Guid.NewGuid().ToString("N"));
            rtc.FatalError += OnFatalError;
            _quality.NetworkQualityChanged += quality => { if (_info != null) EventDispatch.Raise(NetworkQualityChanged, quality); };
            rtc.VideoPublished += OnPublished;
            rtc.VideoUnpublished += OnUnpublished;
            rtc.SeiReceived += OnSei;
            rtc.FrameReceived += OnFrame;
        }
        public void ValidatePlatform() => _rtc.ValidatePlatform();
        public async Task ConnectAsync(RtcJoinInfo info, RealtimeVideoFormat format, CancellationToken cancellationToken)
        {
            _info = info;
            try
            {
                await _rtc.JoinAsync(info, format, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                _roomCancellation = new CancellationTokenSource();
                _heartbeat = RunHeartbeatAsync(_roomCancellation.Token);
            }
            catch { await DisconnectAsync(); throw; }
        }
        public async Task DisconnectAsync()
        {
            _roomCancellation?.Cancel();
            await StopGenerationAsync();
            await _heartbeat;
            _roomCancellation?.Dispose();
            _roomCancellation = null;
            _info = null;
            await _rtc.LeaveAsync();
        }
        public async Task<string> StartGenerationAsync(RealtimeContext context, RealtimeVideoFormat format, CancellationToken cancellationToken)
        {
            if (_info == null || _roomCancellation == null || _taskId != null)
                throw new XmaxException(XmaxErrorCode.RtcError, "RTC connection is unavailable or generation is already active.");
            cancellationToken.ThrowIfCancellationRequested();
            var taskId = _taskIdFactory();
            _taskId = taskId;
            _matchedStream = null;
            _confirmation = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _generationCancellation = CancellationTokenSource.CreateLinkedTokenSource(_roomCancellation.Token);
            try
            {
                _timing?.BeginSignal(taskId);
                _rtc.SendRoomMessage(RtcRoomEvent.Start(_info.UserId, taskId, format, context));
                _timing?.SignalSent(taskId);
                XmaxLogger.Info("Room", () => "Generation start signal sent.");
                _sei = RunSeiAsync(taskId, _generationCancellation.Token);
                await AsyncDeadline.WaitAsync(_confirmation.Task, _generationTimeout, cancellationToken);
                return taskId;
            }
            catch { await StopGenerationAsync(); throw; }
        }
        public void ChangeGenerationCondition(RealtimeContext context, RealtimeVideoFormat format, string taskId)
        {
            RequireTask(taskId);
            _rtc.SendRoomMessage(RtcRoomEvent.ChangeCondition(_info.UserId, format, context));
        }
        public void SendTracks(IReadOnlyList<XmaxTrackPoint> tracks, string taskId)
        {
            RequireTask(taskId);
            _rtc.SendRoomMessage(RtcRoomEvent.Tracks(_info.UserId, taskId, tracks));
        }
        public void PushFrame(XmaxVideoFrame frame) => _rtc.PushFrame(frame);
        public async Task StopGenerationAsync()
        {
            var taskId = _taskId;
            _taskId = null;
            _matchedStream = null;
            _confirmation?.TrySetCanceled();
            _confirmation = null;
            _generationCancellation?.Cancel();
            await _sei;
            _generationCancellation?.Dispose();
            _generationCancellation = null;
            if (taskId != null && _info != null)
            {
                try { _rtc.SendRoomMessage(RtcRoomEvent.Stop(_info.UserId, taskId)); }
                catch (Exception exception) { XmaxLogger.Failure("Room", exception); }
            }
        }
        private bool Accept(RemoteStream key) => _info != null && key.RoomId == _info.RoomId &&
            (string.IsNullOrEmpty(_info.BotName) || key.UserId == _info.BotName);
        private void OnPublished(RemoteStream key)
        {
            if (!Accept(key)) return;
            try { _rtc.SubscribeVideo(key); }
            catch (Exception exception) { OnFatalError(RealtimeErrorHandler.Wrap(exception)); }
        }
        private void OnUnpublished(RemoteStream key)
        {
            if (_matchedStream.HasValue && _matchedStream.Value.Equals(key)) _matchedStream = null;
        }
        private void OnSei(RemoteStream key, byte[] data)
        {
            if (_taskId == null || !Accept(key)) return;
            var message = Encoding.UTF8.GetString(data ?? Array.Empty<byte>()).Trim('\0', ' ', '\r', '\n', '\t');
            if (message != _taskId) return;
            _timing?.MatchSei(_taskId);
            // Rebind after unpublish/republish, even when initial confirmation completed.
            _matchedStream = key;
            _confirmation?.TrySetResult(true);
        }
        private void OnFrame(RemoteStream key, XmaxVideoFrame frame)
        {
            if (_taskId == null || !_matchedStream.HasValue || !_matchedStream.Value.Equals(key)) return;
            EventDispatch.Raise(FrameReceived, frame);
        }
        private void OnFatalError(XmaxException exception)
        {
            if (_info == null) return;
            _confirmation?.TrySetException(exception);
            EventDispatch.Raise(FatalError, exception);
        }
        private void RequireTask(string taskId)
        {
            if (_info == null || string.IsNullOrEmpty(taskId) || taskId != _taskId)
                throw new XmaxException(XmaxErrorCode.RtcError, "Realtime generation task does not match.");
        }
        private async Task RunSeiAsync(string taskId, CancellationToken cancellationToken)
        {
            try
            {
                // Yield before callbacks so the owning Task is assigned before cleanup can reenter.
                await Task.Yield();
                var data = Encoding.UTF8.GetBytes(taskId);
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (_taskId != taskId) return;
                    _rtc.SendSei(data);
                    await Task.Delay(66, cancellationToken);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) { if (!cancellationToken.IsCancellationRequested) OnFatalError(RealtimeErrorHandler.Wrap(exception)); }
        }
        private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    await Task.Delay(10000, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    _rtc.SendRoomMessage(RtcRoomEvent.Heartbeat(_info.UserId));
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) { if (!cancellationToken.IsCancellationRequested) OnFatalError(RealtimeErrorHandler.Wrap(exception)); }
        }
    }
}
