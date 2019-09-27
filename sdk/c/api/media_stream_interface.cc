/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */

#include "api/media_stream_interface.h"
#include "rtc_base/ref_counted_object.h"
#include "sdk/c/api/media_stream_interface.h"

namespace webrtc {

class DelegatingAudioSourceInterface : public AudioSourceInterface {
  public:
   DelegatingAudioSourceInterface(
       void* context,
       const struct AudioSourceInterfaceFunctions* functions) {
     context_ = context;
     functions_ = functions;
   }

   ~DelegatingAudioSourceInterface() { functions_->on_destruction(context_); }

   void RegisterObserver(ObserverInterface* observer) {
     functions_->register_observer(context_, rtc::ToC(observer));
   }

    void UnregisterObserver(ObserverInterface* observer) {
      functions_->unregister_observer(context_, rtc::ToC(observer));
    }

    void AddSink(AudioTrackSinkInterface* sink) {
      functions_->add_sink(context_, rtc::ToC(sink));
    }

    void RemoveSink(AudioTrackSinkInterface* sink) {
      functions_->remove_sink(context_, rtc::ToC(sink));
    }

    SourceState state() const {
      return static_cast<SourceState>(functions_->state(context_));
    }

    bool remote() const { return functions_->remote(context_); }

   private:
    void* context_;
    const struct AudioSourceInterfaceFunctions* functions_;
};

}

extern "C" WebrtcMediaSourceInterface*
webrtcAudioSourceInterfaceToWebrtcMediaSourceInterface(
    WebrtcAudioSourceInterface* source) {
  return rtc::ToC(
      static_cast<webrtc::MediaSourceInterface*>(rtc::ToCplusplus(source)));
}

extern "C" void webrtcAudioTrackInterfaceAddSink(
    WebrtcAudioTrackInterface* track,
    WebrtcAudioTrackSinkInterface* sink) {
  rtc::ToCplusplus(track)->AddSink(rtc::ToCplusplus(sink));
}

extern "C" WebrtcMediaStreamTrackInterface*
webrtcAudioTrackInterfaceToWebrtcMediaStreamTrackInterface(
    WebrtcAudioTrackInterface* track) {
  return rtc::ToC(
      static_cast<webrtc::MediaStreamTrackInterface*>(rtc::ToCplusplus(track)));
}

extern "C" void webrtcAudioTrackSinkInterfaceOnData(
    WebrtcAudioTrackSinkInterface* sink,
    const void* audio_data,
    int bits_per_sample,
    int sample_rate,
    size_t number_of_channels,
    size_t number_of_frames) {
  rtc::ToCplusplus(sink)->OnData(audio_data, bits_per_sample, sample_rate,
                                 number_of_channels, number_of_frames);
}

extern "C" void webrtcMediaSourceInterfaceRelease(
    const WebrtcMediaSourceInterface* source) {
  rtc::ToCplusplus(source)->Release();
}

extern "C" WebrtcVideoTrackSourceInterface*
webrtcMediaSourceInterfaceToWebrtcVideoTrackSourceInterface(
    WebrtcMediaSourceInterface* source) {
  return rtc::ToC(static_cast<webrtc::VideoTrackSourceInterface*>(
      rtc::ToCplusplus(source)));
}

extern "C" void webrtcMediaStreamInterfaceRelease(
    const WebrtcMediaStreamInterface* stream) {
  rtc::ToCplusplus(stream)->Release();
}

extern "C" RtcString* webrtcMediaStreamTrackInterfaceId(
    const WebrtcMediaStreamTrackInterface* track) {
  return rtc::ToC(new auto(rtc::ToCplusplus(track)->id()));
}

extern "C" void webrtcMediaStreamTrackInterfaceRelease(
    const WebrtcMediaStreamTrackInterface* track) {
  rtc::ToCplusplus(track)->Release();
}

extern "C" const char* webrtcMediaStreamTrackInterfaceKAudioKind() {
  return webrtc::MediaStreamTrackInterface::kAudioKind;
}

extern "C" const char* webrtcMediaStreamTrackInterfaceKind(
    const WebrtcMediaStreamTrackInterface* track) {
  auto kind = rtc::ToCplusplus(track)->kind();

  if (kind == webrtc::MediaStreamTrackInterface::kAudioKind) {
    return webrtc::MediaStreamTrackInterface::kAudioKind;
  }

  if (kind == webrtc::MediaStreamTrackInterface::kVideoKind) {
    return webrtc::MediaStreamTrackInterface::kVideoKind;
  }

  RTC_NOTREACHED();

  return nullptr;
}

extern "C" const char* webrtcMediaStreamTrackInterfaceKVideoKind() {
  return webrtc::MediaStreamTrackInterface::kVideoKind;
}

extern "C" WebrtcAudioTrackInterface*
webrtcMediaStreamTrackInterfaceToWebrtcAudioTrackInterface(
    WebrtcMediaStreamTrackInterface* track) {
  return rtc::ToC(
      static_cast<webrtc::AudioTrackInterface*>(rtc::ToCplusplus(track)));
}

extern "C" WebrtcVideoTrackInterface*
webrtcMediaStreamTrackInterfaceToWebrtcVideoTrackInterface(
    WebrtcMediaStreamTrackInterface* track) {
  return rtc::ToC(
      static_cast<webrtc::VideoTrackInterface*>(rtc::ToCplusplus(track)));
}

extern "C" WebrtcAudioSourceInterface* webrtcNewAudioSourceInterface(
    void* context,
    const struct AudioSourceInterfaceFunctions* functions) {
  auto source =
      new rtc::RefCountedObject<webrtc::DelegatingAudioSourceInterface>(
          context, functions);

  source->AddRef();

  return rtc::ToC(static_cast<webrtc::AudioSourceInterface*>(source));
}

extern "C" void webrtcVideoTrackInterfaceAddOrUpdateSink(
    WebrtcVideoTrackInterface* track,
    RtcVideoSinkInterface* sink,
    const struct RtcVideoSinkWants* cwants) {
  rtc::VideoSinkWants wants;

  wants.rotation_applied = cwants->rotation_applied;
  wants.black_frames = cwants->black_frames;
  wants.max_pixel_count = cwants->max_pixel_count;
  wants.max_framerate_fps = cwants->max_framerate_fps;

  if (cwants->has_target_pixel_count) {
    wants.target_pixel_count = cwants->target_pixel_count;
  }

  return rtc::ToCplusplus(track)->AddOrUpdateSink(rtc::ToCplusplus(sink),
                                                  wants);
}

extern "C" WebrtcMediaStreamTrackInterface*
webrtcVideoTrackInterfaceToWebrtcMediaStreamTrackInterface(
    WebrtcVideoTrackInterface* track) {
  return rtc::ToC(
      static_cast<webrtc::MediaStreamTrackInterface*>(rtc::ToCplusplus(track)));
}
