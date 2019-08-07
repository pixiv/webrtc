using Rtc;
using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    public abstract class MediaSourceInterface : IDisposable
    {
        [DllImport("webrtc_c")]
        private static extern void webrtcMediaSourceInterfaceRelease(
            IntPtr ptr
        );

        internal IntPtr Ptr;

        internal MediaSourceInterface()
        {
        }

        ~MediaSourceInterface()
        {
            webrtcMediaSourceInterfaceRelease(Ptr);
        }

        public enum SourceState
        {
            Initializing,
            Live,
            Enabled,
            Muted
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcMediaSourceInterfaceRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }

    public sealed class MediaStreamInterface : IDisposable
    {
        [DllImport("webrtc_c")]
        private static extern void webrtcMediaStreamInterfaceRelease(
            IntPtr ptr
        );

        internal IntPtr Ptr;

        internal MediaStreamInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~MediaStreamInterface()
        {
            webrtcMediaStreamInterfaceRelease(Ptr);
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcMediaStreamInterfaceRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }

    public abstract class MediaStreamTrackInterface : IDisposable
    {
        [DllImport("webrtc_c")]
        private static extern String webrtcMediaStreamTrackInterfaceId(
            IntPtr ptr
        );

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcMediaStreamTrackInterfaceKAudioKind();

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcMediaStreamTrackInterfaceKVideoKind();

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcMediaStreamTrackInterfaceKind(
            IntPtr ptr
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcMediaStreamTrackInterfaceRelease(
            IntPtr ptr
        );

        internal IntPtr Ptr;

        internal MediaStreamTrackInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~MediaStreamTrackInterface()
        {
            webrtcMediaStreamTrackInterfaceRelease(Ptr);
        }

        internal static MediaStreamTrackInterface Wrap(IntPtr ptr)
        {
            var kind = webrtcMediaStreamTrackInterfaceKind(ptr);

            if (kind == webrtcMediaStreamTrackInterfaceKAudioKind())
            {
                return new AudioTrackInterface(ptr);
            }

            if (kind == webrtcMediaStreamTrackInterfaceKVideoKind())
            {
                return new VideoTrackInterface(ptr);
            }

            throw new ArgumentException(nameof(ptr));
        }

        public string ID
        {
            get
            {
                if (Ptr == IntPtr.Zero)
                {
                    throw new ObjectDisposedException(null);
                }

                using (var id = webrtcMediaStreamTrackInterfaceId(Ptr))
                {
                    return id.ToString();
                }
            }
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcMediaStreamTrackInterfaceRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }

    public abstract class VideoTrackSourceInterface : MediaSourceInterface
    {
        [DllImport("webrtc_c")]
        internal static extern IntPtr webrtcMediaSourceInterfaceToWebrtcVideoTrackSourceInterface(
            IntPtr ptr
        );

        internal VideoTrackSourceInterface()
        {
        }
    }

    public sealed class VideoTrackInterface : MediaStreamTrackInterface
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

        [DllImport("webrtc_c")]
        internal static extern IntPtr webrtcMediaStreamTrackInterfaceToWebrtcVideoTrackInterface(
            IntPtr stream
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcVideoTrackInterfaceAddOrUpdateSink(
            IntPtr stream,
            IntPtr sink,
            [MarshalAs(UnmanagedType.LPStruct)] RtcVideoSinkWants wants
        );

        internal VideoTrackInterface(IntPtr ptr) : base(ptr)
        {
        }

        public void AddOrUpdateSink(VideoSinkInterface sink, VideoSinkWants wants)
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

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
                webrtcMediaStreamTrackInterfaceToWebrtcVideoTrackInterface(Ptr),
                sink.Ptr,
                unmanagedWants
            );
        }
    }

    public sealed class AudioTrackInterface : MediaStreamTrackInterface
    {
        internal AudioTrackInterface(IntPtr ptr) : base(ptr)
        {
        }
    }
}
