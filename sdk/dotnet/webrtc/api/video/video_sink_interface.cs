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
    public interface IDisposableVideoSinkInterface :
        IDisposable, IVideoSinkInterface
    {
    }

    public interface IManagedVideoSink
    {
        void OnFrame(ReadOnlyVideoFrame frame);
        void OnDiscardedFrame();
    }

    public interface IVideoSinkInterface
    {
        IntPtr Ptr { get; }
    }

    public sealed class DisposableVideoSinkInterface :
        DisposablePtr, IDisposableVideoSinkInterface
    {
        IntPtr IVideoSinkInterface.Ptr => Ptr;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DestructionHandler(IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FrameHandler(IntPtr context, IntPtr ptr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DiscardedFrameHandler(IntPtr context);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr rtcNewVideoSinkInterface(
            IntPtr context,
            IntPtr functions
        );

        private static readonly FunctionPtrArray s_functions = new FunctionPtrArray(
            (DestructionHandler)OnDestruction,
            (FrameHandler)OnFrame,
            (DiscardedFrameHandler)OnDiscardedFrame
        );

        private static IManagedVideoSink GetContextTarget(IntPtr context)
        {
            return (IManagedVideoSink)(((GCHandle)context).Target);
        }

        [MonoPInvokeCallback(typeof(DestructionHandler))]
        private static void OnDestruction(IntPtr context)
        {
            ((GCHandle)context).Free();
        }

        [MonoPInvokeCallback(typeof(FrameHandler))]
        private static void OnFrame(IntPtr context, IntPtr frame)
        {
            GetContextTarget(context).OnFrame(new ReadOnlyVideoFrame(frame));
        }

        [MonoPInvokeCallback(typeof(DiscardedFrameHandler))]
        private static void OnDiscardedFrame(IntPtr context)
        {
            GetContextTarget(context).OnDiscardedFrame();
        }

        public DisposableVideoSinkInterface(IManagedVideoSink sink)
        {
            Ptr = rtcNewVideoSinkInterface(
                (IntPtr)GCHandle.Alloc(sink),
                s_functions.Ptr
            );
        }

        private protected override void FreePtr()
        {
            Interop.VideoSinkInterface.Delete(Ptr);
        }
    }
}

namespace Pixiv.Rtc.Interop
{
    public static class VideoSinkInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "rtcDeleteVideoSinkInterface")]
        public static extern void Delete(IntPtr ptr);
    }
}
