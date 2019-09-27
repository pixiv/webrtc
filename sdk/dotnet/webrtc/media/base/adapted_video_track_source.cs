/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;
using System;
using System.Runtime.InteropServices;

namespace Pixiv.Rtc
{
    public abstract class AdaptedVideoTrackSource :
        DisposablePtr, IDisposableVideoTrackSourceInterface
    {
        IntPtr IMediaSourceInterface.Ptr =>
            Interop.AdaptedVideoTrackSource.ToWebrtcMediaSourceInterface(Ptr);

        IntPtr IVideoTrackSourceInterface.Ptr =>
            Webrtc.Interop.VideoTrackSourceInterface.FromWebrtcMediaSourceInterface(
                ((IMediaSourceInterface)this).Ptr);

        private enum NeedsDenoisingEnum
        {
            Default,
            False,
            True
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate NeedsDenoisingEnum NeedsDenoisingGetter(IntPtr ptr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate MediaSourceInterface.SourceState SourceStateGetter(
            IntPtr ptr
        );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private delegate bool BoolGetter(IntPtr ptr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DestructionHandler(IntPtr ptr);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void rtcAdaptedVideoTrackSourceOnFrame(
            IntPtr source,
            IntPtr frame
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool rtcAdaptedVideoTrackSourceAdaptFrame(
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

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr rtcNewAdaptedVideoTrackSource(
            int requiredAlignment,
            IntPtr context,
            IntPtr functions
        );

        private static readonly FunctionPtrArray s_functions = new FunctionPtrArray(
            (DestructionHandler)OnDestruction,
            (SourceStateGetter)GetState,
            (BoolGetter)GetRemote,
            (BoolGetter)GetIsScreencast,
            (NeedsDenoisingGetter)GetNeedsDenoising
        );

        [MonoPInvokeCallback(typeof(DestructionHandler))]
        private static void OnDestruction(IntPtr context)
        {
            var handle = (GCHandle)context;
            var source = (AdaptedVideoTrackSource)handle.Target;

            if (source._handle.IsAllocated)
            {
                source._handle.Free();
            }

            handle.Free();
        }

        [MonoPInvokeCallback(typeof(SourceStateGetter))]
        private static MediaSourceInterface.SourceState GetState(IntPtr context)
        {
            return ((AdaptedVideoTrackSource)((GCHandle)context).Target).State;
        }

        [MonoPInvokeCallback(typeof(BoolGetter))]
        private static bool GetRemote(IntPtr context)
        {
            return ((AdaptedVideoTrackSource)((GCHandle)context).Target).Remote;
        }

        [MonoPInvokeCallback(typeof(BoolGetter))]
        private static bool GetIsScreencast(IntPtr context)
        {
            var target = (AdaptedVideoTrackSource)((GCHandle)context).Target;
            return target.IsScreencast;
        }

        [MonoPInvokeCallback(typeof(NeedsDenoisingGetter))]
        private static NeedsDenoisingEnum GetNeedsDenoising(IntPtr context)
        {
            var target = (AdaptedVideoTrackSource)((GCHandle)context).Target;

            switch (target.NeedsDenoising)
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
        }

        private GCHandle _handle;

        protected AdaptedVideoTrackSource(int requiredAlignment)
        {
            Ptr = rtcNewAdaptedVideoTrackSource(
                requiredAlignment,
                (IntPtr)GCHandle.Alloc(this, GCHandleType.Weak),
                s_functions.Ptr
            );
        }

        private protected override void FreePtr()
        {
            Webrtc.Interop.MediaSourceInterface.Release(
                Interop.AdaptedVideoTrackSource.ToWebrtcMediaSourceInterface(
                    Ptr));
        }

        protected void OnFrame(IVideoFrame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            rtcAdaptedVideoTrackSourceOnFrame(Ptr, frame.Ptr);
            GC.KeepAlive(frame);
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
            return rtcAdaptedVideoTrackSourceAdaptFrame(
                Ptr,
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

        public override void ReleasePtr()
        {
            _handle = GCHandle.Alloc(this);
            base.ReleasePtr();
        }

        public AdaptedVideoTrackSource() : this(1) { }

        public abstract MediaSourceInterface.SourceState State { get; }
        public abstract bool Remote { get; }
        public abstract bool IsScreencast { get; }
        public abstract bool? NeedsDenoising { get; }
    }
}

namespace Pixiv.Rtc.Interop
{
    public static class AdaptedVideoTrackSource
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcMediaSourceInterfaceToRtcAdaptedVideoTrackSource")]
        public static extern IntPtr FromRtcAdaptedVideoTrackSource(
            IntPtr ptr
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "rtcAdaptedVideoTrackSourceToWebrtcMediaSourceInterface")]
        public static extern IntPtr ToWebrtcMediaSourceInterface(
            IntPtr ptr
        );
    }
}
