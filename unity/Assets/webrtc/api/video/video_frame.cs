using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    public class ConstVideoFrame
    {
        [DllImport("webrtc_c")]
        private static extern int webrtcVideoFrameWidth(IntPtr ptr);

        [DllImport("webrtc_c")]
        private static extern int webrtcVideoFrameHeight(IntPtr ptr);

        internal IntPtr Ptr;

        internal ConstVideoFrame(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public int Width => webrtcVideoFrameWidth(Ptr);
        public int Height => webrtcVideoFrameHeight(Ptr);
        public uint Size => (uint)Width * (uint)Height;
    }

    public sealed class VideoFrame : ConstVideoFrame, IDisposable
    {
        [DllImport("webrtc_c")]
        private static extern void webrtcDeleteVideoFrame(IntPtr ptr);

        public sealed class Builder : IDisposable
        {
            internal IntPtr Ptr = webrtcNewVideoFrameBuilder();

            [DllImport("webrtc_c")]
            private static extern void webrtcDeleteVideoFrameBuilder(
                IntPtr builder
            );

            [DllImport("webrtc_c")]
            private static extern IntPtr webrtcNewVideoFrameBuilder();

            [DllImport("webrtc_c")]
            private static extern IntPtr webrtcVideoFrameBuilderBuild(
                IntPtr builder
            );

            [DllImport("webrtc_c")]
            private static extern void webrtcVideoFrameBuilder_set_video_frame_buffer(
                IntPtr builder,
                IntPtr buffer
            );

            [DllImport("webrtc_c")]
            private static extern void webrtcVideoFrameBuilder_set_timestamp_ms(
                IntPtr builder,
                long timestampMs
            );

            [DllImport("webrtc_c")]
            private static extern void webrtcVideoFrameBuilder_set_timestamp_us(
                IntPtr builder,
                long timestampUs
            );

            [DllImport("webrtc_c")]
            private static extern void webrtcVideoFrameBuilder_set_timestamp_rtp(
                IntPtr builder,
                uint timestampRtp
            );

            [DllImport("webrtc_c")]
            private static extern void webrtcVideoFrameBuilder_set_ntp_time_ms(
                IntPtr builder,
                long ntpTimeMs
            );

            [DllImport("webrtc_c")]
            private static extern void webrtcVideoFrameBuilder_set_rotation(
                IntPtr builder,
                VideoRotation rotation
            );

            [DllImport("webrtc_c")]
            private static extern void webrtcVideoFrameBuilder_set_color_space(
                IntPtr builder,
                IntPtr colorSpace
            );

            [DllImport("webrtc_c")]
            private static extern void webrtcVideoFrameBuilder_set_id(
                IntPtr builder,
                ushort id
            );

            ~Builder()
            {
                webrtcDeleteVideoFrameBuilder(Ptr);
            }

            public void Dispose()
            {
                if (Ptr == IntPtr.Zero)
                {
                    return;
                }

                webrtcDeleteVideoFrameBuilder(Ptr);
                Ptr = IntPtr.Zero;
                GC.SuppressFinalize(this);
            }

            public VideoFrame Build()
            {
                if (Ptr == IntPtr.Zero)
                {
                    throw new ObjectDisposedException(null);
                }

                return new VideoFrame(webrtcVideoFrameBuilderBuild(Ptr));
            }

            public VideoFrameBuffer VideoFrameBuffer
            {
                set
                {
                    if (Ptr == IntPtr.Zero)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    webrtcVideoFrameBuilder_set_video_frame_buffer(
                        Ptr, value.Ptr);
                }
            }

            public long TimestampMs
            {
                set
                {
                    if (Ptr == IntPtr.Zero)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    webrtcVideoFrameBuilder_set_timestamp_ms(Ptr, value);
                }
            }

            public long TimestampUs
            {
                set
                {
                    if (Ptr == IntPtr.Zero)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    webrtcVideoFrameBuilder_set_timestamp_us(Ptr, value);
                }
            }

            public uint TimestampRtp
            {
                set
                {
                    if (Ptr == IntPtr.Zero)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    webrtcVideoFrameBuilder_set_timestamp_rtp(Ptr, value);
                }
            }

            public long NtpTimeMs
            {
                set
                {
                    if (Ptr == IntPtr.Zero)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    webrtcVideoFrameBuilder_set_ntp_time_ms(Ptr, value);
                }
            }

            public VideoRotation Rotation
            {
                set
                {
                    if (Ptr == IntPtr.Zero)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    webrtcVideoFrameBuilder_set_rotation(Ptr, value);
                }
            }

            public IntPtr ColorSpace
            {
                set
                {
                    if (Ptr == IntPtr.Zero)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    webrtcVideoFrameBuilder_set_color_space(Ptr, value);
                }
            }

            public ushort ID
            {
                set
                {
                    if (Ptr == IntPtr.Zero)
                    {
                        throw new ObjectDisposedException(null);
                    }

                    webrtcVideoFrameBuilder_set_id(Ptr, value);
                }
            }
        }

        internal VideoFrame(IntPtr ptr) : base(ptr)
        {
        }

        ~VideoFrame()
        {
            webrtcDeleteVideoFrame(Ptr);
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcDeleteVideoFrame(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }
}
