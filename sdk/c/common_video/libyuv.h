/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_COMMON_VIDEO_LIBYUV_H_
#define SDK_C_COMMON_VIDEO_LIBYUV_H_

enum WebrtcVideoType {
  WEBRTC_VIDEO_TYPE_UNKNOWN,
  WEBRTC_VIDEO_TYPE_I420,
  WEBRTC_VIDEO_TYPE_IYUV,
  WEBRTC_VIDEO_TYPE_RGB24,
  WEBRTC_VIDEO_TYPE_ABGR,
  WEBRTC_VIDEO_TYPE_ARGB,
  WEBRTC_VIDEO_TYPE_ARGB4444,
  WEBRTC_VIDEO_TYPE_RGB565,
  WEBRTC_VIDEO_TYPE_ARGB1555,
  WEBRTC_VIDEO_TYPE_YUY2,
  WEBRTC_VIDEO_TYPE_YV12,
  WEBRTC_VIDEO_TYPE_UYVY,
  WEBRTC_VIDEO_TYPE_MJPEG,
  WEBRTC_VIDEO_TYPE_NV21,
  WEBRTC_VIDEO_TYPE_NV12,
  WEBRTC_VIDEO_TYPE_BGRA,
};

#include <stdint.h>
#include "sdk/c/api/video/i420_buffer.h"
#include "sdk/c/api/video/video_frame.h"

#ifdef __cplusplus
extern "C" {
#endif

RTC_EXPORT int webrtcConvertFromI420(const WebrtcVideoFrame* src_frame,
                                     enum WebrtcVideoType dst_video_type,
                                     int dst_sample_size,
                                     uint8_t* dst_frame);

RTC_EXPORT int webrtcConvertToI420(enum WebrtcVideoType src_video_type,
                                   int src_sample_size,
                                   int src_width,
                                   int src_height,
                                   const uint8_t* src_frame,
                                   WebrtcI420Buffer* dst_frame,
                                   int crop_x,
                                   int crop_y);

#ifdef __cplusplus
}
#endif

#endif
