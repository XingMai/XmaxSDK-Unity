using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bytertc;
using UnityEngine;
using RtcVideoFrame = bytertc.VideoFrame;

namespace Xmax.SDK
{
    internal sealed class RtcManager : IRtcManager
    {
        private const int JoinTimeoutMilliseconds = 15000;
        private const int VideoTimestampWarning = -202;
        private RTCVideo _engine;
        private IRTCVideoRoom _room;
        private IDisposable _lease;
        private TaskCompletionSource<bool> _joinCompletion;
        private string _roomId = string.Empty;
        private string _localUserId = string.Empty;
        private bool _didLogVideoTimestampWarning;
        public event Action<XmaxException> FatalError;
        public event Action<RtcNetworkQuality> NetworkQualityChanged;
        public event Action<RemoteStream> VideoPublished;
        public event Action<RemoteStream> VideoUnpublished;
        public event Action<RemoteStream, byte[]> SeiReceived;
        public event Action<RemoteStream, XmaxVideoFrame> FrameReceived;
        public void ValidatePlatform()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                throw new XmaxException(
                    XmaxErrorCode.NotSupported,
                    "XmaxSDK RTC currently supports Android Player only. RTC cannot be started in the Unity Editor.");
            }

        }
        public async Task JoinAsync(
            RtcJoinInfo joinInfo,
            RealtimeVideoFormat videoFormat,
            CancellationToken cancellationToken)
        {
            ValidatePlatform();
            await LeaveAsync();
            cancellationToken.ThrowIfCancellationRequested();

            _lease = await RtcEngineManager.AcquireAsync(cancellationToken);
            _roomId = joinInfo.RoomId;
            _localUserId = joinInfo.UserId;
            _didLogVideoTimestampWarning = false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
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

                _joinCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                var joinResult = _room.JoinRoom(
                    joinInfo.Token,
                    new UserInfo { UserID = joinInfo.UserId, ExtraInfo = string.Empty },
                    new MultiRoomConfig(
                        RoomProfileType.kRoomProfileTypeCommunication,
                        false,
                        false,
                        false));
                CheckResult(joinResult, "JoinRoom");
                await AsyncDeadline.WaitAsync(
                    _joinCompletion.Task,
                    JoinTimeoutMilliseconds,
                    cancellationToken);

                _room.PublishStream(MediaStreamType.kMediaStreamTypeVideo);
            }
            catch
            {
                await LeaveAsync();
                throw;
            }
        }

        public void PushFrame(XmaxVideoFrame frame)
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
                    XmaxLogger.Rtc.Warn(() =>
                        "RTC reported a video timestamp interval warning; " +
                        "the frame was accepted and streaming will continue.");
                }
                return;
            }
            CheckResult(pushResult, "PushExternalVideoFrame");
        }

        public Task LeaveAsync()
        {
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

            _roomId = string.Empty;
            _localUserId = string.Empty;
            _lease?.Dispose();
            _lease = null;
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
            room.OnNetworkQualityEvent += OnNetworkQuality;
            room.OnRoomStateChangedEvent += OnRoomStateChanged;
            room.OnRoomErrorEvent += OnRoomError;
            room.OnUserPublishStreamEvent += OnUserPublishStream;
            room.OnUserUnPublishStreamEvent += OnUserUnpublishStream;
        }

        private void UnbindRoomEvents(IRTCVideoRoom room)
        {
            room.OnNetworkQualityEvent -= OnNetworkQuality;
            room.OnRoomStateChangedEvent -= OnRoomStateChanged;
            room.OnRoomErrorEvent -= OnRoomError;
            room.OnUserPublishStreamEvent -= OnUserPublishStream;
            room.OnUserUnPublishStreamEvent -= OnUserUnpublishStream;
        }

        private void OnNetworkQuality(string roomId, NetworkQualityStats local, List<NetworkQualityStats> remote, int count)
        {
            if (_room != null && roomId == _roomId)
                EventDispatch.Raise(NetworkQualityChanged, RtcQualityConverter.Convert(local));
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
            if (roomId == _roomId && IsVideo(type)) VideoPublished?.Invoke(new RemoteStream(roomId, userId));
        }
        private void OnUserUnpublishStream(string roomId, string userId, MediaStreamType type, StreamRemoveReason reason)
        {
            if (roomId == _roomId && IsVideo(type)) VideoUnpublished?.Invoke(new RemoteStream(roomId, userId));
        }
        public void SubscribeVideo(RemoteStream key)
        {
            if (_engine == null || _room == null || key.RoomId != _roomId) return;
            _engine.SetRemoteVideoSink(ToNative(key), VideoSinkPixelFormat.kI420);
            _room.SubscribeStream(key.UserId, MediaStreamType.kMediaStreamTypeVideo);
        }
        private void OnSeiMessageReceived(RemoteStreamKey key, byte[] buffer)
        {
            if (_engine != null && key.RoomID == _roomId && key.streamIndex == StreamIndex.kStreamIndexMain)
                SeiReceived?.Invoke(FromNative(key), buffer);
        }
        private bool OnRemoteVideoFrame(RemoteStreamKey key, RtcVideoFrame frame)
        {
            if (_engine == null || key.RoomID != _roomId || key.streamIndex != StreamIndex.kStreamIndexMain ||
                frame.PixelFormat != VideoPixelFormat.kVideoPixelFormatI420 || frame.NumberOfPlanes < 3) return true;
            try
            {
                var converted = XmaxVideoFrame.CreateI420(frame.PlaneData[0], frame.PlaneData[1], frame.PlaneData[2],
                    frame.Width, frame.Height, frame.PlaneLineSize[0], frame.PlaneLineSize[1], frame.PlaneLineSize[2],
                    frame.TimestampUs, (XmaxVideoRotation)(int)frame.Rotation);
                FrameReceived?.Invoke(FromNative(key), converted);
            }
            catch (Exception exception) { XmaxLogger.Rtc.Failure(exception); }
            return true;
        }
        public void SendSei(byte[] data)
        {
            CheckResult(_engine?.SendSEIMessage(data, data.Length, (int)StreamIndex.kStreamIndexMain, 1, 0) ?? -1, "SendSEIMessage");
        }
        private static RemoteStream FromNative(RemoteStreamKey key) => new RemoteStream(key.RoomID, key.UserID);
        private static RemoteStreamKey ToNative(RemoteStream key) => new RemoteStreamKey { RoomID = key.RoomId, UserID = key.UserId, streamIndex = StreamIndex.kStreamIndexMain };
        public void SendRoomMessage(string message)
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

        private static bool IsVideo(MediaStreamType type)
        {
            return type == MediaStreamType.kMediaStreamTypeVideo || type == MediaStreamType.kMediaStreamTypeBoth;
        }

        private static int EstimateBitrate(RealtimeVideoFormat format)
        {
            return Math.Max(300, (int)Math.Ceiling((double)format.Width * format.Height * format.Fps * 0.1 / 1000.0));
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
