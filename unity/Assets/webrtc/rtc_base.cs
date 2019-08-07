using System;
using System.Runtime.InteropServices;

namespace Rtc
{
    public sealed class Thread : IDisposable
    {
        internal IntPtr Ptr;

        [DllImport("webrtc_c")]
        private static extern IntPtr rtcCreateThread();

        [DllImport("webrtc_c")]
        private static extern void rtcDeleteThread(IntPtr ptr);

        [DllImport("webrtc_c")]
        private static extern void rtcThreadRun(IntPtr ptr);

        [DllImport("webrtc_c")]
        private static extern void rtcThreadStart(IntPtr ptr);

        public static Thread Create()
        {
            return new Thread(rtcCreateThread());
        }

        internal Thread(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~Thread()
        {
            rtcDeleteThread(Ptr);
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            rtcDeleteThread(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        public void Run()
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            rtcThreadRun(Ptr);
        }

        public void Start()
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            rtcThreadStart(Ptr);
        }
    }

    public readonly struct ThreadManager
    {
#pragma warning disable 0649
        private readonly IntPtr Ptr;
#pragma warning restore 0649

        [DllImport("webrtc_c")]
        private static extern IntPtr rtcThreadManagerWrapCurrentThread(
            IntPtr ptr
        );

        public bool IsDefault => Ptr == IntPtr.Zero;

        public Thread WrapCurrentThread()
        {
            if (IsDefault)
            {
                throw new InvalidOperationException();
            }

            return new Thread(rtcThreadManagerWrapCurrentThread(Ptr));
        }

        public static extern ThreadManager Instance
        {
            [DllImport("webrtc_c", EntryPoint = "rtcThreadManagerInstance")]
            get;
        }
    }
}
