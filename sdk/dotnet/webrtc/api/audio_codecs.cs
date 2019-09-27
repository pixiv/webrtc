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
    public interface IAudioDecoderFactory
    {
        IntPtr Ptr { get; }
    }

    public interface IAudioEncoderFactory
    {
        IntPtr Ptr { get; }
    }

    public interface IDisposableAudioDecoderFactory :
        IAudioDecoderFactory, Rtc.IDisposable
    {
    }

    public interface IDisposableAudioEncoderFactory :
        IAudioEncoderFactory, Rtc.IDisposable
    {
    }

    public static class AudioDecoderFactory
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcCreateBuiltinAudioDecoderFactory();

        public static DisposableAudioDecoderFactory CreateBuiltin()
        {
            return new DisposableAudioDecoderFactory(
                webrtcCreateBuiltinAudioDecoderFactory()
            );
        }
    }

    public static class AudioEncoderFactory
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcCreateBuiltinAudioEncoderFactory();

        public static DisposableAudioEncoderFactory CreateBuiltin()
        {
            return new DisposableAudioEncoderFactory(
                webrtcCreateBuiltinAudioEncoderFactory()
            );
        }
    }

    public sealed class DisposableAudioDecoderFactory :
        Rtc.DisposablePtr, IDisposableAudioDecoderFactory
    {
        IntPtr IAudioDecoderFactory.Ptr => Ptr;

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcCreateBuiltinAudioDecoderFactory();

        public DisposableAudioDecoderFactory(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.AudioDecoderFactory.Release(Ptr);
        }
    }

    public sealed class DisposableAudioEncoderFactory :
        Rtc.DisposablePtr, IDisposableAudioEncoderFactory
    {
        IntPtr IAudioEncoderFactory.Ptr => Ptr;

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcCreateBuiltinAudioEncoderFactory();

        public DisposableAudioEncoderFactory(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.AudioEncoderFactory.Release(Ptr);
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class AudioDecoderFactory
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcAudioDecoderFactoryRelease")]
        public static extern void Release(IntPtr ptr);
    }

    public static class AudioEncoderFactory
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcAudioEncoderFactoryRelease")]
        public static extern void Release(IntPtr ptr);
    }
}
