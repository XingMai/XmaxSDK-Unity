using System;
using System.Runtime.InteropServices;

#if UNITY_ANDROID
using UnityEngine;
#if UNITY_2018_3_OR_NEWER
using UnityEngine.Android;
#endif
#endif


namespace bytertc
{
    public class RTCVideoEngineCXXBridge
    {
#if UNITY_ANDROID
        private static AndroidJavaObject m_context = null;

        private static AndroidJavaObject Context
        {
            get
            {
                if( m_context == null )
                {
                    using( AndroidJavaObject unityClass = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" ) )
                    {
                        AndroidJavaObject activity = unityClass.GetStatic<AndroidJavaObject>( "currentActivity" );
                        m_context = activity.Call<AndroidJavaObject>("getApplicationContext");
                        string msg = "test msg activity raw:" + activity.GetRawObject() + " context raw:" + Context.GetRawObject();
                        ByteRTCLog.LogInfo(msg);
                    }
                }

                return m_context;
            }
        }


        private static AndroidJavaObject m_FrameConvertor = null;
        private static AndroidJavaObject FrameConvertor
        {
            get
            {
                if( m_FrameConvertor == null )
                {
                    m_FrameConvertor = new AndroidJavaObject("com.ss.bytertc.engine.utils.VideoFrameConverter");
                    string msg = "Get Frame Convertor :" + m_FrameConvertor.GetRawObject() + " context raw:" + m_FrameConvertor.GetRawObject();
                    ByteRTCLog.LogInfo(msg);   
                }
                return m_FrameConvertor;
            }
        }

        //TODO C# VideoFram to Java
        private static AndroidJavaObject m_sharedEGLContext = null;
        private static long m_sharedEGLContextHandle = 0;

        public static int GetCurrentEGLContext()
        {
            //using (AndroidJavaClass sharedEGLContext = new AndroidJavaClass("com.ss.bytertc.engine.utils.EglBaseUtils"))
            using (AndroidJavaClass sharedEGLContext = new AndroidJavaClass("com.bytedance.realx.video.EglBaseUtils"))
            {
                m_sharedEGLContext = sharedEGLContext.CallStatic<AndroidJavaObject>("getCurrentContext");
                m_sharedEGLContextHandle = m_sharedEGLContext.Call<long>("getNativeHandle");
            }

            int ret = (m_sharedEGLContextHandle == 0 ? -1 : 0);
            ByteRTCLog.LogInfo("GetCurrentEGLContext result " + ret);
            return ret;

        }
        public static void RequestScreenSharePermission()
        {
            using (AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call("requestForScreenSharing");
            }

        }
        public static void StopScreenSharePermission()
        {
            using (AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call("StopScreenSharing");
            }
        }
#endif

        public static IntPtr CreateRTCVideo(string appID, VideoSDKEngineCallback callback, string parameters)
        {
#if UNITY_ANDROID
            //AndroidJavaObject system = new AndroidJavaClass("java.lang.System");
            // system.CallStatic("loadLibrary", "volcenginertc");
            //system.CallStatic("loadLibrary", "byteaudio");
            string msg = "CreateRTCEngine,Init Context:" + Context + " raw:" + Context.GetRawObject();
            ByteRTCLog.LogInfo(msg);
            CXX_VideoSDKSetApplicationContext(Context.GetRawObject());
#endif
            IntPtr RCTEngine = CXX_VideoSDKCreateRTCVideo(appID, callback, parameters);
#if UNITY_ANDROID
            long VideoPtr = CXX_VideoSDKGetEnginePtr(RCTEngine);
            //AndroidJavaClass NetworkType = new AndroidJavaClass("com.ss.bytertc.engine.unity.NetworkTypeHelper");
            //NetworkType.CallStatic("registerCallback", VideoPtr);
#endif

            return RCTEngine;

        }

        public static void DestroyRTCVideo(IntPtr engine)
        {
#if UNITY_ANDROID
            long VideoPtr = CXX_VideoSDKGetEnginePtr(engine);
            //AndroidJavaClass NetworkType = new AndroidJavaClass("com.ss.bytertc.engine.unity.NetworkTypeHelper");
            //NetworkType.CallStatic("unregisterCallback", VideoPtr);
#endif
            CXX_VideoSDKDestroyRTCVideo(engine);
        }

        public static IntPtr GetErrorDescription(int code)
        {
            return CXX_VideoSDKGetErrorDescription(code);
        }

        public static IntPtr GetSDKVersion()
        {
            return CXX_VideoSDKGetSDKVersion();
        }

        public static int SetBusinessId(IntPtr engine, string businessID)
        {
            return CXX_VideoSDKSetBusinessId(engine, businessID);
        }

        public static int Feedback(IntPtr engine, Int64 problemFeedbackOption, IntPtr problemFeedbackInfo)
        {
            return CXX_VideoSDKFeedback(engine, problemFeedbackOption, problemFeedbackInfo);
        }

        public static void SetRuntimeParameters(IntPtr engine, string jsonString)
        {
            CXX_VideoSDKSetRuntimeParameters(engine, jsonString);
        }

        public static IntPtr CreateRTCRoom(IntPtr engine, string roomID)
        {
            return CXX_VideoSDKCreateRTCRoom(engine, roomID);
        }

        public static int MuteAudioCapture(IntPtr engine, StreamIndex index, bool mute)
        {
            return CXX_VideoSDKMuteAudioCapture(engine, index, mute);
        }

        public static void SetCaptureVolume(IntPtr engine, StreamIndex index, int volume)
        {
            CXX_VideoSDKSetCaptureVolume(engine, index, volume);
        }

        public static void SetPlaybackVolume(IntPtr engine, int volume)
        {
            CXX_VideoSDKSetPlaybackVolume(engine, volume);
        }

        public static void StartAudioCapture(IntPtr engine)
        {
            CXX_VideoSDKStartAudioCapture(engine);
        }

        public static void StopAudioCapture(IntPtr engine)
        {
            CXX_VideoSDKStopAudioCapture(engine);
        }

        public static void SetAudioProfile(IntPtr engine, AudioProfileType audioProfile)
        {
            CXX_VideoSDKSetAudioProfile(engine, audioProfile);
        }

        public static void SetAudioScenario(IntPtr engine, AudioScenarioType scenario)
        {
            CXX_VideoSDKSetAudioScenario(engine, scenario);
        }

        public static int SetAudioRoute(IntPtr engine, AudioRouteDevice device)
        {
            return CXX_VideoSDKSetAudioRoute(engine, device);
        }

        public static int EnableAudioPropertiesReport(IntPtr engine, AudioPropertiesConfig config)
        {
            return CXX_VideoSDKEnableAudioPropertiesReport(engine, config);
        }

        public static int EnableVocalInstrumentBalance(IntPtr engine, bool enable)
        {
            return CXX_VideoSDKEnableVocalInstrumentBalance(engine, enable);
        }

        //video manager
        public static int StartVideoCapture(IntPtr engine)
        {
            return CXX_VideoSDKStartVideoCapture(engine);
        }

        public static int StopVideoCapture(IntPtr engine)
        {
            return CXX_VideoSDKStopVideoCapture(engine);
        }

        public static int SetVideoCaptureConfig(IntPtr engine, VideoCaptureConfig videoCaptureConfig)
        {
            return CXX_VideoSDKSetVideoCaptureConfig(engine, videoCaptureConfig);
        }

        public static int SetVideoEncoderConfig1(IntPtr engine, VideoEncoderConfig maxSolution)
        {
            return CXX_VideoSDKSetVideoEncoderConfig1(engine, maxSolution);
        }

        public static int SetVideoEncoderConfig2(IntPtr engine, VideoEncoderConfig[] channelSolutions, int solutionNum)
        {
            return CXX_VideoSDKSetVideoEncoderConfig2(engine, channelSolutions, solutionNum);
        }

        public static int SetScreenVideoEncoderConfig(IntPtr engine, ScreenVideoEncoderConfig screenSolution)
        {
            return CXX_VideoSDKSetScreenVideoEncoderConfig(engine, screenSolution);
        }

        public static int SetVideoSourceType(IntPtr engine, StreamIndex streamIndex,VideoSourceType type)
        {
            return CXX_VideoSDKSetVideoSourceType(engine, streamIndex, type);
        }

        public static int SetLocalVideoMirrorType(IntPtr engine, MirrorType mirrorType)
        {
            return CXX_VideoSDKSetLocalVideoMirrorType(engine, mirrorType);
        }

        public static int SetVideoRotationMode(IntPtr engine, VideoRotationMode rotationMode)
        {
            return CXX_VideoSDKSetVideoRotationMode(engine, rotationMode);
        }

        public static int SetLocalVideoSink(IntPtr engine, StreamIndex index, VideoSinkPixelFormat requiredFormat)
        {
            return CXX_VideoSDKSetLocalVideoSink(engine, index, requiredFormat);
        }

        public static int SetRemoteVideoSink(IntPtr engine, RemoteStreamKey streamKey, VideoSinkPixelFormat requiredFormat)
        {
            return CXX_VideoSDKSetRemoteVideoSink(engine, streamKey, requiredFormat);
        }


        // screen

        public static IntPtr GetScreenCaptureSourceList(IntPtr engine)
        {
            return CXX_VideoSDKGetScreenCaptureSourceList(engine);
        }

        public static int StartScreenVideoCapture(IntPtr engine, ScreenCaptureSourceInfo sourceInfo, ScreenCaptureParameters captureParams)
        {
            return CXX_VideoSDKStartScreenVideoCapture(engine, sourceInfo, captureParams);
        }

        public static int StopScreenVideoCapture(IntPtr engine)
        {
            return CXX_VideoSDKStopScreenVideoCapture(engine);
        }

        public static int StartScreenCapture(IntPtr engine, ScreenMediaType type, string bundleId)
        {
             return CXX_VideoSDKStartScreenCapture(engine,type,bundleId);
        }

        public static int UpdateScreenCaptureRegion(IntPtr engine, Rectangle regionRect)
        {
            return CXX_VideoSDKUpdateScreenCaptureRegion(engine, regionRect);
        }

        public static int UpdateScreenCaptureHighlightConfig(IntPtr engine, HighlightConfig highlightConfig)
        {
            return CXX_VideoSDKUpdateScreenCaptureHighlightConfig(engine, highlightConfig);
        }

        public static int UpdateScreenCaptureMouseCursor(IntPtr engine, MouseCursorCaptureState captureMouseCursor)
        {
            return CXX_VideoSDKUpdateScreenCaptureMouseCursor(engine, captureMouseCursor);
        }

        public static int  UpdateScreenCaptureFilterConfig(IntPtr engine, ScreenFilterConfig filterConfig)
        {
            return CXX_VideoSDKUpdateScreenCaptureFilterConfig(engine, filterConfig);
        }

        public static IntPtr GetThumbnail(IntPtr engine, ScreenCaptureSourceType type, IntPtr sourceID, int maxWidth, int maxHeight)
        {
            return CXX_VideoSDKGetThumbnail(engine, type, sourceID, maxWidth, maxHeight);
        }

        public static void ReleaseVideoFrame(IntPtr videoFrameCallbackPtr)
        {
            CXX_VideoSDKReleaseVideoFrame(videoFrameCallbackPtr);
        }

        public static int SetScreenAudioSourceType(IntPtr engine, AudioSourceType sourceType)
        {
            return CXX_VideoSDKSetScreenAudioSourceType(engine, sourceType);
        }

        public static int SetAudioSourceType(IntPtr engine, AudioSourceType sourceType)
        {
            return CXX_VideoSDKSetAudioSourceType(engine, sourceType);
        }

        public static int SetScreenAudioStreamIndex(IntPtr engine, StreamIndex index)
        {
            return CXX_VideoSDKSetScreenAudioStreamIndex(engine, index);
        }

        public static int StartScreenAudioCapture(IntPtr engine)
        {
#if UNITY_ANDROID
            RequestScreenSharePermission();
            return 0;
#else
            return CXX_VideoSDKStartScreenAudioCapture(engine);
#endif
        }

        public static int StopScreenAudioCapture(IntPtr engine)
        {
#if UNITY_ANDROID
            StopScreenSharePermission();
            return 0;
#else
            return CXX_VideoSDKStopScreenAudioCapture(engine);
#endif
        }

        public static int PushScreenAudioFrame(IntPtr engine, ref AudioFrameCallback frame)
        {
            return CXX_VideoSDKPushScreenAudioFrame(engine, ref frame);
        }

        public static int PushExternalAudioFrame(IntPtr engine, ref AudioFrameCallback frame)
        {
            return CXX_VideoSDKPushExternalAudioFrame(engine, ref frame);
        }

        public static int SetVoiceReverbType(IntPtr engine, VoiceReverbType voiceReverb)
        {
            return CXX_VideoSDKSetVoiceReverbType(engine, voiceReverb);
        }
        public static int SetEarMonitorMode(IntPtr engine, EarMonitorMode mode)
        {
            return CXX_VideoSDKSetEarMonitorMode(engine, mode);
        }

        public static int SetEarMonitorVolume(IntPtr engine, int volume)
        {
            return CXX_VideoSDKSetEarMonitorVolume(engine, volume);
        }
        public static int SetRemoteAudioPlaybackVolume(IntPtr engine, string roomID, string userID, int volume)
        {
             return CXX_VideoSDKSetRemoteAudioPlaybackVolume(engine, roomID, userID, volume);
        }
 
        public static int SetDefaultAudioRoute(IntPtr engine, AudioRouteDevice route)
        {
            return CXX_VideoSDKSetDefaultAudioRoute(engine, route);
        }
        public static AudioRouteDevice GetAudioRoute(IntPtr engine)
        {
            return CXX_VideoSDKGetAudioRoute(engine);
        }
        public static int EnableAudioFrameCallback(IntPtr engine, AudioFrameCallbackMethod method, AudioFormat format)
        {
            return CXX_VideoSDKEnableAudioFrameCallback(engine, method, format);
        }

        public static int DisableAudioFrameCallback(IntPtr engine, AudioFrameCallbackMethod method)
        {
            return CXX_VideoSDKDisableAudioFrameCallback(engine, method);
        }
        public static int StartAudioRecording(IntPtr engine, AudioRecordingConfig config)
        {
            return CXX_VideoSDKStartAudioRecording(engine, config);
        }

        public static int StopAudioRecording(IntPtr engine)
        {
            return CXX_VideoSDKStopAudioRecording(engine);
        }

        public static int SendStreamSyncInfo(IntPtr engine, int stream_index, byte[] data, int length, int repeat_count)
        {
            return CXX_VideoSDKSendStreamSyncInfo(engine, stream_index, data, length, repeat_count);
        }

        public static int SendSEIMessage(IntPtr engine, int stream_index, byte[] data, int length, int repeat_count, int sei_mode)
        {
            return CXX_VideoSDKSendSEIMessage(engine, stream_index, data, length, repeat_count, sei_mode);
        }
        public static int EnableAudioProcessor(IntPtr engine, AudioProcessorMethod method, AudioFormat format)
        {
            return CXX_VideoSDKEnableAudioProcessor(engine, method, format);
        }

        public static int DisableAudioProcessor(IntPtr engine, AudioProcessorMethod method)
        {
            return CXX_VideoSDKDisableAudioProcessor(engine, method);
        }

        public static IntPtr AudioDevice_EnumerateAudioPlaybackDevices(IntPtr engine)
        {
            return CXX_VideoSDK_AudioDevice_EnumerateAudioPlaybackDevices(engine);
        }

        public static IntPtr AudioDevice_EnumerateAudioCaptureDevices(IntPtr engine)
        {
            return CXX_VideoSDK_AudioDevice_EnumerateAudioCaptureDevices(engine);
        }

        public static void AudioDevice_FollowSystemPlaybackDevice(IntPtr engine, Boolean followed)
        {
            CXX_VideoSDK_AudioDevice_FollowSystemPlaybackDevice(engine, followed);
        }

        public static void AudioDevice_FollowSystemCaptureDevice(IntPtr engine, Boolean followed)
        {
            CXX_VideoSDK_AudioDevice_FollowSystemCaptureDevice(engine, followed);
        }

        public static int AudioDevice_SetAudioPlaybackDevice(IntPtr engine, string deviceID)
        {
            return CXX_VideoSDK_AudioDevice_SetAudioPlaybackDevice(engine, deviceID);
        }

        public static int AudioDevice_SetAudioCaptureDevice(IntPtr engine,  string deviceID)
        {
            return CXX_VideoSDK_AudioDevice_SetAudioCaptureDevice(engine, deviceID);
        }

        public static int AudioDevice_SetAudioPlaybackDeviceVolume(IntPtr engine, uint volume)
        {
            return CXX_VideoSDK_AudioDevice_SetAudioPlaybackDeviceVolume(engine, volume);
        }

        public static int AudioDevice_GetAudioPlaybackDeviceVolume(IntPtr engine, ref uint volume)
        {
            return CXX_VideoSDK_AudioDevice_GetAudioPlaybackDeviceVolume(engine, out volume);
        }

        public static int AudioDevice_SetAudioCaptureDeviceVolume(IntPtr engine, uint volume)
        {
            return CXX_VideoSDK_AudioDevice_SetAudioCaptureDeviceVolume(engine, volume);
        }

        public static int AudioDevice_GetAudioCaptureDeviceVolume(IntPtr engine, ref uint volume)
        {
            return CXX_VideoSDK_AudioDevice_GetAudioCaptureDeviceVolume(engine, out volume);
        }

        public static int AudioDevice_SetAudioPlaybackDeviceMute(IntPtr engine, bool mute)
        {
            return CXX_VideoSDK_AudioDevice_SetAudioPlaybackDeviceMute(engine, mute);
        }

        public static int AudioDevice_GetAudioPlaybackDeviceMute(IntPtr engine, ref bool mute)
        {
            return CXX_VideoSDK_AudioDevice_GetAudioPlaybackDeviceMute(engine, out mute);
        }

        public static int AudioDevice_SetAudioCaptureDeviceMute(IntPtr engine, bool mute)
        {
            return CXX_VideoSDK_AudioDevice_SetAudioCaptureDeviceMute(engine, mute);
        }

        public static int AudioDevice_GetAudioCaptureDeviceMute(IntPtr engine, ref bool mute)
        {
            return CXX_VideoSDK_AudioDevice_GetAudioCaptureDeviceMute(engine, out mute);
        }

        public static int AudioDevice_GetAudioPlaybackDevice(IntPtr engine, ref byte deviceID)
        {
            return CXX_VideoSDK_AudioDevice_GetAudioPlaybackDevice(engine, ref deviceID);
        }

        public static int AudioDevice_GetAudioCaptureDevice(IntPtr engine,  ref byte deviceID)
        {
            return CXX_VideoSDK_AudioDevice_GetAudioCaptureDevice(engine, ref deviceID);
        }

        // video device
        public static IntPtr VideoDevice_EnumerateVideoCaptureDevices(IntPtr engine)
        {
            return CXX_VideoSDK_VideoDevice_EnumerateVideoCaptureDevices(engine);
        }

        public static int VideoDevice_SetVideoCaptureDevice(IntPtr engine, string deviceID)
        {
            return CXX_VideoSDK_VideoDevice_SetVideoCaptureDevice(engine, deviceID);
        }

        public static int VideoDevice_GetVideoCaptureDevice(IntPtr engine, ref byte deviceID)
        {
            return CXX_VideoSDK_VideoDevice_GetVideoCaptureDevice(engine,ref deviceID);
        }

        public static int DeviceCollection_GetCount(IntPtr collection)
        {
            return CXX_VideoSDK_DeviceCollection_GetCount(collection);
        }

        public static int DeviceCollection_GetDevice(IntPtr collection, int index, ref byte deviceName, ref byte deviceID)
        {
            return CXX_VideoSDK_DeviceCollection_GetDevice(collection, index, ref deviceName, ref deviceID);
        }

        public static void DeviceCollection_Release(IntPtr collection)
        {
            CXX_VideoSDK_DeviceCollection_Release(collection);
        }

        // screen capture source list
        public static int ScreenCaptureSourceList_GetCount(IntPtr sourceList)
        {
            return CXX_VideoSDK_ScreenCaptureSourceList_GetCount(sourceList);
        }

        public static ScreenCaptureSourceInfo ScreenCaptureSourceList_GetSourceInfo(IntPtr sourceList, int index)
        {
            ScreenCaptureSourceInfoForNative infoForNative = new ScreenCaptureSourceInfoForNative();
          
            CXX_VideoSDK_ScreenCaptureSourceList_GetSourceInfo(sourceList, index, ref infoForNative);
            ScreenCaptureSourceInfo info = new ScreenCaptureSourceInfo();
            info.SourceID = infoForNative.SourceID;
            info.type = infoForNative.type;
          //  info.application = infoForNative.application;
            info.PrimaryMonitor = infoForNative.PrimaryMonitor;
            info.pid = infoForNative.pid;

            var buffer = new byte[infoForNative.length];
            Marshal.Copy(infoForNative.SourceName, buffer, 0, infoForNative.length);

            if (infoForNative.AppLength > 0)
            {
                var bufferApplication = new byte[infoForNative.AppLength];
                Marshal.Copy(infoForNative.application, bufferApplication, 0, infoForNative.AppLength);
                info.application = global::System.Text.Encoding.UTF8.GetString(bufferApplication, 0, infoForNative.AppLength);
            }

            info.SourceName = global::System.Text.Encoding.UTF8.GetString(buffer, 0, infoForNative.length);
           
            return info;
        }

        public static void ScreenCaptureSourceList_Release(IntPtr sourceList)
        {
            CXX_VideoSDK_ScreenCaptureSourceList_Release(sourceList);
        }

        public static int SwitchCamera(IntPtr engine,CameraID cameraId)
        {
            return CXX_VideoSDKSwitchCamera(engine, cameraId);
        }

    

        public static int SetDummyCaptureImagePath(IntPtr engine, string file_path)
        {
             return CXX_VideoSDKSetDummyCaptureImagePath( engine,  file_path);
        }

        public static int RequestRemoteVideoKeyFrame(IntPtr engine, ref RemoteStreamKey stream_info)
        {
             return CXX_VideoSDKRequestRemoteVideoKeyFrame( engine, ref  stream_info);
        }

        public static int StartPlayPublicStream(IntPtr engine, string public_stream_id)
        {
                return CXX_VideoSDKStartPlayPublicStream( engine,  public_stream_id);

        }
        public static int StopPlayPublicStream(IntPtr engine, string public_stream_id)
        {
            return CXX_VideoSDKStopPlayPublicStream(engine, public_stream_id);

        }
        public static int StartPushPublicStream(IntPtr engine, string public_stream_id, IntPtr public_stream)
        {
            return CXX_VideoSDKStartPushPublicStream(engine, public_stream_id, public_stream);

        }
        public static int StopPushPublicStream(IntPtr engine, string public_stream_id)
        {
            return CXX_VideoSDKStopPushPublicStream(engine, public_stream_id);

        }


        public static int SetPublicStreamAudioPlaybackVolume(IntPtr engine, string public_stream_id, int volume)
        {
            return CXX_VideoSDKSetPublicStreamAudioPlaybackVolume( engine,  public_stream_id,  volume);

            //[DllImport(libname, EntryPoint = "VideoSDKUpdatePublicStreamParam")]
            //private static extern int VideoSDKUpdatePublicStreamParam( IntPtr engine,const char* public_stream_id,bytertc::IPublicStreamParam param);

            //设置变声特效类型
        }

        //开启本地音效混响效果
        public static int SetVoiceChangerType(IntPtr engine, VoiceChangerType voice_changer)
        {
            return CXX_VideoSDKSetVoiceChangerType( engine,  voice_changer);
        }
        public static int SetLocalVoicePitch(IntPtr engine, int pitch)
        {
            return CXX_VideoSDKSetLocalVoicePitch( engine,  pitch);
        }

        public static int SetLocalVoiceEqualization(IntPtr engine, VoiceEqualizationConfig config)
        {
            return CXX_VideoSDKSetLocalVoiceEqualization( engine,  config);
        }
        public static int SetLocalVoiceReverbParam(IntPtr engine, VoiceReverbConfig param)
        {
            return CXX_VideoSDKSetLocalVoiceReverbParam( engine,  param);
        }
        public static int EnableLocalVoiceReverb(IntPtr engine, bool enable)
        {
            return CXX_VideoSDKEnableLocalVoiceReverb( engine,  enable);
        }
        public static int SetVideoCaptureRotation(IntPtr engine, VideoRotation rotation)
        {
            return CXX_VideoSDKSetVideoCaptureRotation( engine,  rotation);
        }
        public static int SetVideoWatermark(IntPtr engine, StreamIndex stream_index, string image_path, RTCWatermarkConfig config)
        {
            return CXX_VideoSDKSetVideoWatermark( engine,  stream_index,  image_path, config);
        }
        public static int ClearVideoWatermark(IntPtr engine, StreamIndex stream_index)
        {
            return CXX_VideoSDKClearVideoWatermark( engine,  stream_index);

        }
        public static int EnableEffectBeauty(IntPtr engine, bool enable)
        {
            return CXX_VideoSDKEnableEffectBeauty( engine,  enable);

        }
        public static int SetBeautyIntensity(IntPtr engine, EffectBeautyMode beauty_mode, float intensity)
        {
            return CXX_VideoSDKSetBeautyIntensity( engine,  beauty_mode,  intensity);
        }
        public static int GetAuthMessage(IntPtr engine, ref IntPtr ppmsg, ref int len)
        {
            return CXX_VideoSDKGetAuthMessage( engine,  ref ppmsg, ref len);
        }
        public static int FreeAuthMessage(IntPtr engine, string pmsg)
        {
            return CXX_VideoSDKFreeAuthMessage( engine,  pmsg);
        }
        public static int InitCVResource(IntPtr engine, string license_file_path, string algo_model_dir)
        {
            return CXX_VideoSDKInitCVResource( engine,  license_file_path,  algo_model_dir);
        }
        public static int EnableVideoEffect(IntPtr engine)
        {
            return CXX_VideoSDKEnableVideoEffect( engine);
        }
        public static int DisableVideoEffect(IntPtr engine)
        {
            return CXX_VideoSDKDisableVideoEffect( engine);
        }
        public static int SetEffectNodes(IntPtr engine, string[] effect_node_paths, int node_num)
        {
            return CXX_VideoSDKSetEffectNodes( engine,  effect_node_paths,  node_num);
        }
        public static int UpdateEffectNode(IntPtr engine, string effect_node_path, string node_key, float node_value)
        {
            return CXX_VideoSDKUpdateEffectNode( engine,  effect_node_path,  node_key,  node_value);
        }
        public static int SetColorFilter(IntPtr engine, string res_path)
        {
            return CXX_VideoSDKSetColorFilter( engine,  res_path);
        }
        public static int SetColorFilterIntensity(IntPtr engine, float intensity)
        {
            return CXX_VideoSDKSetColorFilterIntensity( engine,  intensity);
        }
        public static int EnableVirtualBackground(IntPtr engine, string background_sticker_path, VirtualBackgroundSource source)
        {
            return CXX_VideoSDKEnableVirtualBackground( engine,  background_sticker_path,  source);
        }
        public static int DisableVirtualBackground(IntPtr engine)
        {
            return CXX_VideoSDKDisableVirtualBackground( engine);
        }
        //开启人脸识别功能，并设置人脸检测结果回调观察者
        public static int VideoSDKEnableFaceDetection(IntPtr engine,  uint interval_ms, string face_model_path)
        {
            return CXX_VideoSDKEnableFaceDetection(engine, interval_ms, face_model_path);
        }
        //关闭人脸识别功能
        public static int VideoSDKDisableFaceDetection(IntPtr engine)
        {
            return CXX_VideoSDKDisableFaceDetection(engine);
        }

        public static int SetVideoDigitalZoomConfig(IntPtr engine, ZoomConfigType type, float size)
        {
            return  CXX_VideoSDKSetVideoDigitalZoomConfig( engine,  type,  size);
        }
        public static int SetVideoDigitalZoomControl(IntPtr engine, ZoomDirectionType direction)
        {
            return  CXX_VideoSDKSetVideoDigitalZoomControl( engine,  direction);
        }
        public static int StartVideoDigitalZoomControl(IntPtr engine, ZoomDirectionType direction)
        {
            return  CXX_VideoSDKStartVideoDigitalZoomControl( engine,  direction);
        }
        public static int StopVideoDigitalZoomControl(IntPtr engine)
        {
            return  CXX_VideoSDKStopVideoDigitalZoomControl( engine);
        }


        public static int PushExternalVideoFrame(IntPtr engine, ref VideoFrameCallback frame)
        {
            int  ret = 0;
#if UNITY_ANDROID
            if(frame.FrameType == VideoFrameType.kVideoFrameTypeGLTexture)  {       
                if(m_sharedEGLContext !=null) {
                    frame.HwaccelContext = m_sharedEGLContext.GetRawObject();
                    frame.AndroidHwaccelContext = m_sharedEGLContext.GetRawObject();      
                }else{
                    ByteRTCLog.LogInfo("please call GetCurrentEGLContext before create engine");
                    return -1;
                } 
            }       
#endif
            ret = CXX_VideoSDKPushExternalVideoFrame(engine,ref frame);
            return ret;    
        }
        public static int PushScreenVideoFrame(IntPtr engine, ref VideoFrameCallback frame)
        {
            return CXX_VideoSDKPushScreenVideoFrame(engine, ref frame);
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        const string libname = "ByteRTCCWrapper";
#elif UNITY_IOS
        const string libname = "__Internal";
#elif UNITY_ANDROID
        const string libname = "ByteRTCCWrapper";
#else
        const string libname = "libVolcEngineRTC";
#endif

#if UNITY_ANDROID
        [DllImport(libname, EntryPoint="VideoSDKSetApplicationContext")]
        private static extern void CXX_VideoSDKSetApplicationContext(IntPtr context);

        [DllImport(libname, EntryPoint = "VideoSDKGetEnginePtr")]
        private static extern long CXX_VideoSDKGetEnginePtr(IntPtr context);

#endif

        [DllImport(libname, EntryPoint = "VideoSDKCreateRTCVideo")]
        private static extern IntPtr CXX_VideoSDKCreateRTCVideo(string appID, VideoSDKEngineCallback callback, string parameters);

        [DllImport(libname, EntryPoint = "VideoSDKDestroyRTCVideo")]
        private static extern void CXX_VideoSDKDestroyRTCVideo(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKGetErrorDescription")]
        private static extern IntPtr CXX_VideoSDKGetErrorDescription(int code);

        [DllImport(libname, EntryPoint = "VideoSDKGetSDKVersion")]
        private static extern IntPtr CXX_VideoSDKGetSDKVersion();

        [DllImport(libname, EntryPoint = "VideoSDKSetBusinessId")]
        private static extern int CXX_VideoSDKSetBusinessId(IntPtr engine, string businessID);

        [DllImport(libname, EntryPoint = "VideoSDKFeedback")]
        private static extern int CXX_VideoSDKFeedback(IntPtr engine,Int64 problemFeedbackOption, IntPtr problemFeedbackInfo);

        [DllImport(libname, EntryPoint = "VideoSDKSetRuntimeParameters")]
        private static extern int CXX_VideoSDKSetRuntimeParameters(IntPtr engine, string jsonString);

        [DllImport(libname, EntryPoint = "VideoSDKMuteAudioCapture")]
        private static extern int CXX_VideoSDKMuteAudioCapture(IntPtr engine, StreamIndex index, bool mute);

        [DllImport(libname, EntryPoint = "VideoSDKSetCaptureVolume")]
        private static extern int CXX_VideoSDKSetCaptureVolume(IntPtr engine, StreamIndex index, int volume);

        [DllImport(libname, EntryPoint = "VideoSDKSetPlaybackVolume")]
        private static extern int CXX_VideoSDKSetPlaybackVolume(IntPtr engine, int volume);

        [DllImport(libname, EntryPoint = "VideoSDKStartAudioCapture")]
        private static extern int CXX_VideoSDKStartAudioCapture(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKStopAudioCapture")]
        private static extern int CXX_VideoSDKStopAudioCapture(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKSetAudioProfile")]
        private static extern int CXX_VideoSDKSetAudioProfile(IntPtr engine, AudioProfileType audioProfile);

        [DllImport(libname, EntryPoint = "VideoSDKSetAudioScenario")]
        private static extern int CXX_VideoSDKSetAudioScenario(IntPtr engine, AudioScenarioType audioScenario);

        [DllImport(libname, EntryPoint = "VideoSDKSetAudioRoute")]
        private static extern int CXX_VideoSDKSetAudioRoute(IntPtr engine, AudioRouteDevice device);

        [DllImport(libname, EntryPoint = "VideoSDKEnableAudioPropertiesReport")]
        private static extern int CXX_VideoSDKEnableAudioPropertiesReport(IntPtr engine, AudioPropertiesConfig config);

        [DllImport(libname, EntryPoint = "VideoSDKEnableVocalInstrumentBalance")]
        private static extern int CXX_VideoSDKEnableVocalInstrumentBalance(IntPtr engine, bool enable);

        //video manager
        [DllImport(libname, EntryPoint = "VideoSDKStartVideoCapture")]
        private static extern int CXX_VideoSDKStartVideoCapture(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKStopVideoCapture")]
        private static extern int CXX_VideoSDKStopVideoCapture(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKSetVideoCaptureConfig")]
        private static extern int CXX_VideoSDKSetVideoCaptureConfig(IntPtr engine, VideoCaptureConfig videoCaptureConfig);

        [DllImport(libname, EntryPoint = "VideoSDKSetVideoEncoderConfig1")]
        private static extern int CXX_VideoSDKSetVideoEncoderConfig1(IntPtr engine, VideoEncoderConfig maxSolution);

        [DllImport(libname, EntryPoint = "VideoSDKSetVideoEncoderConfig2")]
        private static extern int CXX_VideoSDKSetVideoEncoderConfig2(IntPtr engine, VideoEncoderConfig[] channelSolutions, int solutionNum);

        [DllImport(libname, EntryPoint = "VideoSDKSetScreenVideoEncoderConfig")]
        private static extern int CXX_VideoSDKSetScreenVideoEncoderConfig(IntPtr engine, ScreenVideoEncoderConfig screenSolution);

        [DllImport(libname, EntryPoint = "VideoSDKSetVideoSourceType")]
        private static extern int CXX_VideoSDKSetVideoSourceType(IntPtr engine, StreamIndex streamIndex, VideoSourceType type);

        [DllImport(libname, EntryPoint = "VideoSDKSetLocalVideoMirrorType")]
        private static extern int CXX_VideoSDKSetLocalVideoMirrorType(IntPtr engine, MirrorType mirrorType);

        [DllImport(libname, EntryPoint = "VideoSDKSetVideoRotationMode")]
        private static extern int CXX_VideoSDKSetVideoRotationMode(IntPtr engine, VideoRotationMode rotationMode);

        [DllImport(libname, EntryPoint = "VideoSDKSetLocalVideoSink")]
        private static extern int CXX_VideoSDKSetLocalVideoSink(IntPtr engine, StreamIndex index, VideoSinkPixelFormat requiredFormat);

        [DllImport(libname, EntryPoint = "VideoSDKSetRemoteVideoSink")]
        private static extern int CXX_VideoSDKSetRemoteVideoSink(IntPtr engine, RemoteStreamKey streamKey, VideoSinkPixelFormat requiredFormat);

        //screen
        [DllImport(libname, EntryPoint = "VideoSDKGetScreenCaptureSourceList")]
        private static extern IntPtr CXX_VideoSDKGetScreenCaptureSourceList(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKStartScreenVideoCapture")]
        private static extern int CXX_VideoSDKStartScreenVideoCapture(IntPtr engine, ScreenCaptureSourceInfo sourceInfo, ScreenCaptureParameters captureParams);

        [DllImport(libname, EntryPoint = "VideoSDKStopScreenVideoCapture")]
        private static extern int CXX_VideoSDKStopScreenVideoCapture(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKStartScreenCapture")]
        private static extern int CXX_VideoSDKStartScreenCapture(IntPtr engine, ScreenMediaType type, string bundleId);

        [DllImport(libname, EntryPoint = "VideoSDKUpdateScreenCaptureRegion")]
        private static extern int CXX_VideoSDKUpdateScreenCaptureRegion(IntPtr engine, Rectangle regionRect);

        [DllImport(libname, EntryPoint = "VideoSDKUpdateScreenCaptureHighlightConfig")]
        private static extern int CXX_VideoSDKUpdateScreenCaptureHighlightConfig(IntPtr engine, HighlightConfig highlightConfig);

        [DllImport(libname, EntryPoint = "VideoSDKUpdateScreenCaptureMouseCursor")]
        private static extern int CXX_VideoSDKUpdateScreenCaptureMouseCursor(IntPtr engine, MouseCursorCaptureState captureMouseCursor);

        [DllImport(libname, EntryPoint = "VideoSDKUpdateScreenCaptureFilterConfig")]
        private static extern int CXX_VideoSDKUpdateScreenCaptureFilterConfig(IntPtr engine, ScreenFilterConfig filterConfig);

        [DllImport(libname, EntryPoint = "VideoSDKGetThumbnail")]
        private static extern IntPtr CXX_VideoSDKGetThumbnail(IntPtr engine, ScreenCaptureSourceType type, IntPtr sourceID, int maxWidth, int maxHeight);

        [DllImport(libname, EntryPoint = "VideoSDKReleaseVideoFrame")]
        private static extern void CXX_VideoSDKReleaseVideoFrame(IntPtr videoFrameCallbackPtr);

        [DllImport(libname, EntryPoint = "VideoSDKSetScreenAudioSourceType")]
        private static extern int CXX_VideoSDKSetScreenAudioSourceType(IntPtr engine, AudioSourceType sourceType);

        [DllImport(libname, EntryPoint = "VideoSDKSetAudioSourceType")]
        private static extern int CXX_VideoSDKSetAudioSourceType(IntPtr engine, AudioSourceType sourceType);

        [DllImport(libname, EntryPoint = "VideoSDKSetScreenAudioStreamIndex")]
        private static extern int CXX_VideoSDKSetScreenAudioStreamIndex(IntPtr engine, StreamIndex index);

        [DllImport(libname, EntryPoint = "VideoSDKStartScreenAudioCapture")]
        private static extern int CXX_VideoSDKStartScreenAudioCapture(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKStopScreenAudioCapture")]
        private static extern int CXX_VideoSDKStopScreenAudioCapture(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKPushScreenAudioFrame")]
        private static extern int CXX_VideoSDKPushScreenAudioFrame(IntPtr engine, ref AudioFrameCallback frame);

        [DllImport(libname, EntryPoint = "VideoSDKPushExternalAudioFrame")]
        private static extern int CXX_VideoSDKPushExternalAudioFrame(IntPtr engine, ref AudioFrameCallback frame);

        [DllImport(libname, EntryPoint = "VideoSDKSetVoiceReverbType")]
        private static extern int CXX_VideoSDKSetVoiceReverbType(IntPtr engine, VoiceReverbType voiceReverb);  //350 add reverberation

        [DllImport(libname, EntryPoint = "VideoSDKSetEarMonitorMode")]
        private static extern int CXX_VideoSDKSetEarMonitorMode(IntPtr engine, EarMonitorMode mode);  //350 add  ear-monitors mode，mobile option

        [DllImport(libname, EntryPoint = "VideoSDKSetEarMonitorVolume")]
        private static extern int CXX_VideoSDKSetEarMonitorVolume(IntPtr engine, int volume);   //350 add  ear-monitors volumn，mobile option

        [DllImport(libname, EntryPoint = "VideoSDKSetRemoteAudioPlaybackVolume")]
        private static extern int CXX_VideoSDKSetRemoteAudioPlaybackVolume(IntPtr engine, string roomID, string userID, int volume);

        [DllImport(libname, EntryPoint = "VideoSDKSetDefaultAudioRoute")]
        private static extern int CXX_VideoSDKSetDefaultAudioRoute(IntPtr engine, AudioRouteDevice route);

        [DllImport(libname, EntryPoint = "VideoSDKGetAudioRoute")]
        private static extern AudioRouteDevice CXX_VideoSDKGetAudioRoute(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKEnableAudioFrameCallback")]
        private static extern int CXX_VideoSDKEnableAudioFrameCallback(IntPtr engine, AudioFrameCallbackMethod method, AudioFormat format);

        [DllImport(libname, EntryPoint = "VideoSDKDisableAudioFrameCallback")]
        private static extern int CXX_VideoSDKDisableAudioFrameCallback(IntPtr engine, AudioFrameCallbackMethod method);
       
        [DllImport(libname, EntryPoint = "VideoSDKStartAudioRecording")]
        private static extern int CXX_VideoSDKStartAudioRecording(IntPtr engine, AudioRecordingConfig config);

        [DllImport(libname, EntryPoint = "VideoSDKStopAudioRecording")]
        private static extern int CXX_VideoSDKStopAudioRecording(IntPtr engine);

        // audio device
        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_EnumerateAudioPlaybackDevices")]
        private static extern IntPtr CXX_VideoSDK_AudioDevice_EnumerateAudioPlaybackDevices(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_EnumerateAudioCaptureDevices")]
        private static extern IntPtr CXX_VideoSDK_AudioDevice_EnumerateAudioCaptureDevices(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_FollowSystemPlaybackDevice")]
        private static extern IntPtr CXX_VideoSDK_AudioDevice_FollowSystemPlaybackDevice(IntPtr engine, Boolean followed);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_FollowSystemCaptureDevice")]
        private static extern IntPtr CXX_VideoSDK_AudioDevice_FollowSystemCaptureDevice(IntPtr engine, Boolean followed);


        [DllImport(libname, EntryPoint = "VideoSDKSendStreamSyncInfo")]
        private static extern int CXX_VideoSDKSendStreamSyncInfo(IntPtr engine, int stream_index, byte[] data, int length, int repeat_count);

        [DllImport(libname, EntryPoint = "VideoSDKSendSEIMessage")]
        private static extern int CXX_VideoSDKSendSEIMessage(IntPtr engine, int stream_index, byte[] data, int length, int repeat_count, int sei_mode);

        [DllImport(libname, EntryPoint = "VideoSDKEnableAudioProcessor")]
        private static extern int CXX_VideoSDKEnableAudioProcessor(IntPtr engine, AudioProcessorMethod audioProcessorMethod, AudioFormat audioFormat);

        [DllImport(libname, EntryPoint = "VideoSDKDisableAudioProcessor")]
        private static extern int CXX_VideoSDKDisableAudioProcessor(IntPtr engine, AudioProcessorMethod audioProcessorMethod);


        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_SetAudioPlaybackDevice")]
        private static extern int CXX_VideoSDK_AudioDevice_SetAudioPlaybackDevice(IntPtr engine, string deviceID);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_SetAudioCaptureDevice")]
        private static extern int CXX_VideoSDK_AudioDevice_SetAudioCaptureDevice(IntPtr engine, string deviceID);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_SetAudioPlaybackDeviceVolume")]
        private static extern int CXX_VideoSDK_AudioDevice_SetAudioPlaybackDeviceVolume(IntPtr engine, uint volume);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_GetAudioPlaybackDeviceVolume")]
        private static extern int CXX_VideoSDK_AudioDevice_GetAudioPlaybackDeviceVolume(IntPtr engine, out uint volume);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_SetAudioCaptureDeviceVolume")]
        private static extern int CXX_VideoSDK_AudioDevice_SetAudioCaptureDeviceVolume(IntPtr engine, uint volume);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_GetAudioCaptureDeviceVolume")]
        private static extern int CXX_VideoSDK_AudioDevice_GetAudioCaptureDeviceVolume(IntPtr engine, out uint volume);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_SetAudioPlaybackDeviceMute")]
        private static extern int CXX_VideoSDK_AudioDevice_SetAudioPlaybackDeviceMute(IntPtr engine, bool mute);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_GetAudioPlaybackDeviceMute")]
        private static extern int CXX_VideoSDK_AudioDevice_GetAudioPlaybackDeviceMute(IntPtr engine, out bool mute);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_SetAudioCaptureDeviceMute")]
        private static extern int CXX_VideoSDK_AudioDevice_SetAudioCaptureDeviceMute(IntPtr engine, bool mute);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_GetAudioCaptureDeviceMute")]
        private static extern int CXX_VideoSDK_AudioDevice_GetAudioCaptureDeviceMute(IntPtr engine, out bool mute);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_GetAudioPlaybackDevice")]
        private static extern int CXX_VideoSDK_AudioDevice_GetAudioPlaybackDevice(IntPtr engine, ref byte deviceID);

        [DllImport(libname, EntryPoint = "VideoSDK_AudioDevice_GetAudioCaptureDevice")]
        private static extern int CXX_VideoSDK_AudioDevice_GetAudioCaptureDevice(IntPtr engine, ref byte deviceID);


        // video device
        [DllImport(libname, EntryPoint = "VideoSDK_VideoDevice_SetVideoCaptureDevice")]
        private static extern int CXX_VideoSDK_VideoDevice_SetVideoCaptureDevice(IntPtr engine, string deviceID);

        [DllImport(libname, EntryPoint = "VideoSDK_VideoDevice_GetVideoCaptureDevice")]
        private static extern int CXX_VideoSDK_VideoDevice_GetVideoCaptureDevice(IntPtr engine,ref byte deviceID);

        [DllImport(libname, EntryPoint = "VideoSDK_VideoDevice_EnumerateVideoCaptureDevices")]
        private static extern IntPtr CXX_VideoSDK_VideoDevice_EnumerateVideoCaptureDevices(IntPtr engine);

        [DllImport(libname, EntryPoint = "VideoSDKCreateRTCRoom")]
        private static extern IntPtr CXX_VideoSDKCreateRTCRoom(IntPtr engine, string roomID);

        [DllImport(libname, EntryPoint = "VideoSDK_DeviceCollection_GetCount")]
        private static extern int CXX_VideoSDK_DeviceCollection_GetCount(IntPtr collection);

        [DllImport(libname, EntryPoint = "VideoSDK_DeviceCollection_GetDevice")]
        private static extern int CXX_VideoSDK_DeviceCollection_GetDevice(IntPtr collection, int dex, ref byte deviceName, ref byte deviceID);

        [DllImport(libname, EntryPoint = "VideoSDK_DeviceCollection_Release")]
        private static extern void CXX_VideoSDK_DeviceCollection_Release(IntPtr collection);

        // screen capture source list
        [DllImport(libname, EntryPoint = "VideoSDK_ScreenCaptureSourceList_GetCount")]
        private static extern int CXX_VideoSDK_ScreenCaptureSourceList_GetCount(IntPtr sourceList);

        [DllImport(libname, EntryPoint = "VideoSDK_ScreenCaptureSourceList_GetSourceInfo")]
        private static extern int CXX_VideoSDK_ScreenCaptureSourceList_GetSourceInfo(IntPtr sourceList, int index,ref ScreenCaptureSourceInfoForNative info);

        [DllImport(libname, EntryPoint = "VideoSDK_ScreenCaptureSourceList_Release")]
        private static extern void CXX_VideoSDK_ScreenCaptureSourceList_Release(IntPtr sourceList);

        [DllImport(libname, EntryPoint = "VideoSDKSwitchCamera")]
        private static extern int CXX_VideoSDKSwitchCamera(IntPtr engine, CameraID cameraId);

        [DllImport(libname, EntryPoint = "VideoSDKPushExternalVideoFrame")]
        private static extern int CXX_VideoSDKPushExternalVideoFrame(IntPtr engine,ref VideoFrameCallback frame);

        [DllImport(libname, EntryPoint = "VideoSDKPushScreenVideoFrame")]
        private static extern int CXX_VideoSDKPushScreenVideoFrame(IntPtr engine, ref VideoFrameCallback frame);



        [DllImport(libname, EntryPoint = "VideoSDKSetDummyCaptureImagePath")]
        private static extern int CXX_VideoSDKSetDummyCaptureImagePath(IntPtr engine,string file_path);

        [DllImport(libname, EntryPoint = "VideoSDKRequestRemoteVideoKeyFrame")]
        private static extern int CXX_VideoSDKRequestRemoteVideoKeyFrame(IntPtr engine, ref RemoteStreamKey stream_info);

        [DllImport(libname, EntryPoint = "VideoSDKStartPlayPublicStream")]
        private static extern int CXX_VideoSDKStartPlayPublicStream(IntPtr engine, string public_stream_id);

        [DllImport(libname, EntryPoint = "VideoSDKStopPlayPublicStream")]
        private static extern int CXX_VideoSDKStopPlayPublicStream(IntPtr engine,string public_stream_id);

        [DllImport(libname, EntryPoint = "VideoSDKStartPushPublicStream")]
        private static extern int CXX_VideoSDKStartPushPublicStream(IntPtr engine, string public_stream_id, IntPtr public_stream);

        [DllImport(libname, EntryPoint = "VideoSDKStopPushPublicStream")]
        private static extern int CXX_VideoSDKStopPushPublicStream(IntPtr engine, string public_stream_id);

        [DllImport(libname, EntryPoint = "VideoSDKSetPublicStreamAudioPlaybackVolume")]
        private static extern int CXX_VideoSDKSetPublicStreamAudioPlaybackVolume(IntPtr engine,string public_stream_id,int volume);

        //[DllImport(libname, EntryPoint = "VideoSDKUpdatePublicStreamParam")]
        //private static extern int VideoSDKUpdatePublicStreamParam( IntPtr engine,const char* public_stream_id,bytertc::IPublicStreamParam param);

        //设置变声特效类型
        [DllImport(libname, EntryPoint = "VideoSDKSetVoiceChangerType")]
        private static extern int CXX_VideoSDKSetVoiceChangerType(IntPtr engine, VoiceChangerType voice_changer);

        //开启本地语音变调功能，多用于K歌场景
        [DllImport(libname, EntryPoint = "VideoSDKSetLocalVoicePitch")]
        private static extern int CXX_VideoSDKSetLocalVoicePitch(IntPtr engine, int pitch);

        //设置本地采集语音的均衡效果，包括内部采集和外部采集，但不包含混音音频文件
        [DllImport(libname, EntryPoint = "VideoSDKSetLocalVoiceEqualization")]
        private static extern int CXX_VideoSDKSetLocalVoiceEqualization(IntPtr engine, VoiceEqualizationConfig config);

        //设置本地采集音频的混响效果，包括内部采集和外部采集，但不包含混音音频文件
        [DllImport(libname, EntryPoint = "VideoSDKSetLocalVoiceReverbParam")]
        private static extern int CXX_VideoSDKSetLocalVoiceReverbParam(IntPtr engine, VoiceReverbConfig param);

        //开启本地音效混响效果
        [DllImport(libname, EntryPoint = "VideoSDKEnableLocalVoiceReverb")]
        private static extern int CXX_VideoSDKEnableLocalVoiceReverb(IntPtr engine, bool enable);


        //设置本端采集的视频帧的旋转角度
        [DllImport(libname, EntryPoint = "VideoSDKSetVideoCaptureRotation")]
        private static extern int CXX_VideoSDKSetVideoCaptureRotation(IntPtr engine, VideoRotation rotation);

        //在指定视频流上添加水印
        [DllImport(libname, EntryPoint = "VideoSDKSetVideoWatermark")]
        private static extern int CXX_VideoSDKSetVideoWatermark(IntPtr engine, StreamIndex stream_index, string image_path, RTCWatermarkConfig config);

        //移除指定视频流的水印
        [DllImport(libname, EntryPoint = "VideoSDKClearVideoWatermark")]
        private static extern int CXX_VideoSDKClearVideoWatermark(IntPtr engine, StreamIndex stream_index);

        //开启/关闭基础美颜
        [DllImport(libname, EntryPoint = "VideoSDKEnableEffectBeauty")]
        private static extern int CXX_VideoSDKEnableEffectBeauty(IntPtr engine, bool enable);

        //调整基础美颜强度
        [DllImport(libname, EntryPoint = "VideoSDKSetBeautyIntensity")]
        private static extern int CXX_VideoSDKSetBeautyIntensity(IntPtr engine, EffectBeautyMode beauty_mode, float intensity);

        ////获取视频特效接口
        ///[DllImport(libname, EntryPoint = "VideoSDKGetVideoEffectInterface")]
        // bytertc::IVideoEffect VideoSDKGetVideoEffectInterface( IntPtr engine);

        //从特效 SDK 获取授权消息，用于获取在线许可证。
        [DllImport(libname, EntryPoint = "VideoSDKGetAuthMessage")]
        private static extern int CXX_VideoSDKGetAuthMessage(IntPtr engine, ref IntPtr ppmsg, ref int len);

        //使用完授权消息字符串后，释放内存
        [DllImport(libname, EntryPoint = "VideoSDKFreeAuthMessage")]
        private static extern int CXX_VideoSDKFreeAuthMessage(IntPtr engine, string pmsg);

        //检查视频特效证书，设置算法模型路径，并初始化特效模块
        [DllImport(libname, EntryPoint = "VideoSDKInitCVResource")]
        private static extern int CXX_VideoSDKInitCVResource(IntPtr engine, string license_file_path, string algo_model_dir);

        //开启高级美颜、滤镜等视频特效
        [DllImport(libname, EntryPoint = "VideoSDKEnableVideoEffect")]
        private static extern int CXX_VideoSDKEnableVideoEffect(IntPtr engine);

        //关闭视频特效
        [DllImport(libname, EntryPoint = "VideoSDKDisableVideoEffect")]
        private static extern int CXX_VideoSDKDisableVideoEffect(IntPtr engine);

        //设置视频特效素材包
        [DllImport(libname, EntryPoint = "VideoSDKSetEffectNodes")]
        private static extern int CXX_VideoSDKSetEffectNodes(IntPtr engine,string[] effect_node_paths,int node_num);

        //设置特效强度
        [DllImport(libname, EntryPoint = "VideoSDKUpdateEffectNode")]
        private static extern int CXX_VideoSDKUpdateEffectNode(IntPtr engine,string effect_node_path,string node_key,float node_value);

        //设置颜色滤镜
        [DllImport(libname, EntryPoint = "VideoSDKSetColorFilter")]
        private static extern int CXX_VideoSDKSetColorFilter(IntPtr engine, string res_path);

        //设置已启用颜色滤镜的强度
        [DllImport(libname, EntryPoint = "VideoSDKSetColorFilterIntensity")]
        private static extern int CXX_VideoSDKSetColorFilterIntensity(IntPtr engine, float intensity);

        //将摄像头采集画面中的人像背景替换为指定图片或纯色背景。
        [DllImport(libname, EntryPoint = "VideoSDKEnableVirtualBackground")]
        private static extern int CXX_VideoSDKEnableVirtualBackground(IntPtr engine, string background_sticker_path, VirtualBackgroundSource source);

        //关闭虚拟背景
        [DllImport(libname, EntryPoint = "VideoSDKDisableVirtualBackground")]
        private static extern int CXX_VideoSDKDisableVirtualBackground(IntPtr engine);

        //开启人脸识别功
        [DllImport(libname, EntryPoint = "VideoSDKEnableFaceDetection")]
        private static extern int CXX_VideoSDKEnableFaceDetection(IntPtr engine, uint interval_ms, string face_model_path);

        //关闭人脸识别功能
        [DllImport(libname, EntryPoint = "VideoSDKDisableFaceDetection")]
        private static extern int CXX_VideoSDKDisableFaceDetection(IntPtr engine);


        //设置本地摄像头数码变焦参数，包括缩放倍数，移动步长
        [DllImport(libname, EntryPoint = "VideoSDKSetVideoDigitalZoomConfig")]
        private static extern int CXX_VideoSDKSetVideoDigitalZoomConfig(IntPtr engine, ZoomConfigType type, float size);

        //控制本地摄像头数码变焦，缩放或移动一次。设置对本地预览画面和发布到远端的视频都生效。
        [DllImport(libname, EntryPoint = "VideoSDKSetVideoDigitalZoomControl")]
        private static extern int CXX_VideoSDKSetVideoDigitalZoomControl(IntPtr engine, ZoomDirectionType direction);

        //开启本地摄像头持续数码变焦，缩放或移动。设置对本地预览画面和发布到远端的视频都生效。
        [DllImport(libname, EntryPoint = "VideoSDKStartVideoDigitalZoomControl")]
        private static extern int CXX_VideoSDKStartVideoDigitalZoomControl(IntPtr engine, ZoomDirectionType direction);

        //停止本地摄像头持续数码变焦。
        [DllImport(libname, EntryPoint = "VideoSDKStopVideoDigitalZoomControl")]
        private static extern int CXX_VideoSDKStopVideoDigitalZoomControl(IntPtr engine);



    }
    public struct ScreenCaptureSourceInfoForNative
    {
        public ScreenCaptureSourceType type;
        public IntPtr SourceID;
        public IntPtr SourceName;
        public int length;
        public IntPtr application;
        public int AppLength;
        public int pid;
        public bool PrimaryMonitor;
        public Rectangle RegionRect;
    };
}