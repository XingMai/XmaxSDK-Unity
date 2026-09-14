using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using bytertc;
using UnityEngine;
using RtcVideoFrame = bytertc.VideoFrame;

namespace Xmax.SDK
{
    /// <summary>
    /// 封装 Android 原生 RTC 引擎、房间及主视频流，并将厂商数据转换为 SDK 类型。
    /// </summary>
    internal sealed class RtcManager : IRtcManager
    {
        // 原生调用约定。
        private const int JoinTimeoutMilliseconds = 15000;
        private const int VideoTimestampWarning = -202;

        // 当前持有的原生资源与独占租约。
        private RTCVideo _engine;
        private IRTCVideoRoom _room;
        private IDisposable _lease;

        // 当前入房操作及回调身份。
        private TaskCompletionSource<bool> _joinCompletion;
        private string _roomId = string.Empty;
        private string _localUserId = string.Empty;

        // 单次连接的诊断去重状态。
        private bool _didLogVideoTimestampWarning;

        /// <summary>
        /// 入房等待之外的引擎或房间致命错误。
        /// </summary>
        public event Action<XmaxException> FatalError;

        /// <summary>
        /// 当前房间的本地网络质量统计更新。
        /// </summary>
        public event Action<RtcNetworkQuality> NetworkQualityChanged;

        /// <summary>
        /// 当前房间的远端用户发布包含视频的流时触发。
        /// </summary>
        public event Action<RemoteStream> VideoPublished;

        /// <summary>
        /// 当前房间的远端用户取消发布视频时触发。
        /// </summary>
        public event Action<RemoteStream> VideoUnpublished;

        /// <summary>
        /// 当前房间主流收到 SEI 数据时触发。
        /// </summary>
        public event Action<RemoteStream, byte[]> SeiReceived;

        /// <summary>
        /// 当前房间远端主流收到可转换的 I420 帧时触发。
        /// </summary>
        public event Action<RemoteStream, XmaxVideoFrame> FrameReceived;

        /// <summary>
        /// 校验当前运行平台支持原生 RTC；目前仅支持 Android Player。
        /// </summary>
        public void ValidatePlatform()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                throw new XmaxException(
                    XmaxErrorCode.NotSupported,
                    "XmaxSDK RTC currently supports Android Player only. RTC cannot be started in the Unity Editor.");
            }

        }

        /// <summary>
        /// 获取独占引擎租约并加入 RTC 房间，成功后发布外部视频；失败或取消时释放资源。
        /// </summary>
        /// <param name="joinInfo">经过校验的 RTC 应用、房间、用户及入房令牌。</param>
        /// <param name="configuration">已校验并解析码率范围的完整视频编码参数。</param>
        /// <param name="cancellationToken">调用方取消令牌；取消后停止等待或撤销当前操作。</param>
        /// <returns>房间加入完成并已请求发布视频的任务。</returns>
        public async Task JoinAsync(
            RtcJoinInfo joinInfo,
            VideoEncodingConfiguration configuration,
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
                CheckResult(
                    _engine.SetVideoEncoderConfig1(ToRtcVideoEncoderConfig(configuration)),
                    "SetVideoEncoderConfig");

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

        /// <summary>
        /// 校验并向已加入房间的原生引擎推送外部视频帧。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
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

        /// <summary>
        /// 取消入房等待，解除回调并尽力销毁房间、引擎和独占租约。
        /// </summary>
        /// <returns>本地 RTC 资源清理流程已结束的任务。</returns>
        public Task LeaveAsync()
        {
            _joinCompletion?.TrySetCanceled();
            _joinCompletion = null;

            var room = _room;
            _room = null;
            if (room != null)
            {
                UnbindRoomEvents(room);

                try
                {
                    room.UnpublishStream(MediaStreamType.kMediaStreamTypeVideo);
                }
                catch
                {
                }

                try
                {
                    room.LeaveRoom();
                }
                catch
                {
                }

                try
                {
                    room.Destroy();
                }
                catch
                {
                }
            }

            var engine = _engine;
            _engine = null;
            if (engine != null)
            {
                UnbindEngineEvents(engine);

                try
                {
                    engine.Release();
                }
                catch
                {
                }
            }

            _roomId = string.Empty;
            _localUserId = string.Empty;
            _lease?.Dispose();
            _lease = null;

            return Task.CompletedTask;
        }

        /// <summary>
        /// 订阅当前原生引擎的错误、SEI 和视频帧事件。
        /// </summary>
        /// <param name="engine">待绑定或解绑回调的原生 RTC 引擎。</param>
        private void BindEngineEvents(RTCVideo engine)
        {
            engine.OnErrorEvent += OnEngineError;
            engine.OnSEIMessageReceivedEvent += OnSeiMessageReceived;
            engine.OnRemoteVideoSinkOnFrameEvent += OnRemoteVideoFrame;
        }

        /// <summary>
        /// 解除当前原生引擎的全部 SDK 事件订阅。
        /// </summary>
        /// <param name="engine">待绑定或解绑回调的原生 RTC 引擎。</param>
        private void UnbindEngineEvents(RTCVideo engine)
        {
            engine.OnErrorEvent -= OnEngineError;
            engine.OnSEIMessageReceivedEvent -= OnSeiMessageReceived;
            engine.OnRemoteVideoSinkOnFrameEvent -= OnRemoteVideoFrame;
        }

        /// <summary>
        /// 订阅房间状态、网络质量及视频发布事件。
        /// </summary>
        /// <param name="room">待绑定或解绑回调的原生 RTC 房间。</param>
        private void BindRoomEvents(IRTCVideoRoom room)
        {
            room.OnNetworkQualityEvent += OnNetworkQuality;
            room.OnRoomStateChangedEvent += OnRoomStateChanged;
            room.OnRoomErrorEvent += OnRoomError;
            room.OnUserPublishStreamEvent += OnUserPublishStream;
            room.OnUserUnPublishStreamEvent += OnUserUnpublishStream;
        }

        /// <summary>
        /// 解除房间的全部 SDK 事件订阅。
        /// </summary>
        /// <param name="room">待绑定或解绑回调的原生 RTC 房间。</param>
        private void UnbindRoomEvents(IRTCVideoRoom room)
        {
            room.OnNetworkQualityEvent -= OnNetworkQuality;
            room.OnRoomStateChangedEvent -= OnRoomStateChanged;
            room.OnRoomErrorEvent -= OnRoomError;
            room.OnUserPublishStreamEvent -= OnUserPublishStream;
            room.OnUserUnPublishStreamEvent -= OnUserUnpublishStream;
        }

        /// <summary>
        /// 只转发当前房间的本地上下行网络质量。
        /// </summary>
        /// <param name="roomId">RTC 房间标识，用于过滤过期或其他房间的回调。</param>
        /// <param name="local">厂商提供的本地上下行网络统计。</param>
        /// <param name="remote">厂商提供的远端用户统计列表，当前转换只使用本地统计。</param>
        /// <param name="count">厂商提供的远端统计数量，当前实现不使用。</param>
        private void OnNetworkQuality(
            string roomId,
            NetworkQualityStats local,
            List<NetworkQualityStats> remote,
            int count)
        {
            if (_room != null && roomId == _roomId)
                EventDispatch.Raise(NetworkQualityChanged, RtcQualityConverter.Convert(local));
        }

        /// <summary>
        /// 入房期间使入房任务失败，否则向上层报告致命错误。
        /// </summary>
        /// <param name="error">厂商报告的引擎或房间错误码。</param>
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

        /// <summary>
        /// 处理当前本地用户的入房结果及后续房间错误。
        /// </summary>
        /// <param name="roomId">RTC 房间标识，用于过滤过期或其他房间的回调。</param>
        /// <param name="userId">RTC 用户标识。</param>
        /// <param name="state">厂商房间状态码，0 表示入房成功。</param>
        /// <param name="extraInfo">厂商提供的房间状态附加信息，当前实现不解析。</param>
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

        /// <summary>
        /// 只处理当前房间错误，优先结束尚未完成的入房等待。
        /// </summary>
        /// <param name="roomId">RTC 房间标识，用于过滤过期或其他房间的回调。</param>
        /// <param name="error">厂商报告的引擎或房间错误码。</param>
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

        /// <summary>
        /// 当前房间有用户发布包含视频的流时通知上层。
        /// </summary>
        /// <param name="roomId">RTC 房间标识，用于过滤过期或其他房间的回调。</param>
        /// <param name="userId">RTC 用户标识。</param>
        /// <param name="type">厂商发布或取消发布的媒体类型。</param>
        private void OnUserPublishStream(string roomId, string userId, MediaStreamType type)
        {
            if (roomId == _roomId && IsVideo(type))
                VideoPublished?.Invoke(new RemoteStream(roomId, userId));
        }

        /// <summary>
        /// 当前房间有用户取消发布视频时通知上层。
        /// </summary>
        /// <param name="roomId">RTC 房间标识，用于过滤过期或其他房间的回调。</param>
        /// <param name="userId">RTC 用户标识。</param>
        /// <param name="type">厂商发布或取消发布的媒体类型。</param>
        /// <param name="reason">厂商报告的取消发布原因，当前实现不区分原因。</param>
        private void OnUserUnpublishStream(
            string roomId,
            string userId,
            MediaStreamType type,
            StreamRemoveReason reason)
        {
            if (roomId == _roomId && IsVideo(type))
                VideoUnpublished?.Invoke(new RemoteStream(roomId, userId));
        }

        /// <summary>
        /// 为当前房间的指定远端用户设置 I420 视频接收并订阅主流。
        /// </summary>
        /// <param name="key">由房间和用户共同标识的远端主流。</param>
        public void SubscribeVideo(RemoteStream key)
        {
            if (_engine == null || _room == null || key.RoomId != _roomId)
                return;

            _engine.SetRemoteVideoSink(ToNative(key), VideoSinkPixelFormat.kI420);
            _room.SubscribeStream(key.UserId, MediaStreamType.kMediaStreamTypeVideo);
        }

        /// <summary>
        /// 仅转发当前房间主流的 SEI 数据。
        /// </summary>
        /// <param name="key">厂商回调或原生调用使用的远端流标识。</param>
        /// <param name="buffer">远端 SEI 原始字节数据。</param>
        private void OnSeiMessageReceived(RemoteStreamKey key, byte[] buffer)
        {
            if (_engine != null && key.RoomID == _roomId && key.streamIndex == StreamIndex.kStreamIndexMain)
                SeiReceived?.Invoke(FromNative(key), buffer);
        }

        /// <summary>
        /// 过滤远端主流 I420 帧并转换为 SDK 帧，隔离转换或回调异常。
        /// </summary>
        /// <param name="key">厂商回调或原生调用使用的远端流标识。</param>
        /// <param name="frame">厂商回调提供的远端帧，仅处理至少三个平面的 I420 格式。</param>
        /// <returns>始终为 true，表示该原生视频回调已处理。</returns>
        private bool OnRemoteVideoFrame(RemoteStreamKey key, RtcVideoFrame frame)
        {
            if (_engine == null || key.RoomID != _roomId || key.streamIndex != StreamIndex.kStreamIndexMain ||
                frame.PixelFormat != VideoPixelFormat.kVideoPixelFormatI420 || frame.NumberOfPlanes < 3)
                return true;

            try
            {
                var converted = XmaxVideoFrame.CreateI420(frame.PlaneData[0], frame.PlaneData[1], frame.PlaneData[2],
                    frame.Width, frame.Height, frame.PlaneLineSize[0], frame.PlaneLineSize[1], frame.PlaneLineSize[2],
                    frame.TimestampUs, (XmaxVideoRotation)(int)frame.Rotation);
                FrameReceived?.Invoke(FromNative(key), converted);
            }
            catch (Exception exception)
            {
                XmaxLogger.Rtc.Failure(exception);
            }

            return true;
        }

        /// <summary>
        /// 向当前主流发送 SEI 数据，原生调用失败时抛出 SDK 错误。
        /// </summary>
        /// <param name="data">用于传输或匹配的 SEI 字节数据。</param>
        public void SendSei(byte[] data)
        {
            CheckResult(
                _engine?.SendSEIMessage(data, data.Length, (int)StreamIndex.kStreamIndexMain, 1, 0) ?? -1,
                "SendSEIMessage");
        }

        /// <summary>
        /// 将厂商远端流标识转换为 SDK 房间和用户标识。
        /// </summary>
        /// <param name="key">厂商回调或原生调用使用的远端流标识。</param>
        /// <returns>不包含厂商流索引的远端流标识。</returns>
        private static RemoteStream FromNative(RemoteStreamKey key) => new RemoteStream(key.RoomID, key.UserID);

        /// <summary>
        /// 将 SDK 远端流标识转换为厂商主流标识。
        /// </summary>
        /// <param name="key">由房间和用户共同标识的远端主流。</param>
        /// <returns>流索引固定为主流的厂商标识。</returns>
        private static RemoteStreamKey ToNative(RemoteStream key) => new RemoteStreamKey
        {
            RoomID = key.RoomId,
            UserID = key.UserId,
            streamIndex = StreamIndex.kStreamIndexMain
        };

        /// <summary>
        /// 向当前 RTC 房间发送业务消息，负返回值转换为 SDK 错误。
        /// </summary>
        /// <param name="message">按房间协议编码的业务消息文本。</param>
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

        /// <summary>
        /// 判断发布类型是否包含视频。
        /// </summary>
        /// <param name="type">厂商发布或取消发布的媒体类型。</param>
        /// <returns>纯视频或音视频类型为 true。</returns>
        private static bool IsVideo(MediaStreamType type)
        {
            return type == MediaStreamType.kMediaStreamTypeVideo || type == MediaStreamType.kMediaStreamTypeBoth;
        }

        /// <summary>
        /// 将完整视频编码参数转换为厂商配置，不计算或覆盖码率。
        /// </summary>
        /// <param name="configuration">已校验并解析默认值的编码参数。</param>
        /// <returns>用于主视频流的厂商编码配置。</returns>
        /// <exception cref="XmaxException">编码偏好不是 SDK 支持的枚举值。</exception>
        internal static VideoEncoderConfig ToRtcVideoEncoderConfig(VideoEncodingConfiguration configuration)
        {
            VideoEncodePreference preference;
            switch (configuration.EncoderPreference)
            {
                case RealtimeVideoEncoderPreference.Auto:
                    preference = VideoEncodePreference.kVideoEncodePreferenceBalance;
                    break;
                case RealtimeVideoEncoderPreference.MaintainFramerate:
                    preference = VideoEncodePreference.kVideoEncodePreferenceFramerate;
                    break;
                case RealtimeVideoEncoderPreference.MaintainQuality:
                    preference = VideoEncodePreference.kVideoEncodePreferenceQuality;
                    break;
                default:
                    throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Unknown video encoder preference.");
            }

            return new VideoEncoderConfig
            {
                Width = configuration.Width,
                Height = configuration.Height,
                FrameRate = configuration.FrameRate,
                MinBitrate = configuration.MinimumBitrate,
                MaxBitrate = configuration.MaximumBitrate,
                EncoderPreference = preference
            };
        }

        /// <summary>
        /// 将 SDK 像素格式映射为厂商像素格式。
        /// </summary>
        /// <param name="format">视频内存的像素排列格式。</param>
        /// <returns>对应的厂商像素格式。</returns>
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

        /// <summary>
        /// 创建传给原生外部视频帧的四阶单位矩阵。
        /// </summary>
        /// <returns>按行排列的 16 个矩阵元素。</returns>
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

        /// <summary>
        /// 将原生调用的负返回值转换为 RTC 异常。
        /// </summary>
        /// <param name="result">原生调用返回值，负数表示失败。</param>
        /// <param name="operation">用于错误说明的原生操作名称。</param>
        private static void CheckResult(int result, string operation)
        {
            if (result < 0)
            {
                throw new XmaxException(XmaxErrorCode.RtcError, $"{operation} failed: {result}.");
            }
        }
    }
}
