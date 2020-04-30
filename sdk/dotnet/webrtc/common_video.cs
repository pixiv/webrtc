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
    public static class Libyuv
    {
        public enum VideoType
        {
            Unknown,
            I420,
            IYUV,
            RGB24,
            ABGR,
            ARGB,
            ARGB4444,
            RGB565,
            ARGB1555,
            YUY2,
            YV12,
            UYVY,
            MJPEG,
            NV21,
            NV12,
            BGRA,
        }

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern int webrtcConvertFromI420(
            IntPtr srcFrame,
            VideoType dstVideoType,
            int dstSampleSize,
            IntPtr dstFrame
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern int webrtcConvertToI420(
            VideoType srcVideoType,
            int srcSampleSize,
            int srcWidth,
            int srcHeight,
            IntPtr srcFrame,
            IntPtr dstFrame,
            int cropX,
            int cropY
        );

        public static bool ConvertFromI420(
            IReadOnlyVideoFrame srcFrame,
            VideoType dstVideoType,
            int dstSampleSize,
            IntPtr dstFrame)
        {
            if (srcFrame == null)
            {
                throw new ArgumentNullException(nameof(srcFrame));
            }

            var result = webrtcConvertFromI420(
                srcFrame.Ptr,
                dstVideoType,
                dstSampleSize,
                dstFrame);

            GC.KeepAlive(srcFrame);

            return result == 0;
        }

        public static bool ConvertToI420(
            VideoType srcVideoType,
            int srcSampleSize,
            int srcWidth,
            int srcHeight,
            IntPtr srcFrame,
            II420Buffer dstFrame,
            int cropX,
            int cropY)
        {
            if (dstFrame == null)
            {
                throw new ArgumentNullException(nameof(dstFrame));
            }

            var result = webrtcConvertToI420(
                srcVideoType,
                srcSampleSize,
                srcWidth,
                srcHeight,
                srcFrame,
                dstFrame.Ptr,
                cropX,
                cropY);

            GC.KeepAlive(dstFrame);

            return result == 0;
        }
    }
}
