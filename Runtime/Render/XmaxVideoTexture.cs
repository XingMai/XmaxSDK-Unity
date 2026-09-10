using System;
using UnityEngine;

namespace Xmax.SDK
{
    /// <summary>
    /// 在 Unity 主线程将 I420 帧上传至可复用纹理；宿主负责材质生命周期及 UV 旋转。
    /// </summary>
    public sealed class XmaxVideoTexture : IDisposable
    {
        /// <summary>
        /// 亮度纹理，由本对象创建并释放；首次上传前为 null。
        /// </summary>
        public Texture2D Y { get; private set; }

        /// <summary>
        /// U 色度纹理，由本对象创建并释放；首次上传前为 null。
        /// </summary>
        public Texture2D U { get; private set; }

        /// <summary>
        /// V 色度纹理，由本对象创建并释放；首次上传前为 null。
        /// </summary>
        public Texture2D V { get; private set; }

        /// <summary>
        /// 最近上传帧的旋转元数据，宿主据此处理显示方向。
        /// </summary>
        public XmaxVideoRotation Rotation { get; private set; }

        // 可复用的紧凑像素缓存。
        private byte[][] _packed;

        // 宿主轨道订阅和材质引用。
        private RealtimeVideoTrack _track;
        private Material _material;

        // 资源释放状态。
        private bool _disposed;

        /// <summary>
        /// 替换视频轨道订阅，并为可选材质设置 Y、U、V 纹理。
        /// </summary>
        /// <param name="track">需要绑定的视频轨道；null 表示解除轨道输入。</param>
        /// <param name="material">宿主拥有的可选材质，其着色器应提供 _YTex、_UTex、_VTex 属性。</param>
        /// <exception cref="ObjectDisposedException">纹理上传器已经释放。</exception>
        public void Bind(RealtimeVideoTrack track, Material material = null)
        {
            RequireAlive();
            if (_track != null)
                _track.FrameReceived -= Upload;

            _track = track;
            _material = material;
            if (track != null)
                track.FrameReceived += Upload;

            ApplyMaterial();
        }

        /// <summary>
        /// 在 Unity 主线程上传 I420 帧，尺寸变化时重建纹理并按行步长拷贝。
        /// </summary>
        /// <param name="frame">需要处理的视频帧；调用期间不得修改其像素平面和行步长。</param>
        /// <exception cref="ObjectDisposedException">纹理上传器已经释放。</exception>
        /// <exception cref="ArgumentNullException">frame 为 null。</exception>
        /// <exception cref="XmaxException">帧数据无效或像素格式不是 I420。</exception>
        public void Upload(XmaxVideoFrame frame)
        {
            RequireAlive();
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            frame.Validate();
            if (frame.PixelFormat != XmaxVideoPixelFormat.I420)
                throw new XmaxException(XmaxErrorCode.NotSupported, "XmaxVideoTexture accepts I420 remote frames.");

            if (Y == null || Y.width != frame.Width || Y.height != frame.Height)
            {
                ReleaseTextures();
                Y = Create(frame.Width, frame.Height);
                U = Create(frame.Width / 2, frame.Height / 2);
                V = Create(frame.Width / 2, frame.Height / 2);
                _packed = new[]
                {
                    new byte[frame.Width * frame.Height],
                    new byte[frame.Width * frame.Height / 4],
                    new byte[frame.Width * frame.Height / 4]
                };
                ApplyMaterial();
            }

            Rotation = frame.Rotation;
            UploadPlane(Y, frame.Planes[0], frame.Strides[0], _packed[0]);
            UploadPlane(U, frame.Planes[1], frame.Strides[1], _packed[1]);
            UploadPlane(V, frame.Planes[2], frame.Strides[2], _packed[2]);
        }

        /// <summary>
        /// 创建无 mipmap 的线性 R8 纹理，使用边缘钳制和双线性过滤。
        /// </summary>
        /// <param name="width">视频宽度，单位为像素。</param>
        /// <param name="height">视频高度，单位为像素。</param>
        /// <returns>由当前纹理上传器负责释放的纹理。</returns>
        private static Texture2D Create(int width, int height) => new Texture2D(
            width,
            height,
            TextureFormat.R8,
            false,
            true)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        /// <summary>
        /// 逐行去除像素平面的步长填充，再上传并应用纹理数据。
        /// </summary>
        /// <param name="texture">需要上传该像素平面的 R8 纹理。</param>
        /// <param name="data">需要处理的像素平面字节数组。</param>
        /// <param name="stride">源像素平面相邻两行之间的字节数。</param>
        /// <param name="packed">可复用的紧凑行布局缓存，容量须覆盖整张纹理。</param>
        private static void UploadPlane(Texture2D texture, byte[] data, int stride, byte[] packed)
        {
            for (var row = 0; row < texture.height; row++)
                Buffer.BlockCopy(data, row * stride, packed, row * texture.width, texture.width);

            texture.LoadRawTextureData(packed);
            texture.Apply(false, false);
        }

        /// <summary>
        /// 将当前平面纹理写入已绑定材质的 _YTex、_UTex、_VTex 属性。
        /// </summary>
        private void ApplyMaterial()
        {
            if (_material == null)
                return;

            _material.SetTexture("_YTex", Y);
            _material.SetTexture("_UTex", U);
            _material.SetTexture("_VTex", V);
        }

        /// <summary>
        /// 拒绝在对象释放后继续绑定或上传帧。
        /// </summary>
        private void RequireAlive()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(XmaxVideoTexture));
        }

        /// <summary>
        /// 释放全部平面纹理并清除上传缓存。
        /// </summary>
        private void ReleaseTextures()
        {
            Destroy(Y);
            Destroy(U);
            Destroy(V);
            Y = U = V = null;
            _packed = null;
        }

        /// <summary>
        /// 按运行或编辑状态销毁 Unity 对象，忽略空引用。
        /// </summary>
        /// <param name="value">需要销毁的 Unity 对象，允许为 null。</param>
        private static void Destroy(UnityEngine.Object value)
        {
            if (value == null)
                return;

            if (Application.isPlaying)
                UnityEngine.Object.Destroy(value);
            else
                UnityEngine.Object.DestroyImmediate(value);
        }

        /// <summary>
        /// 在 Unity 主线程解除轨道订阅并释放自有纹理，清空材质纹理引用；可重复调用。
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            if (_track != null)
                _track.FrameReceived -= Upload;

            _track = null;
            ReleaseTextures();
            ApplyMaterial();
            _material = null;
            _disposed = true;
        }
    }
}
