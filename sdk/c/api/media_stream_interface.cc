#include "api/media_stream_interface.h"

struct RtcVideoSinkWants {
  bool rotation_applied;
  bool black_frames;
  bool has_target_pixel_count;
  int max_pixel_count;
  int target_pixel_count;
  int max_framerate_fps;
};

RTC_EXPORT extern "C" void webrtcMediaSourceInterfaceRelease(
    const void* source) {
  static_cast<const webrtc::MediaSourceInterface*>(source)->Release();
}

RTC_EXPORT extern "C" void* webrtcMediaSourceInterfaceToWebrtcVideoTrackSourceInterface(
    void* source) {
  return static_cast<webrtc::VideoTrackSourceInterface*>(
    static_cast<webrtc::MediaSourceInterface*>(source));
}

RTC_EXPORT extern "C" void webrtcMediaStreamInterfaceRelease(
    const void* stream) {
  static_cast<const webrtc::MediaStreamInterface*>(stream)->Release();
}

RTC_EXPORT extern "C" void* webrtcMediaStreamTrackInterfaceId(
    const void* track) {
  return new auto(static_cast<const webrtc::MediaStreamTrackInterface*>(track)->id());
}

RTC_EXPORT extern "C" void webrtcMediaStreamTrackInterfaceRelease(
    const void* track) {
  static_cast<const webrtc::MediaStreamTrackInterface*>(track)->Release();
}

RTC_EXPORT extern "C" const char* webrtcMediaStreamTrackInterfaceKAudioKind() {
  return webrtc::MediaStreamTrackInterface::kAudioKind;
}

RTC_EXPORT extern "C" const char* webrtcMediaStreamTrackInterfaceKind(
    const void* source) {
  auto kind = static_cast<const webrtc::MediaStreamTrackInterface*>(source)->kind();

  if (kind == webrtc::MediaStreamTrackInterface::kAudioKind) {
    return webrtc::MediaStreamTrackInterface::kAudioKind;
  }

  if (kind == webrtc::MediaStreamTrackInterface::kVideoKind) {
    return webrtc::MediaStreamTrackInterface::kVideoKind;
  }

  RTC_NOTREACHED();
}

RTC_EXPORT extern "C" const char* webrtcMediaStreamTrackInterfaceKVideoKind() {
  return webrtc::MediaStreamTrackInterface::kVideoKind;
}

RTC_EXPORT extern "C" void* webrtcMediaStreamTrackInterfaceToWebrtcVideoTrackInterface(
    void* track) {
  return static_cast<webrtc::VideoTrackInterface*>(
    static_cast<webrtc::MediaStreamTrackInterface*>(track)
  );
}

RTC_EXPORT extern "C" void webrtcVideoTrackInterfaceAddOrUpdateSink(
    void* track,
    void* sink,
    const RtcVideoSinkWants* cwants) {
  rtc::VideoSinkWants wants;

  wants.rotation_applied = cwants->rotation_applied;
  wants.black_frames = cwants->black_frames;
  wants.max_pixel_count = cwants->max_pixel_count;
  wants.max_framerate_fps = cwants->max_framerate_fps;

  if (cwants->has_target_pixel_count) {
    wants.target_pixel_count = cwants->target_pixel_count;
  }

  return static_cast<webrtc::VideoTrackInterface*>(track)->AddOrUpdateSink(
    static_cast<rtc::VideoSinkInterface<webrtc::VideoFrame>*>(sink), wants);
}
