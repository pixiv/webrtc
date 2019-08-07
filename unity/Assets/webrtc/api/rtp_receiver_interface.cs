using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    public sealed class RtpReceiverInterface : IDisposable
    {
        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcRtpReceiverInterfaceTrack(
            IntPtr ptr
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcRtpReceiverInterfaceRelease(
            IntPtr ptr
        );

        internal IntPtr Ptr;

        internal RtpReceiverInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~RtpReceiverInterface()
        {
            webrtcRtpReceiverInterfaceRelease(Ptr);
        }

        public MediaStreamTrackInterface Track()
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            return MediaStreamTrackInterface.Wrap(
                webrtcRtpReceiverInterfaceTrack(Ptr)
            );
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcRtpReceiverInterfaceRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }
}
