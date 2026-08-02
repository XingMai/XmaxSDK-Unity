using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

namespace bytertc
{
    public class RangeAudio : IRangeAudio
    {
      //  #region Events
      //  public event OnRangeAudioInfoEventHandler OnRangeAudioInfoEvent;
      //  #endregion


        #region Variables
        private static Dictionary<string, RangeAudio> _instances = new Dictionary<string, RangeAudio>();
        private IntPtr _rangeAudio = default(IntPtr);
        private string _roomID;
        private RangeAudioCallback _callback;
        #endregion

        public RangeAudio(IntPtr rangeAudio, string roomID) {
            _rangeAudio = rangeAudio;
            _roomID = roomID;
            _instances.Add(_roomID, this);
         //   EnableRangeAudio(false);
        }

        public void EnableRangeAudio(bool enable) {
            string logInfo = string.Format("enable:{0}", enable);
            ByteRTCLog.ReportApiCall("EnableRangeAudio", logInfo);
            if (_rangeAudio == default(IntPtr))
            {
                return;
            }
            RTCPositionAudioCXXBridge.EnableRangeAudio(_rangeAudio, enable);
        }

        public int UpdateReceiveRange(ReceiveRange range) {
            string logInfo = string.Format("range.min:{0}, range.max:{1}", range.min, range.max);
            ByteRTCLog.ReportApiCall("UpdateReceiveRange", logInfo);
            if (_rangeAudio == default(IntPtr))
            {
                return -1;
            }
            return RTCPositionAudioCXXBridge.UpdateReceiveRange(_rangeAudio, range);
        }

        Position ChangeCoordinateAxis(Position position)
        {
            Position rtcPosition = new Position();
            rtcPosition.x = position.z;
            rtcPosition.y = position.x;
            rtcPosition.z = position.y;
            return rtcPosition;
        }

        public int UpdatePosition(Position pos) {
            string logInfo = string.Format("pos.x:{0}, pos.y:{1}, pos.z:{2}", pos.x, pos.y, pos.z);
            ByteRTCLog.ReportApiCall("UpdatePosition", logInfo);
            if (_rangeAudio == default(IntPtr))
            {
                return -1;
            }
            return RTCPositionAudioCXXBridge.RAUpdatePosition(_rangeAudio, pos);
        }
        public int SetAttenuationModel(AttenuationType type, float coefficient)
        {
            string logInfo = string.Format("AttenuationType:{0}, coefficient:{1}", type, coefficient);
            ByteRTCLog.ReportApiCall("SetAttenuationModel", logInfo);
            if (_rangeAudio == default(IntPtr))
            {
                return -1;
            }
            return RTCPositionAudioCXXBridge.SetAttenuationModel(_rangeAudio, type, coefficient);
        }

        public void SetNoAttenuationFlags(string[] flags, int len)
        {
            string logInfo = string.Format("flags:{0}, len:{1}", flags[0], len);
            ByteRTCLog.ReportApiCall("SetAttenuationModel", logInfo);
            if (_rangeAudio == default(IntPtr))
            {
                return;
            }
            RTCPositionAudioCXXBridge.SetNoAttenuationFlags(_rangeAudio, flags, len);
        }
        //public void RegisterRangeAudioObserver(OnRangeAudioInfoEventHandler handler) {
        //    if (_rangeAudio == default(IntPtr))
        //    {
        //        return;
        //    }
        //    OnRangeAudioInfoEvent = handler;
        //    _callback = new RangeAudioCallback();
        //    _callback.OnRangeAudioInfo = OnRangeAudioInfo;
        //    RTCPositionAudioCXXBridge.RegisterRangeAudioObserver(_rangeAudio, _callback);
        //}

        public void Release() {
            _rangeAudio = default(IntPtr);
            _instances.Remove(_roomID);
        }

//        [MonoPInvokeCallback(typeof(OnRangeAudioInfoCallback))]
//        public static void OnRangeAudioInfo(string roomID, IntPtr infos, uint len)
//        {
//            //string logInfo = string.Format("roomID:{0}, len:{1}", roomID, len);
//            //ByteRTCLog.ReportCallback("OnRangeAudioInfo", logInfo);

//            int size = Marshal.SizeOf(typeof(RangeAudioInfo));
//            List<RangeAudioInfo> rangeInfoList = new List<RangeAudioInfo>();
//            for (int i = 0; i < len; ++i)
//            {
//                RangeAudioInfo rangeInfo = new RangeAudioInfo();
//                IntPtr ptr = (IntPtr)((long)infos + i * size);
//#if UNITY_2019_3_OR_NEWER
//                rangeInfo = Marshal.PtrToStructure<RangeAudioInfo>(ptr);
//#else
//                rangeInfo = (RangeAudioInfo)Marshal.PtrToStructure(ptr, typeof(RangeAudioInfo));
//#endif
//                rangeInfoList.Add(rangeInfo);
//            }
//            try
//            {
//                //string message = "work thread OnRangeAudioInfo start\n";
//                string message = "work thread";
//                for (int i = 0; i < rangeInfoList.Count; i++)
//                {
//                    message += string.Format(" index:{0} user_id:{1} factor {2}", i, rangeInfoList[i].user_id, rangeInfoList[i].factor);
//                }
//                Loom.QueueOnMainThread(() =>
//                {
//                    Debug.Log(message);
//                });

//                foreach (KeyValuePair<string, RangeAudio> kvp in _instances)
//                {
//                    if (kvp.Key == roomID && kvp.Value.OnRangeAudioInfoEvent != null)
//                    {
//                        var copyAudioInfoList = rangeInfoList;

//                        Loom.QueueOnMainThread(() =>
//                        {
//                            try
//                            {
//                                // Debug.Log("Unity thread OnRangeAudioInfoEvent start");

//                                string logInfo = "unity thread";
//                                for (int i = 0; i < rangeInfoList.Count; i++)
//                                {
//                                    logInfo += string.Format(" index:{0} user_id:{1} factor {2}", i, copyAudioInfoList[i].user_id, copyAudioInfoList[i].factor);
//                                }
//                                Debug.Log(logInfo);
//                                kvp.Value.OnRangeAudioInfoEvent(roomID, copyAudioInfoList);
//                            }
//                            catch (Exception ex)
//                            {
//                                Debug.LogWarning(string.Format("Unity thread exception occur!!!!! ex = {0}",ex));
//                            }
//                            finally
//                            {
//                                //Debug.Log("Unity thread OnRangeAudioInfoEvent end");
//                            }

//                        });
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.LogWarning(string.Format("work thread OnRangeAudioInfo exception {0}", ex));     
//            }
//            finally
//            {
//                //Loom.QueueOnMainThread(() =>
//                //{
//                //    Debug.Log("work thread OnRangeAudioInfo end");
//                //});
//            }

//        }

    }

    public delegate void OnRangeAudioInfoCallback(string roomID, IntPtr infos, uint len);

    public struct RangeAudioCallback
    {
        public OnRangeAudioInfoCallback OnRangeAudioInfo;
    }
}