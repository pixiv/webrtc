/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_MEDIA_BASE_ADAPTED_VIDEO_TRACK_SOURCE_H_
#define SDK_C_MEDIA_BASE_ADAPTED_VIDEO_TRACK_SOURCE_H_

#include <stdint.h>
#include "sdk/c/api/media_stream_interface.h"
#include "sdk/c/api/video/video_frame.h"
#include "sdk/c/media/base/video_adapter.h"

#ifdef __cplusplus
#include "media/base/adapted_video_track_source.h"

extern "C" {
#else
#include <stdbool.h>
#endif

RTC_C_CLASS(rtc::AdaptedVideoTrackSource, RtcAdaptedVideoTrackSource)

RTC_EXPORT void rtcAdaptedVideoTrackSourceOnFrame(
    RtcAdaptedVideoTrackSource* source,
    const WebrtcVideoFrame* frame);

RTC_EXPORT WebrtcMediaSourceInterface*
rtcAdaptedVideoTrackSourceToWebrtcMediaSourceInterface(
    RtcAdaptedVideoTrackSource* source);

RTC_EXPORT bool rtcAdaptedVideoTrackSourceAdaptFrame(
    RtcAdaptedVideoTrackSource* source,
    int width,
    int height,
    int64_t time_us,
    int* out_width,
    int* out_height,
    int* crop_width,
    int* crop_height,
    int* crop_x,
    int* crop_y);

RTC_EXPORT bool rtcAdaptedVideoTrackSource_apply_rotation(
    RtcAdaptedVideoTrackSource* source);

RTC_EXPORT CricketVideoAdapter* rtcAdaptedVideoTrackSource_video_adapter(
    RtcAdaptedVideoTrackSource* source);

RTC_EXPORT RtcAdaptedVideoTrackSource*
webrtcMediaSourceInterfaceToRtcAdaptedVideoTrackSource(
    WebrtcMediaSourceInterface* source);

RTC_EXPORT RtcAdaptedVideoTrackSource* rtcNewAdaptedVideoTrackSource(
    int required_alignment,
    void* context,
    const struct WebrtcVideoTrackSourceInterfaceFunctions* functions);

#ifdef __cplusplus
}
#endif

#endif
