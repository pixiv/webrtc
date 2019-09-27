/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_VIDEO_VIDEO_FRAME_H_
#define SDK_C_API_VIDEO_VIDEO_FRAME_H_

#include <stdint.h>
#include "sdk/c/api/video/color_space.h"
#include "sdk/c/api/video/video_frame_buffer.h"
#include "sdk/c/api/video/video_rotation.h"
#include "sdk/c/interop.h"

#ifdef __cplusplus
#include "api/video/video_frame.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::VideoFrame, WebrtcVideoFrame)
RTC_C_CLASS(webrtc::VideoFrame::Builder, WebrtcVideoFrameBuilder)

struct WebrtcVideoFrameUpdateRect {
  int offset_x;
  int offset_y;
  int width;
  int height;
};

RTC_EXPORT void webrtcDeleteVideoFrame(WebrtcVideoFrame* frame);

RTC_EXPORT void webrtcDeleteVideoFrameBuilder(WebrtcVideoFrameBuilder* builder);

RTC_EXPORT WebrtcVideoFrameBuilder* webrtcNewVideoFrameBuilder(void);

RTC_EXPORT WebrtcVideoFrame* webrtcVideoFrameBuilderBuild(
    WebrtcVideoFrameBuilder* builder);

RTC_EXPORT int webrtcVideoFrameWidth(const WebrtcVideoFrame* frame);

RTC_EXPORT int webrtcVideoFrameHeight(const WebrtcVideoFrame* frame);

RTC_EXPORT void webrtcVideoFrameBuilder_set_video_frame_buffer(
    WebrtcVideoFrameBuilder* builder,
    WebrtcVideoFrameBuffer* buffer);

RTC_EXPORT void webrtcVideoFrameBuilder_set_timestamp_ms(
    WebrtcVideoFrameBuilder* builder,
    int64_t timestamp_ms);

RTC_EXPORT void webrtcVideoFrameBuilder_set_timestamp_us(
    WebrtcVideoFrameBuilder* builder,
    int64_t timestamp_us);

RTC_EXPORT void webrtcVideoFrameBuilder_set_timestamp_rtp(
    WebrtcVideoFrameBuilder* builder,
    uint32_t timestamp_rtp);

RTC_EXPORT void webrtcVideoFrameBuilder_set_ntp_time_ms(
    WebrtcVideoFrameBuilder* builder,
    int64_t ntp_time_ms);

RTC_EXPORT void webrtcVideoFrameBuilder_set_rotation(
    WebrtcVideoFrameBuilder* builder,
    enum WebrtcVideoRotation rotation);

RTC_EXPORT void webrtcVideoFrameBuilder_set_color_space(
    WebrtcVideoFrameBuilder* builder,
    const WebrtcColorSpace* color_space);

RTC_EXPORT void webrtcVideoFrameBuilder_set_id(WebrtcVideoFrameBuilder* builder,
                                               uint16_t id);

RTC_EXPORT void webrtcVideoFrameBuilder_set_update_rect(
    WebrtcVideoFrameBuilder* builder,
    const struct WebrtcVideoFrameUpdateRect* update_rect);

#ifdef __cplusplus
}
#endif

#endif
