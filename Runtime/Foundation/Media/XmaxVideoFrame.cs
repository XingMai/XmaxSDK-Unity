using System;

namespace Xmax.SDK
{
    /// <summary>
    /// 外部视频帧支持的内存像素布局。
    /// </summary>
    public enum XmaxVideoPixelFormat
    {
        /// <summary>
        /// 分别存储 Y、U、V 的 4:2:0 平面格式。
        /// </summary>
        I420,

        /// <summary>
        /// 每像素四字节，依次为 R、G、B、A。
        /// </summary>
        Rgba,

        /// <summary>
        /// 每像素四字节，依次为 B、G、R、A。
        /// </summary>
        Bgra
    }

    /// <summary>
    /// 视频帧携带的旋转角度元数据；不会直接旋转像素内存。
    /// </summary>
    public enum XmaxVideoRotation
    {
        /// <summary>
        /// 不旋转。
        /// </summary>
        Rotation0 = 0,

        /// <summary>
        /// 旋转 90 度。
        /// </summary>
        Rotation90 = 90,

        /// <summary>
        /// 旋转 180 度。
        /// </summary>
        Rotation180 = 180,

        /// <summary>
        /// 旋转 270 度。
        /// </summary>
        Rotation270 = 270
    }

    /// <summary>
    /// 引用像素平面及行步长的视频帧；工厂方法不复制输入数组，跨回调保留时使用 Clone。
    /// </summary>
    public sealed class XmaxVideoFrame
    {
        /// <summary>
        /// 视频帧的内存像素布局。
        /// </summary>
        public XmaxVideoPixelFormat PixelFormat { get; }

        /// <summary>
        /// 视频宽度，单位为像素，使用时须为正偶数。
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// 视频高度，单位为像素，使用时须为正偶数。
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// 原始像素平面引用；I420 为 Y、U、V 三个平面，RGBA/BGRA 为单平面。
        /// </summary>
        public byte[][] Planes { get; }

        /// <summary>
        /// 每个像素平面相邻两行起点之间的字节数。
        /// </summary>
        public int[] Strides { get; }

        /// <summary>
        /// 传递给 RTC 的微秒时间戳；默认值为 0。
        /// </summary>
        public long TimestampMicroseconds { get; }

        /// <summary>
        /// 随帧传递的旋转元数据。
        /// </summary>
        public XmaxVideoRotation Rotation { get; }

        /// <summary>
        /// 保存已经校验的像素平面和帧元数据，不复制数组。
        /// </summary>
        /// <param name="pixelFormat">视频内存的像素排列格式。</param>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <param name="planes">按像素格式排列的平面数组，不进行深拷贝。</param>
        /// <param name="strides">各像素平面每行的字节数。</param>
        /// <param name="timestampMicroseconds">传递给 RTC 的微秒时间戳，默认原样传递 0。</param>
        /// <param name="rotation">视频旋转元数据，支持 0、90、180 和 270 度，不改变像素数组。</param>
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

        /// <summary>
        /// 校验并包装 RGBA 数据，保留输入数组引用。
        /// </summary>
        /// <param name="data">需要处理的像素平面字节数组。</param>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <param name="stride">每行字节数；小于等于零时使用宽度乘以 4 的紧凑布局。</param>
        /// <param name="timestampMicroseconds">传递给 RTC 的微秒时间戳，默认原样传递 0。</param>
        /// <param name="rotation">视频旋转元数据，支持 0、90、180 和 270 度，不改变像素数组。</param>
        /// <returns>使用指定数据和元数据的视频帧。</returns>
        /// <exception cref="XmaxException">帧尺寸、旋转、步长或像素平面容量不符合格式要求。</exception>
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

        /// <summary>
        /// 校验并包装 BGRA 数据，保留输入数组引用。
        /// </summary>
        /// <param name="data">需要处理的像素平面字节数组。</param>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <param name="stride">每行字节数；小于等于零时使用宽度乘以 4 的紧凑布局。</param>
        /// <param name="timestampMicroseconds">传递给 RTC 的微秒时间戳，默认原样传递 0。</param>
        /// <param name="rotation">视频旋转元数据，支持 0、90、180 和 270 度，不改变像素数组。</param>
        /// <returns>使用指定数据和元数据的视频帧。</returns>
        /// <exception cref="XmaxException">帧尺寸、旋转、步长或像素平面容量不符合格式要求。</exception>
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

        /// <summary>
        /// 校验并包装 Y、U、V 平面，保留输入数组引用。
        /// </summary>
        /// <param name="y">Y 亮度平面，数组不会被复制。</param>
        /// <param name="u">U 色度平面，宽高为亮度平面的一半，数组不会被复制。</param>
        /// <param name="v">V 色度平面，宽高为亮度平面的一半，数组不会被复制。</param>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <param name="strideY">Y 平面的每行字节数；小于等于零时使用视频宽度。</param>
        /// <param name="strideU">U 平面的每行字节数；小于等于零时使用视频宽度的一半。</param>
        /// <param name="strideV">V 平面的每行字节数；小于等于零时使用视频宽度的一半。</param>
        /// <param name="timestampMicroseconds">传递给 RTC 的微秒时间戳，默认原样传递 0。</param>
        /// <param name="rotation">视频旋转元数据，支持 0、90、180 和 270 度，不改变像素数组。</param>
        /// <returns>使用指定平面和元数据的 I420 视频帧。</returns>
        /// <exception cref="XmaxException">帧尺寸、旋转、步长或像素平面容量不符合格式要求。</exception>
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

        /// <summary>
        /// 重新校验帧尺寸、旋转、平面数量、行步长和缓冲区容量。
        /// </summary>
        internal void Validate()
        {
            ValidateDimensions(Width, Height);
            ValidateRotation(Rotation);
            if (PixelFormat == XmaxVideoPixelFormat.I420)
            {
                if (Planes == null || Strides == null || Planes.Length < 3 || Strides.Length < 3)
                {
                    throw new XmaxException(
                        XmaxErrorCode.InvalidConfiguration,
                        "An I420 frame requires Y, U and V planes.");
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

        /// <summary>
        /// 校验并包装每像素四字节的交错像素数据。
        /// </summary>
        /// <param name="format">视频内存的像素排列格式。</param>
        /// <param name="data">需要处理的像素平面字节数组。</param>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <param name="stride">每行字节数；小于等于零时使用宽度乘以 4 的紧凑布局。</param>
        /// <param name="timestampMicroseconds">传递给 RTC 的微秒时间戳，默认原样传递 0。</param>
        /// <param name="rotation">视频旋转元数据，支持 0、90、180 和 270 度，不改变像素数组。</param>
        /// <returns>引用原始像素数组的 RGBA 或 BGRA 帧。</returns>
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
            if ((long)width * 4 > int.MaxValue)
                throw new XmaxException(XmaxErrorCode.InvalidConfiguration, "Video row is too large.");

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

        /// <summary>
        /// 校验视频宽高为正偶数。
        /// </summary>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        private static void ValidateDimensions(int width, int height)
        {
            if (width <= 0 || height <= 0 || width % 2 != 0 || height % 2 != 0)
            {
                throw new XmaxException(
                    XmaxErrorCode.InvalidConfiguration,
                    "Video frame width and height must be positive even numbers.");
            }
        }

        /// <summary>
        /// 校验后深拷贝全部像素平面和行步长，供回调结束后保留。
        /// </summary>
        /// <returns>不与原帧共享像素和步长数组的副本。</returns>
        /// <exception cref="XmaxException">帧尺寸、旋转、步长或像素平面容量不符合格式要求。</exception>
        public XmaxVideoFrame Clone()
        {
            Validate();
            var planes = new byte[Planes.Length][];

            for (var i = 0; i < planes.Length; i++)
                planes[i] = (byte[])Planes[i].Clone();

            return new XmaxVideoFrame(
                PixelFormat,
                Width,
                Height,
                planes,
                (int[])Strides.Clone(),
                TimestampMicroseconds,
                Rotation);
        }

        /// <summary>
        /// 按行步长和高度校验像素平面容量，并使用长整数防止长度溢出。
        /// </summary>
        /// <param name="data">需要处理的像素平面字节数组。</param>
        /// <param name="stride">源像素平面实际每行字节数，须不小于有效像素字节数。</param>
        /// <param name="rowBytes">每行有效像素所需的最少字节数。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <param name="name">用于错误说明的像素平面名称。</param>
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

        /// <summary>
        /// 校验旋转角度为 0、90、180 或 270 度。
        /// </summary>
        /// <param name="rotation">视频旋转元数据，支持 0、90、180 和 270 度，不改变像素数组。</param>
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
