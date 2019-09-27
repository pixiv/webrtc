/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */

using System;
using System.Runtime.InteropServices;

namespace Pixiv.Webrtc
{
    public interface IAudioDeviceModule
    {
        IntPtr Ptr { get; }
    }

    public interface IAudioProcessing
    {
        IntPtr Ptr { get; }
    }

    public interface IAudioProcessingBuilder
    {
        IntPtr Ptr { get; }
    }

    public interface IDisposableAudioProcessing :
        Rtc.IDisposable, IAudioProcessing
    {
    }

    public interface IDisposableAudioProcessingBuilder :
        Rtc.IDisposable, IAudioProcessingBuilder
    {
    }

    public sealed class DisposableAudioProcessing :
        Rtc.DisposablePtr, IDisposableAudioProcessing
    {
        IntPtr IAudioProcessing.Ptr => Ptr;

        public DisposableAudioProcessing(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.AudioProcessing.Release(Ptr);
        }
    }

    public sealed class DisposableAudioProcessingBuilder :
        Rtc.DisposablePtr, IDisposableAudioProcessingBuilder
    {
        IntPtr IAudioProcessingBuilder.Ptr => Ptr;

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcNewAudioProcessingBuilder();

        public DisposableAudioProcessingBuilder()
        {
            Ptr = webrtcNewAudioProcessingBuilder();
        }

        private protected override void FreePtr()
        {
            Interop.AudioProcessingBuilder.Delete(Ptr);
        }
    }

    public static class AudioProcessingBuilderExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcAudioProcessingBuilderCreate(
            IntPtr builder
        );

        public static DisposableAudioProcessing Create(
            this IAudioProcessingBuilder builder)
        {
            return new DisposableAudioProcessing(
                webrtcAudioProcessingBuilderCreate(builder.Ptr)
            );
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class AudioProcessing
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcReleaseAudioProcessing")]
        public static extern void Release(IntPtr ptr);
    }

    public static class AudioProcessingBuilder
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDeleteAudioProcessingBuilder")]
        public static extern void Delete(IntPtr ptr);
    }
}
