using System;
using UnityEngine;

namespace Xmax.SDK
{
    /// <summary>Reusable I420 texture upload for RawImage, world-space quads and XR materials.
    /// Call on the Unity main thread. The caller owns its material and UV rotation.</summary>
    public sealed class XmaxVideoTexture : IDisposable
    {
        public Texture2D Y { get; private set; }
        public Texture2D U { get; private set; }
        public Texture2D V { get; private set; }
        public XmaxVideoRotation Rotation { get; private set; }
        private byte[][] _packed;
        private RealtimeVideoTrack _track;
        private Material _material;
        private bool _disposed;

        /// <summary>Bind a material whose shader samples _YTex, _UTex and _VTex.</summary>
        public void Bind(RealtimeVideoTrack track, Material material = null)
        {
            RequireAlive();
            if (_track != null) _track.FrameReceived -= Upload;
            _track = track;
            _material = material;
            if (track != null) track.FrameReceived += Upload;
            ApplyMaterial();
        }
        public void Upload(XmaxVideoFrame frame)
        {
            RequireAlive();
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            frame.Validate();
            if (frame.PixelFormat != XmaxVideoPixelFormat.I420)
                throw new XmaxException(XmaxErrorCode.NotSupported, "XmaxVideoTexture accepts I420 remote frames.");
            if (Y == null || Y.width != frame.Width || Y.height != frame.Height)
            {
                ReleaseTextures();
                Y = Create(frame.Width, frame.Height);
                U = Create(frame.Width / 2, frame.Height / 2);
                V = Create(frame.Width / 2, frame.Height / 2);
                _packed = new[] { new byte[frame.Width * frame.Height], new byte[frame.Width * frame.Height / 4], new byte[frame.Width * frame.Height / 4] };
                ApplyMaterial();
            }
            Rotation = frame.Rotation;
            UploadPlane(Y, frame.Planes[0], frame.Strides[0], _packed[0]);
            UploadPlane(U, frame.Planes[1], frame.Strides[1], _packed[1]);
            UploadPlane(V, frame.Planes[2], frame.Strides[2], _packed[2]);
        }
        private static Texture2D Create(int width, int height) => new Texture2D(width, height, TextureFormat.R8, false, true)
            { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
        private static void UploadPlane(Texture2D texture, byte[] data, int stride, byte[] packed)
        {
            for (var row = 0; row < texture.height; row++) Buffer.BlockCopy(data, row * stride, packed, row * texture.width, texture.width);
            texture.LoadRawTextureData(packed);
            texture.Apply(false, false);
        }
        private void ApplyMaterial()
        {
            if (_material == null) return;
            _material.SetTexture("_YTex", Y);
            _material.SetTexture("_UTex", U);
            _material.SetTexture("_VTex", V);
        }
        private void RequireAlive() { if (_disposed) throw new ObjectDisposedException(nameof(XmaxVideoTexture)); }
        private void ReleaseTextures()
        {
            Destroy(Y); Destroy(U); Destroy(V);
            Y = U = V = null;
            _packed = null;
        }
        private static void Destroy(UnityEngine.Object value)
        {
            if (value == null) return;
            if (Application.isPlaying) UnityEngine.Object.Destroy(value);
            else UnityEngine.Object.DestroyImmediate(value);
        }
        public void Dispose()
        {
            if (_disposed) return;
            if (_track != null) _track.FrameReceived -= Upload;
            _track = null;
            ReleaseTextures();
            ApplyMaterial();
            _material = null;
            _disposed = true;
        }
    }
}
