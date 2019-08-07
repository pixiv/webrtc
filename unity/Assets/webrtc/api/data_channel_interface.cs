using System;

namespace Webrtc
{
    public sealed class DataChannelInterface : IDisposable
    {
        [System.Runtime.InteropServices.DllImport("webrtc_c")]
        private static extern void webrtcDataChannelInterfaceRelease(
            IntPtr ptr
        );

        internal IntPtr Ptr;

        internal DataChannelInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~DataChannelInterface()
        {
            webrtcDataChannelInterfaceRelease(Ptr);
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcDataChannelInterfaceRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }
}
