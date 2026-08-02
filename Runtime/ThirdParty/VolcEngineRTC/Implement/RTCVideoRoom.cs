using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;
using UNBridgeLib.LitJson;

namespace bytertc
{
    public class RTCVideoRoom : IRTCVideoRoom
    {
        #region Events
        public event OnRoomStateChangedEventHandler OnRoomStateChangedEvent;
        public event OnStreamStateChangedEventHandler OnStreamStateChangedEvent;
        // public event OnJoinRoomResultEventHandler OnJoinRoomResultEvent;
        public event OnLeaveRoomEventHandler OnLeaveRoomEvent;
        public event OnRoomWarningEventHandler OnRoomWarningEvent;
        public event OnRoomErrorEventHandler OnRoomErrorEvent;

        public event OnUserJoinedEventHandler OnUserJoinedEvent;
        public event OnUserLeaveEventHandler OnUserLeaveEvent;

        public event OnRoomMessageReceivedEventHandler OnRoomMessageReceivedEvent;
        public event OnUserMessageReceivedEventHandler OnUserMessageReceivedEvent;
        public event OnUserMessageSendResultEventHandler OnUserMessageSendResultEvent;
        public event OnRoomMessageSendResultEventHandler OnRoomMessageSendResultEvent;

        public event OnUserPublishStreamEventHandler OnUserPublishStreamEvent;
        public event OnUserUnPublishStreamEventHandler OnUserUnPublishStreamEvent;
        public event OnUserPublishScreenEventHandler OnUserPublishScreenEvent;
        public event OnUserUnPublishScreenEventHandler OnUserUnPublishScreenEvent;
        public event OnStreamSubscribedEventHandler OnStreamSubscribedEvent;

        public event OnTokenWillExpireEventHandler OnTokenWillExpireEvent;
        public event OnAudioStreamBannedEventHandler OnAudioStreamBannedEvent;
        public event OnVideoStreamBannedEventHandler OnVideoStreamBannedEvent;
        public event OnNetworkQualityEventHandler OnNetworkQualityEvent;
        public event OnLocalStreamStatsEventHandler OnLocalStreamStatsEvent;
        public event OnRemoteStreamStatsEventHandler OnRemoteStreamStatsEvent;

        public event OnForwardStreamStateChangedEventHandler OnForwardStreamStateChangedEvent;
        public event OnForwardStreamEventEventHandler OnForwardStreamEventEvent;
        public event OnAVSyncStateChangeCallback OnAVSyncStateChangeEvent;
        #endregion


        #region Variables


        #endregion

        private static Dictionary<string, RTCVideoRoom> _instances = new Dictionary<string, RTCVideoRoom>();
        private IntPtr _rtcRoom = default(IntPtr);
        private string _roomID;
        private RTCVideoSDKRoomCallback _callback;

		private IRangeAudio _rangeAudio = null;
		private ISpatialAudio _spatialAudio = null;

		public static bool RoomIsExist(string roomID) {
            if (_instances == null) {
                return false;
            }
            if (_instances.ContainsKey(roomID)){
                return true;
            }
            return false;
        }

        public static void Release()
        {
            if (_instances == null)
            {
                return ;
            }
            List<IRTCVideoRoom> rooms = new List<IRTCVideoRoom>();

            var keys = _instances.Keys;
            foreach(var key in keys){
                IRTCVideoRoom room= _instances[key];
                rooms.Add(room);
            }

            foreach(var room in rooms) {
                room.Destroy();
            }

            rooms.Clear();
            _instances.Clear();
        }
        public void Initialize(IntPtr room, string roomID)
        {
            _rtcRoom = room;
            _roomID = roomID;
            _instances.Add(_roomID, this);
            _callback.OnRoomStateChanged = OnRoomStateChanged;
            _callback.OnStreamStateChanged = OnStreamStateChanged;
            _callback.OnLeaveRoomEvent = OnLeaveRoom;
            _callback.OnRoomWarningEvent = OnRoomWarning;
            _callback.OnRoomErrorEvent = OnRoomError;
            _callback.OnUserJoinedEvent = OnUserJoined;
            _callback.OnUserLeaveEvent = OnUserLeave;

            _callback.OnRoomMessageReceived = OnRoomMessageReceived;
            _callback.OnUserMessageReceived = OnUserMessageReceived;
            _callback.OnRoomMessageSendResult = OnRoomMessageSendResult;
            _callback.OnUserMessageSendResult = OnUserMessageSendResult;

            _callback.OnUserPublishStreamEvent = OnUserPublishStream;
            _callback.OnUserUnPublishStreamEvent = OnUserUnPublishStream;
            _callback.OnUserPublishScreenEvent = OnUserPublishScreen;
            _callback.OnUserUnPublishScreenEvent = OnUserUnPublishScreen;
            _callback.OnStreamSubscribedEvent = OnStreamSubscribed;

            _callback.OnTokenWillExpireEvent = OnTokenWillExpire;
            _callback.OnAudioStreamBannedEvent = OnAudioStreamBanned;
            _callback.OnVideoStreamBannedEvent = OnVideoStreamBanned;
            _callback.OnNetworkQualityEvent = OnNetworkQuality;
            _callback.OnLocalStreamStatsEvent = OnLocalStreamStats;
            _callback.OnRemoteStreamStatsEvent = OnRemoteStreamStats;

            _callback.OnForwardStreamStateChangedEvent = OnForwardStreamStateChanged;
            _callback.OnForwardStreamEventEvent = OnForwardStreamEvent;
            _callback.OnAVSyncStateChangeEvent = OnAVSyncStateChange;

            RTCVideoRoomCXXBridge.SetRTCRoomEventHandler(_rtcRoom, _callback);
        }

        void IRTCVideoRoom.Destroy()
        {
            ByteRTCLog.ReportApiCall("Destroy", "");
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.Destroy(_rtcRoom);
            _rtcRoom = default(IntPtr);
            _instances.Remove(_roomID);
            if (_rangeAudio != null) {
                _rangeAudio.Release();
                _rangeAudio = null;
            }
            if (_spatialAudio != null) {
                _spatialAudio.Release();
                _spatialAudio = null;
            }
        }

        int IRTCVideoRoom.JoinRoom(string token, UserInfo info, MultiRoomConfig roomConfig)
        {
            string logInfo = string.Format("token:{0}, UserID:{1}, ProfileType:{2}",
                    token, info.UserID, roomConfig);
            ByteRTCLog.ReportApiCall("JoinRoom", logInfo);
            MultiRoomConfigStruct roomConfigStruct = new MultiRoomConfigStruct();
            roomConfigStruct.roomProfileType = roomConfig.roomProfileType;
            roomConfigStruct.is_auto_publish = roomConfig.is_auto_publish;
            roomConfigStruct.isAutoSubscribeAudio = roomConfig.isAutoSubscribeAudio;
            roomConfigStruct.isAutoSubscribeVideo = roomConfig.isAutoSubscribeVideo;
            roomConfigStruct.remoteVideoConfig = roomConfig.remoteVideoConfig;
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.JoinRoom(_rtcRoom, token, info, roomConfigStruct);
        }

        void IRTCVideoRoom.LeaveRoom()
        {
            ByteRTCLog.ReportApiCall("LeaveRoom", "");
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.LeaveRoom(_rtcRoom);
        }

        void IRTCVideoRoom.UpdateToken(string token)
        {
            string logInfo = string.Format("token:{0}", token);
            ByteRTCLog.ReportApiCall("UpdateToken", logInfo);
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.UpdateToken(_rtcRoom, token);
        }

        void IRTCVideoRoom.SetUserVisibility(bool enable)
        {
            string logInfo = string.Format("enable:{0}", enable);
            ByteRTCLog.ReportApiCall("SetUserVisibility", logInfo);
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.SetUserVisibility(_rtcRoom, enable);
        }

        void IRTCVideoRoom.PublishStream(MediaStreamType type)
        {
            ByteRTCLog.ReportApiCall("PublishStream", type.ToString());
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.PublishStream(_rtcRoom, type);
        }

        void IRTCVideoRoom.UnpublishStream(MediaStreamType type)
        {
            ByteRTCLog.ReportApiCall("UnpublishStream", type.ToString());
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.UnpublishStream(_rtcRoom, type);
        }

        void IRTCVideoRoom.PublishScreen(MediaStreamType type)
        {
            ByteRTCLog.ReportApiCall("PublishScreen", type.ToString());
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.PublishScreen(_rtcRoom, type);
        }

        void IRTCVideoRoom.UnpublishScreen(MediaStreamType type)
        {
            ByteRTCLog.ReportApiCall("UnpublishScreen", type.ToString());
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.UnpublishScreen(_rtcRoom, type);
        }

        void IRTCVideoRoom.SubscribeStream(string userID, MediaStreamType type)
        {
            string logInfo = string.Format("userID:{0}, type:{1}", userID, type);
            ByteRTCLog.ReportApiCall("SubscribeStream", logInfo);
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.SubscribeStream(_rtcRoom, userID, type);
        }

        void IRTCVideoRoom.UnsubscribeStream(string userID, MediaStreamType type)
        {
            string logInfo = string.Format("userID:{0}, type:{1}", userID, type);
            ByteRTCLog.ReportApiCall("UnsubscribeStream", logInfo);
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.UnsubscribeStream(_rtcRoom, userID, type);
        }

        void IRTCVideoRoom.SubscribeScreen(string userID, MediaStreamType type)
        {
            string logInfo = string.Format("userID:{0}, type:{1}", userID, type);
            ByteRTCLog.ReportApiCall("SubscribeScreen", logInfo);
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.SubscribeScreen(_rtcRoom, userID, type);
        }

        void IRTCVideoRoom.UnsubscribeScreen(string userID, MediaStreamType type)
        {
            string logInfo = string.Format("userID:{0}, type:{1}", userID, type);
            ByteRTCLog.ReportApiCall("UnsubscribeScreen", logInfo);
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.UnsubscribeScreen(_rtcRoom, userID, type);
        }

        void IRTCVideoRoom.PauseAllSubscribedStream(PauseResumeControlMediaType mediaType)
        {
            ByteRTCLog.ReportApiCall("PauseAllSubscribedStream", "");
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.PauseAllSubscribedStream(_rtcRoom, mediaType);
        }

        void IRTCVideoRoom.ResumeAllSubscribedStream(PauseResumeControlMediaType mediaType)
        {
            ByteRTCLog.ReportApiCall("ResumeAllSubscribedStream", "");
            if (_rtcRoom == default(IntPtr))
            {
                return;
            }
            RTCVideoRoomCXXBridge.ResumeAllSubscribedStream(_rtcRoom, mediaType);
        }
        int IRTCVideoRoom.UnsubscribeAllStreams(MediaStreamType type)
        {
            ByteRTCLog.ReportApiCall("ResumeAllSubscribedStream", "");
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.UnsubscribeAllStreams(_rtcRoom, type);
        }
        int IRTCVideoRoom.SubscribeAllStreams(MediaStreamType type)
        {
            ByteRTCLog.ReportApiCall("ResumeAllSubscribedStream", "");
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.SubscribeAllStreams(_rtcRoom, type);
        }
        int IRTCVideoRoom.StartForwardStreamToRooms(ForwardStreamConfiguration configuration)
        {
            ByteRTCLog.ReportApiCall("StartForwardStreamToRooms", "");
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.StartForwardStreamToRooms(_rtcRoom, configuration);
        }
        int IRTCVideoRoom.StopForwardStreamToRooms()
        {
            ByteRTCLog.ReportApiCall("StopForwardStreamToRooms", "");
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.StopForwardStreamToRooms(_rtcRoom);
        }
        int IRTCVideoRoom.UpdateForwardStreamToRooms(ForwardStreamConfiguration configuration)
        {
            ByteRTCLog.ReportApiCall("UpdateForwardStreamToRooms", "");
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.UpdateForwardStreamToRooms(_rtcRoom, configuration);
        }
        int IRTCVideoRoom.PauseForwardStreamToAllRooms()
        {
            ByteRTCLog.ReportApiCall("PauseForwardStreamToAllRooms", "");
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.PauseForwardStreamToAllRooms(_rtcRoom);
        }
        int IRTCVideoRoom.ResumeForwardStreamToAllRooms()
        {
            ByteRTCLog.ReportApiCall("ResumeForwardStreamToAllRooms", "");
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.ResumeForwardStreamToAllRooms(_rtcRoom);
        }
        int IRTCVideoRoom.SetMultiDeviceAVSync(string audio_user_id)
        {
            string logInfo = string.Format("audio_user_id:{0}", audio_user_id);
            ByteRTCLog.ReportApiCall("SetMultiDeviceAVSync", logInfo);
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.SetMultiDeviceAVSync(_rtcRoom, audio_user_id);
        }

        public IRangeAudio GetRangeAudio()
        {
            if (_rangeAudio == null)
            {
                _rangeAudio = new RangeAudio(RTCVideoRoomCXXBridge.GetRangeAudio(_rtcRoom), _roomID);
            }
            return _rangeAudio;
        }

        public ISpatialAudio GetSpatialAudio()
        {
            if (_spatialAudio == null)
            {
                _spatialAudio = new SpatialAudio(RTCVideoRoomCXXBridge.GetSpatialAudio(_rtcRoom), _roomID);
            }
            return _spatialAudio;
        }

        public long SendUserMessage(string userID, string message)
        {
            string logInfo = string.Format("userID:{0}, message:{1}", userID, message);
            ByteRTCLog.ReportApiCall("SendUserMessage", "");
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.SendUserMessage(_rtcRoom, userID, message);
        }

        public long SendRoomMessage(string message)
        {
            string logInfo = string.Format("message:{0}", message);
            ByteRTCLog.ReportApiCall("SendRoomMessage", "");
            if (_rtcRoom == default(IntPtr))
            {
                return -1;
            }
            return RTCVideoRoomCXXBridge.SendRoomMessage(_rtcRoom, message);
        }

        [MonoPInvokeCallback(typeof(OnRoomStateChangedEventHandler))]
        public static void OnRoomStateChanged(string roomID, string userID, int state, string ExtraInfo)
        {
            string logInfo = string.Format("roomID:{0}, userID:{1}, type:{2}, errorCode:{3}",
                    roomID, userID, state, ExtraInfo);
            ByteRTCLog.ReportCallback("OnRoomStateChanged", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnRoomStateChangedEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnRoomStateChangedEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnRoomStateChangedEvent(roomID, userID, state, ExtraInfo);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnStreamStateChangedEventHandler))]
        public static void OnStreamStateChanged(string roomID, string userID, int state, string ExtraInfo)
        {
            string logInfo = string.Format("roomID:{0}, userID:{1}, type:{2}, errorCode:{3}",
                    roomID, userID, state, ExtraInfo);
            ByteRTCLog.ReportCallback("OnStreamStateChanged", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnStreamStateChangedEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnStreamStateChangedEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnStreamStateChangedEvent(roomID, userID, state, ExtraInfo);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnLeaveRoomEventHandler))]
        public static void OnLeaveRoom(string roomID, RtcRoomStats stats)
        {
            string logInfo = string.Format("roomID:{0}, rtt:{1}", roomID, stats.rtt);
            ByteRTCLog.ReportCallback("OnLeaveRoom", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnLeaveRoomEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnLeaveRoomEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnLeaveRoomEvent(roomID, stats);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnRoomWarningEventHandler))]
        public static void OnRoomWarning(string roomID, int warn)
        {
            string logInfo = string.Format("roomID:{0}, warn:{1}", roomID, warn);
            ByteRTCLog.ReportCallback("OnRoomWarning", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnRoomWarningEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnRoomWarningEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnRoomWarningEvent(roomID, warn);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnRoomErrorEventHandler))]
        public static void OnRoomError(string roomID, int err)
        {
            string logInfo = string.Format("roomID:{0}, err:{1}", roomID, err);
            ByteRTCLog.ReportCallback("OnRoomError", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnRoomErrorEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnRoomErrorEvent == null) {
                            return;
                        }
                        kvp.Value.OnRoomErrorEvent(roomID, err);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnUserJoinedEventHandler))]
        public static void OnUserJoined(string roomID, UserInfo userInfo, int elapsed)
        {
            string logInfo = string.Format("roomID:{0}, UserID:{1}, elapsed:{2}", roomID, userInfo.UserID, elapsed);
            ByteRTCLog.ReportCallback("OnUserJoined", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnUserJoinedEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnUserJoinedEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnUserJoinedEvent(roomID, userInfo, elapsed);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnUserLeaveEventHandler))]
        public static void OnUserLeave(string roomID, string userID, UserOfflineReason reason)
        {
           
            string logInfo = string.Format("roomID:{0}, UserID:{1}, reason:{2}", roomID, userID, reason);
            Debug.Log("logInfo="+ logInfo);
            ByteRTCLog.ReportCallback("OnUserLeave", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                Debug.Log("logInfo1=" + logInfo);
                if (kvp.Key == roomID && kvp.Value.OnUserLeaveEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnUserLeaveEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnUserLeaveEvent(roomID, userID, reason);
                    });
                }
            }
            Debug.Log("logInfo2=" + logInfo);
        }

        [MonoPInvokeCallback(typeof(OnUserPublishStreamEventHandler))]
        public static void OnUserPublishStream(string roomID, string userID, MediaStreamType type)
        {
            string logInfo = string.Format("roomID:{0}, UserID:{1}", roomID, userID);
            ByteRTCLog.ReportCallback("OnUserPublishStream", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnUserPublishStreamEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnUserPublishStreamEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnUserPublishStreamEvent(roomID, userID, type);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnUserUnPublishStreamEventHandler))]
        public static void OnUserUnPublishStream(string roomID, string userID, MediaStreamType type, StreamRemoveReason reason)
        {
            string logInfo = string.Format("roomID:{0}, UserID:{1}, reason:{2}", roomID, userID, reason);
            ByteRTCLog.ReportCallback("OnUserUnPublishStream", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnUserUnPublishStreamEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnUserUnPublishStreamEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnUserUnPublishStreamEvent(roomID, userID, type, reason);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnUserPublishScreenEventHandler))]
        public static void OnUserPublishScreen(string roomID, string userID, MediaStreamType type)
        {
            string logInfo = string.Format("roomID:{0}, UserID:{1}", roomID, userID);
            ByteRTCLog.ReportCallback("OnUserPublishScreen", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnUserPublishScreenEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnUserPublishScreenEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnUserPublishScreenEvent(roomID, userID, type);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnUserUnPublishScreenEventHandler))]
        public static void OnUserUnPublishScreen(string roomID, string userID, MediaStreamType type, StreamRemoveReason reason)
        {
            string logInfo = string.Format("roomID:{0}, UserID:{1}, reason:{2}", roomID, userID, reason);
            ByteRTCLog.ReportCallback("OnUserUnPublishScreen", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnUserUnPublishScreenEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnUserUnPublishScreenEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnUserUnPublishScreenEvent(roomID, userID, type, reason);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnStreamSubscribedEventHandler))]
        public static void OnStreamSubscribed(string roomID, SubscribeState stateCode, string userID, SubscribeConfig info)
        {
            string logInfo = string.Format("roomID:{0}, UserID:{1}, config:{2}", roomID, userID, info);
            ByteRTCLog.ReportCallback("OnStreamSubscribed", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnStreamSubscribedEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnStreamSubscribedEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnStreamSubscribedEvent(roomID, stateCode, userID, info);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnTokenWillExpireEventHandler))]
        public static void OnTokenWillExpire(string roomID)
        {
            string logInfo = string.Format("roomID:{0}", roomID);
            ByteRTCLog.ReportCallback("OnTokenWillExpire", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnTokenWillExpireEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnTokenWillExpireEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnTokenWillExpireEvent(roomID);
                    });
                }
            }
        }
        [MonoPInvokeCallback(typeof(OnForwardStreamStateChangedCallback))]
        public static void OnForwardStreamStateChanged(IntPtr infos, int info_count)
        {
            int size = Marshal.SizeOf(typeof(ForwardStreamStateInfo));
            List<ForwardStreamStateInfo> streamStateChangedLists = new List<ForwardStreamStateInfo>();
            
            for (int i = 0; i < info_count; i++)
            {
                ForwardStreamStateInfo streamStateChanged = new ForwardStreamStateInfo();
                IntPtr ptr = (IntPtr)((long)infos + i * size);

                streamStateChanged = Marshal.PtrToStructure<ForwardStreamStateInfo>(ptr);
                streamStateChangedLists.Add(streamStateChanged);

            }
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Value.OnForwardStreamStateChangedEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnForwardStreamStateChangedEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnForwardStreamStateChangedEvent(streamStateChangedLists, info_count);
                    });
                }
            }
        }
        [MonoPInvokeCallback(typeof(OnForwardStreamEventCallback))]
        public static void OnForwardStreamEvent(IntPtr infos, int info_count)
        {
            int size = Marshal.SizeOf(typeof(ForwardStreamEventInfo));
            List<ForwardStreamEventInfo> streamStateChangedLists = new List<ForwardStreamEventInfo>();
            for (int i = 0; i < info_count; i++)
            {
                ForwardStreamEventInfo streamStateChanged = new ForwardStreamEventInfo();
                IntPtr ptr = (IntPtr)((long)infos + i * size);
                streamStateChanged = Marshal.PtrToStructure<ForwardStreamEventInfo>(ptr);
                streamStateChangedLists.Add(streamStateChanged);
            }

            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Value.OnForwardStreamEventEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnForwardStreamEventEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnForwardStreamEventEvent(streamStateChangedLists, info_count);
                    });
                }
            }
        }
        [MonoPInvokeCallback(typeof(OnAVSyncStateChangeCallback))]
        public static void OnAVSyncStateChange(AVSyncState state)
        {
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if ( kvp.Value.OnAVSyncStateChangeEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnAVSyncStateChangeEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnAVSyncStateChangeEvent(state);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnAudioStreamBannedEventHandler))]
        public static void OnAudioStreamBanned(string roomID, string userID, bool banned)
        {
            string logInfo = string.Format("roomID:{0} userID:{1} banned:{2}", roomID,userID,banned);
            ByteRTCLog.ReportCallback("OnAudioStreamBanned", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnAudioStreamBannedEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnAudioStreamBannedEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnAudioStreamBannedEvent(roomID,userID,banned);
                    });
                }
            }
        }


        [MonoPInvokeCallback(typeof(OnRoomMessageReceivedEventHandler))]
        public static void OnRoomMessageReceived(string roomID, string userID, string message)
        {
            string logInfo = string.Format("roomID:{0} userID:{1} message:{2}", roomID, userID, message);
            ByteRTCLog.ReportCallback("OnRoomMessageReceived", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnRoomMessageReceivedEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnRoomMessageReceivedEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnRoomMessageReceivedEvent(roomID, userID, message);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnUserMessageReceivedEventHandler))]
        public static void OnUserMessageReceived(string roomID, string userID, string message)
        {
            string logInfo = string.Format("roomID:{0} userID:{1} message:{2}", roomID, userID, message);
            ByteRTCLog.ReportCallback("OnUserMessageReceived", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnUserMessageReceivedEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnUserMessageReceivedEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnUserMessageReceivedEvent(roomID, userID, message);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnUserMessageSendResultEventHandler))]
        public static void OnUserMessageSendResult(string roomID, long msgID, int error)
        {
            string logInfo = string.Format("roomID:{0} msgID:{1} error:{2}", roomID, msgID, error);
            ByteRTCLog.ReportCallback("OnUserMessageSendResult", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnUserMessageSendResultEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnUserMessageSendResultEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnUserMessageSendResultEvent(roomID, msgID, error);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnRoomMessageSendResultEventHandler))]
        public static void OnRoomMessageSendResult(string roomID, long msgID, int error)
        {
            string logInfo = string.Format("roomID:{0} msgID:{1} error:{2}", roomID, msgID, error);
            ByteRTCLog.ReportCallback("OnRoomMessageSendResult", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnRoomMessageSendResultEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnRoomMessageSendResultEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnRoomMessageSendResultEvent(roomID, msgID, error);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnVideoStreamBannedEventHandler))]
        public static void OnVideoStreamBanned(string roomID, string userID, bool banned)
        {
            string logInfo = string.Format("roomID:{0} userID:{1} banned:{2}", roomID, userID, banned);
            ByteRTCLog.ReportCallback("OnVideoStreamBanned", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnVideoStreamBannedEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnVideoStreamBannedEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnVideoStreamBannedEvent(roomID, userID, banned);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnNetworkQualityCallback))]
        public static void OnNetworkQuality(string roomID, NetworkQualityStats localQuality, IntPtr remoteQualities, int remoteQualityNum)
        {
            //string logInfo = string.Format("OnNetworkQuality: {0}", remoteQualityNum);
            //ByteRTCLog.ReportCallback("OnNetworkQuality", logInfo);

            int size = Marshal.SizeOf(typeof(NetworkQualityStats));
            List<NetworkQualityStats> remoteQualityLists = new List<NetworkQualityStats>();
            for (int i = 0; i < remoteQualityNum; ++i)
            {
                NetworkQualityStats remoteQuality = new NetworkQualityStats();
                IntPtr ptr = (IntPtr)((long)remoteQualities + i * size);
#if UNITY_2019_3_OR_NEWER
                remoteQuality = Marshal.PtrToStructure<NetworkQualityStats>(ptr);
#else
                        remoteQuality = (NetworkQualityStats)Marshal.PtrToStructure(ptr, typeof(NetworkQualityStats));
#endif
                remoteQualityLists.Add(remoteQuality);
            }

            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Key == roomID && kvp.Value.OnNetworkQualityEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnNetworkQualityEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnNetworkQualityEvent(roomID, localQuality, remoteQualityLists, remoteQualityNum);
                    });
                }
            }
        }

        [MonoPInvokeCallback(typeof(OnLocalStreamStatsCallback))]
        public static void OnLocalStreamStats(LocalStreamStats stats)
        {
            string logInfo = string.Format("is_screen:{0} ", stats.is_screen);
      //      ByteRTCLog.ReportCallback("OnLocalStreamStats", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Value.OnLocalStreamStatsEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnLocalStreamStatsEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnLocalStreamStatsEvent(stats);
                    });
                }
            }
        }
        [MonoPInvokeCallback(typeof(OnRemoteStreamStatsCallback))]
        public static void OnRemoteStreamStats(RemoteStreamStats stats)
        {
            string logInfo = string.Format("is_screen:{0}  user_id:{1}", stats.is_screen,stats.uid);
     //       ByteRTCLog.ReportCallback("OnRemoteStreamStats", logInfo);
            foreach (KeyValuePair<string, RTCVideoRoom> kvp in _instances)
            {
                if (kvp.Value.OnRemoteStreamStatsEvent != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        if (kvp.Value == null || kvp.Value.OnRemoteStreamStatsEvent == null)
                        {
                            return;
                        }
                        kvp.Value.OnRemoteStreamStatsEvent(stats);
                    });
                }
            }
        }
    }

    #region CallbackType


    #endregion

    public struct MultiRoomConfigStruct {
        public RoomProfileType roomProfileType;
        public bool is_auto_publish;
        public bool isAutoSubscribeAudio;
        public bool isAutoSubscribeVideo;
        public RemoteVideoConfig remoteVideoConfig;
    }

    public struct RTCVideoSDKRoomCallback
    {
        public OnRoomStateChangedEventHandler OnRoomStateChanged;
        public OnStreamStateChangedEventHandler OnStreamStateChanged;
        public OnLeaveRoomEventHandler OnLeaveRoomEvent;
        public OnRoomWarningEventHandler OnRoomWarningEvent;
        public OnRoomErrorEventHandler OnRoomErrorEvent;
        public OnUserJoinedEventHandler OnUserJoinedEvent;
        public OnUserLeaveEventHandler OnUserLeaveEvent;

        public OnRoomMessageReceivedEventHandler OnRoomMessageReceived;
        public OnUserMessageReceivedEventHandler OnUserMessageReceived;
        public OnUserMessageSendResultEventHandler OnUserMessageSendResult;
        public OnRoomMessageSendResultEventHandler OnRoomMessageSendResult;

        public OnUserPublishStreamEventHandler OnUserPublishStreamEvent;
        public OnUserUnPublishStreamEventHandler OnUserUnPublishStreamEvent;
        public OnUserPublishScreenEventHandler OnUserPublishScreenEvent;
        public OnUserUnPublishStreamEventHandler OnUserUnPublishScreenEvent;
        public OnStreamSubscribedEventHandler OnStreamSubscribedEvent;

        public OnTokenWillExpireEventHandler OnTokenWillExpireEvent;
        public OnAudioStreamBannedEventHandler OnAudioStreamBannedEvent;
        public OnVideoStreamBannedEventHandler OnVideoStreamBannedEvent;
        public OnNetworkQualityCallback OnNetworkQualityEvent;
        public OnLocalStreamStatsCallback OnLocalStreamStatsEvent;
        public OnRemoteStreamStatsCallback OnRemoteStreamStatsEvent;

        public OnForwardStreamStateChangedCallback OnForwardStreamStateChangedEvent;
        public OnForwardStreamEventCallback OnForwardStreamEventEvent;
        public OnAVSyncStateChangeCallback OnAVSyncStateChangeEvent;
    }
}