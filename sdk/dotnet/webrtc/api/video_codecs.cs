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
    public interface IDisposableVideoDecoderFactory :
        Rtc.IDisposable, IVideoDecoderFactory
    {
    }

    public interface IDisposableVideoEncoderFactory :
        Rtc.IDisposable, IVideoEncoderFactory
    {
    }

    public interface IVideoDecoderFactory
    {
        IntPtr Ptr { get; }
    }

    public interface IVideoEncoderFactory
    {
        IntPtr Ptr { get; }
    }

    public sealed class DisposableVideoDecoderFactory :
        DisposablePtr, IDisposableVideoDecoderFactory
    {
        IntPtr IVideoDecoderFactory.Ptr => Ptr;

        public DisposableVideoDecoderFactory(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.VideoDecoderFactory.Delete(Ptr);
        }
    }

    public sealed class DisposableVideoEncoderFactory :
        DisposablePtr, IDisposableVideoEncoderFactory
    {
        IntPtr IVideoEncoderFactory.Ptr => Ptr;

        public DisposableVideoEncoderFactory(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.VideoEncoderFactory.Delete(Ptr);
        }
    }

    public static class VideoDecoderFactory
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcCreateBuiltinVideoDecoderFactory();

        public static DisposableVideoDecoderFactory CreateBuiltin()
        {
            return new DisposableVideoDecoderFactory(
                webrtcCreateBuiltinVideoDecoderFactory());
        }
    }

    public static class VideoEncoderFactory
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcCreateBuiltinVideoEncoderFactory();

        public static DisposableVideoEncoderFactory CreateBuiltin()
        {
            return new DisposableVideoEncoderFactory(
                webrtcCreateBuiltinVideoEncoderFactory());
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class VideoDecoderFactory
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDeleteVideoDecoderFactory")]
        public static extern void Delete(IntPtr ptr);
    }

    public static class VideoEncoderFactory
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDeleteVideoEncoderFactory")]
        public static extern void Delete(IntPtr ptr);
    }
}
