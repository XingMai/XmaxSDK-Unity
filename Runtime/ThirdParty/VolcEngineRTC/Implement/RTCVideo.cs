using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;
using UNBridgeLib.LitJson;

namespace bytertc
{
    public class RTCVideo : IRTCVideo
    {
        #region Events
        public event OnWarningEventHandler OnWarningEvent;

        public event OnErrorEventHandler OnErrorEvent;

        public event OnLogReportEventHandler OnLogReportEvent;


        public event OnConnectionStateChangedEventHandler OnConnectionStateChangedEvent;

        public event OnNetworkTypeChangedEventHandler OnNetworkTypeChangedEvent;

        public event OnSysStatsEventHandler OnSysStatsEvent;

        // audio
        public event OnAudioRouteChangedEventHandler OnAudioRouteChangedEvent;

        public event OnAudioDeviceStateChangedEventHandler OnAudioDeviceStateChangedEvent;

        public event OnAudioDeviceWarningEventHandler OnAudioDeviceWarningEvent;

        public event OnLocalAudioPropertiesReportEventHandler OnLocalAudioPropertiesReportEvent;

        public event OnRemoteAudioStateChangedEventHandler OnRemoteAudioStateChangedEvent;

        public event OnRemoteAudioPropertiesReportEventHandler OnRemoteAudioPropertiesReportEvent;

        public event OnCreateRoomStateChangedEventHandler OnCreateRoomStateChangedEvent;

        public event OnAudioRecordingStateUpdateEventHandler OnAudioRecordingStateUpdateEvent;

        public event OnRecordAudioFrameOriginalEventHandler OnRecordAudioFrameOriginalEvent;

        public event OnRecordAudioFrameEventHandler OnRecordAudioFrameEvent;

        public event OnPlaybackAudioFrameEventHandler OnPlaybackAudioFrameEvent;

        public event OnRemoteUserAudioFrameEventHandler OnRemoteUserAudioFrameEvent;

        public event OnMixedAudioFrameEventHandler OnMixedAudioFrameEvent;

        public event OnRecordScreenAudioFrameEventHandler OnRecordScreenAudioFrameEvent;

        //video
        public event OnVideoDeviceStateChangedEventHandler OnVideoDeviceStateChangedEvent;

        public event OnVideoDeviceWarningEventHandler OnVideoDeviceWarningEvent;

        // local video sink
        public event OnLocalVideoSinkOnFrameEventHandler OnLocalVideoSinkOnFrameEvent;

        public event OnLocalVideoSinkGetRenderElapseEventHandler OnLocalVideoSinkGetRenderElapseEvent;

        public event OnLocalVideoSinkReleaseEventHandler OnLocalVideoSinkReleaseEvent;

        // remote video sink
        public event OnRemoteVideoSinkOnFrameEventHandler OnRemoteVideoSinkOnFrameEvent;

        public event OnRemoteVideoSinkGetRenderElapseEventHandler OnRemoteVideoSinkGetRenderElapseEvent;

        public event OnRemoteVideoSinkReleaseEventHandler OnRemoteVideoSinkReleaseEvent;

        // room audio
        public event OnUserStartAudioCaptureEventHandler OnUserStartAudioCaptureEvent;

        public event OnUserStopAudioCaptureEventHandler OnUserStopAudioCaptureEvent;

        public event OnFirstLocalAudioFrameEventHandler OnFirstLocalAudioFrameEvent;

        public event OnLocalAudioStateChangedEventHandler OnLocalAudioStateChangedEvent;

        public event OnAudioFrameSendStateChangedEventHandler OnAudioFrameSendStateChangedEvent;

        public event OnAudioFramePlayStateChangedEventHandler OnAudioFramePlayStateChangedEvent;

        public event OnProcessRecordAudioFrameEventHandler OnProcessRecordAudioFrameEvent;

        public event OnProcessPlaybackAudioFrameEventHandler OnProcessPlaybackAudioFrameEvent;

        public event OnProcessRemoteUserAudioFrameEventHandler OnProcessRemoteUserAudioFrameEvent;

        public event OnProcessEarMonitorAudioFrameEventHandler OnProcessEarMonitorAudioFrameEvent;

        public event OnProcessScreenAudioFrameEventHandler OnProcessScreenAudioFrameEvent;

        public event OnStreamSyncInfoReceivedEventHandler OnStreamSyncInfoReceivedEvent;

        public event OnSEIMessageReceivedEventHandler OnSEIMessageReceivedEvent;
        public event OnSEIMessageUpdateEventHandler OnSEIMessageUpdateEvent;

        // room video
        public event OnUserStartVideoCaptureEventHandler OnUserStartVideoCaptureEvent;

        public event OnUserStopVideoCaptureEventHandler OnUserStopVideoCaptureEvent;

        public event OnFirstLocalVideoFrameCapturedEventHandler OnFirstLocalVideoFrameCapturedEvent;

        public event OnLocalVideoSizeChangedEventHandler OnLocalVideoSizeChangedEvent;

        public event OnLocalVideoStateChangedEventHandler OnLocalVideoStateChangedEvent;

        public event OnVideoFrameSendStateChangedEventHandler OnVideoFrameSendStateChangedEvent;

        public event OnScreenVideoFrameSendStateChangedEventHandler OnScreenVideoFrameSendStateChangedEvent;

        public event OnVideoFramePlayStateChangedEventHandler OnVideoFramePlayStateChangedEvent;

        public event OnRemoteVideoSizeChangedEventHandler OnRemoteVideoSizeChangedEvent;  //349 add

        public event OnRemoteVideoStateChangedEventHandler OnRemoteVideoStateChangedEvent;  //349 add

        public event OnFirstRemoteAudioFrameEventHandler OnFirstRemoteAudioFrameEvent;

        public event OnFirstRemoteVideoFrameDecodedEventHandler OnFirstRemoteVideoFrameDecodedEvent;

        public event OnFirstRemoteVideoFrameRenderedEventHandler OnFirstRemoteVideoFrameRenderedEvent;

        public event OnPlayPublicStreamResultEventHandler OnPlayPublicStreamResultEvent;
        public event OnFaceDetectResultEventHandler OnFaceDetectResultEvent;
        public event OnExpressionDetectResultEventHandler OnExpressionDetectResultEvent;

        #endregion


        #region Variables
        private static RTCVideo _instance = null;

        private static int _roomCount = 0;

        private RTCVideoEngineParams _initParams;


        private IntPtr _videoEngine = default(IntPtr);

        private VideoSDKEngineCallback _callback;


        #endregion

        public int CreateRTCVideo(RTCVideoEngineParams initParams)
        {
            _instance = this;
            _initParams = initParams;
            _initParams.Params = initParams.Params;
            Loom.Initialize();
            if (_videoEngine != default(IntPtr))
            {
                ByteRTCLog.LogInfo("RTCEngine has been initialized.");
                return -1;
            }
            if (_initParams.Params == null)
            {
                _initParams.Params = new Dictionary<string, object>();
            }

            string parameter = JsonMapper.ToJson(_initParams.Params);

            //callback
            _callback.OnWarning = OnWarning;
            _callback.OnError = OnError;
            _callback.OnLogReport = OnLogReport;

            _callback.OnConnectionStateChanged = OnConnectionStateChanged;
            _callback.OnNetworkTypeChanged = OnNetworkTypeChanged;
            _callback.OnSysStats = OnSysStats;

            _callback.OnAudioRouteChanged = OnAudioRouteChanged;
            _callback.OnAudioDeviceStateChanged = OnAudioDeviceStateChanged;
            _callback.OnAudioDeviceWarning = OnAudioDeviceWarning;
            _callback.OnLocalAudioPropertiesReport = OnLocalAudioPropertiesReport;
            _callback.OnRemoteAudioPropertiesReport = OnRemoteAudioPropertiesReport;

            _callback.OnCreateRoomStateChanged = OnCreateRoomStateChanged;
            _callback.OnAudioRecordingStateUpdate = OnAudioRecordingStateUpdate;
            _callback.OnRecordAudioFrameOriginal = OnRecordAudioFrameOriginal;
            _callback.OnRecordAudioFrame = OnRecordAudioFrame;
            _callback.OnPlaybackAudioFrame = OnPlaybackAudioFrame;
            _callback.OnRemoteUserAudioFrame = OnRemoteUserAudioFrame;
            _callback.OnMixedAudioFrame = OnMixedAudioFrame;
            _callback.OnRecordScreenAudioFrame = OnRecordScreenAudioFrame;

            _callback.OnProcessRecordAudioFrame = OnProcessRecordAudioFrame;
            _callback.OnProcessPlaybackAudioFrame = OnProcessPlaybackAudioFrame;
            _callback.OnProcessRemoteUserAudioFrame = OnProcessRemoteUserAudioFrame;
            _callback.OnProcessEarMonitorAudioFrame = OnProcessEarMonitorAudioFrame;
            _callback.OnProcessScreenAudioFrame = OnProcessScreenAudioFrame;
            _callback.OnStreamSyncInfoReceived = OnStreamSyncInfoReceived;

            _callback.OnSEIMessageReceived = OnSEIMessageReceived;
            _callback.OnSEIMessageUpdate = OnSEIMessageUpdate;

            _callback.OnVideoDeviceStateChanged = OnVideoDeviceStateChanged;
            _callback.OnVideoDeviceWarning = OnVideoDeviceWarning;

            _callback.OnLocalVideoSinkOnFrame = OnLocalVideoSinkOnFrame;
            _callback.OnLocalVideoSinkGetRenderElapse = OnLocalVideoSinkGetRenderElapse;
            _callback.OnLocalVideoSinkRelease = OnLocalVideoSinkRelease;

            _callback.OnRemoteVideoSinkOnFrame = OnRemoteVideoSinkOnFrame;
            _callback.OnRemoteVideoSinkGetRenderElapse = OnRemoteVideoSinkGetRenderElapse;
            _callback.OnRemoteVideoSinkRelease = OnRemoteVideoSinkRelease;

         
            _callback.OnUserStartAudioCapture = OnUserStartAudioCapture;
            _callback.OnUserStopAudioCapture = OnUserStopAudioCapture;
            _callback.OnFirstLocalAudioFrame = OnFirstLocalAudioFrame;
            _callback.OnLocalAudioStateChanged = OnLocalAudioStateChanged;
            _callback.OnAudioFrameSendStateChanged = OnAudioFrameSendStateChanged;
            _callback.OnAudioFramePlayStateChanged = OnAudioFramePlayStateChanged;

            _callback.OnUserStartVideoCapture = OnUserStartVideoCapture;
            _callback.OnUserStopVideoCapture = OnUserStopVideoCapture;
            _callback.OnFirstLocalVideoFrameCaptured = OnFirstLocalVideoFrameCaptured;
            _callback.OnLocalVideoSizeChanged = OnLocalVideoSizeChanged;
            _callback.OnLocalVideoStateChanged = OnLocalVideoStateChanged;
            _callback.OnVideoFrameSendStateChanged = OnVideoFrameSendStateChanged;
            _callback.OnScreenVideoFrameSendStateChanged = OnScreenVideoFrameSendStateChanged;
            _callback.OnVideoFramePlayStateChanged = OnVideoFramePlayStateChanged;

            _callback.OnRemoteVideoSizeChanged = OnRemoteVideoSizeChanged;
            _callback.OnRemoteVideoStateChanged = OnRemoteVideoStateChanged;

            _callback.OnRemoteAudioStateChanged = OnRemoteAudioStateChanged;

            _callback.OnFirstRemoteAudioFrame = OnFirstRemoteAudioFrame;
            _callback.OnFirstRemoteVideoFrameDecoded = OnFirstRemoteVideoFrameDecoded;
            _callback.OnFirstRemoteVideoFrameRendered = OnFirstRemoteVideoFrameRendered;
            _callback.OnPlayPublicStreamResult = OnPlayPublicStreamResult;
            _callback.OnFaceDetectResult = OnFaceDetectResult;
            _callback.OnExpressionDetectResult = OnExpressionDetectResult;

            _videoEngine = RTCVideoEngineCXXBridge.CreateRTCVideo(_initParams.AppID, _callback, parameter);
            if (_videoEngine == null)
            {
                return -2;
            }

            string logInfo = string.Format("AppID: {0}, params: {1}", _initParams.AppID, parameter);
            ByteRTCLog.ReportApiCall("CreateRTCVideo", logInfo);
            return 0;
        }

        public void Release()
        {
            ByteRTCLog.ReportApiCall("ReleaseRTCEngine", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }

            RTCVideoRoom.Release();
            RTCVideoEngineCXXBridge.DestroyRTCVideo(_videoEngine);
            _videoEngine = default(IntPtr);
            ByteRTCLog.ReportApiCall("-ReleaseRTCEngine", "");
        }
            //please call before Initialize 
#if UNITY_ANDROID 
        public static int GetCurrentEGLContext()
        {
            return RTCVideoEngineCXXBridge.GetCurrentEGLContext();
        }
#endif

        public string GetErrorDescription(int code)
        {
            string logInfo = string.Format("code: {0}", code);
            ByteRTCLog.ReportApiCall("GetErrorDescription", logInfo);
            IntPtr errorDescriptionPtr = RTCVideoEngineCXXBridge.GetErrorDescription(code);
            string  errorDescription = Marshal.PtrToStringAnsi(errorDescriptionPtr);
            return errorDescription;
        }

        public string GetSDKVersion()
        {
            IntPtr sdkVersionPtr = RTCVideoEngineCXXBridge.GetSDKVersion();
            string sdkVersion = Marshal.PtrToStringAnsi(sdkVersionPtr);
            ByteRTCLog.ReportApiCall("GetSDKVersion", sdkVersion);
            return sdkVersion;
        }

        public int SetBusinessId(string businessID)
        {
            ByteRTCLog.ReportApiCall("SetBusinessId", businessID);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetBusinessId(_videoEngine, businessID);
        }

        public int Feedback(String roomID, String userID, List<int> typeArray, string problemDescription)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("type", typeArray);
            dict.Add("problem_desc", problemDescription);
            dict.Add("os_version", System.Environment.OSVersion.ToString());
            dict.Add("network_type", "UNKNON");

            string parameter = JsonMapper.ToJson(dict);

            ByteRTCLog.ReportApiCall("Feedback", parameter);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            int feedbackTypes = 0;
            for (int i = 0; i < typeArray.ToArray().Length; i++)
            {
                feedbackTypes |= typeArray.ToArray()[i];
            }

            ProblemFeedbackRoomInfo problemFeedbackRoomInfo;
            problemFeedbackRoomInfo.room_id = roomID;
            problemFeedbackRoomInfo.user_id = userID;

            ProblemFeedbackInfo problemFeedbackInfo;
            problemFeedbackInfo.problem_desc = problemDescription;
            problemFeedbackInfo.room_info = RTCUtils.StructToIntPtr<ProblemFeedbackRoomInfo>(problemFeedbackRoomInfo);
            problemFeedbackInfo.room_info_count = 1;

            IntPtr problemFeedbackInfoIntptr = RTCUtils.StructToIntPtr<ProblemFeedbackInfo>(problemFeedbackInfo);

            return RTCVideoEngineCXXBridge.Feedback(_videoEngine, feedbackTypes, problemFeedbackInfoIntptr);
         
        }

        public void SetRuntimeParameters(string jsonString)
        {
            ByteRTCLog.ReportApiCall("SetRuntimeParameters", jsonString);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetRuntimeParameters(_videoEngine, jsonString);
        }

        public IRTCVideoRoom CreateRTCRoom(string roomID)
        {
            ByteRTCLog.ReportApiCall("CreateRTCRoom", roomID);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return null;
            }
            if (RTCVideoRoom.RoomIsExist(roomID)) {
                ByteRTCLog.LogWarning(string.Format("room id:{0} is exist",roomID));
                return null;
            }
            IntPtr room = RTCVideoEngineCXXBridge.CreateRTCRoom(_videoEngine, roomID);
            if (room == default(IntPtr)) {
                ByteRTCLog.LogWarning(string.Format("create rtc room failed,room id:{0}",roomID));
                return null;
            }
            RTCVideoRoom videoRoom = new RTCVideoRoom();
            videoRoom.Initialize(room, roomID);
            return videoRoom;
        }

        public int MuteAudioCapture(StreamIndex index, bool mute)
        {
            string logInfo = string.Format("index: {0}, mute: {1}", index, mute);
            ByteRTCLog.ReportApiCall("MuteAudioCapture", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.MuteAudioCapture(_videoEngine, index, mute);
        }

        public void SetCaptureVolume(StreamIndex index, int volume)
        {
            string logInfo = string.Format("index: {0}, volume: {1}", index, volume);
            ByteRTCLog.ReportApiCall("SetCaptureVolume", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetCaptureVolume(_videoEngine, index, volume);
        }

        public void SetPlaybackVolume(int volume)
        {
            string logInfo = string.Format("volume: {0}", volume);
            ByteRTCLog.ReportApiCall("SetPlaybackVolume", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetPlaybackVolume(_videoEngine, volume);
        }

        public void StartAudioCapture()
        {
            ByteRTCLog.ReportApiCall("StartAudioCapture", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.StartAudioCapture(_videoEngine);
        }

        public void StopAudioCapture()
        {
            ByteRTCLog.ReportApiCall("StopAudioCapture", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.StopAudioCapture(_videoEngine);
        }

        public void SetAudioProfile(AudioProfileType audioProfile)
        {
            string logInfo = string.Format("AudioProfileType: {0}", audioProfile);
            ByteRTCLog.ReportApiCall("SetAudioProfile", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetAudioProfile(_videoEngine, audioProfile);
        }

        public void SetAudioScenario(AudioScenarioType scenario)
        {
            string logInfo = string.Format("AudioScenrioType: {0}",scenario);
            ByteRTCLog.ReportApiCall("SetAudioScenario", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetAudioScenario(_videoEngine, scenario);
        }

        public int SetAudioRoute(AudioRouteDevice device)
        {
            string logInfo = string.Format("AudioRouteDevice: {0}", device);
            ByteRTCLog.ReportApiCall("SetAudioRoute", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetAudioRoute(_videoEngine, device);
        }

        public int EnableAudioPropertiesReport(AudioPropertiesConfig config)
        {
            string logInfo = string.Format("interval: {0}, enableSpectrum: {1}, enableVad: {2}", config.Interval, config.EnableSpectrum, config.EnableVad);
            ByteRTCLog.ReportApiCall("EnableAudioPropertiesReport", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.EnableAudioPropertiesReport(_videoEngine, config);
        }

        public void EnableVocalInstrumentBalance(bool enable)
        {
            string logInfo = string.Format("enable: {0}", enable);
            ByteRTCLog.ReportApiCall("EnableVocalInstrumentBalance", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.EnableVocalInstrumentBalance(_videoEngine, enable);
        }

        public int SetVoiceReverbType(VoiceReverbType voiceReverb)
        {
            string logInfo = string.Format("VoiceReverbType: {0}", voiceReverb);
            ByteRTCLog.ReportApiCall("SetVoiceReverbType", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetVoiceReverbType(_videoEngine, voiceReverb);
        }
        public void SetEarMonitorMode(EarMonitorMode mode)
        {
            string logInfo = string.Format("EarMonitorMode: {0}", mode);
            ByteRTCLog.ReportApiCall("SetEarMonitorMode", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetEarMonitorMode(_videoEngine, mode);
        }

        public void SetEarMonitorVolume(int volume)
        {
            string logInfo = string.Format("volume: {0}", volume);
            ByteRTCLog.ReportApiCall("SetEarMonitorVolume", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetEarMonitorVolume(_videoEngine, volume);
        }
        public int SetVoiceChangerType(VoiceChangerType voice_changer)
        {
            string logInfo = string.Format("voice_changer: {0}", voice_changer);
            ByteRTCLog.ReportApiCall("SetVoiceChangerType", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetVoiceChangerType(_videoEngine, voice_changer);
        }
        public int SetLocalVoicePitch(int pitch)
        {
            string logInfo = string.Format("pitch: {0}", pitch);
            ByteRTCLog.ReportApiCall("SetLocalVoicePitch", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetLocalVoicePitch(_videoEngine, pitch);
        }
        public int SetLocalVoiceEqualization(VoiceEqualizationConfig config)
        {
            string logInfo = string.Format("VoiceEqualizationConfig: {0}", config);
            ByteRTCLog.ReportApiCall("SetLocalVoiceEqualization", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetLocalVoiceEqualization(_videoEngine, config);
        }
        public int SetLocalVoiceReverbParam(VoiceReverbConfig param)
        {
            string logInfo = string.Format("VoiceReverbConfig room_size: {0},pre_delay: {1},wet_gain: {2},dry_gain: {3},decay_time: {4},damping: {5}",
                param.room_size,param.pre_delay,param.wet_gain,param.dry_gain,param.decay_time,param.damping);
            ByteRTCLog.ReportApiCall("SetLocalVoiceReverbParam", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetLocalVoiceReverbParam(_videoEngine, param);
        }
        public int EnableLocalVoiceReverb(bool enable)
        {
            string logInfo = string.Format("enable: {0}", enable);
            ByteRTCLog.ReportApiCall("EnableLocalVoiceReverb", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.EnableLocalVoiceReverb(_videoEngine, enable);
        }
        public  int SetDummyCaptureImagePath( string file_path)
        {
            string logInfo = string.Format("SetDummyCaptureImagePath: {0}", file_path);
            ByteRTCLog.ReportApiCall("SetDummyCaptureImagePath", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetDummyCaptureImagePath(_videoEngine, file_path);
        }

        public  int RequestRemoteVideoKeyFrame(ref RemoteStreamKey stream_info)
        {
            string logInfo = string.Format("RequestRemoteVideoKeyFrame: {0}", stream_info);
            ByteRTCLog.ReportApiCall("RequestRemoteVideoKeyFrame", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.RequestRemoteVideoKeyFrame(_videoEngine, ref stream_info);
        }

        public  int StartPlayPublicStream(string public_stream_id)
        {
            string logInfo = string.Format("StartPlayPublicStream: {0}", public_stream_id);
            ByteRTCLog.ReportApiCall("StartPlayPublicStream", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.StartPlayPublicStream(_videoEngine, public_stream_id);
        }

        public  int StopPlayPublicStream( string public_stream_id)
        {
            string logInfo = string.Format("StopPushPublicStream: {0}", public_stream_id);
            ByteRTCLog.ReportApiCall("StopPushPublicStream", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.StopPlayPublicStream(_videoEngine, public_stream_id);

        }
        
        public int StartPushPublicStream(string public_stream_id, IntPtr public_stream)
        {
            string logInfo = string.Format("StartPushPublicStream: {0}", public_stream_id);
            ByteRTCLog.ReportApiCall("StartPushPublicStream", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.StartPushPublicStream(_videoEngine, public_stream_id, public_stream);
        }
       public  int StopPushPublicStream(string public_stream_id)
        {
            string logInfo = string.Format("StopPushPublicStream: {0}", public_stream_id);
            ByteRTCLog.ReportApiCall("StopPushPublicStream", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.StopPushPublicStream(_videoEngine, public_stream_id);
        }

        public  int SetPublicStreamAudioPlaybackVolume( string public_stream_id, int volume)
        {
            string logInfo = string.Format("SetPublicStreamAudioPlaybackVolume public_stream_id: {0} volume:{1} ", public_stream_id, volume);
            ByteRTCLog.ReportApiCall("SetPublicStreamAudioPlaybackVolume", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetPublicStreamAudioPlaybackVolume(_videoEngine, public_stream_id,  volume);

            //[DllImport(libname, EntryPoint = "VideoSDKUpdatePublicStreamParam")]
            //private static extern int VideoSDKUpdatePublicStreamParam( IntPtr engine,const char* public_stream_id,bytertc::IPublicStreamParam param);

            //设置变声特效类型
        }
        public int SetVideoCaptureRotation(VideoRotation rotation)
        {
            string logInfo = string.Format("VideoRotation: {0}", rotation);
            ByteRTCLog.ReportApiCall("SetVideoCaptureRotation", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetVideoCaptureRotation(_videoEngine, rotation);
        }
        public int SetVideoWatermark(StreamIndex stream_index, string image_path, RTCWatermarkConfig config)
        {
            string logInfo = string.Format("stream_index:{0}, image_path:{1} ,config: {2}", stream_index, image_path, config);
            ByteRTCLog.ReportApiCall("SetVideoWatermark", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetVideoWatermark(_videoEngine, stream_index,  image_path,  config);
        }
        public int ClearVideoWatermark(StreamIndex stream_index)
        {
            string logInfo = string.Format("stream_index: {0}", stream_index);
            ByteRTCLog.ReportApiCall("ClearVideoWatermark", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.ClearVideoWatermark(_videoEngine, stream_index);
        }
        public int EnableEffectBeauty(bool enable)
        {
            string logInfo = string.Format("enable: {0}", enable);
            ByteRTCLog.ReportApiCall("EnableEffectBeauty", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.EnableEffectBeauty(_videoEngine, enable);
        }
        public int SetBeautyIntensity(EffectBeautyMode beauty_mode, float intensity)
        {
            string logInfo = string.Format("beauty_mode: {0} intensity:{1}", beauty_mode, intensity);
            ByteRTCLog.ReportApiCall("SetBeautyIntensity", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetBeautyIntensity(_videoEngine, beauty_mode,  intensity);
        }
        public int GetAuthMessage(ref IntPtr ppmsg, ref int len)
        {
            string logInfo = string.Format("GetAuthMessage: {0}", ppmsg);
            ByteRTCLog.ReportApiCall("GetAuthMessage", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.GetAuthMessage(_videoEngine, ref ppmsg,  ref len);
        }
        public int FreeAuthMessage(string pmsg)
        {
            string logInfo = string.Format("pmsg: {0}", pmsg);
            ByteRTCLog.ReportApiCall("FreeAuthMessage", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.FreeAuthMessage(_videoEngine, pmsg);
        }
        public int InitCVResource(string license_file_path, string algo_model_dir)
        {
            string logInfo = string.Format("license_file_path: {0}, algo_model_dir: {1}", license_file_path, algo_model_dir);
            ByteRTCLog.ReportApiCall("InitCVResource", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.InitCVResource(_videoEngine, license_file_path,  algo_model_dir);
        }
        public int EnableVideoEffect()
        {
            string logInfo = string.Format("EnableVideoEffect");
            ByteRTCLog.ReportApiCall("EnableVideoEffect", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
       
            return RTCVideoEngineCXXBridge.EnableVideoEffect(_videoEngine);
        }
        public int DisableVideoEffect()
        {
            string logInfo = string.Format("DisableVideoEffect");
            ByteRTCLog.ReportApiCall("DisableVideoEffect", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.DisableVideoEffect(_videoEngine);
        }
        public int SetEffectNodes(string[] effect_node_paths, int node_num)
        {
            string logInfo = string.Format("effect_node_paths: {0} node_num{1} ", effect_node_paths[0], node_num);
            ByteRTCLog.ReportApiCall("SetEffectNodes", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetEffectNodes(_videoEngine, effect_node_paths,  node_num);
        }
        public int UpdateEffectNode(string effect_node_path, string node_key, float node_value)
        {
            string logInfo = string.Format("effect_node_path: {0}, node_key{1},node_value{2}", effect_node_path,  node_key,  node_value);
            ByteRTCLog.ReportApiCall("UpdateEffectNode", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.UpdateEffectNode(_videoEngine, effect_node_path, node_key, node_value);
        }
        public int SetColorFilter(string res_path)
        {
            string logInfo = string.Format("res_path: {0}", res_path);
            ByteRTCLog.ReportApiCall("SetColorFilter", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetColorFilter(_videoEngine, res_path);
        }
        public int SetColorFilterIntensity(float intensity)
        {
            string logInfo = string.Format("intensity: {0}", intensity);
            ByteRTCLog.ReportApiCall("SetColorFilterIntensity", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetColorFilterIntensity(_videoEngine, intensity);
        }
        public int EnableVirtualBackground(string background_sticker_path, VirtualBackgroundSource source)
        {
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            string logInfo = string.Format("background_sticker_path: {0}  source:{1} ", background_sticker_path, source);
            ByteRTCLog.ReportApiCall("EnableVirtualBackground", logInfo);
            return RTCVideoEngineCXXBridge.EnableVirtualBackground(_videoEngine, background_sticker_path,  source);
        }
        public int DisableVirtualBackground()
        {
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            string logInfo = "DisableVirtualBackground";
            ByteRTCLog.ReportApiCall("DisableVirtualBackground", logInfo);
            return RTCVideoEngineCXXBridge.DisableVirtualBackground(_videoEngine);
        }

        //开启人脸识别功能，并设置人脸检测结果回调观察者
        public int VideoSDKEnableFaceDetection(uint interval_ms, string face_model_path)
        {
            string logInfo = string.Format("interval_ms: {0} face_model_path:{1}", interval_ms, face_model_path);
            ByteRTCLog.ReportApiCall("VideoSDKEnableFaceDetection", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.VideoSDKEnableFaceDetection(_videoEngine, interval_ms, face_model_path);
        }

        //关闭人脸识别功能
        public int VideoSDKDisableFaceDetection()
        {
            string logInfo = string.Format("VideoSDKDisableFaceDetection");
            ByteRTCLog.ReportApiCall("VideoSDKDisableFaceDetection", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.VideoSDKDisableFaceDetection(_videoEngine);
        }
        public int SetVideoDigitalZoomConfig(ZoomConfigType type, float size)
        {
            string logInfo = string.Format("ZoomConfigType: {0} size:{1}", type,size);
            ByteRTCLog.ReportApiCall("SetVideoDigitalZoomConfig", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetVideoDigitalZoomConfig(_videoEngine, type,  size);
        }
        public int SetVideoDigitalZoomControl(ZoomDirectionType direction)
        {
            string logInfo = string.Format("ZoomDirectionType: {0}", direction);
            ByteRTCLog.ReportApiCall("SetVideoDigitalZoomControl", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetVideoDigitalZoomControl(_videoEngine, direction);
        }
        public int StartVideoDigitalZoomControl(ZoomDirectionType direction)
        {
            string logInfo = string.Format("ZoomDirectionType: {0}", direction);
            ByteRTCLog.ReportApiCall("StartVideoDigitalZoomControl", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.StartVideoDigitalZoomControl(_videoEngine, direction);
        }
        public int StopVideoDigitalZoomControl()
        {
            string logInfo = string.Format("StopVideoDigitalZoomControl");
            ByteRTCLog.ReportApiCall("StopVideoDigitalZoomControl", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.StopVideoDigitalZoomControl(_videoEngine);
        }

        public int EnableAudioProcessor(AudioProcessorMethod method, AudioFormat format)
        {
            string logInfo = string.Format("AudioProcessMethod: {0}, AudioFormat: {1}", method, format);
            ByteRTCLog.ReportApiCall("EnableAudioProcessor", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1 ;
            }
             return RTCVideoEngineCXXBridge.EnableAudioProcessor(_videoEngine, method, format);
        }

        public int DisableAudioProcessor(AudioProcessorMethod method)
        {
            string logInfo = string.Format("AudioProcessMethod: {0}", method);
            ByteRTCLog.ReportApiCall("DisableAudioProcessor", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
        //    RTCVideoEngineCXXBridge.SetEarMonitorVolume(_videoEngine, volume);
            return RTCVideoEngineCXXBridge.DisableAudioProcessor(_videoEngine, method);
        }
        public  int SetRemoteAudioPlaybackVolume(string roomID, string userID, int volume)
        {
            string logInfo = string.Format("roomID:{0},userID:{1}, volume:{2}", roomID, userID, volume);
            ByteRTCLog.ReportApiCall("SetRemoteAudioPlaybackVolume", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetRemoteAudioPlaybackVolume(_videoEngine, roomID, userID, volume);
        }
        public int SetDefaultAudioRoute(AudioRouteDevice route)
        {
            string logInfo = string.Format("AudioRouteDevice: {0}", route);
            ByteRTCLog.ReportApiCall("SetDefaultAudioRoute", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetDefaultAudioRoute(_videoEngine, route);
        }

        public AudioRouteDevice GetAudioRoute()
        {
            ByteRTCLog.ReportApiCall("GetAudioRoute", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return AudioRouteDevice.kAudioRouteDefault;
            }
            return RTCVideoEngineCXXBridge.GetAudioRoute(_videoEngine);
        }
        public int EnableAudioFrameCallback(AudioFrameCallbackMethod method, AudioFormat format)
        {
            string logInfo = string.Format("AudioFrameCallbackMethod: {0}, AudioFormat: {1}", method, format);
            ByteRTCLog.ReportApiCall("EnableAudioFrameCallback", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.EnableAudioFrameCallback(_videoEngine, method, format);
        }

        public int DisableAudioFrameCallback(AudioFrameCallbackMethod method)
        {
            string logInfo = string.Format("AudioFrameCallbackMethod: {0}", method);
            ByteRTCLog.ReportApiCall("DisableAudioFrameCallback", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.DisableAudioFrameCallback(_videoEngine, method);
        }
        public int StartAudioRecording(AudioRecordingConfig config)
        {
            string logInfo = string.Format("absolute_file_name: {0}, frame_source: {1}, sample_rate: {2}, channel: {3}, quality: {4}", config.absolute_file_name,
                config.frame_source, config.sample_rate, config.channel, config.quality);
            ByteRTCLog.ReportApiCall("StartAudioRecording", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.StartAudioRecording(_videoEngine, config);
        }

        public int StopAudioRecording()
        {
            ByteRTCLog.ReportApiCall("StartAudioRecording", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.StopAudioRecording(_videoEngine);
        }


        public int SendStreamSyncInfo(byte[] data, int length, int stream_index, int repeat_count)
        {
            string dataStr = System.Text.Encoding.Default.GetString(data);
            string logInfo = string.Format("data: {0}, length: {1}, streamIndex: {2}, repeatCount: {3}",
                dataStr, length, stream_index, repeat_count);
            ByteRTCLog.ReportApiCall("SendStreamSyncInfo", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SendStreamSyncInfo(_videoEngine, stream_index, data, length, repeat_count);
        }
        public int SendSEIMessage(byte[] data, int length, int stream_index, int repeat_count, int sei_mode)
        {
            string dataStr = System.Text.Encoding.Default.GetString(data);
            string logInfo = string.Format("data: {0}, length: {1}, streamIndex: {2}, repeatCount: {3}",
                dataStr, length, stream_index, repeat_count);
            ByteRTCLog.ReportApiCall("SendSEIMessage", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SendSEIMessage(_videoEngine, stream_index, data, length, repeat_count, sei_mode);
        }
        //video manager
        public void StartVideoCapture()
        {
            ByteRTCLog.ReportApiCall("StartVideoCapture", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.StartVideoCapture(_videoEngine);
        }

        public void StopVideoCapture()
        {
            ByteRTCLog.ReportApiCall("StopVideoCapture", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.StopVideoCapture(_videoEngine);
        }

        public int SetVideoCaptureConfig(VideoCaptureConfig videoCaptureConfig)
        {
            string logInfo = string.Format("capturePreference: {0}, width: {1}, height: {2}, frameRate: {3}", videoCaptureConfig.CapturePreference, videoCaptureConfig.Width, videoCaptureConfig.Height, videoCaptureConfig.FrameRate);
            ByteRTCLog.ReportApiCall("SetVideoCaptureConfig", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetVideoCaptureConfig(_videoEngine, videoCaptureConfig);
        }

        public int SetVideoEncoderConfig1(VideoEncoderConfig maxSolution)
        {
            string logInfo = string.Format("width: {0}, height: {1}, frameRate: {2}, maxBitrate: {3}, encoderPreference: {4}", maxSolution.Width, maxSolution.Height, maxSolution.FrameRate, maxSolution.MaxBitrate, maxSolution.EncoderPreference);
            ByteRTCLog.ReportApiCall("SetVideoEncoderConfig1", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetVideoEncoderConfig1(_videoEngine, maxSolution);
        }

        public int SetVideoEncoderConfig2(VideoEncoderConfig[] channelSolutions, int solutionNum)
        {
            string logInfo = string.Format("soutionNum: {0}", solutionNum);
            ByteRTCLog.ReportApiCall("SetVideoEncoderConfig2", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetVideoEncoderConfig2(_videoEngine, channelSolutions, solutionNum);
        }

        public int SetScreenVideoEncoderConfig(ScreenVideoEncoderConfig screenSolution)
        {
            ByteRTCLog.ReportApiCall("SetScreenVideoEncoderConfig", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetScreenVideoEncoderConfig(_videoEngine, screenSolution);
        }

        public void SetVideoSourceType(StreamIndex streamIndex, VideoSourceType type)
        {
            string logInfo = string.Format("type: {0}", type);
            ByteRTCLog.ReportApiCall("SetVideoSourceType", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetVideoSourceType(_videoEngine, streamIndex, type);
        }

        public void SetLocalVideoMirrorType(MirrorType mirrorType)
        {
            string logInfo = string.Format("mirrorType: {0}", mirrorType);
            ByteRTCLog.ReportApiCall("SetLocalVideoMirrorType", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetLocalVideoMirrorType(_videoEngine, mirrorType);
        }

        public int SetVideoRotationMode(VideoRotationMode rotationMode)
        {
            string logInfo = string.Format("rotationMode: {0}", rotationMode);
            ByteRTCLog.ReportApiCall("SetVideoRotationMode", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SetVideoRotationMode(_videoEngine, rotationMode);
        }

        public void SetLocalVideoSink(StreamIndex index, VideoSinkPixelFormat requiredFormat)
        {
            string logInfo = string.Format("StreamIndex: {0}, VideoSinkPixelFormat: {1}", index, requiredFormat);
            ByteRTCLog.ReportApiCall("SetLocalVideoSink", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetLocalVideoSink(_videoEngine, index, requiredFormat);
        }

        public void SetRemoteVideoSink(RemoteStreamKey streamKey, VideoSinkPixelFormat requiredFormat)
        {
            string logInfo = string.Format("VideoSinkPixelFormat: {0}", requiredFormat);
            ByteRTCLog.ReportApiCall("SetRemoteVideoSink", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetRemoteVideoSink(_videoEngine, streamKey, requiredFormat);
        }

        // screen
        public IScreenCaptureSourceList GetScreenCaptureSourceList()
        {
            ByteRTCLog.ReportApiCall("GetScreenCaptureSourceList", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return null;
            }
            IntPtr ptr = RTCVideoEngineCXXBridge.GetScreenCaptureSourceList(_videoEngine);
            ScreenCaptureSourceList screenCaptureSourceList = new ScreenCaptureSourceList();
            screenCaptureSourceList.Initialize(ptr);
            return screenCaptureSourceList;
        }

        public int StartScreenVideoCapture(ScreenCaptureSourceInfo sourceInfo, ScreenCaptureParameters captureParams)
        {
            ByteRTCLog.ReportApiCall("StartScreenVideoCapture", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.StartScreenVideoCapture(_videoEngine, sourceInfo, captureParams);
        }

        public void StopScreenVideoCapture()
        {
            ByteRTCLog.ReportApiCall("StopScreenVideoCapture", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.StopScreenVideoCapture(_videoEngine);
        }

        public void UpdateScreenCaptureRegion(Rectangle regionRect)
        {
            ByteRTCLog.ReportApiCall("UpdateScreenCaptureRegion", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.UpdateScreenCaptureRegion(_videoEngine, regionRect);
        }

        public void UpdateScreenCaptureHighlightConfig(HighlightConfig highlightConfig)
        {
            ByteRTCLog.ReportApiCall("UpdateScreenCaptureHighlightConfig", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.UpdateScreenCaptureHighlightConfig(_videoEngine, highlightConfig);
        }

        public void UpdateScreenCaptureMouseCursor(MouseCursorCaptureState captureMouseCursor)
        {
            string logInfo = string.Format("captureMouseCursor: {0}", captureMouseCursor);
            ByteRTCLog.ReportApiCall("UpdateScreenCaptureMouseCursor", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.UpdateScreenCaptureMouseCursor(_videoEngine, captureMouseCursor);
        }

        public void UpdateScreenCaptureFilterConfig(ScreenFilterConfig filterConfig)
        {
            ByteRTCLog.ReportApiCall("UpdateScreenCaptureFilterConfig", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.UpdateScreenCaptureFilterConfig(_videoEngine, filterConfig);
        }

        public VideoFrame GetThumbnail(ScreenCaptureSourceType type, IntPtr sourceID, int maxWidth, int maxHeight)
        {
            ByteRTCLog.ReportApiCall("GetThumbnail", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return new VideoFrame();
            }
            IntPtr videoFrameCallbackPtr = RTCVideoEngineCXXBridge.GetThumbnail(_videoEngine, type, sourceID, maxWidth, maxHeight);
            VideoFrameCallback videoFrameCallback = Marshal.PtrToStructure<VideoFrameCallback>(videoFrameCallbackPtr);
            VideoFrame videoFrame = new VideoFrame();
            if (!ConvertVideoFrameCallback2VideoFrame(ref videoFrame, videoFrameCallback))
            {
                ByteRTCLog.LogWarning("Convert video frame failed");
                return new VideoFrame();
            }
            ReleaseVideoFrame(videoFrameCallbackPtr);
            return videoFrame;
        }

        public void ReleaseVideoFrame(IntPtr videoFrameCallbackPtr)
        {
            ByteRTCLog.ReportApiCall("ReleaseVideoFrame", "");
            RTCVideoEngineCXXBridge.ReleaseVideoFrame(videoFrameCallbackPtr);
        }


        public void SetScreenAudioSourceType(AudioSourceType sourceType)
        {
            string logInfo = string.Format("type: {0}", sourceType);
            ByteRTCLog.ReportApiCall("SetScreenAudioSourceType", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetScreenAudioSourceType(_videoEngine, sourceType);
        }

        public void SetScreenAudioStreamIndex(StreamIndex index)
        {
            string logInfo = string.Format("index: {0}", index);
            ByteRTCLog.ReportApiCall("SetScreenAudioStreamIndex", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetScreenAudioStreamIndex(_videoEngine, index);
        }

        public void StartScreenAudioCapture()
        {
            ByteRTCLog.ReportApiCall("StartScreenAudioCapture", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.StartScreenAudioCapture(_videoEngine);
        }

        public void StopScreenAudioCapture()
        {
            ByteRTCLog.ReportApiCall("StopScreenAudioCapture", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.StopScreenAudioCapture(_videoEngine);
        }

        public int PushScreenAudioFrame(AudioFrame frame)
        {
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            AudioFrameCallback cbFrame = new AudioFrameCallback();
            ConvertAudioFrame2AudioFrameCallback(ref cbFrame, frame);
            return RTCVideoEngineCXXBridge.PushScreenAudioFrame(_videoEngine, ref cbFrame);
        }

        public int SwitchCamera(CameraID cameraId)
        {
            string logInfo = string.Format("cameraId: {0}", cameraId);
            ByteRTCLog.ReportApiCall("SwitchCamera", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.SwitchCamera(_videoEngine, cameraId);
        }

        public int PushExternalVideoFrame(VideoFrame frame)
        {
            //ByteRTCLog.ReportApiCall("PushExternalVideoFrame", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            VideoFrameCallback callbackFrame = new VideoFrameCallback();
            bool bSuccess = ConvertVideoFrame2VideoFrameCallback(ref callbackFrame, frame);
            if (bSuccess == false)
            {
                return -2;
            }
            return RTCVideoEngineCXXBridge.PushExternalVideoFrame(_videoEngine, ref callbackFrame);
        }
        public int PushScreenVideoFrame(VideoFrame frame)
        {
            //ByteRTCLog.ReportApiCall("PushScreenFrame", "");
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }

            VideoFrameCallback callbackFrame = new VideoFrameCallback();
            bool bSuccess = ConvertVideoFrame2VideoFrameCallback(ref callbackFrame, frame);
            if (bSuccess == false)
            {
                return -2;
            }
            return RTCVideoEngineCXXBridge.PushScreenVideoFrame(_videoEngine, ref callbackFrame);
        }

        //audio device related functions
        public IAudioDeviceManager GetAudioDeviceManager()
		{
			ByteRTCLog.ReportApiCall("GetAudioDeviceManager", "");
			AudioDeviceManager deviceManager = new AudioDeviceManager();
			if (_videoEngine == default(IntPtr))
			{
				ByteRTCLog.LogWarning("RTCEngine is null");
				return null;
			}
			deviceManager.InitAudioDeviceManager(_videoEngine);
			return deviceManager;
		}

		public IVideoDeviceManager GetVideoDeviceManager()
		{
			ByteRTCLog.ReportApiCall("GetVideoDeviceManager", "");
			VideoDeviceManager deviceManager = new VideoDeviceManager();
			if (_videoEngine == default(IntPtr))
			{
				ByteRTCLog.LogWarning("RTCEngine is null");
				return null;
			}
			deviceManager.InitVideoDeviceManager(_videoEngine);
			return deviceManager;
		}

		[MonoPInvokeCallback(typeof(OnWarningEventHandler))]
        public static void OnWarning(int warn)
        {
            string logInfo = string.Format("warn: {0}", warn);
            ByteRTCLog.ReportCallback("OnWarning", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnWarningEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnWarningEvent != null)
                    {
                        _instance.OnWarningEvent(warn);
                    }  
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnErrorEventHandler))]
        public static void OnError(int err)
        {
            string logInfo = string.Format("error: {0}", err);
            ByteRTCLog.ReportCallback("OnError", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnErrorEvent != null)
            {
                Loom.QueueOnMainThread(() => 
                {
                    if (_instance != null && _instance.OnErrorEvent != null)
                    {
                        _instance.OnErrorEvent(err);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnLogReportEventHandler))]
        public static void OnLogReport(string logType, string logContent)
        {
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnLogReportEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnLogReportEvent != null)
                    {
                        _instance.OnLogReportEvent(logType, logContent);
                    }
                });
            }
        }



        [MonoPInvokeCallback(typeof(OnConnectionStateChangedEventHandler))]
        public static void OnConnectionStateChanged(ConnectionState state)
        {
            string logInfo = string.Format("ConnectionState: {0}", state);
            ByteRTCLog.ReportCallback("OnConnectionStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnConnectionStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnConnectionStateChangedEvent != null)
                    {
                        _instance.OnConnectionStateChangedEvent(state);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnNetworkTypeChangedEventHandler))]
        public static void OnNetworkTypeChanged(NetworkType type)
        {
            string logInfo = string.Format("NetworkType: {0}", type);
            ByteRTCLog.ReportCallback("OnNetworkTypeChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnNetworkTypeChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnNetworkTypeChangedEvent != null)
                    {
                        _instance.OnNetworkTypeChangedEvent(type);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnSysStatsEventHandler))]
        public static void OnSysStats(SysStats stats)
        {
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnSysStatsEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnSysStatsEvent != null)
                    {
                        _instance.OnSysStatsEvent(stats);
                    }       
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnAudioRouteChangedEventHandler))]
        public static void OnAudioRouteChanged(AudioRouteDevice device)
        {
            string logInfo = string.Format("AudioRouteDevice: {0}", device);
            ByteRTCLog.ReportCallback("OnAudioRouteChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnAudioRouteChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnAudioRouteChangedEvent != null)
                    {
                        _instance.OnAudioRouteChangedEvent(device);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnAudioDeviceStateChangedEventHandler))]
        public static void OnAudioDeviceStateChanged(string deviceID, RTCAudioDeviceType deviceType, MediaDeviceState deviceState, MediaDeviceError deviceError)
        {
            string logInfo = string.Format("deviceID: {0}, RTCAudioDeviceType: {1}, MediaDeviceState: {2}, MediaDeviceError: {3}", deviceID, deviceType, deviceState, deviceError);
            ByteRTCLog.ReportCallback("OnAudioDeviceStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnAudioDeviceStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnAudioDeviceStateChangedEvent != null)
                    {
                        _instance.OnAudioDeviceStateChangedEvent(deviceID, deviceType, deviceState, deviceError);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnAudioDeviceWarningEventHandler))]
        public static void OnAudioDeviceWarning(string deviceID, RTCAudioDeviceType deviceType, MediaDeviceWarning deviceWarning)
        {
            string logInfo = string.Format("deviceID: {0}, RTCAudioDeviceType: {1}, MeidaDeviceWarning: {2}", deviceID, deviceType, deviceWarning);
            ByteRTCLog.ReportCallback("OnAudioDeviceWarning", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnAudioDeviceWarningEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnAudioDeviceWarningEvent != null)
                    {
                        _instance.OnAudioDeviceWarningEvent(deviceID, deviceType, deviceWarning);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnCreateRoomStateChangedEventHandler))]
        public static void OnCreateRoomStateChanged(string room_id, int error_code)
        {
            string logInfo = string.Format("room_id: {0}, error_code: {1}", room_id, error_code);
            ByteRTCLog.ReportCallback("OnCreateRoomStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return;
            }
            if (_instance.OnCreateRoomStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnCreateRoomStateChangedEvent != null)
                    {
                        _instance.OnCreateRoomStateChangedEvent(room_id, error_code);
                    }
                });
            }
        }
        [MonoPInvokeCallback(typeof(OnAudioRecordingStateUpdateEventHandler))]
        public static void OnAudioRecordingStateUpdate(AudioRecordingState state, AudioRecordingErrorCode error_code)
        {
            string logInfo = string.Format("AudioRecordingState: {0}, AudioRecordingErrorCode: {1}", state, error_code);
            ByteRTCLog.ReportCallback("OnAudioRecordingStateUpdate", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }

            if (_instance.OnAudioRecordingStateUpdateEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnAudioRecordingStateUpdateEvent != null)
                    {
                        _instance.OnAudioRecordingStateUpdateEvent(state, error_code);
                    }
                });
            }
        }
        [MonoPInvokeCallback(typeof(OnRecordAudioFrameOriginalCallback))]
        public static void OnRecordAudioFrameOriginal(AudioFrameCallback audioFrameCallback)
        {
            AudioFrame audioFrame = new AudioFrame();
            ConvertAudioFrameCallback2AudioFrame(audioFrameCallback, out audioFrame);
            ByteRTCLog.ReportCallback("OnRecordAudioFrameOriginal", "");
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return;
            }
            if (_instance.OnRecordAudioFrameOriginalEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnRecordAudioFrameOriginalEvent != null)
                    {
                        _instance.OnRecordAudioFrameOriginalEvent(audioFrame);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnRecordAudioFrameCallback))]
        public static void OnRecordAudioFrame(AudioFrameCallback audioFrameCallback)
        {
            AudioFrame audioFrame = new AudioFrame();
            ConvertAudioFrameCallback2AudioFrame(audioFrameCallback, out audioFrame);
            ByteRTCLog.ReportCallback("OnRecordAudioFrame", "");
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return;
            }
            if (_instance.OnRecordAudioFrameEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnRecordAudioFrameEvent != null)
                    {
                        _instance.OnRecordAudioFrameEvent(audioFrame);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnPlaybackAudioFrameCallback))]
        public static void OnPlaybackAudioFrame(AudioFrameCallback audioFrameCallback)
        {
            AudioFrame audioFrame = new AudioFrame();
            ConvertAudioFrameCallback2AudioFrame(audioFrameCallback, out audioFrame);
            ByteRTCLog.ReportCallback("OnPlaybackAudioFrame", "");
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return;
            }
            if (_instance.OnPlaybackAudioFrameEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnPlaybackAudioFrameEvent != null)
                        _instance.OnPlaybackAudioFrameEvent(audioFrame);
                });
            }
        }


        [MonoPInvokeCallback(typeof(OnRemoteUserAudioFrameCallback))]
        public static void OnRemoteUserAudioFrame(RemoteStreamKey remoteStreamKey, AudioFrameCallback audioFrameCallback)
        {
            AudioFrame audioFrame = new AudioFrame();
            ConvertAudioFrameCallback2AudioFrame(audioFrameCallback, out audioFrame);
            string logInfo = string.Format("roomID: {0}, userID: {1}", remoteStreamKey.RoomID, remoteStreamKey.UserID);
            ByteRTCLog.ReportCallback("OnRemoteUserAudioFrame", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return;
            }
            if (_instance.OnRemoteUserAudioFrameEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnRemoteUserAudioFrameEvent != null)
                        _instance.OnRemoteUserAudioFrameEvent(remoteStreamKey, audioFrame);
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnMixedAudioFrameCallback))]
        public static void OnMixedAudioFrame(AudioFrameCallback audioFrameCallback)
        {
            AudioFrame audioFrame = new AudioFrame();
            ConvertAudioFrameCallback2AudioFrame(audioFrameCallback, out audioFrame);
            ByteRTCLog.ReportCallback("OnMixedAudioFrame", "");
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return;
            }
            if (_instance.OnMixedAudioFrameEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnMixedAudioFrameEvent != null)
                        _instance.OnMixedAudioFrameEvent(audioFrame);
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnRecordScreenAudioFrameCallback))]
        public static void OnRecordScreenAudioFrame(AudioFrameCallback audioFrameCallback)
        {
            AudioFrame audioFrame = new AudioFrame();
            ConvertAudioFrameCallback2AudioFrame(audioFrameCallback, out audioFrame);
            ByteRTCLog.ReportCallback("OnRecordScreenAudioFrame", "");
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return;
            }
            if (_instance.OnRecordScreenAudioFrameEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnRecordScreenAudioFrameEvent != null)
                        _instance.OnRecordScreenAudioFrameEvent(audioFrame);
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnLocalAudioPropertiesReportCallback))]
        public static void OnLocalAudioPropertiesReport(IntPtr audioPropertiesInfos, int audioPropertiesInfoNumber)
        {
            string logInfo = string.Format("audioPropertiesInfoNumber: {0}", audioPropertiesInfoNumber);
            ByteRTCLog.ReportCallback("OnLocalAudioPropertiesReport", logInfo);
            int size = Marshal.SizeOf(typeof(LocalAudioPropertiesInfo));
            List<LocalAudioPropertiesInfo> localAudioPropertiesList = new List<LocalAudioPropertiesInfo>();
            for (int i = 0; i < audioPropertiesInfoNumber; ++i)
            {
                LocalAudioPropertiesInfo localAudioProperties = new LocalAudioPropertiesInfo();
                IntPtr ptr = (IntPtr)((long)audioPropertiesInfos + i * size);
#if UNITY_2019_3_OR_NEWER
                localAudioProperties = Marshal.PtrToStructure<LocalAudioPropertiesInfo>(ptr);
#else
                localAudioProperties = (LocalAudioPropertiesInfo)Marshal.PtrToStructure(ptr, typeof(LocalAudioPropertiesInfo));
#endif
                localAudioPropertiesList.Add(localAudioProperties);
            }
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnLocalAudioPropertiesReportEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnLocalAudioPropertiesReportEvent != null)
                    {
                        _instance.OnLocalAudioPropertiesReportEvent(localAudioPropertiesList, audioPropertiesInfoNumber);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnRemoteAudioPropertiesReportCallback))]
        public static void OnRemoteAudioPropertiesReport(IntPtr audioPropertiesInfos, int audioPropertiesInfoNumber, int totalRemoteVolume)
        {
            string logInfo = string.Format("audioPropertiesInfoNumber: {0}, totalRemoteVolume: {1}", audioPropertiesInfoNumber, totalRemoteVolume);
            ByteRTCLog.ReportCallback("OnRemoteAudioPropertiesReport", logInfo);
            int size = Marshal.SizeOf(typeof(RemoteAudioPropertiesInfo));
            List<RemoteAudioPropertiesInfo> remoteAudioPropertiesList = new List<RemoteAudioPropertiesInfo>();
            for (int i = 0; i < audioPropertiesInfoNumber; ++i)
            {
                RemoteAudioPropertiesInfo remoteAudioProperties = new RemoteAudioPropertiesInfo();
                IntPtr ptr = (IntPtr)((long)audioPropertiesInfos + i * size);
#if UNITY_2019_3_OR_NEWER
                remoteAudioProperties = Marshal.PtrToStructure<RemoteAudioPropertiesInfo>(ptr);
#else
                remoteAudioProperties = (RemoteAudioPropertiesInfo)Marshal.PtrToStructure(ptr, typeof(RemoteAudioPropertiesInfo));
#endif
                remoteAudioPropertiesList.Add(remoteAudioProperties);
            }
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnRemoteAudioPropertiesReportEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnRemoteAudioPropertiesReportEvent != null)
                    {
                        _instance.OnRemoteAudioPropertiesReportEvent(remoteAudioPropertiesList, audioPropertiesInfoNumber, totalRemoteVolume);
                    }                  
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnVideoDeviceStateChangedEventHandler))]
        public static void OnVideoDeviceStateChanged(string deviceID, RTCVideoDeviceType deviceType, MediaDeviceState deviceState, MediaDeviceError deviceError)
        {
            string logInfo = string.Format("deviceID: {0}, deviceType: {1}, deviceState: {2}, deviceError: {3}", deviceID, deviceType, deviceState, deviceError);
            ByteRTCLog.ReportCallback("OnVideoDeviceStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnVideoDeviceStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnVideoDeviceStateChangedEvent != null)
                    {
                        _instance.OnVideoDeviceStateChangedEvent(deviceID, deviceType, deviceState, deviceError);
                    }
                    
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnVideoDeviceWarningEventHandler))]
        public static void OnVideoDeviceWarning(string deviceID, RTCVideoDeviceType deviceType, MediaDeviceWarning deviceWarning)
        {
            string logInfo = string.Format("deviceID: {0}, deviceType: {1}, deviceWarning: {2}", deviceID, deviceType, deviceWarning);
            ByteRTCLog.ReportCallback("OnVideoDeviceWarning", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnVideoDeviceWarningEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnVideoDeviceWarningEvent != null)
                    {
                        _instance.OnVideoDeviceWarningEvent(deviceID, deviceType, deviceWarning);
                    }      
                });
            }
        }

    

        // local video sink
        [MonoPInvokeCallback(typeof(OnLocalVideoSinkOnFrameCallback))]
        public static bool OnLocalVideoSinkOnFrame(StreamIndex index, IntPtr videoFrameCallbackPtr)
        {
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return false;
            }

            VideoFrameCallback videoFrameCallback = Marshal.PtrToStructure<VideoFrameCallback>(videoFrameCallbackPtr);
            VideoFrame videoFrame = new VideoFrame();
            if (!ConvertVideoFrameCallback2VideoFrame(ref videoFrame, videoFrameCallback)) {
                return false;
            }
            
            if (_instance.OnLocalVideoSinkOnFrameEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnLocalVideoSinkOnFrameEvent != null)
                    {
                        _instance.OnLocalVideoSinkOnFrameEvent(index, videoFrame);
                    }
                });
            }
            return true;
        }

        static int localVideoSinkElapseIndex = 0;
        [MonoPInvokeCallback(typeof(OnLocalVideoSinkGetRenderElapseEventHandler))]
        public static int OnLocalVideoSinkGetRenderElapse(StreamIndex index)
        {
            if (localVideoSinkElapseIndex++ % 60 == 0)
            {
                string logInfo = string.Format("streamIndex: {0}", index);
                ByteRTCLog.ReportCallback("OnLocalVideoSinkGetRenderElapse", logInfo);
            }

            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            if (_instance.OnLocalVideoSinkGetRenderElapseEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnLocalVideoSinkGetRenderElapseEvent != null)
                    {
                        _instance.OnLocalVideoSinkGetRenderElapseEvent(index);
                    }
                });
            }
            return 0;
        }

        [MonoPInvokeCallback(typeof(OnLocalVideoSinkReleaseEventHandler))]
        public static void OnLocalVideoSinkRelease(StreamIndex index)
        {
            string logInfo = string.Format("streamIndex: {0}", index);
            ByteRTCLog.ReportCallback("OnLocalVideoSinkRelease", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnLocalVideoSinkReleaseEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnLocalVideoSinkReleaseEvent != null)
                    {
                        _instance.OnLocalVideoSinkReleaseEvent(index);
                    }
                });
            }
        }

        // remote video sink
        [MonoPInvokeCallback(typeof(OnRemoteVideoSinkOnFrameCallback))]
        public static bool OnRemoteVideoSinkOnFrame(RemoteStreamKey streamKey, IntPtr videoFrameCallbackPtr)
        {
            VideoFrameCallback videoFrameCallback = Marshal.PtrToStructure<VideoFrameCallback>(videoFrameCallbackPtr);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return false;
            }
            VideoFrame videoFrame = new VideoFrame();
            if (!ConvertVideoFrameCallback2VideoFrame(ref videoFrame, videoFrameCallback))
            {
                return false;
            }
            if (_instance.OnRemoteVideoSinkOnFrameEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnRemoteVideoSinkOnFrameEvent != null)
                    {
                        _instance.OnRemoteVideoSinkOnFrameEvent(streamKey, videoFrame);
                    }
                });
            }
            return true;
        }

       static bool ConvertVideoFrameCallback2VideoFrame(ref VideoFrame dstFrame, VideoFrameCallback srcFrame) {
            if (srcFrame.PixelFormat != VideoPixelFormat.kVideoPixelFormatI420
                && srcFrame.PixelFormat!=VideoPixelFormat.kVideoPixelFormatARGB) {
                ByteRTCLog.LogWarning(string.Format("src frame pixel format is {0}",srcFrame.PixelFormat));
                return false;
            }
            dstFrame.FrameType = srcFrame.FrameType;
            dstFrame.PixelFormat = srcFrame.PixelFormat;
            dstFrame.TimestampUs = srcFrame.TimestampUs;
            dstFrame.Width = srcFrame.Width;
            dstFrame.Height = srcFrame.Height;
            dstFrame.Rotation = srcFrame.Rotation;
            dstFrame.ColorSpace = srcFrame.ColorSpace;
            dstFrame.NumberOfPlanes = srcFrame.NumberOfPlanes;
            int num = srcFrame.NumberOfPlanes;
            dstFrame.PlaneLineSize = new int[num];
            Array.Copy(srcFrame.PlaneLineSize, dstFrame.PlaneLineSize, num);
       
            byte[][] buffer = new byte[num][];
            int planeSize = dstFrame.Width * dstFrame.Height;
            if (srcFrame.PixelFormat == VideoPixelFormat.kVideoPixelFormatI420)
            {
                for (int i = 0; i < num; ++i)
                {
                    if (i == 0)
                    {
                        int planeTotalSize = srcFrame.Height * srcFrame.PlaneLineSize[i];
                        buffer[i] = new byte[planeTotalSize];
                        Marshal.Copy(srcFrame.PlaneData[i], buffer[i], 0, planeTotalSize);
                    }
                    else if (i == 1 || i == 2)
                    {
                        int planeTotalSize = srcFrame.Height * srcFrame.PlaneLineSize[i]/2;
                        buffer[i] = new byte[planeTotalSize];
                        Marshal.Copy(srcFrame.PlaneData[i], buffer[i], 0, planeTotalSize);
                    }
                }
            }else if(srcFrame.PixelFormat == VideoPixelFormat.kVideoPixelFormatARGB) {
                buffer[0] = new byte[planeSize * 4];
                Marshal.Copy(srcFrame.PlaneData[0], buffer[0], 0, planeSize * 4);
            }
            
            dstFrame.PlaneData = buffer;
            int extraDataSize = srcFrame.ExtraDataInfoSize;
            dstFrame.ExtraDataInfoSize = extraDataSize;
            dstFrame.ExtraDataInfo = new byte[extraDataSize];
            if (extraDataSize != 0 && srcFrame.ExtraDataInfo != null)
            {
                Marshal.Copy(srcFrame.ExtraDataInfo, dstFrame.ExtraDataInfo, 0, extraDataSize);
            }
            int supSize = srcFrame.SupplementaryInfoSize;
            dstFrame.SupplementaryInfoSize = supSize;
            dstFrame.SupplementaryInfo = new byte[supSize];
            if (supSize != 0 && srcFrame.SupplementaryInfo != null)
            {
                Marshal.Copy(srcFrame.SupplementaryInfo, dstFrame.SupplementaryInfo, 0, supSize);
            }
            return true;
        }
        static bool ConvertAudioFrame2AudioFrameCallback(ref AudioFrameCallback dstFrame, AudioFrame srcFrame)
        {
            dstFrame.FrameType = srcFrame.FrameType;
            dstFrame.Channel = srcFrame.Channel;
            dstFrame.TimestampUs = srcFrame.TimestampUs;
            dstFrame.SampleRate = srcFrame.SampleRate;
            if (srcFrame.DataSize != 0)
            {
                dstFrame.DataSize = srcFrame.DataSize;
                dstFrame.Data = Marshal.UnsafeAddrOfPinnedArrayElement(srcFrame.Data, 0);
            }
            return true;
        }
        public static void ConvertAudioFrameCallback2AudioFrame(AudioFrameCallback srcAudioFrame, out AudioFrame dstAudioFrame)
        {
            dstAudioFrame.TimestampUs = srcAudioFrame.TimestampUs;
            dstAudioFrame.SampleRate = srcAudioFrame.SampleRate;
            dstAudioFrame.Channel = srcAudioFrame.Channel;
            int size = srcAudioFrame.DataSize;
            dstAudioFrame.Data = new byte[size];
            Marshal.Copy(srcAudioFrame.Data, dstAudioFrame.Data, 0, size);
            dstAudioFrame.DataSize = size;
            dstAudioFrame.FrameType = srcAudioFrame.FrameType;
            dstAudioFrame.IsMutedData = srcAudioFrame.IsMutedData;
        }


        static bool ConvertVideoFrame2VideoFrameCallback(ref VideoFrameCallback dstFrame, VideoFrame srcFrame)
        {
            //if (srcFrame.PixelFormat != VideoPixelFormat.kVideoPixelFormatI420
            //    && srcFrame.PixelFormat != VideoPixelFormat.kVideoPixelFormatARGB)
            //{
            //    ByteRTCLog.LogWarning(string.Format("src frame pixel format is {0}", srcFrame.PixelFormat));
            //    return false;
            //}
            dstFrame.FrameType = srcFrame.FrameType;
            dstFrame.PixelFormat = srcFrame.PixelFormat;
            dstFrame.TimestampUs = srcFrame.TimestampUs;
            dstFrame.Width = srcFrame.Width;
            dstFrame.Height = srcFrame.Height;
            dstFrame.Rotation = srcFrame.Rotation;
            dstFrame.ColorSpace = srcFrame.ColorSpace;
            dstFrame.NumberOfPlanes = srcFrame.NumberOfPlanes;
            int num = srcFrame.NumberOfPlanes;
            dstFrame.PlaneLineSize = srcFrame.PlaneLineSize;
            dstFrame.PlaneData = new IntPtr[4];
            dstFrame.Matrix = new float[16];
            Array.Copy(srcFrame.Matrix, dstFrame.Matrix, 16);
            //dstFrame.HwaccelContext = srcFrame.hwaccel_context; // Set by myself;
            dstFrame.TextureId = srcFrame.TextureId;
            for (int i = 0; i < num; i++)
            {
                dstFrame.PlaneData[i] = Marshal.UnsafeAddrOfPinnedArrayElement(srcFrame.PlaneData[i], 0);
            }


            dstFrame.PlaneLineSize = new int[4];
            if(num != 0){
            Array.Copy(srcFrame.PlaneLineSize, dstFrame.PlaneLineSize, num);
            }

            dstFrame.ExtraDataInfoSize = srcFrame.ExtraDataInfoSize;
            if (srcFrame.ExtraDataInfoSize != 0)
            {
                dstFrame.ExtraDataInfoSize = srcFrame.ExtraDataInfoSize;
                dstFrame.ExtraDataInfo = Marshal.UnsafeAddrOfPinnedArrayElement(srcFrame.ExtraDataInfo, 0);
            }

            dstFrame.SupplementaryInfoSize = srcFrame.SupplementaryInfoSize;
            if (srcFrame.SupplementaryInfoSize != 0)
            {
                dstFrame.SupplementaryInfoSize = srcFrame.SupplementaryInfoSize;
                dstFrame.SupplementaryInfo = Marshal.UnsafeAddrOfPinnedArrayElement(srcFrame.SupplementaryInfo, 0);
            }
            return true;
        }



        static int RemoteVideoSinkGetRenderElapseIndex = 0;
        [MonoPInvokeCallback(typeof(OnRemoteVideoSinkGetRenderElapseEventHandler))]
        public static int OnRemoteVideoSinkGetRenderElapse(RemoteStreamKey streamKey)
        {
            
            if (RemoteVideoSinkGetRenderElapseIndex++ % 60 == 0)
            {
                string logInfo = string.Format("streamIndex: {0}", streamKey);
                ByteRTCLog.ReportCallback("OnRemoteVideoSinkGetRenderElapse", logInfo);
            }
            
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            if (_instance.OnRemoteVideoSinkGetRenderElapseEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnRemoteVideoSinkGetRenderElapseEvent != null)
                    {
                        _instance.OnRemoteVideoSinkGetRenderElapseEvent(streamKey);
                    }     
                });
            }
            return 0;
        }

        [MonoPInvokeCallback(typeof(OnRemoteVideoSinkReleaseEventHandler))]
        public static void OnRemoteVideoSinkRelease(RemoteStreamKey streamKey)
        {
            string logInfo = string.Format("streamIndex: {0}", streamKey);
            ByteRTCLog.ReportCallback("OnRemoteVideoSinkRelease", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnRemoteVideoSinkReleaseEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnRemoteVideoSinkReleaseEvent != null)
                    {
                        _instance.OnRemoteVideoSinkReleaseEvent(streamKey);
                    }    
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnUserStartAudioCaptureEventHandler))]
        public static void OnUserStartAudioCapture(string roomID, string userID)
        {
            string logInfo = string.Format("roomID:{0}, UserID:{1}", roomID, userID);
            ByteRTCLog.ReportCallback("OnUserStartAudioCapture", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnUserStartAudioCaptureEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnUserStartAudioCaptureEvent != null)
                    {
                        _instance.OnUserStartAudioCaptureEvent(roomID, userID);
                    }
                });
            }
        }
      

        [MonoPInvokeCallback(typeof(OnUserStopAudioCaptureEventHandler))]
        public static void OnUserStopAudioCapture(string roomID, string userID)
        {
            string logInfo = string.Format("roomID:{0}, UserID:{1}", roomID, userID);
            ByteRTCLog.ReportCallback("OnUserStopAudioCapture", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnUserStopAudioCaptureEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnUserStopAudioCaptureEvent != null)
                    {
                        _instance.OnUserStopAudioCaptureEvent(roomID, userID);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnFirstLocalAudioFrameEventHandler))]
        public static void OnFirstLocalAudioFrame(StreamIndex index)
        {
            string logInfo = string.Format("streamIndex: {0}", index);
            ByteRTCLog.ReportCallback("OnFirstLocalAudioFrame", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnFirstLocalAudioFrameEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnFirstLocalAudioFrameEvent != null)
                    {
                        _instance.OnFirstLocalAudioFrameEvent(index);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnLocalAudioStateChangedEventHandler))]
        public static void OnLocalAudioStateChanged(LocalAudioStreamState state, LocalAudioStreamError error)
        {
            string logInfo = string.Format("state: {0}, error: {1}", state, error);
            ByteRTCLog.ReportCallback("OnLocalAudioStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnLocalAudioStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnLocalAudioStateChangedEvent != null)
                    {
                        _instance.OnLocalAudioStateChangedEvent(state, error);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnAudioFrameSendStateChangedEventHandler))]
        public static void OnAudioFrameSendStateChanged(string roomID, RtcUser user, FirstFrameSendState state)
        {
            string logInfo = string.Format("userID: {0}, metaData: {1}, state: {2}", user.UserID, user.MetaData, state);
            ByteRTCLog.ReportCallback("OnAudioFrameSendStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnAudioFrameSendStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnAudioFrameSendStateChangedEvent != null)
                    {
                        _instance.OnAudioFrameSendStateChangedEvent(roomID ,user, state);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnAudioFramePlayStateChangedEventHandler))]
        public static void OnAudioFramePlayStateChanged(string roomID, RtcUser user, FirstFramePlayState state)
        {
            string logInfo = string.Format("userID: {0}, metaData: {1}, state: {2}", user.UserID, user.MetaData, state);
            ByteRTCLog.ReportCallback("OnAudioFramePlayStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnAudioFramePlayStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnAudioFramePlayStateChangedEvent != null)
                    {
                        _instance.OnAudioFramePlayStateChangedEvent(roomID, user, state);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnUserStartVideoCaptureEventHandler))]
        public static void OnUserStartVideoCapture(string roomID,string userID)
        {
            string logInfo = string.Format("userID: {0}", userID);
            ByteRTCLog.ReportCallback("OnUserStartVideoCapture", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnUserStartVideoCaptureEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnUserStartVideoCaptureEvent != null)
                    {
                        _instance.OnUserStartVideoCaptureEvent(roomID, userID);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnUserStopVideoCaptureEventHandler))]
        public static void OnUserStopVideoCapture(string roomID, string userID)
        {
            string logInfo = string.Format("userID: {0}", userID);
            ByteRTCLog.ReportCallback("OnUserStopVideoCapture", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnUserStopVideoCaptureEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnUserStopVideoCaptureEvent != null)
                    {
                        _instance.OnUserStopVideoCaptureEvent(roomID, userID);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnFirstLocalVideoFrameCapturedEventHandler))]
        public static void OnFirstLocalVideoFrameCaptured(StreamIndex index, VideoFrameInfo info)
        {
            string logInfo = string.Format("index: {0}, width: {1}, height: {2}, rotation: {3}", index, info.width, info.height, info.rotation);
            ByteRTCLog.ReportCallback("OnFirstLocalVideoFrameCaptured", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnFirstLocalVideoFrameCapturedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnFirstLocalVideoFrameCapturedEvent != null)
                    {
                        _instance.OnFirstLocalVideoFrameCapturedEvent(index, info);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnLocalVideoSizeChangedEventHandler))]
        public static void OnLocalVideoSizeChanged(StreamIndex index, VideoFrameInfo info)
        {
            string logInfo = string.Format("index: {0}, width: {1}, height: {2}, rotation: {3}", index, info.width, info.height, info.rotation);
            ByteRTCLog.ReportCallback("OnLocalVideoSizeChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnLocalVideoSizeChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnLocalVideoSizeChangedEvent != null)
                    {
                        _instance.OnLocalVideoSizeChangedEvent(index, info);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnLocalVideoStateChangedEventHandler))]
        public static void OnLocalVideoStateChanged(StreamIndex index, LocalVideoStreamState state, LocalVideoStreamError error)
        {
            string logInfo = string.Format("index: {0}, state: {1}, error: {2}", index, state, error);
            ByteRTCLog.ReportCallback("OnLocalVideoStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnLocalVideoStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnLocalVideoStateChangedEvent != null)
                    {
                        _instance.OnLocalVideoStateChangedEvent(index, state, error);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnVideoFrameSendStateChangedEventHandler))]
        public static void OnVideoFrameSendStateChanged(string roomID, RtcUser user, FirstFrameSendState state)
        {
            string logInfo = string.Format("userID: {0}, metaData: {1}, state: {2}", user.UserID, user.MetaData, state);
            ByteRTCLog.ReportCallback("OnVideoFrameSendStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnVideoFrameSendStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnVideoFrameSendStateChangedEvent != null)
                    {
                        _instance.OnVideoFrameSendStateChangedEvent(roomID, user, state);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnScreenVideoFrameSendStateChangedEventHandler))]
        public static void OnScreenVideoFrameSendStateChanged(string roomID, RtcUser user, FirstFrameSendState state)
        {
            string logInfo = string.Format("userID: {0}, metaData: {1}, state: {2}", user.UserID, user.MetaData, state);
            ByteRTCLog.ReportCallback("OnScreenVideoFrameSendStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnScreenVideoFrameSendStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnScreenVideoFrameSendStateChangedEvent != null)
                    {
                        _instance.OnScreenVideoFrameSendStateChangedEvent(roomID, user, state);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnVideoFramePlayStateChangedEventHandler))]
        public static void OnVideoFramePlayStateChanged(string roomID, RtcUser user, FirstFramePlayState state)
        {
            string logInfo = string.Format("userID: {0}, metaData: {1}, state: {2}", user.UserID, user.MetaData, state);
            ByteRTCLog.ReportCallback("OnVideoFramePlayStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnVideoFramePlayStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnVideoFramePlayStateChangedEvent != null)
                    {
                        _instance.OnVideoFramePlayStateChangedEvent(roomID, user, state);
                    }
                });
            }
        }
        [MonoPInvokeCallback(typeof(OnRemoteVideoStateChangedEventHandler))]
        public static void OnRemoteVideoStateChanged(RemoteStreamKey key, RemoteVideoState state, RemoteVideoStateChangeReason reason)
        {
            string logInfo = string.Format("key:{0}, state{1}, reason{2}",key, state, reason);
            ByteRTCLog.ReportCallback("OnRemoteVideoStateChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnRemoteVideoStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnRemoteVideoStateChangedEvent != null)
                    {
                        _instance.OnRemoteVideoStateChangedEvent(key, state, reason);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnRemoteVideoSizeChangedEventHandler))]
        public static void OnRemoteVideoSizeChanged(RemoteStreamKey key, VideoFrameInfo info)
        {
            string logInfo = string.Format("streamKey:{0}",  key);
            ByteRTCLog.ReportCallback("OnRemoteVideoSizeChanged", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnRemoteVideoSizeChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    if (_instance != null && _instance.OnRemoteVideoSizeChangedEvent != null)
                    {
                        _instance.OnRemoteVideoSizeChangedEvent(key, info);
                    }
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnRemoteAudioStateChangedEventHandler))]
        public static void OnRemoteAudioStateChanged(RemoteStreamKey key, RemoteAudioState state, RemoteAudioStateChangeReason reason)
        {
            string logInfo = string.Format("UserID:{0}, state:{1}, reason:{2}", key.UserID, state, reason);
            ByteRTCLog.ReportCallback("OnRemoteAudioStateChanged", logInfo);

            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnRemoteAudioStateChangedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                   
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnFirstRemoteAudioFrameEventHandler))]
        public static void OnFirstRemoteAudioFrame(RemoteStreamKey key)
        {
            string logInfo = string.Format("UserID:{0}", key.UserID);
            ByteRTCLog.ReportCallback("OnFirstRemoteAudioFrame", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnFirstRemoteAudioFrameEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (_instance != null && _instance.OnFirstRemoteAudioFrameEvent != null)
                        {
                            _instance.OnFirstRemoteAudioFrameEvent(key);
                        }
                    });
                }
            
        }

        [MonoPInvokeCallback(typeof(OnFirstRemoteVideoFrameDecodedEventHandler))]
        public static void OnFirstRemoteVideoFrameDecoded(RemoteStreamKey key, VideoFrameInfo info)
        {
            string logInfo = string.Format(" streamKey:{0}", key);
            ByteRTCLog.ReportCallback("OnFirstRemoteVideoFrameDecoded", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnFirstRemoteVideoFrameDecodedEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        _instance.OnFirstRemoteVideoFrameDecodedEvent( key, info);
                    });
                }
        }

        [MonoPInvokeCallback(typeof(OnFirstRemoteVideoFrameRenderedEventHandler))]
        public static void OnFirstRemoteVideoFrameRendered(RemoteStreamKey key, VideoFrameInfo info)
        {
            string logInfo = string.Format(" streamKey:{0}", key);
            ByteRTCLog.ReportCallback("OnFirstRemoteVideoFrameRendered", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnFirstRemoteVideoFrameRenderedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    _instance.OnFirstRemoteVideoFrameRenderedEvent(key, info);
                });
            }
        }



        [MonoPInvokeCallback(typeof(OnPlayPublicStreamResultEventHandler))]
        public static void OnPlayPublicStreamResult(string public_stream_id, PublicStreamErrorCode result)
        {
            string logInfo = string.Format(" public_stream_id:{0},result:{1}", public_stream_id, result);
            ByteRTCLog.ReportCallback("OnPlayPublicStreamResult", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnPlayPublicStreamResultEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    _instance.OnPlayPublicStreamResultEvent(public_stream_id, result);
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnFaceDetectResultEventHandler))]
        public static void OnFaceDetectResult(FaceDetectResult result)
        {
            string logInfo = string.Format(" FaceDetectResult:{0}", result.rect[0].X);
            ByteRTCLog.ReportCallback("OnFaceDetectResult", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnFaceDetectResultEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    _instance.OnFaceDetectResultEvent(result);
                });
            }
        }
        [MonoPInvokeCallback(typeof(OnExpressionDetectResultEventHandler))]
        public static void OnExpressionDetectResult(ExpressionDetectResult result)
        {
            string logInfo = string.Format(" ExpressionDetectResult:{0}", result);
            ByteRTCLog.ReportCallback("ExpressionDetectResult", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            if (_instance.OnExpressionDetectResultEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    _instance.OnExpressionDetectResultEvent(result);
                });
            }
        }

        [MonoPInvokeCallback(typeof(OnProcessRecordAudioFrameEventHandler))]
        public static int OnProcessRecordAudioFrame(AudioFrameCallback audioFrameCallback)
        {
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return -1;
            }
            if (_instance != null && _instance.OnProcessRecordAudioFrameEvent != null)
            {
                _instance.OnProcessRecordAudioFrameEvent(audioFrameCallback);
            }
            return -1;
        }

        [MonoPInvokeCallback(typeof(OnProcessPlaybackAudioFrameEventHandler))]
        public static int OnProcessPlaybackAudioFrame(AudioFrameCallback audioFrameCallback)
        {
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return -1;
            }
            if (_instance.OnProcessPlaybackAudioFrameEvent != null)
            {
                return _instance.OnProcessPlaybackAudioFrameEvent(audioFrameCallback);
            }
            return -1;
        }

        [MonoPInvokeCallback(typeof(OnProcessRemoteUserAudioFrameEventHandler))]
        public static int OnProcessRemoteUserAudioFrame(RemoteStreamKey streamInfo, AudioFrameCallback audioFrameCallback)
        {
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return -1;
            }
            if (_instance.OnProcessRemoteUserAudioFrameEvent != null)
            {
                return _instance.OnProcessRemoteUserAudioFrameEvent(streamInfo, audioFrameCallback);
            }
            return -1;
        }

        [MonoPInvokeCallback(typeof(OnProcessEarMonitorAudioFrameEventHandler))]
        public static int OnProcessEarMonitorAudioFrame(AudioFrameCallback audioFrameCallback)
        {
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return -1;
            }
            if (_instance.OnProcessEarMonitorAudioFrameEvent != null)
            {
                return _instance.OnProcessEarMonitorAudioFrameEvent(audioFrameCallback);
            }
            return -1;
        }

        [MonoPInvokeCallback(typeof(OnProcessScreenAudioFrameEventHandler))]
        public static int OnProcessScreenAudioFrame(AudioFrameCallback audioFrameCallback)
        {
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return -1;
            }
            if (_instance.OnProcessScreenAudioFrameEvent != null)
            {
                _instance.OnProcessScreenAudioFrameEvent(audioFrameCallback);
                return 0;
            }
            return -1;
        }
        [MonoPInvokeCallback(typeof(OnStreamSyncInfoReceivedCallback))]
        public static void OnStreamSyncInfoReceived(RemoteStreamKey streamKey, SyncInfoStreamType streamType, IntPtr bufferPtr, int size)
        {
            byte[] buffer = new byte[size];
            Marshal.Copy(bufferPtr, buffer, 0, size);
            string text = System.Text.Encoding.UTF8.GetString(buffer);
            string logInfo = string.Format("RoomID: {0}, UserID: {1}, SyncInfoStreamType: {2}, data:{3}, length:{4}", streamKey.RoomID, streamKey.UserID, streamType, text, buffer.Length);
            ByteRTCLog.ReportCallback("OnStreamSyncInfoReceived", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return;
            }
            if (_instance.OnStreamSyncInfoReceivedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    _instance.OnStreamSyncInfoReceivedEvent(streamKey, streamType, buffer);
                });
            }
        }
        [MonoPInvokeCallback(typeof(OnSEIMessageReceivedCallback))]
        public static void OnSEIMessageReceived(RemoteStreamKey streamKey, IntPtr bufferPtr, int size)
        {
            byte[] buffer = new byte[size];
            Marshal.Copy(bufferPtr, buffer, 0, size);
            string text = System.Text.Encoding.UTF8.GetString(buffer);
            string logInfo = string.Format("RoomID: {0}, UserID: {1},  data:{2}, length:{3}", streamKey.RoomID, streamKey.UserID, text, buffer.Length);
            ByteRTCLog.ReportCallback("OnSEIMessageReceived", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCAudio is null");
                return;
            }
            if (_instance.OnSEIMessageReceivedEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    _instance.OnSEIMessageReceivedEvent(streamKey, buffer);
                });
            }
        }
        [MonoPInvokeCallback(typeof(OnSEIMessageUpdateCallback))]
        public static void OnSEIMessageUpdate(RemoteStreamKey stream_key, SEIStreamEventType type)
        {
            string logInfo = string.Format("SEIStreamEventType: {0}", type);
            ByteRTCLog.ReportCallback("OnSEIMessageUpdate", logInfo);
            if (_instance == null)
            {
                ByteRTCLog.LogWarning("RTCVideo is null");
                return;
            }
            if (_instance.OnSEIMessageUpdateEvent != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    _instance.OnSEIMessageUpdateEvent(stream_key, type);
                });
            }
        }
        public int PushExternalAudioFrame(AudioFrame frame)
        {
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return -1;
            }
            AudioFrameCallback cbFrame = new AudioFrameCallback();
            ConvertAudioFrame2AudioFrameCallback(ref cbFrame, frame);
            return RTCVideoEngineCXXBridge.PushExternalAudioFrame(_videoEngine, ref cbFrame);
        }
        public void StartScreenCapture(ScreenMediaType type, string bundleId)
        {
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return ;
            }
            RTCVideoEngineCXXBridge.StartScreenCapture(_videoEngine, type, bundleId);
        }

        public void SetAudioSourceType(AudioSourceType sourceType)
        {
            string logInfo = string.Format("type: {0}", sourceType);
            ByteRTCLog.ReportApiCall("SetAudioSourceType", logInfo);
            if (_videoEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("RTCEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.SetAudioSourceType(_videoEngine, sourceType);
        }
    }

    #region CallbackStruct

    public struct VideoFrameCallback
    {
        public VideoFrameType FrameType;
        public VideoPixelFormat PixelFormat;
        public VideoContentType ContentType;
        public long TimestampUs; // = 0;
        public int Width; // = 0;
        public int Height; // = 0;
        public VideoRotation Rotation;
        public ColorSpace ColorSpace;
        public int NumberOfPlanes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public IntPtr[] PlaneData;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] PlaneLineSize;
        public IntPtr ExtraDataInfo;
        public int ExtraDataInfoSize; // = 0;
        public IntPtr SupplementaryInfo;
        public int SupplementaryInfoSize; // = 0;
        public IntPtr HwaccelBuffer;
        public IntPtr HwaccelContext;
#if UNITY_ANDROID
        public IntPtr AndroidHwaccelContext;
#endif
       [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] Matrix;
        public int TextureId;
       // public CameraID mCameraID;
       // public FovVideoTileInfo FovTileInfo;
        public IntPtr cpp_class_ptr;
    };
    
    #endregion

    #region CallbackType

    public delegate void OnLocalAudioPropertiesReportCallback(IntPtr audioPropertiesInfos, int audioPropertiesInfoNumber);

    public delegate void OnRemoteAudioPropertiesReportCallback(IntPtr audioPropertiesInfos, int audioPropertiesInfoNumber, int totalRemoteVolume);

    public delegate bool OnLocalVideoSinkOnFrameCallback(StreamIndex index, IntPtr videoFrameCallback);

    public delegate bool OnRemoteVideoSinkOnFrameCallback(RemoteStreamKey streamKey, IntPtr videoFrameCallback);
    
    public delegate void OnNetworkQualityCallback(string roomID, NetworkQualityStats localQuality,IntPtr remoteQualities, int remoteQualityNum);

    public delegate void OnLocalStreamStatsCallback(LocalStreamStats  stats);
    public delegate void OnRemoteStreamStatsCallback(RemoteStreamStats stats);
    public delegate void OnStreamSyncInfoReceivedCallback(RemoteStreamKey streamKey, SyncInfoStreamType streamType, IntPtr bufferPtr, int size);

    public delegate void OnSEIMessageReceivedCallback(RemoteStreamKey streamKey,  IntPtr bufferPtr, int size);
    public delegate void OnSEIMessageUpdateCallback(RemoteStreamKey stream_key, SEIStreamEventType type);

    public delegate void OnRecordAudioFrameOriginalCallback(AudioFrameCallback audioFrameCallback);

    public delegate void OnRecordAudioFrameCallback(AudioFrameCallback audioFrameCallback);

    public delegate void OnPlaybackAudioFrameCallback(AudioFrameCallback audioFrameCallback);

    public delegate void OnRemoteUserAudioFrameCallback(RemoteStreamKey remoteStreamKey, AudioFrameCallback audioFrameCallback);

    public delegate void OnMixedAudioFrameCallback(AudioFrameCallback audioFrameCallback);

    public delegate void OnRecordScreenAudioFrameCallback(AudioFrameCallback audioFrameCallback);
    #endregion

    public struct VideoSDKEngineCallback
    {
        public OnWarningEventHandler OnWarning;
        public OnErrorEventHandler OnError;
        public OnLogReportEventHandler OnLogReport;

        public OnConnectionStateChangedEventHandler OnConnectionStateChanged;
        public OnNetworkTypeChangedEventHandler OnNetworkTypeChanged;
        public OnSysStatsEventHandler OnSysStats;

        // audio
        public OnAudioRouteChangedEventHandler OnAudioRouteChanged;
        public OnAudioDeviceStateChangedEventHandler OnAudioDeviceStateChanged;
        public OnAudioDeviceWarningEventHandler OnAudioDeviceWarning;
        public OnLocalAudioPropertiesReportCallback OnLocalAudioPropertiesReport;
        public OnRemoteAudioPropertiesReportCallback OnRemoteAudioPropertiesReport;

        public OnCreateRoomStateChangedEventHandler OnCreateRoomStateChanged;
        public OnAudioRecordingStateUpdateEventHandler OnAudioRecordingStateUpdate;

        public OnRecordAudioFrameOriginalCallback OnRecordAudioFrameOriginal;
        public OnRecordAudioFrameCallback OnRecordAudioFrame;
        public OnPlaybackAudioFrameCallback OnPlaybackAudioFrame;
        public OnRemoteUserAudioFrameCallback OnRemoteUserAudioFrame;
        public OnMixedAudioFrameCallback OnMixedAudioFrame;
        public OnRecordScreenAudioFrameCallback OnRecordScreenAudioFrame;

        public OnProcessRecordAudioFrameEventHandler OnProcessRecordAudioFrame;

        public OnProcessPlaybackAudioFrameEventHandler OnProcessPlaybackAudioFrame;

        public OnProcessRemoteUserAudioFrameEventHandler OnProcessRemoteUserAudioFrame;

        public OnProcessEarMonitorAudioFrameEventHandler OnProcessEarMonitorAudioFrame;

        public OnProcessScreenAudioFrameEventHandler OnProcessScreenAudioFrame;

        public OnStreamSyncInfoReceivedCallback OnStreamSyncInfoReceived;

        public OnSEIMessageReceivedCallback OnSEIMessageReceived;
        public OnSEIMessageUpdateCallback OnSEIMessageUpdate;

        //video
        public OnVideoDeviceStateChangedEventHandler OnVideoDeviceStateChanged;

        public OnVideoDeviceWarningEventHandler OnVideoDeviceWarning;

        // local video sink
        public OnLocalVideoSinkOnFrameCallback OnLocalVideoSinkOnFrame;

        public OnLocalVideoSinkGetRenderElapseEventHandler OnLocalVideoSinkGetRenderElapse;

        public OnLocalVideoSinkReleaseEventHandler OnLocalVideoSinkRelease;

        // remote video sink
        public OnRemoteVideoSinkOnFrameCallback OnRemoteVideoSinkOnFrame;

        public OnRemoteVideoSinkGetRenderElapseEventHandler OnRemoteVideoSinkGetRenderElapse;

        public OnRemoteVideoSinkReleaseEventHandler OnRemoteVideoSinkRelease;

        // room audio
        public OnUserStartAudioCaptureEventHandler OnUserStartAudioCapture;

        public OnUserStopAudioCaptureEventHandler OnUserStopAudioCapture;

        public OnFirstLocalAudioFrameEventHandler OnFirstLocalAudioFrame;

        public OnLocalAudioStateChangedEventHandler OnLocalAudioStateChanged;

        public OnRemoteAudioStateChangedEventHandler OnRemoteAudioStateChanged;   //349 add

        public OnAudioFrameSendStateChangedEventHandler OnAudioFrameSendStateChanged;

        public OnAudioFramePlayStateChangedEventHandler OnAudioFramePlayStateChanged;

        // room video
        public OnUserStartVideoCaptureEventHandler OnUserStartVideoCapture;

        public OnUserStopVideoCaptureEventHandler OnUserStopVideoCapture;

        public OnFirstLocalVideoFrameCapturedEventHandler OnFirstLocalVideoFrameCaptured;

        public OnLocalVideoSizeChangedEventHandler OnLocalVideoSizeChanged;

        public OnLocalVideoStateChangedEventHandler OnLocalVideoStateChanged;

        public OnVideoFrameSendStateChangedEventHandler OnVideoFrameSendStateChanged;

        public OnScreenVideoFrameSendStateChangedEventHandler OnScreenVideoFrameSendStateChanged;

        public OnVideoFramePlayStateChangedEventHandler OnVideoFramePlayStateChanged;

        public OnRemoteVideoSizeChangedEventHandler OnRemoteVideoSizeChanged;  //349 add

        public OnRemoteVideoStateChangedEventHandler OnRemoteVideoStateChanged;  //349 add

        public OnFirstRemoteAudioFrameEventHandler OnFirstRemoteAudioFrame;

        public OnFirstRemoteVideoFrameDecodedEventHandler OnFirstRemoteVideoFrameDecoded;

        public OnFirstRemoteVideoFrameRenderedEventHandler OnFirstRemoteVideoFrameRendered;

        public OnPlayPublicStreamResultEventHandler OnPlayPublicStreamResult;
        public OnFaceDetectResultEventHandler OnFaceDetectResult;
        public OnExpressionDetectResultEventHandler OnExpressionDetectResult;
    }
}