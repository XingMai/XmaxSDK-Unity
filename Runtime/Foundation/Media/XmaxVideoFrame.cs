using System;

namespace Xmax.SDK
{
    public enum XmaxVideoPixelFormat
    {
        I420,
        Rgba,
        Bgra
    }

    public enum XmaxVideoRotation
    {
        Rotation0 = 0,
        Rotation90 = 90,
        Rotation180 = 180,
        Rotation270 = 270
    }

    public sealed class XmaxVideoFrame
    {
        public XmaxVideoPixelFormat PixelFormat { get; }
        public int Width { get; }
        public int Height { get; }
        public byte[][] Planes { get; }
        public int[] Strides { get; }
        public long TimestampMicroseconds { get; }
        public XmaxVideoRotation Rotation { get; }

        private XmaxVideoFrame(
            XmaxVideoPixelFormat pixelFormat,
            int width,
            int height,
            byte[][] planes,
            int[] strides,
            long timestampMicroseconds,
            XmaxVideoRotation rotation)
        {
            PixelFormat = pixelFormat;
            Width = width;
            Height = height;
            Planes = planes;
            Strides = strides;
            TimestampMicroseconds = timestampMicroseconds;
            Rotation = rotation;
        }

        public static XmaxVideoFrame CreateRgba(
            byte[] data,
            int width,
            int height,
            int stride = 0,
            long timestampMicroseconds = 0,
            XmaxVideoRotation rotation = XmaxVideoRotation.Rotation0)
        {
            return CreatePacked(
                XmaxVideoPixelFormat.Rgba,
                data,
                width,
                height,
                stride,
                timestampMicroseconds,
                rotation);
        }

        public static XmaxVideoFrame CreateBgra(
            byte[] data,
            int width,
            int height,
            int stride = 0,
            long timestampMicroseconds = 0,
            XmaxVideoRotation rotation = XmaxVideoRotation.Rotation0)
        {
            return CreatePacked(
                XmaxVideoPixelFormat.Bgra,
                data,
                width,
                height,
                stride,
                timestampMicroseconds,
                rotation);
        }

        public static XmaxVideoFrame CreateI420(
            byte[] y,
            byte[] u,
            byte[] v,
            int width,
            int height,
            int strideY = 0,
            int strideU = 0,
            int strideV = 0,
            long timestampMicroseconds = 0,
            XmaxVideoRotation rotation = XmaxVideoRotation.Rotation0)
        {
            ValidateDimensions(width, height);
            strideY = strideY <= 0 ? width : strideY;
            strideU = strideU <= 0 ? width / 2 : strideU;
            strideV = strideV <= 0 ? width / 2 : strideV;
            ValidatePlane(y, strideY, width, height, nameof(y));
            ValidatePlane(u, strideU, width / 2, height / 2, nameof(u));
            ValidatePlane(v, strideV, width / 2, height / 2, nameof(v));
            ValidateRotation(rotation);
            return new XmaxVideoFrame(
                XmaxVideoPixelFormat.I420,
                width,
                height,
                new[] { y, u, v },
                new[] { strideY, strideU, strideV },
                timestampMicroseconds,
                rotation);
        }

        internal void Validate()
        {
            ValidateDimensions(Width, Height);
            ValidateRotation(Rotation);
            if (PixelFormat == XmaxVideoPixelFormat.I420)
            {
                if (Planes == null || Strides == null || Planes.Length < 3 || Strides.Length < 3)
                {
                    throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "An I420 frame requires Y, U and V planes.");
                }
                ValidatePlane(Planes[0], Strides[0], Width, Height, "Y");
                ValidatePlane(Planes[1], Strides[1], Width / 2, Height / 2, "U");
                ValidatePlane(Planes[2], Strides[2], Width / 2, Height / 2, "V");
            }
            else
            {
                if (Planes == null || Strides == null || Planes.Length < 1 || Strides.Length < 1)
                {
                    throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "A packed frame requires one plane.");
                }
                ValidatePlane(Planes[0], Strides[0], (long)Width * 4, Height, "RGBA/BGRA");
            }
        }

        private static XmaxVideoFrame CreatePacked(
            XmaxVideoPixelFormat format,
            byte[] data,
            int width,
            int height,
            int stride,
            long timestampMicroseconds,
            XmaxVideoRotation rotation)
        {
            ValidateDimensions(width, height);
            if ((long)width * 4 > int.MaxValue) throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Video row is too large.");
            stride = stride <= 0 ? width * 4 : stride;
            ValidatePlane(data, stride, (long)width * 4, height, nameof(data));
            ValidateRotation(rotation);
            return new XmaxVideoFrame(
                format,
                width,
                height,
                new[] { data },
                new[] { stride },
                timestampMicroseconds,
                rotation);
        }

        private static void ValidateDimensions(int width, int height)
        {
            if (width <= 0 || height <= 0 || width % 2 != 0 || height % 2 != 0)
            {
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Video frame width and height must be positive even numbers.");
            }
        }

        /// <summary>Copy planes and strides when retaining a frame beyond its callback.</summary>
        public XmaxVideoFrame Clone()
        {
            Validate();
            var planes = new byte[Planes.Length][];
            for (var i = 0; i < planes.Length; i++) planes[i] = (byte[])Planes[i].Clone();
            return new XmaxVideoFrame(PixelFormat, Width, Height, planes, (int[])Strides.Clone(), TimestampMicroseconds, Rotation);
        }

        private static void ValidatePlane(byte[] data, int stride, long rowBytes, int height, string name)
        {
            var minimumLength = (long)stride * height;
            if (stride < rowBytes || minimumLength > int.MaxValue || data == null || data.Length < minimumLength)
            {
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    $"Video plane {name} is missing or smaller than {minimumLength} bytes.");
            }
        }

        private static void ValidateRotation(XmaxVideoRotation rotation)
        {
            if (rotation != XmaxVideoRotation.Rotation0 &&
                rotation != XmaxVideoRotation.Rotation90 &&
                rotation != XmaxVideoRotation.Rotation180 &&
                rotation != XmaxVideoRotation.Rotation270)
            {
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, $"Unsupported video rotation: {rotation}.");
            }
        }
    }
}
