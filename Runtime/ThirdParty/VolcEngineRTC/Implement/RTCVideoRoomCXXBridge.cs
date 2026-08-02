using System;
using System.Runtime.InteropServices;

namespace bytertc
{
    public class RTCVideoRoomCXXBridge
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        const string libname = "ByteRTCCWrapper";
#elif UNITY_IOS
    const string libname = "__Internal";
#elif UNITY_ANDROID
    const string libname = "ByteRTCCWrapper";
#else
        const string libname = "libByteRTCCWrapper";    
#endif


        public static void Destroy(IntPtr room)
        {
            CXX_VideoSDKDestroy(room);
        }
        public static int JoinRoom(IntPtr room, string token, UserInfo info, MultiRoomConfigStruct roomConfig)
        {
            return CXX_VideoSDKJoinRoom(room, token, info, roomConfig);
        }

        public static void LeaveRoom(IntPtr room)
        {
            CXX_VideoSDKLeaveRoom(room);
        }

        public static void UpdateToken(IntPtr room, string token)
        {
            CXX_VideoSDKUpdateToken(room, token);
        }

        public static void SetUserVisibility(IntPtr room, bool enable)
        {
            CXX_VideoSDKSetUserVisibility(room, enable);
        }

        public static void SetRTCRoomEventHandler(IntPtr room, RTCVideoSDKRoomCallback callback)
        {
            CXX_VideoSDKSetRTCRoomEventHandler(room, callback);
        }


        public static void PublishStream(IntPtr room, MediaStreamType type)
        {
            CXX_VideoSDKPublishStream(room, type);
        }

        public static void UnpublishStream(IntPtr room, MediaStreamType type)
        {
            CXX_VideoSDKUnpublishStream(room, type);
        }

        public static void PublishScreen(IntPtr room, MediaStreamType type)
        {
            CXX_VideoSDKPublishScreen(room, type);
        }

        public static void UnpublishScreen(IntPtr room, MediaStreamType type)
        {
            CXX_VideoSDKUnpublishScreen(room, type);
        }

        public static void SubscribeStream(IntPtr room, string userID, MediaStreamType type)
        {
            CXX_VideoSDKSubscribeStream(room, userID, type);
        }

        public static void UnsubscribeStream(IntPtr room, string userID, MediaStreamType type)
        {
            CXX_VideoSDKUnsubscribeStream(room, userID, type);
        }

        public static void SubscribeScreen(IntPtr room, string userID, MediaStreamType type)
        {
            CXX_VideoSDKSubscribeScreen(room, userID, type);
        }

        public static void UnsubscribeScreen(IntPtr room, string userID, MediaStreamType type)
        {
            CXX_VideoSDKUnsubscribeScreen(room, userID, type);
        }

        public static void PauseAllSubscribedStream(IntPtr room, PauseResumeControlMediaType mediaType)
        {
            CXX_VideoSDKPauseAllSubscribedStream(room, mediaType);
        }

        public static void ResumeAllSubscribedStream(IntPtr room, PauseResumeControlMediaType mediaType)
        {
            CXX_VideoSDKResumeAllSubscribedStream(room, mediaType);
        }
        public static int UnsubscribeAllStreams(IntPtr room, MediaStreamType mediaType)
        {
            return CXX_VideoSDKUnsubscribeAllStreams(room, mediaType);
        }

        public static int SubscribeAllStreams(IntPtr room, MediaStreamType mediaType)
        {
            return CXX_VideoSDKSubscribeAllStreams(room, mediaType);
        }


        public static IntPtr GetRangeAudio(IntPtr room)
        {
            return CXX_VideoSDKGetRangeAudio(room);
        }

        public static IntPtr GetSpatialAudio(IntPtr room)
        {
            return CXX_VideoSDKGetSpatialAudio(room);
        }

        public static long SendUserMessage(IntPtr room, string userID, string message)
        {
            return CXX_VideoSDKSendUserMessage(room, userID, message);
        }

        public static long SendRoomMessage(IntPtr room, string message)
        {
            return CXX_VideoSDKSendRoomMessage(room, message);
        }

        public static int StartForwardStreamToRooms(IntPtr room, ForwardStreamConfiguration configuration)
        {
            return CXX_VideoSDKStartForwardStreamToRooms( room,  configuration);
        }
        public static int StopForwardStreamToRooms(IntPtr room)
        {
            return CXX_VideoSDKStopForwardStreamToRooms( room);
        }
        public static int UpdateForwardStreamToRooms(IntPtr room, ForwardStreamConfiguration configuration)
        {
            return CXX_VideoSDKUpdateForwardStreamToRooms( room,  configuration);
        }
        public static int PauseForwardStreamToAllRooms(IntPtr room)
        {
            return CXX_VideoSDKPauseForwardStreamToAllRooms( room);
        }
        public static int ResumeForwardStreamToAllRooms(IntPtr room)
        {
            return CXX_VideoSDKResumeForwardStreamToAllRooms( room);
        }
        public static int SetMultiDeviceAVSync(IntPtr room, string audio_user_id)
        {
            return CXX_VideoSDKSetMultiDeviceAVSync( room,  audio_user_id);
        }


        [DllImport(libname, EntryPoint = "VideoSDKDestroyRTCRoom")]
        private static extern void CXX_VideoSDKDestroy(IntPtr room);

        [DllImport(libname, EntryPoint = "VideoSDKJoinRoom")]
        private static extern int CXX_VideoSDKJoinRoom(IntPtr room, string token, UserInfo info, MultiRoomConfigStruct roomConfig);

        [DllImport(libname, EntryPoint = "VideoSDKLeaveRoom")]
        private static extern void CXX_VideoSDKLeaveRoom(IntPtr room);

        [DllImport(libname, EntryPoint = "VideoSDKUpdateToken")]
        private static extern void CXX_VideoSDKUpdateToken(IntPtr room, string token);

        [DllImport(libname, EntryPoint = "VideoSDKSetUserVisibility")]
        private static extern void CXX_VideoSDKSetUserVisibility(IntPtr room, bool enable);

        [DllImport(libname, EntryPoint = "VideoSDKSetRtcRoomEventHandler")]
        private static extern void CXX_VideoSDKSetRTCRoomEventHandler(IntPtr room, RTCVideoSDKRoomCallback callback);

        [DllImport(libname, EntryPoint = "VideoSDKPublishStream")]
        private static extern void CXX_VideoSDKPublishStream(IntPtr room, MediaStreamType type);

        [DllImport(libname, EntryPoint = "VideoSDKUnpublishStream")]
        private static extern void CXX_VideoSDKUnpublishStream(IntPtr room, MediaStreamType type);

        [DllImport(libname, EntryPoint = "VideoSDKSubscribeStream")]
        private static extern void CXX_VideoSDKSubscribeStream(IntPtr room, string userID, MediaStreamType type);

        [DllImport(libname, EntryPoint = "VideoSDKUnsubscribeStream")]
        private static extern void CXX_VideoSDKUnsubscribeStream(IntPtr room, string userID, MediaStreamType type);

        [DllImport(libname, EntryPoint = "VideoSDKPauseAllSubscribedStream")]
        private static extern void CXX_VideoSDKPauseAllSubscribedStream(IntPtr room, PauseResumeControlMediaType type);

        [DllImport(libname, EntryPoint = "VideoSDKResumeAllSubscribedStream")]
        private static extern void CXX_VideoSDKResumeAllSubscribedStream(IntPtr room, PauseResumeControlMediaType type);

        [DllImport(libname, EntryPoint = "VideoSDKSubscribeAllStreams")]
        private static extern int CXX_VideoSDKSubscribeAllStreams(IntPtr room, MediaStreamType type);

        [DllImport(libname, EntryPoint = "VideoSDKUnsubscribeAllStreams")]
        private static extern int CXX_VideoSDKUnsubscribeAllStreams(IntPtr room, MediaStreamType type);

        //  [DllImport(libname, EntryPoint="VideoSDKMuteRemoteAudio")]
        //  private static extern void CXX_VideoSDKMuteRemoteAudio(IntPtr room, string userID, bool muted);

        //  [DllImport(libname, EntryPoint="VideoSDKMuteRemoteVideo")]
        //  private static extern void CXX_VideoSDKMuteRemoteVideo(IntPtr room, string userID, bool muted);

        [DllImport(libname, EntryPoint = "VideoSDKSubscribeScreen")]
        private static extern void CXX_VideoSDKSubscribeScreen(IntPtr room, string userID, MediaStreamType type);

        [DllImport(libname, EntryPoint = "VideoSDKUnsubscribeScreen")]
        private static extern void CXX_VideoSDKUnsubscribeScreen(IntPtr room, string userID, MediaStreamType type);

        [DllImport(libname, EntryPoint = "VideoSDKPublishScreen")]
        private static extern void CXX_VideoSDKPublishScreen(IntPtr room, MediaStreamType type);

        [DllImport(libname, EntryPoint = "VideoSDKUnpublishScreen")]
        private static extern void CXX_VideoSDKUnpublishScreen(IntPtr room, MediaStreamType type);

        [DllImport(libname, EntryPoint = "VideoSDKGetRangeAudio")]
        private static extern IntPtr CXX_VideoSDKGetRangeAudio(IntPtr room);

        [DllImport(libname, EntryPoint = "VideoSDKGetSpatialAudio")]
        private static extern IntPtr CXX_VideoSDKGetSpatialAudio(IntPtr room);

        [DllImport(libname, EntryPoint = "VideoSDKSendUserMessage")]
        private static extern long CXX_VideoSDKSendUserMessage(IntPtr room, string userID, string message);

        [DllImport(libname, EntryPoint = "VideoSDKSendRoomMessage")]
        private static extern long CXX_VideoSDKSendRoomMessage(IntPtr room, string message);

        [DllImport(libname, EntryPoint = "VideoSDKStartForwardStreamToRooms")]
        private static extern int CXX_VideoSDKStartForwardStreamToRooms(IntPtr room, ForwardStreamConfiguration configuration);

        [DllImport(libname, EntryPoint = "VideoSDKStopForwardStreamToRooms")]
        private static extern int CXX_VideoSDKStopForwardStreamToRooms(IntPtr room);

        [DllImport(libname, EntryPoint = "VideoSDKUpdateForwardStreamToRooms")]
        private static extern int CXX_VideoSDKUpdateForwardStreamToRooms(IntPtr room, ForwardStreamConfiguration configuration);

        [DllImport(libname, EntryPoint = "VideoSDKPauseForwardStreamToAllRooms")]
        private static extern int CXX_VideoSDKPauseForwardStreamToAllRooms(IntPtr room);

        [DllImport(libname, EntryPoint = "VideoSDKResumeForwardStreamToAllRooms")]
        private static extern int CXX_VideoSDKResumeForwardStreamToAllRooms(IntPtr room);

        [DllImport(libname, EntryPoint = "VideoSDKSetMultiDeviceAVSync")]
        private static extern int CXX_VideoSDKSetMultiDeviceAVSync(IntPtr room, string audio_user_id);
    }
}