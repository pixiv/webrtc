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
    public interface IDisposableVideoFrame : IVideoFrame, Rtc.IDisposable
    {
    }

    public interface IReadOnlyVideoFrame
    {
        IntPtr Ptr { get; }
    }

    public interface IVideoFrame : IReadOnlyVideoFrame
    {
    }

    public sealed class DisposableVideoFrame :
        Rtc.DisposablePtr, IDisposableVideoFrame
    {
        IntPtr IReadOnlyVideoFrame.Ptr => Ptr;

        public DisposableVideoFrame(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.VideoFrame.Delete(Ptr);
        }
    }

    public sealed class ReadOnlyVideoFrame : IReadOnlyVideoFrame
    {
        public IntPtr Ptr { get; }

        public ReadOnlyVideoFrame(IntPtr ptr)
        {
            Ptr = ptr;
        }
    }

    public static class ReadOnlyVideoFrameExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern int webrtcVideoFrameWidth(IntPtr ptr);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern int webrtcVideoFrameHeight(IntPtr ptr);

        public static int Width(this IReadOnlyVideoFrame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            var width = webrtcVideoFrameWidth(frame.Ptr);
            GC.KeepAlive(frame);

            return width;
        }

        public static int Height(this IReadOnlyVideoFrame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            var height = webrtcVideoFrameHeight(frame.Ptr);
            GC.KeepAlive(frame);

            return height;
        }

        public static uint Size(this IReadOnlyVideoFrame frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            var width = (uint)webrtcVideoFrameWidth(frame.Ptr);
            var height = (uint)webrtcVideoFrameHeight(frame.Ptr);
            GC.KeepAlive(frame);

            return width * height;
        }
    }

    public static class VideoFrame
    {
        public interface IBuilder
        {
            IntPtr Ptr { get; }
        }

        public interface IDisposableBuilder : IBuilder, Rtc.IDisposable
        {
        }

        public sealed class Builder :
            Rtc.DisposablePtr, Webrtc.VideoFrame.IDisposableBuilder
        {
            [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
            private static extern IntPtr webrtcNewVideoFrameBuilder();

            IntPtr Webrtc.VideoFrame.IBuilder.Ptr => Ptr;

            public Builder(IntPtr ptr)
            {
                Ptr = ptr;
            }

            public Builder()
            {
                Ptr = webrtcNewVideoFrameBuilder();
            }

            private protected override void FreePtr()
            {
                Interop.VideoFrame.Builder.Delete(Ptr);
            }
        }
    }

    public static class VideoFrameBuilderExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcVideoFrameBuilderBuild(
            IntPtr builder
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcVideoFrameBuilder_set_video_frame_buffer(
            IntPtr builder,
            IntPtr buffer
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcVideoFrameBuilder_set_timestamp_ms(
            IntPtr builder,
            long timestampMs
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcVideoFrameBuilder_set_timestamp_us(
            IntPtr builder,
            long timestampUs
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcVideoFrameBuilder_set_timestamp_rtp(
            IntPtr builder,
            uint timestampRtp
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcVideoFrameBuilder_set_ntp_time_ms(
            IntPtr builder,
            long ntpTimeMs
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcVideoFrameBuilder_set_rotation(
            IntPtr builder,
            VideoRotation rotation
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcVideoFrameBuilder_set_color_space(
            IntPtr builder,
            IntPtr colorSpace
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcVideoFrameBuilder_set_id(
            IntPtr builder,
            ushort id
        );

        public static DisposableVideoFrame Build(
            this VideoFrame.IBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var frame = webrtcVideoFrameBuilderBuild(builder.Ptr);
            GC.KeepAlive(builder);

            return new DisposableVideoFrame(frame);
        }

        public static void SetVideoFrameBuffer(
            this VideoFrame.IBuilder builder, IVideoFrameBuffer value)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            webrtcVideoFrameBuilder_set_video_frame_buffer(
                builder.Ptr, value.Ptr);

            GC.KeepAlive(builder);
            GC.KeepAlive(value);
        }

        public static void SetTimestampMs(
            this VideoFrame.IBuilder builder, long value)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            webrtcVideoFrameBuilder_set_timestamp_ms(builder.Ptr, value);
            GC.KeepAlive(builder);
        }

        public static void SetTimstampRtp(
            this VideoFrame.IBuilder builder, uint value)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            webrtcVideoFrameBuilder_set_timestamp_rtp(builder.Ptr, value);
            GC.KeepAlive(builder);
        }

        public static void SetNtpTimeMs(
            this VideoFrame.IBuilder builder, long value)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            webrtcVideoFrameBuilder_set_ntp_time_ms(builder.Ptr, value);
            GC.KeepAlive(builder);
        }

        public static void SetRotation(
            this VideoFrame.IBuilder builder, VideoRotation value)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            webrtcVideoFrameBuilder_set_rotation(builder.Ptr, value);
            GC.KeepAlive(builder);
        }

        public static void SetColorSpace(
            this VideoFrame.IBuilder builder, IntPtr value)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            webrtcVideoFrameBuilder_set_color_space(builder.Ptr, value);
            GC.KeepAlive(builder);
        }

        public static void SetID(
            this VideoFrame.IBuilder builder, ushort value)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            webrtcVideoFrameBuilder_set_id(builder.Ptr, value);
            GC.KeepAlive(builder);
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class VideoFrame
    {
        public static class Builder
        {
            [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDeleteVideoFrameBuilder")]
            public static extern void Delete(IntPtr builder);
        }

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDeleteVideoFrame")]
        public static extern void Delete(IntPtr ptr);
    }
}
