using System;
using System.Runtime.InteropServices;

namespace bytertc
{
    public class RTCPositionAudioCXXBridge
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

        public static void EnableRangeAudio(IntPtr rangeAudio, bool enable)
        {
            CXX_RangeAudioEnableRangeAudio(rangeAudio, enable);
        }

        public static int UpdateReceiveRange(IntPtr rangeAudio, ReceiveRange range)
        {
            return CXX_RangeAudioUpdateReceiveRange(rangeAudio, range);
        }

        public static int RAUpdatePosition(IntPtr rangeAudio, Position pos)
        {
            return CXX_RangeAudioUpdatePosition(rangeAudio, pos);
        }
        public static int SetAttenuationModel(IntPtr rangeAudio, AttenuationType type, float coefficient)
        {
            return CXX_RangeAudioSetAttenuationModel(rangeAudio, type, coefficient);
        }

        public static void SetNoAttenuationFlags(IntPtr rangeAudio, string[] flags, int len)
        {
            CXX_RangeAudioSetNoAttenuationFlags(rangeAudio, flags, len);
        }

        public static void DisableRemoteOrientation(IntPtr spatialAudio)
        {
            CXX_SpatialAudioDisableRemoteOrientation(spatialAudio);
        }

        public static void EnableSpatialAudio(IntPtr spatialAudio, bool enable)
        {
            CXX_SpatialAudioEnableSpatialAudio(spatialAudio, enable);
        }

        public static int SAUpdatePosition(IntPtr spatialAudio, Position pos)
        {
            return CXX_SpatialAudioUpdatePosition(spatialAudio, pos);
        }

        public static int UpdateSelfOrientation(IntPtr spatialAudio, HumanOrientation orientation)
        {
            return CXX_SpatialAudioUpdateSelfOrientation(spatialAudio, orientation);
        }

        [DllImport(libname, EntryPoint="VideoSDKRangeAudioEnableRangeAudio")]
        private static extern void CXX_RangeAudioEnableRangeAudio(IntPtr rangeAudio, bool enable);

        [DllImport(libname, EntryPoint= "VideoSDKRangeAudioUpdateReceiveRange")]
        private static extern int CXX_RangeAudioUpdateReceiveRange(IntPtr rangeAudio, ReceiveRange range);

        [DllImport(libname, EntryPoint= "VideoSDKRangeAudioUpdatePosition")]
        private static extern int CXX_RangeAudioUpdatePosition(IntPtr rangeAudio, Position pos);

        [DllImport(libname, EntryPoint = "VideoSDKSpatialAudioDisableRemoteOrientation")]
        private static extern void CXX_SpatialAudioDisableRemoteOrientation(IntPtr spatialAudio);

        [DllImport(libname, EntryPoint = "VideoSDKRangeAudioSetAttenuationModel")]
        private static extern int CXX_RangeAudioSetAttenuationModel(IntPtr rangeAudio, AttenuationType type, float coefficient);

        [DllImport(libname, EntryPoint = "VideoSDKRangeAudioSetNoAttenuationFlags")]
        private static extern void CXX_RangeAudioSetNoAttenuationFlags(IntPtr rangeAudio, string[] flags, int len);

        [DllImport(libname, EntryPoint= "VideoSDKSpatialAudioEnableSpatialAudio")]
        private static extern void CXX_SpatialAudioEnableSpatialAudio(IntPtr spatialAudio, bool enable);

        [DllImport(libname, EntryPoint= "VideoSDKSpatialAudioUpdatePosition")]
        private static extern int CXX_SpatialAudioUpdatePosition(IntPtr spatialAudio, Position pos);

        [DllImport(libname, EntryPoint= "VideoSDKSpatialAudioUpdateSelfOrientation")]
        private static extern int CXX_SpatialAudioUpdateSelfOrientation(IntPtr spatialAudio, HumanOrientation orientation);
    }
}
