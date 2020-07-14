using Pixiv.Rtc;
using System;
using System.Runtime.InteropServices;

namespace Pixiv.Webrtc
{
    public static class TurnServer
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webrtcCreateTURNServer(
            IntPtr thread,
            string local_addr,
            string ip_addr,
            int min_port,
            int max_port);
    }
}
