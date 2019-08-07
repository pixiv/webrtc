using System;
using System.Runtime.InteropServices;
using Webrtc;

namespace Rtc
{
    public abstract class AdaptedVideoTrackSource : VideoTrackSourceInterface
    {
        private enum NeedsDenoisingEnum
        {
            Default,
            False,
            True
        }

        private delegate NeedsDenoisingEnum NeedsDenoisingGetter(IntPtr ptr);

        private delegate SourceState SourceStateGetter(IntPtr ptr);

        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool BoolGetter(IntPtr ptr);

        private delegate void DestructionHandler(IntPtr ptr);

        [DllImport("webrtc_c")]
        private static extern void rtcUnprotectedAdaptedVideoTrackSourceOnFrame(
            IntPtr source,
            IntPtr frame
        );

        [DllImport("webrtc_c")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool rtcUnprotectedAdaptedVideoTrackSourceAdaptFrame(
            IntPtr source,
            int width,
            int height,
            long timeUs,
            out int outWidth,
            out int outHeight,
            out int cropWidth,
            out int cropHeight,
            out int cropX,
            out int cropY);

        [DllImport("webrtc_c")]
        private static extern IntPtr rtcUnprotectedAdaptedVideoTrackSourceToWebrtcMediaSourceInterface(
            IntPtr ptr
        );

        [DllImport("webrtc_c")]
        private static extern IntPtr rtcNewUnprotectedAdaptedVideoTrackSource(
            int requiredAlignment,
            IntPtr context,
            DestructionHandler onDesuction,
            SourceStateGetter state,
            BoolGetter remote,
            BoolGetter isScreencast,
            NeedsDenoisingGetter needsDenoising
        );

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcMediaSourceInterfaceToRtcUnprotectedAdaptedVideoTrackSource(
            IntPtr ptr
        );

        private static readonly DestructionHandler s_onDestruction = context =>
            ((GCHandle)context).Free();

        private static readonly SourceStateGetter s_state = context =>
            ((AdaptedVideoTrackSource)((GCHandle)context).Target).State;

        private static readonly BoolGetter s_remote = context =>
            ((AdaptedVideoTrackSource)((GCHandle)context).Target).Remote;

        private static readonly BoolGetter s_isScreencast = context =>
            ((AdaptedVideoTrackSource)((GCHandle)context).Target).IsScreencast;

        private static readonly NeedsDenoisingGetter s_needsDenoising = context => {
            switch (((AdaptedVideoTrackSource)((GCHandle)context).Target).NeedsDenoising)
            {
                case null:
                    return NeedsDenoisingEnum.Default;

                case false:
                    return NeedsDenoisingEnum.False;

                case true:
                    return NeedsDenoisingEnum.True;

                default:
                    throw new Exception();
            }
        };

        protected AdaptedVideoTrackSource(int requiredAlignment)
        {
            Ptr = rtcUnprotectedAdaptedVideoTrackSourceToWebrtcMediaSourceInterface(
                rtcNewUnprotectedAdaptedVideoTrackSource(
                    requiredAlignment,
                    (IntPtr)GCHandle.Alloc(this),
                    s_onDestruction,
                    s_state,
                    s_remote,
                    s_isScreencast,
                    s_needsDenoising
                )
            );
        }

        protected void OnFrame(VideoFrame frame)
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            rtcUnprotectedAdaptedVideoTrackSourceOnFrame(
                webrtcMediaSourceInterfaceToRtcUnprotectedAdaptedVideoTrackSource(Ptr),
                frame.Ptr
            );
        }

        protected bool AdaptFrame(
            int width,
            int height,
            long timeUs,
            out int outWidth,
            out int outHeight,
            out int cropWidth,
            out int cropHeight,
            out int cropX,
            out int cropY
        ) {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            return rtcUnprotectedAdaptedVideoTrackSourceAdaptFrame(
                webrtcMediaSourceInterfaceToRtcUnprotectedAdaptedVideoTrackSource(Ptr),
                width,
                height,
                timeUs,
                out outWidth,
                out outHeight,
                out cropWidth,
                out cropHeight,
                out cropX,
                out cropY);
        }

        public AdaptedVideoTrackSource() : this(1) { }

        public abstract SourceState State { get; }
        public abstract bool Remote { get; }
        public abstract bool IsScreencast { get; }
        public abstract bool? NeedsDenoising { get; }
    }
}
