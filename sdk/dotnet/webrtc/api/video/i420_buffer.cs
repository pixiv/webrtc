/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using System;
using System.Runtime.InteropServices;

namespace Pixiv.Webrtc
{
    public interface IDisposableI420Buffer : II420Buffer, IDisposableI420BufferInterface
    {
    }

    public interface II420Buffer : II420BufferInterface
    {
        new IntPtr Ptr { get; }
    }

    public sealed class DisposableI420Buffer :
        Rtc.DisposablePtr, IDisposableI420Buffer
    {
        IntPtr II420Buffer.Ptr => Ptr;

        IntPtr II420BufferInterface.Ptr =>
            Interop.I420BufferInterface.FromWebrtcVideoFrameBuffer(
                ((IVideoFrameBuffer)this).Ptr);

        IntPtr IVideoFrameBuffer.Ptr =>
            Interop.I420Buffer.ToWebrtcVideoFrameBuffer(
                ((II420Buffer)this).Ptr);

        public DisposableI420Buffer(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.VideoFrameBuffer.Release(
                Interop.I420Buffer.ToWebrtcVideoFrameBuffer(Ptr));
        }
    }

    public static class I420Buffer
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcCreateI420Buffer(
            int width,
            int height,
            int strideY,
            int strideU,
            int strideV
        );

        public static DisposableI420Buffer Create(
            int width,
            int height,
            int strideY,
            int strideU,
            int strideV)
        {
            return new DisposableI420Buffer(webrtcCreateI420Buffer(
                width,
                height,
                strideY,
                strideU,
                strideV
            ));
        }

        public static DisposableI420Buffer Create(int width, int height)
        {
            return Create(
                width,
                height,
                width,
                (width + 1) / 2,
                (width + 1) / 2
            );
        }
    }

    public static class I420BufferExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcI420BufferScaleFrom(
            IntPtr buffer,
            IntPtr src
        );

        public static void ScaleFrom(
            this II420Buffer dest, II420BufferInterface src)
        {
            if (dest == null)
            {
                throw new ArgumentNullException(nameof(dest));
            }

            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            webrtcI420BufferScaleFrom(dest.Ptr, src.Ptr);

            GC.KeepAlive(dest);
            GC.KeepAlive(src);
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class I420Buffer
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcVideoFrameBufferToWebrtcI420Buffer")]
        public static extern IntPtr FromWebrtcVideoFrameBuffer(IntPtr ptr);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcI420BufferToWebrtcVideoFrameBuffer")]
        public static extern IntPtr ToWebrtcVideoFrameBuffer(IntPtr ptr);
    }
}
