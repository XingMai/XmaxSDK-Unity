using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;

namespace bytertc
{
    public class SpatialAudio : ISpatialAudio
    {
        #region Variables
        private IntPtr _spatialAudio = default(IntPtr);
        private string _roomID;
        #endregion

        public SpatialAudio(IntPtr spatialAudio, string roomID) {
            _spatialAudio = spatialAudio;
            _roomID = roomID;
			EnableSpatialAudio(false);
		}

        public void EnableSpatialAudio(bool enable) {
            string logInfo = string.Format("enable:{0}", enable);
            ByteRTCLog.ReportApiCall("EnableSpatialAudio", logInfo);
            if (_spatialAudio == default(IntPtr))
            {
                return;
            }
            RTCPositionAudioCXXBridge.EnableSpatialAudio(_spatialAudio, enable);
        }
        Position ChangeCoordinateAxis(Position position)
        {
            Position rtcPosition = new Position();
            rtcPosition.x = position.z;
            rtcPosition.y = position.x;
            rtcPosition.z = position.y;
            return rtcPosition;
        }

        Orientation ChangeOrientation(Orientation position)
        {
            Orientation rtcOrientation = new Orientation();
            rtcOrientation.x = position.z;
            rtcOrientation.y = position.x;
            rtcOrientation.z = position.y;
            return rtcOrientation;
        }

        public int UpdatePosition(Position pos) {
            string logInfo = string.Format("pos.x:{0}, pos.y:{1}, pos.z:{2}", pos.x, pos.y, pos.z);
            ByteRTCLog.ReportApiCall("UpdatePosition", logInfo);
            if (_spatialAudio == default(IntPtr))
            {
                return -1;
            }
            return RTCPositionAudioCXXBridge.SAUpdatePosition(_spatialAudio, pos);
        }

        public int UpdateSelfOrientation(HumanOrientation orientation) {
            string logInfo = string.Format(" forward: ({0},{1},{2}), right: ({3},{4},{5}), up: ({6},{7},{8})", 
                orientation.forward.x, orientation.forward.y, orientation.forward.z, orientation.right.x,
                orientation.right.y, orientation.right.z, orientation.up.x, orientation.up.y, orientation.up.z);
            ByteRTCLog.ReportApiCall("UpdateSelfOrientation", logInfo);
            if (_spatialAudio == default(IntPtr))
            {
                return -1;
            }
            return RTCPositionAudioCXXBridge.UpdateSelfOrientation(_spatialAudio, orientation);
        }
        public void DisableRemoteOrientation()
        {
            ByteRTCLog.ReportApiCall("DisableRemoteOrientation", "");
            if (_spatialAudio == default(IntPtr))
            {
                return;
            }
            RTCPositionAudioCXXBridge.DisableRemoteOrientation(_spatialAudio);
        }


        public void Release() {
            _spatialAudio = default(IntPtr);
        }
    }
}