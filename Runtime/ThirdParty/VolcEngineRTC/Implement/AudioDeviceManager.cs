using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

namespace bytertc
{
    public class AudioDeviceManager : IAudioDeviceManager
    {
        private IntPtr _rtcEngine = default(IntPtr);

        public void InitAudioDeviceManager(IntPtr engine)
        {
            _rtcEngine = engine;
        }

        public IDeviceCollection EnumerateAudioPlaybackDevices()
        {
            ByteRTCLog.ReportApiCall("EnumerateAudioPlaybackDevices", "");
            DeviceCollection deviceCollection = new DeviceCollection();
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return null;
            }
            IntPtr collection = RTCVideoEngineCXXBridge.AudioDevice_EnumerateAudioPlaybackDevices(_rtcEngine);
            deviceCollection.InitDeviceCollection(collection);
            return deviceCollection;
        }

        public IDeviceCollection EnumerateAudioCaptureDevices()
        {
            ByteRTCLog.ReportApiCall("EnumerateAudioCaptureDevices", "");
            DeviceCollection deviceCollection = new DeviceCollection();
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return null;
            }
            IntPtr collection = RTCVideoEngineCXXBridge.AudioDevice_EnumerateAudioCaptureDevices(_rtcEngine);
            deviceCollection.InitDeviceCollection(collection);
            return deviceCollection;
        }

        public void FollowSystemPlaybackDevice(Boolean followed)
        {
            string logInfo = string.Format("followed: {0}", followed);
            ByteRTCLog.ReportApiCall("FollowSystemPlaybackDevice", logInfo);
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.AudioDevice_FollowSystemPlaybackDevice(_rtcEngine, followed);
        }

        public void FollowSystemCaptureDevice(Boolean followed)
        {
            string logInfo = string.Format("followed: {0}", followed);
            ByteRTCLog.ReportApiCall("FollowSystemCaptureDevice", logInfo);
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return;
            }
            RTCVideoEngineCXXBridge.AudioDevice_FollowSystemCaptureDevice(_rtcEngine, followed);
        }

        public int SetAudioPlaybackDevice(string deviceID)
        {
            string logInfo = string.Format("deviceID: {0}", deviceID);
            ByteRTCLog.ReportApiCall("SetAudioPlaybackDevice", logInfo);
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.AudioDevice_SetAudioPlaybackDevice(_rtcEngine, deviceID);
        }

        public int SetAudioCaptureDevice(string deviceID)
        {
            string logInfo = string.Format("deviceID: {0}", deviceID);
            ByteRTCLog.ReportApiCall("SetAudioCaptureDevice", logInfo);
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.AudioDevice_SetAudioCaptureDevice(_rtcEngine, deviceID);
        }

        public int SetAudioPlaybackDeviceVolume(uint volume)
        {
            string logInfo = string.Format("volume: {0}", volume);
            ByteRTCLog.ReportApiCall("SetAudioPlaybackDeviceVolume", logInfo);
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.AudioDevice_SetAudioPlaybackDeviceVolume(_rtcEngine, volume);
        }

        public int GetAudioPlaybackDeviceVolume(ref uint volume)
        {
            ByteRTCLog.ReportApiCall("GetAudioPlaybackDeviceVolume", "");
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.AudioDevice_GetAudioPlaybackDeviceVolume(_rtcEngine,ref volume);
        }

        public int SetAudioCaptureDeviceVolume(uint volume)
        {
            string logInfo = string.Format("volume: {0}", volume);
            ByteRTCLog.ReportApiCall("SetAudioCaptureDeviceVolume", logInfo);
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.AudioDevice_SetAudioCaptureDeviceVolume(_rtcEngine, volume);
        }

        public int GetAudioCaptureDeviceVolume(ref uint volume)
        {
            ByteRTCLog.ReportApiCall("GetAudioCaptureDeviceVolume", "");
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.AudioDevice_GetAudioCaptureDeviceVolume(_rtcEngine, ref volume);
        }

        public int SetAudioPlaybackDeviceMute(bool mute)
        {
            string logInfo = string.Format("volume: {0}", mute);
            ByteRTCLog.ReportApiCall("SetAudioPlaybackDeviceMute", logInfo);
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.AudioDevice_SetAudioPlaybackDeviceMute(_rtcEngine, mute);
        }

        public int GetAudioPlaybackDeviceMute(ref bool mute)
        {
            ByteRTCLog.ReportApiCall("GetAudioPlaybackDeviceMute", "");
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.AudioDevice_GetAudioPlaybackDeviceMute(_rtcEngine, ref mute);
        }

        public int SetAudioCaptureDeviceMute(bool mute)
        {
            string logInfo = string.Format("mute: {0}", mute);
            ByteRTCLog.ReportApiCall("SetAudioCaptureDeviceMute", logInfo);
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.AudioDevice_SetAudioCaptureDeviceMute(_rtcEngine, mute);
        }

        public int GetAudioCaptureDeviceMute(ref bool mute)
        {
            ByteRTCLog.ReportApiCall("GetAudioCaptureDeviceMute", "");
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.AudioDevice_GetAudioCaptureDeviceMute(_rtcEngine, ref mute);
        }

        public int GetAudioPlaybackDevice(ref string deviceID)
        {
            ByteRTCLog.ReportApiCall("GetAudioPlaybackDevice", "");
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
			byte[] deviceIDStr = new byte[512];
			int ret = RTCVideoEngineCXXBridge.AudioDevice_GetAudioPlaybackDevice(_rtcEngine, ref deviceIDStr[0]);
			deviceID = global::System.Text.Encoding.UTF8.GetString(deviceIDStr, 0, deviceIDStr.Length);
			return ret;
		}

        public int GetAudioCaptureDevice(ref string deviceID)
        {
            ByteRTCLog.ReportApiCall("GetAudioCaptureDevice", "");
            if (_rtcEngine == default(IntPtr))
            {
                ByteRTCLog.LogWarning("SDKEngine is null");
                return -1;
            }
			byte[] deviceIDStr = new byte[512];
			int ret = RTCVideoEngineCXXBridge.AudioDevice_GetAudioCaptureDevice(_rtcEngine,ref deviceIDStr[0]);
			deviceID = global::System.Text.Encoding.UTF8.GetString(deviceIDStr, 0, deviceIDStr.Length);
			return ret;
		}

    }
}