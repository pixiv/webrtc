#include "api/video/video_frame.h"

RTC_EXPORT extern "C" void webrtcDeleteVideoFrame(void* frame) {
  delete static_cast<webrtc::VideoFrame*>(frame);
}

RTC_EXPORT extern "C" void webrtcDeleteVideoFrameBuilder(void* builder) {
  delete static_cast<webrtc::VideoFrame::Builder*>(builder);
}

RTC_EXPORT extern "C" void* webrtcNewVideoFrameBuilder() {
  return new webrtc::VideoFrame::Builder();
}

RTC_EXPORT extern "C" void* webrtcVideoFrameBuilderBuild(void* builder) {
  return new webrtc::VideoFrame(static_cast<webrtc::VideoFrame::Builder*>(builder)->build());
}

RTC_EXPORT extern "C" int webrtcVideoFrameWidth(const void* frame) {
  return static_cast<const webrtc::VideoFrame*>(frame)->width();
}

RTC_EXPORT extern "C" int webrtcVideoFrameHeight(const void* frame) {
  return static_cast<const webrtc::VideoFrame*>(frame)->height();
}

RTC_EXPORT extern "C" void webrtcVideoFrameBuilder_set_video_frame_buffer(
    void* builder,
    void* buffer) {
  static_cast<webrtc::VideoFrame::Builder*>(builder)->set_video_frame_buffer(
    static_cast<webrtc::VideoFrameBuffer*>(buffer));
}

RTC_EXPORT extern "C" void webrtcVideoFrameBuilder_set_timestamp_ms(
    void* builder,
    int64_t timestamp_ms) {
  static_cast<webrtc::VideoFrame::Builder*>(builder)->set_timestamp_ms(timestamp_ms);
}

RTC_EXPORT extern "C" void webrtcVideoFrameBuilder_set_timestamp_us(
    void* builder,
    int64_t timestamp_us) {
  static_cast<webrtc::VideoFrame::Builder*>(builder)->set_timestamp_us(timestamp_us);
}

RTC_EXPORT extern "C" void webrtcVideoFrameBuilder_set_timestamp_rtp(
    void* builder,
    uint32_t timestamp_rtp) {
  static_cast<webrtc::VideoFrame::Builder*>(builder)->set_timestamp_rtp(timestamp_rtp);
}

RTC_EXPORT extern "C" void webrtcVideoFrameBuilder_set_ntp_time_ms(
    void* builder,
    int64_t ntp_time_ms) {
  static_cast<webrtc::VideoFrame::Builder*>(builder)->set_ntp_time_ms(ntp_time_ms);
}

RTC_EXPORT extern "C" void webrtcVideoFrameBuilder_set_rotation(
    void* builder,
    webrtc::VideoRotation rotation) {
  static_cast<webrtc::VideoFrame::Builder*>(builder)->set_rotation(rotation);
}

RTC_EXPORT extern "C" void webrtcVideoFrameBuilder_set_color_space(
    void* builder,
    const void* color_space) {
  static_cast<webrtc::VideoFrame::Builder*>(builder)->set_color_space(
    static_cast<const webrtc::ColorSpace*>(color_space));
}

RTC_EXPORT extern "C" void webrtcVideoFrameBuilder_set_id(void* builder,
                                                          uint16_t id) {
  static_cast<webrtc::VideoFrame::Builder*>(builder)->set_id(id);
}

RTC_EXPORT extern "C" void webrtcVideoFrameBuilder_set_update_rect(
    void* builder,
    webrtc::VideoFrame::UpdateRect update_rect) {
  static_cast<webrtc::VideoFrame::Builder*>(builder)->set_update_rect(update_rect);
}
