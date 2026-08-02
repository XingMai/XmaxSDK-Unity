using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using bytertc;
using UnityEngine;
using RtcVideoFrame = bytertc.VideoFrame;

namespace Xmax.SDK
{
    internal sealed class VolcRtcController
    {
        private const int JoinTimeoutMilliseconds = 15000;
        private const int GenerationTimeoutMilliseconds = 30000;
        private const int RemoteDisplayDelayMilliseconds = 300;
        private const int RoomHeartbeatMilliseconds = 10000;
        private const int TaskSeiMilliseconds = 66;
        private const int VideoTimestampWarning = -202;

        private RTCVideo _engine;
        private IRTCVideoRoom _room;
        private CancellationTokenSource _lifetimeCancellation;
        private TaskCompletionSource<bool> _joinCompletion;
        private TaskCompletionSource<bool> _generationCompletion;
        private RealtimeVideoTrack _remoteTrack;
        private RealtimeVideoFormat _videoFormat;
        private string _roomId = string.Empty;
        private string _localUserId = string.Empty;
        private string _botName = string.Empty;
        private string _currentTaskId = string.Empty;
        private RemoteStreamKey? _matchedRemoteStream;
        private bool _didLogVideoTimestampWarning;

        internal event Action<XmaxException> FatalError;

        internal async Task JoinAsync(
            RtcJoinInfo joinInfo,
            RealtimeVideoFormat videoFormat,
            RealtimeVideoTrack remoteTrack,
            CancellationToken cancellationToken)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                throw new XmaxException(
                    XmaxErrorCode.NotSupported,
                    "XmaxSDK RTC currently supports Android Player only. RTC cannot be started in the Unity Editor.");
            }

            await LeaveAsync();
            cancellationToken.ThrowIfCancellationRequested();

            _videoFormat = videoFormat;
            _remoteTrack = remoteTrack;
            _roomId = joinInfo.RoomId;
            _localUserId = joinInfo.UserId;
            _botName = joinInfo.BotName ?? string.Empty;
            _didLogVideoTimestampWarning = false;
            _lifetimeCancellation = new CancellationTokenSource();

            try
            {
                _engine = new RTCVideo();
                BindEngineEvents(_engine);
                CheckResult(
                    _engine.CreateRTCVideo(new RTCVideoEngineParams
                    {
                        AppID = joinInfo.AppId,
                        Params = new Dictionary<string, object>()
                    }),
                    "CreateRTCVideo");

                _engine.SetVideoSourceType(StreamIndex.kStreamIndexMain, VideoSourceType.VideoSourceTypeExternal);
                CheckResult(_engine.SetVideoEncoderConfig1(new VideoEncoderConfig
                {
                    Width = videoFormat.Width,
                    Height = videoFormat.Height,
                    FrameRate = videoFormat.Fps,
                    MaxBitrate = EstimateBitrate(videoFormat),
                    MinBitrate = 0
                }), "SetVideoEncoderConfig");

                _room = _engine.CreateRTCRoom(joinInfo.RoomId);
                if (_room == null)
                {
                    throw new XmaxException(XmaxErrorCode.RtcError, "CreateRTCRoom returned null.");
                }
                BindRoomEvents(_room);

                _joinCompletion = new TaskCompletionSource<bool>();
                var joinResult = _room.JoinRoom(
                    joinInfo.Token,
                    new UserInfo { UserID = joinInfo.UserId, ExtraInfo = string.Empty },
                    new MultiRoomConfig(
                        RoomProfileType.kRoomProfileTypeCommunication,
                        false,
                        false,
                        false));
                CheckResult(joinResult, "JoinRoom");
                await AwaitWithTimeoutAsync(
                    _joinCompletion.Task,
                    JoinTimeoutMilliseconds,
                    "RTC join room timed out.",
                    cancellationToken);

                _room.PublishStream(MediaStreamType.kMediaStreamTypeVideo);
                StartRoomHeartbeat(_lifetimeCancellation.Token);
            }
            catch
            {
                await LeaveAsync();
                throw;
            }
        }

        internal void PushFrame(XmaxVideoFrame frame)
        {
            if (_engine == null || _room == null)
            {
                throw new XmaxException(XmaxErrorCode.RtcError, "RTC room is not joined.");
            }
            frame.Validate();

            var rtcFrame = new RtcVideoFrame
            {
                FrameType = VideoFrameType.kVideoFrameTypeRawMemory,
                PixelFormat = ToRtcPixelFormat(frame.PixelFormat),
                TimestampUs = frame.TimestampMicroseconds,
                Width = frame.Width,
                Height = frame.Height,
                Rotation = (VideoRotation)(int)frame.Rotation,
                ColorSpace = bytertc.ColorSpace.kColorSpaceUnknown,
                NumberOfPlanes = frame.PixelFormat == XmaxVideoPixelFormat.I420 ? 3 : 1,
                PlaneData = frame.Planes,
                PlaneLineSize = frame.Strides,
                ExtraDataInfo = Array.Empty<byte>(),
                ExtraDataInfoSize = 0,
                SupplementaryInfo = Array.Empty<byte>(),
                SupplementaryInfoSize = 0,
                Matrix = IdentityMatrix()
            };
            var pushResult = _engine.PushExternalVideoFrame(rtcFrame);
            if (pushResult == VideoTimestampWarning)
            {
                if (!_didLogVideoTimestampWarning)
                {
                    _didLogVideoTimestampWarning = true;
                    Debug.LogWarning(
                        "[XmaxSDK] RTC reported a video timestamp interval warning; " +
                        "the frame was accepted and streaming will continue.");
                }
                return;
            }
            CheckResult(pushResult, "PushExternalVideoFrame");
        }

        internal async Task<string> StartGenerationAsync(
            RealtimeContext context,
            RealtimeVideoFormat videoFormat,
            CancellationToken cancellationToken)
        {
            if (_room == null || _engine == null || string.IsNullOrEmpty(_localUserId))
            {
                throw new XmaxException(XmaxErrorCode.RtcError, "RTC room is not joined.");
            }
            if (!string.IsNullOrEmpty(_currentTaskId))
            {
                throw new XmaxException(XmaxErrorCode.RtcError, "Realtime generation is already active.");
            }

            _matchedRemoteStream = null;
            var taskId = $"task-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            _currentTaskId = taskId;
            _generationCompletion = new TaskCompletionSource<bool>();
            try
            {
                SendRoomMessage(RtcRoomEvent.Start(_localUserId, taskId, videoFormat, context));
                StartTaskSei(taskId, _lifetimeCancellation.Token);
                await AwaitWithTimeoutAsync(
                    _generationCompletion.Task,
                    GenerationTimeoutMilliseconds,
                    "Realtime generation start timed out.",
                    cancellationToken);
                return taskId;
            }
            catch
            {
                StopGeneration();
                throw;
            }
        }

        internal void ChangeGenerationCondition(
            RealtimeContext context,
            RealtimeVideoFormat videoFormat,
            string taskId)
        {
            if (_room == null || string.IsNullOrEmpty(_localUserId) || _currentTaskId != taskId)
            {
                throw new XmaxException(XmaxErrorCode.RtcError, "Realtime generation task does not match.");
            }
            SendRoomMessage(RtcRoomEvent.ChangeCondition(_localUserId, videoFormat, context));
        }

        internal void StopGeneration()
        {
            var taskId = _currentTaskId;
            _currentTaskId = string.Empty;
            _matchedRemoteStream = null;
            _generationCompletion?.TrySetException(new XmaxException(
                XmaxErrorCode.RtcError,
                "Realtime generation start was cancelled."));
            _generationCompletion = null;
            if (!string.IsNullOrEmpty(taskId) && _room != null && !string.IsNullOrEmpty(_localUserId))
            {
                TrySendRoomMessage(RtcRoomEvent.Stop(_localUserId, taskId));
            }
        }

        internal Task LeaveAsync()
        {
            StopGeneration();
            _lifetimeCancellation?.Cancel();
            _lifetimeCancellation?.Dispose();
            _lifetimeCancellation = null;
            _joinCompletion?.TrySetCanceled();
            _joinCompletion = null;

            var room = _room;
            _room = null;
            if (room != null)
            {
                UnbindRoomEvents(room);
                try { room.UnpublishStream(MediaStreamType.kMediaStreamTypeVideo); } catch { }
                try { room.LeaveRoom(); } catch { }
                try { room.Destroy(); } catch { }
            }

            var engine = _engine;
            _engine = null;
            if (engine != null)
            {
                UnbindEngineEvents(engine);
                try { engine.Release(); } catch { }
            }

            _remoteTrack = null;
            _roomId = string.Empty;
            _localUserId = string.Empty;
            _botName = string.Empty;
            _matchedRemoteStream = null;
            return Task.CompletedTask;
        }

        private void BindEngineEvents(RTCVideo engine)
        {
            engine.OnErrorEvent += OnEngineError;
            engine.OnSEIMessageReceivedEvent += OnSeiMessageReceived;
            engine.OnRemoteVideoSinkOnFrameEvent += OnRemoteVideoFrame;
        }

        private void UnbindEngineEvents(RTCVideo engine)
        {
            engine.OnErrorEvent -= OnEngineError;
            engine.OnSEIMessageReceivedEvent -= OnSeiMessageReceived;
            engine.OnRemoteVideoSinkOnFrameEvent -= OnRemoteVideoFrame;
        }

        private void BindRoomEvents(IRTCVideoRoom room)
        {
            room.OnRoomStateChangedEvent += OnRoomStateChanged;
            room.OnRoomErrorEvent += OnRoomError;
            room.OnUserPublishStreamEvent += OnUserPublishStream;
            room.OnUserUnPublishStreamEvent += OnUserUnpublishStream;
        }

        private void UnbindRoomEvents(IRTCVideoRoom room)
        {
            room.OnRoomStateChangedEvent -= OnRoomStateChanged;
            room.OnRoomErrorEvent -= OnRoomError;
            room.OnUserPublishStreamEvent -= OnUserPublishStream;
            room.OnUserUnPublishStreamEvent -= OnUserUnpublishStream;
        }

        private void OnEngineError(int error)
        {
            var exception = new XmaxException(XmaxErrorCode.RtcError, $"RTC engine error: {error}.");
            if (_joinCompletion != null && !_joinCompletion.Task.IsCompleted)
            {
                _joinCompletion.TrySetException(exception);
                return;
            }
            FatalError?.Invoke(exception);
        }

        private void OnRoomStateChanged(string roomId, string userId, int state, string extraInfo)
        {
            if (roomId != _roomId || userId != _localUserId)
            {
                return;
            }
            if (state == 0)
            {
                _joinCompletion?.TrySetResult(true);
            }
            else if (_joinCompletion != null && !_joinCompletion.Task.IsCompleted)
            {
                _joinCompletion?.TrySetException(new XmaxException(
                    XmaxErrorCode.RtcError,
                    $"RTC room state changed with error: {state}."));
            }
            else
            {
                FatalError?.Invoke(new XmaxException(
                    XmaxErrorCode.RtcError,
                    $"RTC room state changed with error: {state}."));
            }
        }

        private void OnRoomError(string roomId, int error)
        {
            if (roomId != _roomId)
            {
                return;
            }
            var exception = new XmaxException(XmaxErrorCode.RtcError, $"RTC room error: {error}.");
            if (_joinCompletion != null && !_joinCompletion.Task.IsCompleted)
            {
                _joinCompletion.TrySetException(exception);
                return;
            }
            FatalError?.Invoke(exception);
        }

        private void OnUserPublishStream(string roomId, string userId, MediaStreamType type)
        {
            if (roomId != _roomId || !IsVideo(type) || (!string.IsNullOrEmpty(_botName) && userId != _botName))
            {
                return;
            }
            var key = new RemoteStreamKey
            {
                RoomID = roomId,
                UserID = userId,
                streamIndex = StreamIndex.kStreamIndexMain
            };
            _engine?.SetRemoteVideoSink(key, VideoSinkPixelFormat.kI420);
            _room?.SubscribeStream(userId, MediaStreamType.kMediaStreamTypeVideo);
        }

        private void OnUserUnpublishStream(
            string roomId,
            string userId,
            MediaStreamType type,
            StreamRemoveReason reason)
        {
            if (roomId == _roomId && IsVideo(type) &&
                _matchedRemoteStream.HasValue && _matchedRemoteStream.Value.UserID == userId)
            {
                _matchedRemoteStream = null;
            }
        }

        private void OnSeiMessageReceived(RemoteStreamKey key, byte[] buffer)
        {
            var taskId = _currentTaskId;
            if (string.IsNullOrEmpty(taskId) || key.RoomID != _roomId ||
                (!string.IsNullOrEmpty(_botName) && key.UserID != _botName))
            {
                return;
            }
            var message = Encoding.UTF8.GetString(buffer ?? Array.Empty<byte>()).Trim('\0', ' ', '\r', '\n', '\t');
            if (message != taskId || _generationCompletion == null || _generationCompletion.Task.IsCompleted)
            {
                return;
            }
            _matchedRemoteStream = key;
            _ = ResolveGenerationAfterDelayAsync(taskId, _lifetimeCancellation?.Token ?? CancellationToken.None);
        }

        private bool OnRemoteVideoFrame(RemoteStreamKey key, RtcVideoFrame frame)
        {
            if (!_matchedRemoteStream.HasValue || _matchedRemoteStream.Value != key ||
                frame.PixelFormat != VideoPixelFormat.kVideoPixelFormatI420 || frame.NumberOfPlanes < 3)
            {
                return true;
            }
            try
            {
                _remoteTrack?.RaiseFrame(XmaxVideoFrame.CreateI420(
                    frame.PlaneData[0],
                    frame.PlaneData[1],
                    frame.PlaneData[2],
                    frame.Width,
                    frame.Height,
                    frame.PlaneLineSize[0],
                    frame.PlaneLineSize[1],
                    frame.PlaneLineSize[2],
                    frame.TimestampUs,
                    (XmaxVideoRotation)(int)frame.Rotation));
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            return true;
        }

        private async Task ResolveGenerationAfterDelayAsync(string taskId, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(RemoteDisplayDelayMilliseconds, cancellationToken);
                if (_currentTaskId == taskId)
                {
                    _generationCompletion?.TrySetResult(true);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async void StartTaskSei(string taskId, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _currentTaskId == taskId)
                {
                    var data = Encoding.UTF8.GetBytes(taskId);
                    var result = _engine?.SendSEIMessage(
                        data,
                        data.Length,
                        (int)StreamIndex.kStreamIndexMain,
                        1,
                        0) ?? -1;
                    if (result < 0)
                    {
                        _generationCompletion?.TrySetException(new XmaxException(
                            XmaxErrorCode.RtcError,
                            $"SendSEIMessage failed: {result}."));
                        return;
                    }
                    await Task.Delay(TaskSeiMilliseconds, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async void StartRoomHeartbeat(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(RoomHeartbeatMilliseconds, cancellationToken);
                    if (_room != null && !string.IsNullOrEmpty(_localUserId))
                    {
                        TrySendRoomMessage(RtcRoomEvent.Heartbeat(_localUserId));
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void SendRoomMessage(string message)
        {
            if (_room == null)
            {
                throw new XmaxException(XmaxErrorCode.RtcError, "RTC room is not joined.");
            }
            var result = _room.SendRoomMessage(message);
            if (result < 0)
            {
                throw new XmaxException(XmaxErrorCode.RtcError, $"SendRoomMessage failed: {result}.");
            }
        }

        private void TrySendRoomMessage(string message)
        {
            try { SendRoomMessage(message); } catch { }
        }

        private static async Task AwaitWithTimeoutAsync(
            Task task,
            int timeoutMilliseconds,
            string timeoutMessage,
            CancellationToken cancellationToken)
        {
            var delay = Task.Delay(timeoutMilliseconds, cancellationToken);
            var completed = await Task.WhenAny(task, delay);
            cancellationToken.ThrowIfCancellationRequested();
            if (completed != task)
            {
                throw new XmaxException(XmaxErrorCode.Timeout, timeoutMessage);
            }
            await task;
        }

        private static bool IsVideo(MediaStreamType type)
        {
            return type == MediaStreamType.kMediaStreamTypeVideo || type == MediaStreamType.kMediaStreamTypeBoth;
        }

        private static int EstimateBitrate(RealtimeVideoFormat format)
        {
            return Math.Max(300, (int)Math.Ceiling(format.Width * format.Height * format.Fps * 0.1 / 1000.0));
        }

        private static VideoPixelFormat ToRtcPixelFormat(XmaxVideoPixelFormat format)
        {
            switch (format)
            {
                case XmaxVideoPixelFormat.I420:
                    return VideoPixelFormat.kVideoPixelFormatI420;
                case XmaxVideoPixelFormat.Rgba:
                    return VideoPixelFormat.kVideoPixelFormatRGBA;
                case XmaxVideoPixelFormat.Bgra:
                    return VideoPixelFormat.kVideoPixelFormatBGRA;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        private static float[] IdentityMatrix()
        {
            return new[]
            {
                1f, 0f, 0f, 0f,
                0f, 1f, 0f, 0f,
                0f, 0f, 1f, 0f,
                0f, 0f, 0f, 1f
            };
        }

        private static void CheckResult(int result, string operation)
        {
            if (result < 0)
            {
                throw new XmaxException(XmaxErrorCode.RtcError, $"{operation} failed: {result}.");
            }
        }
    }
}
