using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    public sealed class AudioDecoderFactory : IDisposable
    {
        internal IntPtr Ptr;

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcCreateBuiltinAudioDecoderFactory();

        [DllImport("webrtc_c")]
        private static extern void webrtcAudioDecoderFactoryRelease(IntPtr ptr);

        private AudioDecoderFactory(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~AudioDecoderFactory()
        {
            webrtcAudioDecoderFactoryRelease(Ptr);
        }

        public static AudioDecoderFactory CreateBuiltin()
        {
            return new AudioDecoderFactory(
                webrtcCreateBuiltinAudioDecoderFactory()
            );
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcAudioDecoderFactoryRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }

    public sealed class AudioEncoderFactory : IDisposable
    {
        internal IntPtr Ptr;

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcCreateBuiltinAudioEncoderFactory();

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcAudioEncoderFactoryRelease(
            IntPtr ptr
        );

        private AudioEncoderFactory(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~AudioEncoderFactory()
        {
            webrtcAudioEncoderFactoryRelease(Ptr);
        }

        public static AudioEncoderFactory CreateBuiltin()
        {
            return new AudioEncoderFactory(
                webrtcCreateBuiltinAudioEncoderFactory()
            );
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcAudioEncoderFactoryRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }
}
