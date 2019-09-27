/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_MEDIA_STREAM_INTERFACE_H_
#define SDK_C_API_MEDIA_STREAM_INTERFACE_H_

#include <stddef.h>
#include "sdk/c/api/video/video_sink_interface.h"

enum WebrtcMediaSourceInterfaceSourceState {
  WEBRTC_MEDIA_SOURCE_INTERFACE_SOURCE_STATE_INITIALIZING,
  WEBRTC_MEDIA_SOURCE_INTERFACE_SOURCE_STATE_LIVE,
  WEBRTC_MEDIA_SOURCE_INTERFACE_SOURCE_STATE_ENDED,
  WEBRTC_MEDIA_SOURCE_INTERFACE_SOURCE_STATE_MUTED
};

enum WebrtcVideoTrackSourceInterfaceNeedsDenoising {
  WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_RESULT_DEFAULT,
  WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_RESULT_FALSE,
  WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_RESULT_TRUE,
};

#ifdef __cplusplus
#include "api/media_stream_interface.h"

extern "C" {
#else
#include <stdbool.h>
#endif

RTC_C_CLASS(webrtc::ObserverInterface, WebrtcObserverInterface)
RTC_C_CLASS(webrtc::MediaSourceInterface, WebrtcMediaSourceInterface)
RTC_C_CLASS(webrtc::MediaStreamInterface, WebrtcMediaStreamInterface)
RTC_C_CLASS(webrtc::MediaStreamTrackInterface, WebrtcMediaStreamTrackInterface)
RTC_C_CLASS(webrtc::VideoTrackSourceInterface, WebrtcVideoTrackSourceInterface)
RTC_C_CLASS(webrtc::VideoTrackInterface, WebrtcVideoTrackInterface)
RTC_C_CLASS(webrtc::AudioTrackSinkInterface, WebrtcAudioTrackSinkInterface)
RTC_C_CLASS(webrtc::AudioSourceInterface, WebrtcAudioSourceInterface)
RTC_C_CLASS(webrtc::AudioTrackInterface, WebrtcAudioTrackInterface)

struct AudioSourceInterfaceFunctions {
  void (*on_destruction)(void*);
  void (*register_observer)(void*, WebrtcObserverInterface*);
  void (*unregister_observer)(void*, WebrtcObserverInterface*);
  enum WebrtcMediaSourceInterfaceSourceState (*state)(void*);
  bool (*remote)(void*);
  void (*add_sink)(void*, WebrtcAudioTrackSinkInterface*);
  void (*remove_sink)(void*, WebrtcAudioTrackSinkInterface*);
};

struct RtcVideoSinkWants {
  bool rotation_applied;
  bool black_frames;
  bool has_target_pixel_count;
  int max_pixel_count;
  int target_pixel_count;
  int max_framerate_fps;
};

struct WebrtcVideoTrackSourceInterfaceFunctions {
  void (*on_destruction)(void*);
  enum WebrtcMediaSourceInterfaceSourceState (*state)(void*);
  bool (*remote)(void*);
  bool (*is_screencast)(void*);
  enum WebrtcVideoTrackSourceInterfaceNeedsDenoising (*needs_denoising)(void*);
};

RTC_EXPORT WebrtcMediaSourceInterface*
webrtcAudioSourceInterfaceToWebrtcMediaSourceInterface(
    WebrtcAudioSourceInterface* source);

RTC_EXPORT void webrtcAudioTrackInterfaceAddSink(
    WebrtcAudioTrackInterface* track,
    WebrtcAudioTrackSinkInterface* sink);

RTC_EXPORT WebrtcMediaStreamTrackInterface*
webrtcAudioTrackInterfaceToWebrtcMediaStreamTrackInterface(
    WebrtcAudioTrackInterface* track);

RTC_EXPORT void webrtcAudioTrackSinkInterfaceOnData(
    WebrtcAudioTrackSinkInterface* sink,
    const void* audio_data,
    int bits_per_sample,
    int sample_rate,
    size_t number_of_channels,
    size_t number_of_frames);

RTC_EXPORT void webrtcMediaSourceInterfaceRelease(
    const WebrtcMediaSourceInterface* source);

RTC_EXPORT WebrtcVideoTrackSourceInterface*
webrtcMediaSourceInterfaceToWebrtcVideoTrackSourceInterface(
    WebrtcMediaSourceInterface* source);

RTC_EXPORT void webrtcMediaStreamInterfaceRelease(
    const WebrtcMediaStreamInterface* stream);

RTC_EXPORT RtcString* webrtcMediaStreamTrackInterfaceId(
    const WebrtcMediaStreamTrackInterface* track);

RTC_EXPORT void webrtcMediaStreamTrackInterfaceRelease(
    const WebrtcMediaStreamTrackInterface* track);

RTC_EXPORT const char* webrtcMediaStreamTrackInterfaceKAudioKind(void);

RTC_EXPORT const char* webrtcMediaStreamTrackInterfaceKind(
    const WebrtcMediaStreamTrackInterface* track);

RTC_EXPORT const char* webrtcMediaStreamTrackInterfaceKVideoKind(void);

RTC_EXPORT WebrtcAudioTrackInterface*
webrtcMediaStreamTrackInterfaceToWebrtcAudioTrackInterface(
    WebrtcMediaStreamTrackInterface* track);

RTC_EXPORT WebrtcVideoTrackInterface*
webrtcMediaStreamTrackInterfaceToWebrtcVideoTrackInterface(
    WebrtcMediaStreamTrackInterface* track);

RTC_EXPORT WebrtcAudioSourceInterface* webrtcNewAudioSourceInterface(
    void* context,
    const struct AudioSourceInterfaceFunctions* functions);

RTC_EXPORT void webrtcVideoTrackInterfaceAddOrUpdateSink(
    WebrtcVideoTrackInterface* track,
    RtcVideoSinkInterface* sink,
    const struct RtcVideoSinkWants* cwants);

RTC_EXPORT WebrtcMediaStreamTrackInterface*
webrtcVideoTrackInterfaceToWebrtcMediaStreamTrackInterface(
    WebrtcVideoTrackInterface* track);

#ifdef __cplusplus
}
#endif

#endif
