#include "common_video/libyuv/include/webrtc_libyuv.h"

RTC_EXPORT extern "C" int webrtcConvertFromI420(
    const void* src_frame,
    webrtc::VideoType dst_video_type,
    int dst_sample_size,
    uint8_t* dst_frame) {
  return webrtc::ConvertFromI420(
    *static_cast<const webrtc::VideoFrame*>(src_frame), dst_video_type,
    dst_sample_size, dst_frame);
}

RTC_EXPORT extern "C" int webrtcConvertToI420(webrtc::VideoType src_video_type,
                                              int src_sample_size,
                                              int src_width,
                                              int src_height,
                                              const uint8_t* src_frame,
                                              void* dst_frame,
                                              int crop_x,
                                              int crop_y) {
  return webrtc::ConvertToI420(
    src_video_type, src_sample_size, src_width, src_height, src_frame,
    static_cast<webrtc::I420Buffer*>(dst_frame), crop_x, crop_y);
}
