using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

namespace bytertc
{
    public class ScreenCaptureSourceList : IScreenCaptureSourceList
    {
        #region Variables
        private IntPtr _sourceList = default(IntPtr);

        #endregion

        public void Initialize(IntPtr sourceList)
        {
            _sourceList = sourceList;
        }

        public int GetCount()
        {
            ByteRTCLog.ReportApiCall("ScreenCaptureSourceList_GetCount", "");
            if (_sourceList == default(IntPtr))
            {
                ByteRTCLog.LogWarning("ScreenCaptureSourceList is null");
                return -1;
            }
            return RTCVideoEngineCXXBridge.ScreenCaptureSourceList_GetCount(_sourceList);
        }

        public ScreenCaptureSourceInfo GetSourceInfo(int index)
        {
            string logInfo = string.Format("index: {0}", index);
            ByteRTCLog.ReportApiCall("GetSourceInfo", logInfo);
            if (_sourceList == default(IntPtr))
            {
                ByteRTCLog.LogWarning("ScreenCaptureSourceList is null");
                return new ScreenCaptureSourceInfo();
            }
            return RTCVideoEngineCXXBridge.ScreenCaptureSourceList_GetSourceInfo(_sourceList, index);
        }

        public void Release()
        {
            ByteRTCLog.ReportApiCall("ScreenCaptureSourceList_Release", "");
            if (_sourceList == default(IntPtr))
            {
                ByteRTCLog.LogWarning("ScreenCaptureSourceList is null");
                return;
            }
            RTCVideoEngineCXXBridge.ScreenCaptureSourceList_GetCount(_sourceList);
            _sourceList = default(IntPtr);
            ByteRTCLog.ReportApiCall("-ScreenCaptureSourceList_Release", "");
        }
    }
}