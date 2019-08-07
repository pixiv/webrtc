using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    public abstract class VideoFrameBuffer : IDisposable
    {
        [DllImport("webrtc_c")]
        private static extern void webrtcVideoFrameBufferRelease(IntPtr ptr);

        internal IntPtr Ptr;

        internal VideoFrameBuffer(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~VideoFrameBuffer()
        {
            webrtcVideoFrameBufferRelease(Ptr);
        }

        public void Dispose()
        {
            webrtcVideoFrameBufferRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }

    public abstract class I420BufferInterface : VideoFrameBuffer
    {
        [DllImport("webrtc_c")]
        internal static extern IntPtr webrtcVideoFrameBufferToWebrtcI420BufferInterface(
            IntPtr ptr
        );

        internal I420BufferInterface(IntPtr ptr) : base(ptr)
        {
        }
    }
}
