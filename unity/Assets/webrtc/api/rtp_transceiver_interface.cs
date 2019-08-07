using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    public sealed class RtpTransceiverInterface : IDisposable
    {
        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcRtpTransceiverInterfaceReceiver(
            IntPtr ptr
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcRtpTransceiverInterfaceRelease(
            IntPtr ptr
        );

        internal IntPtr Ptr;

        internal RtpTransceiverInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~RtpTransceiverInterface()
        {
            webrtcRtpTransceiverInterfaceRelease(Ptr);
        }

        public RtpReceiverInterface Receiver()
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            return new RtpReceiverInterface(
                webrtcRtpTransceiverInterfaceReceiver(Ptr)
            );
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcRtpTransceiverInterfaceRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }
}
