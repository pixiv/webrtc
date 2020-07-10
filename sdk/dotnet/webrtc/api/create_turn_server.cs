using Pixiv.Rtc;
using System;
using System.Runtime.InteropServices;

namespace Pixiv.Webrtc
{
    public static class TurnServer
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcCreateTURNServer(
            string local_addr,
            string ip_addr);
    }
}
