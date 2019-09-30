/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/video/video_frame.h"

extern "C" void webrtcDeleteVideoFrame(WebrtcVideoFrame* frame) {
  delete rtc::ToCplusplus(frame);
}

extern "C" void webrtcDeleteVideoFrameBuilder(
    WebrtcVideoFrameBuilder* builder) {
  delete rtc::ToCplusplus(builder);
}

extern "C" WebrtcVideoFrameBuilder* webrtcNewVideoFrameBuilder() {
  return rtc::ToC(new webrtc::VideoFrame::Builder());
}

extern "C" WebrtcVideoFrame* webrtcVideoFrameBuilderBuild(
    WebrtcVideoFrameBuilder* builder) {
  return rtc::ToC(new webrtc::VideoFrame(rtc::ToCplusplus(builder)->build()));
}

extern "C" int webrtcVideoFrameWidth(const WebrtcVideoFrame* frame) {
  return rtc::ToCplusplus(frame)->width();
}

extern "C" int webrtcVideoFrameHeight(const WebrtcVideoFrame* frame) {
  return rtc::ToCplusplus(frame)->height();
}

extern "C" void webrtcVideoFrameBuilder_set_video_frame_buffer(
    WebrtcVideoFrameBuilder* builder,
    WebrtcVideoFrameBuffer* buffer) {
  rtc::ToCplusplus(builder)->set_video_frame_buffer(rtc::ToCplusplus(buffer));
}

extern "C" void webrtcVideoFrameBuilder_set_timestamp_ms(
    WebrtcVideoFrameBuilder* builder,
    int64_t timestamp_ms) {
  rtc::ToCplusplus(builder)->set_timestamp_ms(timestamp_ms);
}

extern "C" void webrtcVideoFrameBuilder_set_timestamp_us(
    WebrtcVideoFrameBuilder* builder,
    int64_t timestamp_us) {
  rtc::ToCplusplus(builder)->set_timestamp_us(timestamp_us);
}

extern "C" void webrtcVideoFrameBuilder_set_timestamp_rtp(
    WebrtcVideoFrameBuilder* builder,
    uint32_t timestamp_rtp) {
  rtc::ToCplusplus(builder)->set_timestamp_rtp(timestamp_rtp);
}

extern "C" void webrtcVideoFrameBuilder_set_ntp_time_ms(
    WebrtcVideoFrameBuilder* builder,
    int64_t ntp_time_ms) {
  rtc::ToCplusplus(builder)->set_ntp_time_ms(ntp_time_ms);
}

extern "C" void webrtcVideoFrameBuilder_set_rotation(
    WebrtcVideoFrameBuilder* builder,
    enum WebrtcVideoRotation crotation) {
  auto rotation = static_cast<webrtc::VideoRotation>(crotation);
  rtc::ToCplusplus(builder)->set_rotation(rotation);
}

extern "C" void webrtcVideoFrameBuilder_set_color_space(
    WebrtcVideoFrameBuilder* builder,
    const WebrtcColorSpace* color_space) {
  rtc::ToCplusplus(builder)->set_color_space(rtc::ToCplusplus(color_space));
}

extern "C" void webrtcVideoFrameBuilder_set_id(WebrtcVideoFrameBuilder* builder,
                                               uint16_t id) {
  rtc::ToCplusplus(builder)->set_id(id);
}

extern "C" void webrtcVideoFrameBuilder_set_update_rect(
    WebrtcVideoFrameBuilder* builder,
    const struct WebrtcVideoFrameUpdateRect* c_update_rect) {
  webrtc::VideoFrame::UpdateRect update_rect{};

  update_rect.offset_x = c_update_rect->offset_x;
  update_rect.offset_y = c_update_rect->offset_y;
  update_rect.width = c_update_rect->width;
  update_rect.height = c_update_rect->height;

  rtc::ToCplusplus(builder)->set_update_rect(update_rect);
}
