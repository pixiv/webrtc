using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Pixiv.Webrtc
{
    public readonly struct RTCDataBuffer { 
    
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcRTCDataChannelMessage(IntPtr ptr, out int len);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool webrtcRTCDataChannelMessageIsBinary(IntPtr ptr);

        public bool isBinary { get; }
        public byte[] Data { get; }
        public string Text { get; }

        public RTCDataBuffer(IntPtr ptr)
        {
            int len;
            var messagePtr = webrtcRTCDataChannelMessage(ptr, out len);
            isBinary = webrtcRTCDataChannelMessageIsBinary(ptr);
            Data = new byte[len];
            Marshal.Copy(messagePtr, Data, 0, len);
            Text = "";
            if (!isBinary)
            {
                Text = UTF8Encoding.UTF8.GetString(Data);
            }
        }        
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class RtcDataBuffer
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDeleteRTCDataBuffer")]
        public static extern void Delete(IntPtr ptr);
    }
}
