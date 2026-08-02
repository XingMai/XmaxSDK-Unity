using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

namespace bytertc
{
    public class DeviceCollection : IDeviceCollection
    {
        #region Events


        #endregion


        #region Variables


        #endregion

        private IntPtr _deviceCollection = default(IntPtr);

        public void InitDeviceCollection(IntPtr collection)
        {
            _deviceCollection = collection;
        }

        public int GetCount()
        {
            ByteRTCLog.ReportApiCall("DeviceCollection_GetCount", "");
            if (_deviceCollection == default(IntPtr))
            {
                ByteRTCLog.LogWarning("DeviceCollection is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.DeviceCollection_GetCount(_deviceCollection);
        }

        public int GetDevice(int index, ref string deviceName, ref string deviceID)
        {
            string logInfo = string.Format("index: {0}", index);
            ByteRTCLog.ReportApiCall("DeviceCollection_GetDevice", logInfo);
            if (_deviceCollection == default(IntPtr))
            {
                ByteRTCLog.LogWarning("DeviceCollection is null");
                return -1;
            }
			byte[] name = new byte[512];
			byte[] id = new byte[512];
            int ret = RTCVideoEngineCXXBridge.DeviceCollection_GetDevice(_deviceCollection, index, ref name[0],  ref id[0]);
			deviceName = global::System.Text.Encoding.UTF8.GetString(name, 0, name.Length);
			deviceID = global::System.Text.Encoding.UTF8.GetString(id, 0, id.Length);
			return ret;
		}

        public void Release()
        {
            ByteRTCLog.ReportApiCall("DeviceCollection_Release", "");
            if (_deviceCollection == default(IntPtr))
            {
                ByteRTCLog.LogWarning("DeviceCollection is null");
                return;
            }
            RTCVideoEngineCXXBridge.DeviceCollection_Release(_deviceCollection);
            _deviceCollection = default(IntPtr);
            ByteRTCLog.ReportApiCall("-DeviceCollection_Release", "");
        }
    }
}