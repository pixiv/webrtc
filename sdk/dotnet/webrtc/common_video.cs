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
            return webrtcConvertFromI420(
                srcFrame.Ptr,
                dstVideoType,
                dstSampleSize,
                dstFrame) == 0;
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
            return webrtcConvertToI420(
                srcVideoType,
                srcSampleSize,
                srcWidth,
                srcHeight,
                srcFrame,
                dstFrame.Ptr,
                cropX,
                cropY) == 0;
        }
    }
}
