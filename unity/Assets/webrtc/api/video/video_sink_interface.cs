using System;
using System.Runtime.InteropServices;

namespace Rtc
{
    public interface IVideoSink
    {
        void OnFrame(Webrtc.ConstVideoFrame frame);
        void OnDiscardedFrame();
    }

    public sealed class VideoSinkInterface : IDisposable
    {
        internal IntPtr Ptr;
        private GCHandle _context;

        private delegate void FrameHandler(IntPtr context, IntPtr ptr);
        private delegate void DiscardedFrameHandler(IntPtr context);

        [DllImport("webrtc_c")]
        private static extern IntPtr rtcNewVideoSinkInterface(
            IntPtr context,
            FrameHandler onFrame,
            DiscardedFrameHandler onDiscardedFrame
        );

        [DllImport("webrtc_c")]
        private static extern void rtcDeleteVideoSinkInterface(IntPtr ptr);

        private static FrameHandler s_onFrame = (context, frame) =>
            GetContextTarget(context).OnFrame(
                new Webrtc.ConstVideoFrame(frame));

        private static DiscardedFrameHandler s_onDiscardedFrame = context =>
            GetContextTarget(context).OnDiscardedFrame();

        private static IVideoSink GetContextTarget(IntPtr context)
        {
            return (IVideoSink)(((GCHandle)context).Target);
        }

        public VideoSinkInterface(IVideoSink sink)
        {
            _context = GCHandle.Alloc(sink);

            Ptr = rtcNewVideoSinkInterface(
                (IntPtr)_context,
                s_onFrame,
                s_onDiscardedFrame
            );
        }

        ~VideoSinkInterface()
        {
            rtcDeleteVideoSinkInterface(Ptr);
            _context.Free();
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            rtcDeleteVideoSinkInterface(Ptr);
            Ptr = IntPtr.Zero;
            _context.Free();
        }
    }
}
