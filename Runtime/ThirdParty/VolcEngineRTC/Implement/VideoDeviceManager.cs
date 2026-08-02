using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

namespace bytertc
{
    public class VideoDeviceManager : IVideoDeviceManager
    {
        #region Events


        #endregion


        #region Variables


        #endregion

        private IntPtr _rtcEngine = default(IntPtr);

        public void InitVideoDeviceManager(IntPtr engine)
        {
            _rtcEngine = engine;
        }

        public IDeviceCollection EnumerateVideoCaptureDevices()
        {
            ByteRTCLog.ReportApiCall("EnumerateAudioPlaybackDevices", "");
            DeviceCollection deviceCollection = new DeviceCollection();
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("AudioSDKEngine is null");
                return null;
            }
            IntPtr collection = RTCVideoEngineCXXBridge.VideoDevice_EnumerateVideoCaptureDevices(_rtcEngine);
            deviceCollection.InitDeviceCollection(collection);
            return deviceCollection;
        }


        public int SetVideoCaptureDevice(string deviceID)
        {
            string logInfo = string.Format("deviceID: {0}", deviceID);
            ByteRTCLog.ReportApiCall("SetAudioCaptureDevice", logInfo);
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("AudioSDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.VideoDevice_SetVideoCaptureDevice(_rtcEngine, deviceID);
        }

        public int GetVideoCaptureDevice(ref string deviceID)
        {
            ByteRTCLog.ReportApiCall("GetVideoCaptureDevice", "");
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("AudioSDKEngine is null");
                return -1;
            }
			byte[] curDeviceID = new byte[512];
            int ret = RTCVideoEngineCXXBridge.VideoDevice_GetVideoCaptureDevice(_rtcEngine, ref curDeviceID[0]);
			deviceID = global::System.Text.Encoding.UTF8.GetString(curDeviceID, 0, curDeviceID.Length);
			return ret;
        }
    }
}