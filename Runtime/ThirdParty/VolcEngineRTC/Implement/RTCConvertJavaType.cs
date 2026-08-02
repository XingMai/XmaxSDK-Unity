using System;
using System.Runtime.InteropServices;

#if UNITY_ANDROID
using UnityEngine;
#if UNITY_2018_3_OR_NEWER
using UnityEngine.Android;
#endif
#endif

#if UNITY_ANDROID
namespace bytertc
{
    public class RTCConvertJavaType{
        
        public static AndroidJavaObject ConvertVideoPixelFormat(VideoPixelFormat vpf)
        {
                AndroidJavaClass pixelFormatTypeClass = new AndroidJavaClass("com.ss.bytertc.engine.data.VideoPixelFormat");
                AndroidJavaObject pixelFormatObj = pixelFormatTypeClass.GetStatic<AndroidJavaObject>("kVideoPixelFormatUnknown");
                switch (vpf)
                {
                    case VideoPixelFormat.kVideoPixelFormatI420:
                    {
                        pixelFormatObj = pixelFormatTypeClass.GetStatic<AndroidJavaObject>("kVideoPixelFormatI420");
                        break;
                    }
                    case VideoPixelFormat.kVideoPixelFormatNV12:
                    {
                        pixelFormatObj = pixelFormatTypeClass.GetStatic<AndroidJavaObject>("kVideoPixelFormatNV12");
                        break;
                    }
                    case VideoPixelFormat.kVideoPixelFormatNV21:
                    {
                        pixelFormatObj = pixelFormatTypeClass.GetStatic<AndroidJavaObject>("kVideoPixelFormatNV21");
                        break;
                    }
                    case VideoPixelFormat.kVideoPixelFormatRGB24:
                    {
                        pixelFormatObj = pixelFormatTypeClass.GetStatic<AndroidJavaObject>("kVideoPixelFormatRGB24");
                        break;
                    }
                    case VideoPixelFormat.kVideoPixelFormatRGBA:
                    {
                        pixelFormatObj = pixelFormatTypeClass.GetStatic<AndroidJavaObject>("kVideoPixelFormatRGBA");
                        break;
                    }
                    case VideoPixelFormat.kVideoPixelFormatARGB:
                    {
                        pixelFormatObj = pixelFormatTypeClass.GetStatic<AndroidJavaObject>("kVideoPixelFormatARGB");
                        break;
                    }
                    case VideoPixelFormat.kVideoPixelFormatBGRA:
                    {
                        pixelFormatObj = pixelFormatTypeClass.GetStatic<AndroidJavaObject>("kVideoPixelFormatBGRA");
                        break;
                    }
                    case VideoPixelFormat.kVideoPixelFormatTexture2D:
                    {
                        pixelFormatObj = pixelFormatTypeClass.GetStatic<AndroidJavaObject>("kVideoPixelFormatTexture2D");
                        break;
                    }
                    case VideoPixelFormat.kVideoPixelFormatTextureOES:
                    {
                        pixelFormatObj = pixelFormatTypeClass.GetStatic<AndroidJavaObject>("kVideoPixelFormatTextureOES");
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
                return pixelFormatObj;
        }

        public static AndroidJavaObject ConvertVideoFrameType(VideoFrameType vft)
        {
                AndroidJavaClass videoFrameTypeClass = new AndroidJavaClass("ccom.ss.bytertc.engine.data.VideoFrameType");
                AndroidJavaObject videoFrameObj = videoFrameTypeClass.GetStatic<AndroidJavaObject>("kVideoFrameTypeRawMemory");
                switch (vft)
                {
                    case VideoFrameType.kVideoFrameTypeGLTexture:
                    {
                        videoFrameObj = videoFrameTypeClass.GetStatic<AndroidJavaObject>("kVideoFrameTypeGLTexture");
                        break;
                    }
                    default:
                    {
                            break;
                    }
                }
                return videoFrameObj;
        }

        public static AndroidJavaObject ConvertColorSpace(ColorSpace cs)
        {
                AndroidJavaClass colorSpaceTypeClass = new AndroidJavaClass("com.ss.bytertc.engine.data.ColorSpace");
                AndroidJavaObject colorSpaceObj = colorSpaceTypeClass.GetStatic<AndroidJavaObject>("kColorSpaceUnknown");
                switch (cs)
                {
                    case ColorSpace.kColorSpaceYCbCrBT601LimitedRange:
                    {
                        colorSpaceObj = colorSpaceTypeClass.GetStatic<AndroidJavaObject>("kColorSpaceYCbCrBT601LimitedRange");
                        break;
                    }
                    case ColorSpace.kColorSpaceYCbCrBT601FullRange:
                    {
                        colorSpaceObj = colorSpaceTypeClass.GetStatic<AndroidJavaObject>("kColorSpaceYCbCrBT601FullRange");
                        break;
                    }
                    case ColorSpace.kColorSpaceYCbCrBT709LimitedRange:
                    {
                        colorSpaceObj = colorSpaceTypeClass.GetStatic<AndroidJavaObject>("kColorSpaceYCbCrBT709LimitedRange");
                        break;
                    }
                     case ColorSpace.kColorSpaceYCbCrBT709FullRange:
                    {
                        colorSpaceObj = colorSpaceTypeClass.GetStatic<AndroidJavaObject>("kColorSpaceYCbCrBT709FullRange");
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
                return colorSpaceObj;
        }

        public static AndroidJavaObject ConvertVideoRotation(VideoRotation vr)
        {
                AndroidJavaClass videoRotationTypeClass = new AndroidJavaClass("com.ss.bytertc.engine.data.VideoRotation");
                AndroidJavaObject videoRotationObj = videoRotationTypeClass.GetStatic<AndroidJavaObject>("VIDEO_ROTATION_0");
                switch (vr)
                {
                    case VideoRotation.kVideoRotation90:
                    {
                        videoRotationObj = videoRotationTypeClass.GetStatic<AndroidJavaObject>("VIDEO_ROTATION_90");
                        break;
                    }
                    case VideoRotation.kVideoRotation180:
                    {
                        videoRotationObj = videoRotationTypeClass.GetStatic<AndroidJavaObject>("VIDEO_ROTATION_180");
                        break;
                    }
                    case VideoRotation.kVideoRotation270:
                    {
                        videoRotationObj = videoRotationTypeClass.GetStatic<AndroidJavaObject>("VIDEO_ROTATION_270");
                        break;
                    }
                    default:
                    {
                            break;
                    }
                }
                return videoRotationObj;
        }
    }  
}
#endif