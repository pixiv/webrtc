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
    public interface IAudioSourceInterface : IMediaSourceInterface
    {
        new IntPtr Ptr { get; }
    }

    public interface IAudioTrackInterface
    {
        IntPtr Ptr { get; }
    }

    public interface IAudioTrackSinkInterface
    {
        IntPtr Ptr { get; }
    }

    public interface IDisposableAudioSourceInterface :
        IAudioSourceInterface, IDisposableMediaSourceInterface
    {
    }

    public interface IDisposableAudioTrackInterface :
        IAudioTrackInterface, Rtc.IDisposable
    {
    }

    public interface IDisposableAudioTrackSinkInterface :
        IAudioTrackSinkInterface, Rtc.IDisposable
    {
    }

    public interface IDisposableMediaSourceInterface :
        IMediaSourceInterface, Rtc.IDisposable
    {
    }

    public interface IDisposableMediaStreamInterface :
        IMediaStreamInterface, Rtc.IDisposable
    {
    }

    public interface IDisposableMediaStreamTrackInterface :
        IMediaStreamTrackInterface, Rtc.IDisposable
    {
    }

    public interface IDisposableVideoTrackSourceInterface :
        IDisposableMediaSourceInterface, IVideoTrackSourceInterface
    {
    }

    public interface IDisposableVideoTrackInterface :
        IVideoTrackInterface, IDisposableMediaStreamTrackInterface
    {
    }

    public interface IManagedNotifierInterface
    {
        void RegisterObserver(ObserverInterface observer);
        void UnregisterObserver(ObserverInterface observer);
    }

    public interface IManagedMediaSourceInterface : IManagedNotifierInterface
    {
        MediaSourceInterface.SourceState State { get; }
        bool Remote { get; }
    }

    public interface IManagedAudioSourceInterface :
        IManagedMediaSourceInterface
    {
        void AddSink(AudioTrackSinkInterface sink);
        void RemoveSink(AudioTrackSinkInterface sink);
    }

    public interface IMediaSourceInterface
    {
        IntPtr Ptr { get; }
    }

    public interface IMediaStreamInterface
    {
        IntPtr Ptr { get; }
    }

    public interface IMediaStreamTrackInterface
    {
        IntPtr Ptr { get; }
    }

    public interface IObserverInterface
    {
        IntPtr Ptr { get; }
    }

    public interface IVideoTrackInterface : IMediaStreamTrackInterface
    {
        new IntPtr Ptr { get; }
    }

    public interface IVideoTrackSourceInterface : IMediaSourceInterface
    {
        new IntPtr Ptr { get; }
    }

    public sealed class DisposableMediaSourceInterface :
        DisposablePtr, IDisposableMediaSourceInterface
    {
        IntPtr IMediaSourceInterface.Ptr => Ptr;

        public DisposableMediaSourceInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.MediaSourceInterface.Release(Ptr);
        }
    }

    public sealed class DisposableMediaStreamInterface :
        DisposablePtr, IDisposableMediaStreamInterface
    {
        IntPtr IMediaStreamInterface.Ptr => Ptr;

        public DisposableMediaStreamInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.MediaStreamInterface.Release(Ptr);
        }
    }

    public abstract class DisposableMediaStreamTrackInterface :
        DisposablePtr, IDisposableMediaStreamTrackInterface
    {
        IntPtr IMediaStreamTrackInterface.Ptr => Ptr;

        internal DisposableMediaStreamTrackInterface()
        {
        }

        private protected override void FreePtr()
        {
            Interop.MediaStreamTrackInterface.Release(Ptr);
        }
    }

    public sealed class DisposableVideoTrackInterface :
        DisposableMediaStreamTrackInterface, IDisposableVideoTrackInterface
    {
        IntPtr IVideoTrackInterface.Ptr =>
            Interop.VideoTrackInterface.FromWebrtcMediaStreamTrackInterface(
                ((IMediaStreamTrackInterface)this).Ptr);

        internal DisposableVideoTrackInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }
    }

    public sealed class DisposableAudioTrackInterface :
        DisposableMediaStreamTrackInterface, IDisposableAudioTrackInterface
    {
        IntPtr IAudioTrackInterface.Ptr =>
            Interop.AudioTrackInterface.FromWebrtcMediaStreamTrackInterface(
                ((IMediaStreamTrackInterface)this).Ptr);

        internal DisposableAudioTrackInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }
    }

    public sealed class DisposableAudioSourceInterface :
        DisposablePtr, IDisposableAudioSourceInterface
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DestructionHandler(IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate MediaSourceInterface.SourceState StateHandler(
            IntPtr context
        );

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate bool RemoteHandler(IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ObserverHandler(IntPtr context, IntPtr observer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SinkHandler(IntPtr context, IntPtr sink);

        private static readonly FunctionPtrArray s_functions = new FunctionPtrArray(
            (DestructionHandler)OnDestruction,
            (ObserverHandler)RegisterObserver,
            (ObserverHandler)UnregisterObserver,
            (StateHandler)State,
            (RemoteHandler)Remote,
            (SinkHandler)AddSink,
            (SinkHandler)RemoveSink
        );

        [MonoPInvokeCallback(typeof(DestructionHandler))]
        private static void OnDestruction(IntPtr context)
        {
            ((GCHandle)context).Free();
        }

        [MonoPInvokeCallback(typeof(ObserverHandler))]
        private static void RegisterObserver(IntPtr context, IntPtr observer)
        {
            var target = ((GCHandle)context).Target;
            var source = (IManagedAudioSourceInterface)target;
            source.RegisterObserver(new ObserverInterface(observer));
        }

        [MonoPInvokeCallback(typeof(ObserverHandler))]
        private static void UnregisterObserver(IntPtr context, IntPtr observer)
        {
            var target = ((GCHandle)context).Target;
            var source = (IManagedAudioSourceInterface)target;
            source.UnregisterObserver(new ObserverInterface(observer));
        }

        [MonoPInvokeCallback(typeof(StateHandler))]
        private static MediaSourceInterface.SourceState State(IntPtr context)
        {
            var target = ((GCHandle)context).Target;
            return ((IManagedAudioSourceInterface)target).State;
        }

        [MonoPInvokeCallback(typeof(RemoteHandler))]
        private static bool Remote(IntPtr context)
        {
            var target = ((GCHandle)context).Target;
            return ((IManagedAudioSourceInterface)target).Remote;
        }

        [MonoPInvokeCallback(typeof(SinkHandler))]
        private static void AddSink(IntPtr context, IntPtr sinkPtr)
        {
            var target = ((GCHandle)context).Target;
            var sink = new AudioTrackSinkInterface(sinkPtr);
            ((IManagedAudioSourceInterface)target).AddSink(sink);
        }

        [MonoPInvokeCallback(typeof(SinkHandler))]
        private static void RemoveSink(IntPtr context, IntPtr sinkPtr)
        {
            var target = ((GCHandle)context).Target;
            var sink = new AudioTrackSinkInterface(sinkPtr);
            ((IManagedAudioSourceInterface)target).RemoveSink(sink);
        }

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcNewAudioSourceInterface(
            IntPtr context,
            IntPtr functions
        );

        IntPtr IMediaSourceInterface.Ptr =>
            Interop.AudioSourceInterface.ToWebrtcMediaSourceInterface(
                ((IAudioSourceInterface)this).Ptr
            );

        IntPtr IAudioSourceInterface.Ptr => Ptr;

        public DisposableAudioSourceInterface(
            IManagedAudioSourceInterface managed)
        {
            Ptr = webrtcNewAudioSourceInterface(
                (IntPtr)GCHandle.Alloc(managed),
                s_functions.Ptr
            );
        }

        private protected override void FreePtr()
        {
            Interop.MediaSourceInterface.Release(
                Interop.AudioSourceInterface.ToWebrtcMediaSourceInterface(
                    Ptr
                )
            );
        }
    }

    public sealed class AudioTrackSinkInterface : IAudioTrackSinkInterface
    {
        public delegate void ManagedDataHandler(
            IntPtr audioData,
            int bitsPerSample,
            int sampleRate,
            int numberOfChannels,
            int numberOfFrame
        );

        public IntPtr Ptr { get; }

        public AudioTrackSinkInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }
    }

    public sealed class ObserverInterface : IObserverInterface
    {
        public IntPtr Ptr { get; }

        public ObserverInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }
    }

    public static class AudioTrackSinkInterfaceExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcAudioTrackSinkInterfaceOnData(
            IntPtr sink,
            IntPtr audioData,
            int bitsPerSample,
            int sampleRate,
            [MarshalAs(UnmanagedType.SysUInt)] int numberOfChannels,
            [MarshalAs(UnmanagedType.SysUInt)] int numberOfFrames
        );

        public static void OnData(
            this IAudioTrackSinkInterface sink,
            IntPtr audioData,
            int bitsPerSample,
            int sampleRate,
            int numberOfChannels,
            int numberOfFrames)
        {
            webrtcAudioTrackSinkInterfaceOnData(
                sink.Ptr,
                audioData,
                bitsPerSample,
                sampleRate,
                numberOfChannels,
                numberOfFrames
            );
        }
    }

    public static class MediaSourceInterface
    {
        public enum SourceState
        {
            Initializing,
            Live,
            Enabled,
            Muted
        }
    }

    public static class AudioTrackInterfaceExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcAudioTrackInterfaceAddSink(
            IntPtr track,
            IntPtr sink
        );

        public static void AddSink(
            this IAudioTrackInterface track,
            IAudioTrackSinkInterface sink)
        {
            webrtcAudioTrackInterfaceAddSink(track.Ptr, sink.Ptr);
        }
    }

    public static class MediaStreamTrackInterfaceExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcMediaStreamTrackInterfaceId(
            IntPtr ptr
        );

        public static string ID(this IMediaStreamTrackInterface track)
        {
            return Rtc.Interop.String.MoveToString(
                webrtcMediaStreamTrackInterfaceId(track.Ptr));
        }
    }

    public static class VideoTrackInterfaceExtension
    {
        private struct RtcVideoSinkWants
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool RotationApplied;

            [MarshalAs(UnmanagedType.I1)]
            public bool BlackFrames;

            [MarshalAs(UnmanagedType.I1)]
            public bool HasTargetPixelCount;

            public int MaxPixelCount;
            public int TargetPixelCount;
            public int MaxFramerateFps;
        }

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcVideoTrackInterfaceAddOrUpdateSink(
            IntPtr stream,
            IntPtr sink,
            in RtcVideoSinkWants wants
        );

        public static void AddOrUpdateSink(
            this IVideoTrackInterface track,
            IVideoSinkInterface sink,
            VideoSinkWants wants)
        {
            var unmanagedWants = new RtcVideoSinkWants();

            unmanagedWants.RotationApplied = wants.RotationApplied;
            unmanagedWants.BlackFrames = wants.BlackFrames;
            unmanagedWants.HasTargetPixelCount = wants.TargetPixelCount.HasValue;
            unmanagedWants.MaxPixelCount = wants.MaxPixelCount;
            unmanagedWants.MaxFramerateFps = wants.MaxFramerateFps;

            if (wants.TargetPixelCount.HasValue)
            {
                unmanagedWants.TargetPixelCount = (int)wants.TargetPixelCount;
            }

            webrtcVideoTrackInterfaceAddOrUpdateSink(
                track.Ptr,
                sink.Ptr,
                unmanagedWants
            );
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class AudioSourceInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcAudioSourceInterfaceToWebrtcMediaSourceInterface")]
        public static extern IntPtr ToWebrtcMediaSourceInterface(IntPtr ptr);
    }

    public static class AudioTrackInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcMediaStreamTrackInterfaceToWebrtcAudioTrackInterface")]
        public static extern IntPtr FromWebrtcMediaStreamTrackInterface(IntPtr ptr);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcAudioTrackInterfaceToWebrtcMediaStreamTrackInterface")]
        public static extern IntPtr ToWebrtcMediaStreamTrackInterface(IntPtr ptr);
    }

    public static class MediaSourceInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcMediaSourceInterfaceRelease")]
        public static extern void Release(IntPtr ptr);
    }

    public static class MediaStreamInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcMediaStreamInterfaceRelease")]
        public static extern void Release(IntPtr ptr);
    }

    public static class MediaStreamTrackInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcMediaStreamTrackInterfaceKAudioKind();

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcMediaStreamTrackInterfaceKVideoKind();

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcMediaStreamTrackInterfaceKind(
            IntPtr ptr
        );

        public static DisposableMediaStreamTrackInterface WrapDisposable(
            IntPtr ptr)
        {
            var kind = webrtcMediaStreamTrackInterfaceKind(ptr);

            if (kind == webrtcMediaStreamTrackInterfaceKAudioKind())
            {
                return new DisposableAudioTrackInterface(ptr);
            }

            if (kind == webrtcMediaStreamTrackInterfaceKVideoKind())
            {
                return new DisposableVideoTrackInterface(ptr);
            }

            throw new ArgumentException(nameof(ptr));
        }

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcMediaStreamTrackInterfaceRelease")]
        public static extern void Release(IntPtr ptr);
    }

    public static class VideoTrackInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcMediaStreamTrackInterfaceToWebrtcVideoTrackInterface")]
        public static extern IntPtr FromWebrtcMediaStreamTrackInterface(IntPtr track);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcVideoTrackInterfaceToWebrtcMediaStreamTrackInterface")]
        public static extern IntPtr ToWebrtcMediaStreamTrackInterface(IntPtr track);
    }

    public static class VideoTrackSourceInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcMediaSourceInterfaceToWebrtcVideoTrackSourceInterface")]
        public static extern IntPtr FromWebrtcMediaSourceInterface(IntPtr ptr);
    }
}
