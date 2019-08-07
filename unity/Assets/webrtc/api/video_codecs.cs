using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    public readonly struct VideoDecoderFactory
    {
#pragma warning disable 0649
        internal readonly IntPtr Ptr;
#pragma warning restore 0649

        public bool IsDefault => Ptr == IntPtr.Zero;

        [DllImport("webrtc_c", EntryPoint = "webrtcCreateBuiltinVideoDecoderFactory")]
        public static extern VideoDecoderFactory CreateBuiltin();
    }

    public readonly struct VideoEncoderFactory
    {
#pragma warning disable 0649
        internal readonly IntPtr Ptr;
#pragma warning restore 0649

        public bool IsDefault => Ptr == IntPtr.Zero;

        [DllImport("webrtc_c", EntryPoint = "webrtcCreateBuiltinVideoEncoderFactory")]
        public static extern VideoEncoderFactory CreateBuiltin();
    }
}
