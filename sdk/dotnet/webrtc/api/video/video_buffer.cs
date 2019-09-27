/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Rtc;
using System;
using System.Runtime.InteropServices;

namespace Pixiv.Webrtc
{
    public interface IDisposableVideoBufferInterface :
        IDisposableVideoSinkInterface, IVideoBufferInterface
    {
    }

    public interface IVideoBufferInterface : IVideoSinkInterface
    {
        new IntPtr Ptr { get; }
    }

    public sealed class DisposableVideoBufferInterface :
        DisposablePtr, IDisposableVideoBufferInterface
    {
        IntPtr IVideoBufferInterface.Ptr => Ptr;

        IntPtr IVideoSinkInterface.Ptr =>
            Interop.VideoBufferInterface.ToRtcVideoSinkInterface(Ptr);

        public DisposableVideoBufferInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Rtc.Interop.VideoSinkInterface.Delete(
                Interop.VideoBufferInterface.ToRtcVideoSinkInterface(Ptr)
            );
        }
    }

    public static class VideoBuffer
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcCreateVideoBuffer();

        public static DisposableVideoBufferInterface Create()
        {
            return new DisposableVideoBufferInterface(
                webrtcCreateVideoBuffer()
            );
        }
    }

    public static class VideoBufferInterfaceExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcVideoBufferInterfaceMoveFrame(
            IntPtr buffer
        );

        public static DisposableVideoFrame MoveFrame(
            this IVideoBufferInterface buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            var frame = webrtcVideoBufferInterfaceMoveFrame(buffer.Ptr);
            GC.KeepAlive(buffer);

            return frame == IntPtr.Zero ?
                null : new DisposableVideoFrame(frame);
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class VideoBufferInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcVideoBufferInterfaceToRtcVideoSinkInterface")]
        public static extern IntPtr ToRtcVideoSinkInterface(IntPtr ptr);
    }
}
