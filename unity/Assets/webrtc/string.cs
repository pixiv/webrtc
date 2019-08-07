using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    internal readonly struct String : IDisposable
    {
        public readonly IntPtr Ptr;

        [DllImport("webrtc_c")]
        private static extern void webrtcDeleteString(IntPtr s);

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcString_c_str(IntPtr s);

        public override string ToString()
        {
            return Marshal.PtrToStringAnsi(webrtcString_c_str(Ptr));
        }

        public void Dispose()
        {
            webrtcDeleteString(Ptr);
        }
    }
}
