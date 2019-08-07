using System;

namespace Webrtc
{
    public sealed class RtpSenderInterface : IDisposable
    {
        [System.Runtime.InteropServices.DllImport("webrtc_c")]
        private static extern void webrtcRtpSenderInterfaceRelease(IntPtr ptr);

        internal IntPtr Ptr;

        internal RtpSenderInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~RtpSenderInterface()
        {
            webrtcRtpSenderInterfaceRelease(Ptr);
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcRtpSenderInterfaceRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }
}
