using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    public sealed class I420Buffer : I420BufferInterface
    {
        internal I420Buffer(IntPtr ptr) : base(ptr)
        {
        }

        [DllImport("webrtc_c")]
        internal static extern IntPtr webrtcI420BufferToWebrtcVideoFrameBuffer(
            IntPtr ptr
        );

        [DllImport("webrtc_c")]
        internal static extern IntPtr webrtcVideoFrameBufferToI420Buffer(
            IntPtr ptr
        );

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcCreateI420Buffer(
            int width,
            int height,
            int strideY,
            int strideU,
            int strideV
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcI420BufferScaleFrom(
            IntPtr buffer,
            IntPtr src
        );

        public static I420Buffer Create(
            int width,
            int height,
            int strideY,
            int strideU,
            int strideV)
        {
            return new I420Buffer(webrtcI420BufferToWebrtcVideoFrameBuffer(
                webrtcCreateI420Buffer(
                    width,
                    height,
                    strideY,
                    strideU,
                    strideV
                )
            ));
        }

        public static I420Buffer Create(int width, int height)
        {
            return Create(
                width,
                height,
                width,
                (width + 1) / 2,
                (width + 1) / 2
            );
        }

        public void ScaleFrom(I420BufferInterface src)
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            webrtcI420BufferScaleFrom(
                webrtcVideoFrameBufferToI420Buffer(Ptr),
                webrtcVideoFrameBufferToWebrtcI420BufferInterface(src.Ptr)
            );
        }
    }
}
