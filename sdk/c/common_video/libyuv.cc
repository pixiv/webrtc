/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/common_video/libyuv.h"
#include "common_video/libyuv/include/webrtc_libyuv.h"

extern "C" int webrtcConvertFromI420(const WebrtcVideoFrame* src_frame,
                                     enum WebrtcVideoType c_dst_video_type,
                                     int dst_sample_size,
                                     uint8_t* dst_frame) {
  auto dst_video_type = static_cast<webrtc::VideoType>(c_dst_video_type);

  return webrtc::ConvertFromI420(*rtc::ToCplusplus(src_frame), dst_video_type,
                                 dst_sample_size, dst_frame);
}

extern "C" int webrtcConvertToI420(enum WebrtcVideoType c_src_video_type,
                                   int src_sample_size,
                                   int src_width,
                                   int src_height,
                                   const uint8_t* src_frame,
                                   WebrtcI420Buffer* dst_frame,
                                   int crop_x,
                                   int crop_y) {
  auto src_video_type = static_cast<webrtc::VideoType>(c_src_video_type);
  return webrtc::ConvertToI420(src_video_type, src_sample_size, src_width,
                               src_height, src_frame,
                               rtc::ToCplusplus(dst_frame), crop_x, crop_y);
}
