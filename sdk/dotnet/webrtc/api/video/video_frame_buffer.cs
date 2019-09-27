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
    public interface IDisposableI420BufferInterface :
        IDisposableVideoFrameBuffer, II420BufferInterface
    {
    }

    public interface IDisposableVideoFrameBuffer :
        IVideoFrameBuffer, Rtc.IDisposable
    {
    }

    public interface II420BufferInterface : IVideoFrameBuffer
    {
        new IntPtr Ptr { get; }
    }

    public interface IVideoFrameBuffer
    {
        IntPtr Ptr { get; }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class I420BufferInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcVideoFrameBufferToWebrtcI420BufferInterface")]
        public static extern IntPtr FromWebrtcVideoFrameBuffer(IntPtr ptr);
    }

    public static class VideoFrameBuffer
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcVideoFrameBufferRelease")]
        public static extern void Release(IntPtr ptr);
    }
}
