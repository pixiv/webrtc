/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_VIDEO_VIDEO_SINK_INTERFACE_H_
#define SDK_C_API_VIDEO_VIDEO_SINK_INTERFACE_H_

#include "sdk/c/api/video/video_frame.h"

#ifdef __cplusplus
#include "api/video/video_sink_interface.h"

extern "C" {
#endif

RTC_C_CLASS(rtc::VideoSinkInterface<webrtc::VideoFrame>, RtcVideoSinkInterface)

struct RtcVideoSinkInterfaceFunctions {
  void (*on_destruction)(void*);
  void (*on_frame)(void*, const WebrtcVideoFrame*);
  void (*on_discarded_frame)(void*);
};

RTC_EXPORT RtcVideoSinkInterface* rtcNewVideoSinkInterface(
    void* context,
    const struct RtcVideoSinkInterfaceFunctions* functions);

RTC_EXPORT void rtcDeleteVideoSinkInterface(RtcVideoSinkInterface* sink);

#ifdef __cplusplus
}
#endif

#endif
