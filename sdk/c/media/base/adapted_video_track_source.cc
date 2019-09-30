/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/media/base/adapted_video_track_source.h"
#include "rtc_base/ref_counted_object.h"

namespace rtc {

class UnprotectedAdaptedVideoTrackSource : public AdaptedVideoTrackSource {
  public:
   explicit UnprotectedAdaptedVideoTrackSource(
       int required_alignment,
       void* context,
       const struct WebrtcVideoTrackSourceInterfaceFunctions* functions)
       : AdaptedVideoTrackSource(required_alignment) {
     context_ = context;
     functions_ = functions;
   }

   static void UnprotectedOnFrame(AdaptedVideoTrackSource* source,
                                  const webrtc::VideoFrame& frame) {
     (source->*&UnprotectedAdaptedVideoTrackSource::OnFrame)(frame);
   }

   static bool UnprotectedAdaptFrame(AdaptedVideoTrackSource* source,
                                     int width,
                                     int height,
                                     int64_t time_us,
                                     int* out_width,
                                     int* out_height,
                                     int* crop_width,
                                     int* crop_height,
                                     int* crop_x,
                                     int* crop_y) {
     return (source->*&UnprotectedAdaptedVideoTrackSource::AdaptFrame)(
         width, height, time_us, out_width, out_height, crop_width, crop_height,
         crop_x, crop_y);
   }

   SourceState state() const override {
     return static_cast<SourceState>(functions_->state(context_));
   }

   bool remote() const override { return functions_->remote(context_); }

   bool is_screencast() const override {
     return functions_->is_screencast(context_);
   }

    absl::optional<bool> needs_denoising() const override {
      switch (functions_->needs_denoising(context_)) {
        case WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_RESULT_DEFAULT:
          return absl::optional<bool>();

        case WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_RESULT_FALSE:
          return absl::optional<bool>(false);

        case WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_RESULT_TRUE:
          return absl::optional<bool>(true);
      }
    }

    static bool unprotected_apply_rotation(AdaptedVideoTrackSource* source) {
      return (source->*&UnprotectedAdaptedVideoTrackSource::apply_rotation)();
    }

    static cricket::VideoAdapter* unprotected_video_adapter(
        AdaptedVideoTrackSource* source) {
      return (source->*&UnprotectedAdaptedVideoTrackSource::video_adapter)();
    }

  protected:
    ~UnprotectedAdaptedVideoTrackSource() {
      functions_->on_destruction(context_);
    }

  private:
    void* context_;
    const struct WebrtcVideoTrackSourceInterfaceFunctions* functions_;
};

}

extern "C" void rtcAdaptedVideoTrackSourceOnFrame(
    RtcAdaptedVideoTrackSource* source,
    const WebrtcVideoFrame* frame) {
  rtc::UnprotectedAdaptedVideoTrackSource::UnprotectedOnFrame(
      rtc::ToCplusplus(source), *rtc::ToCplusplus(frame));
}

extern "C" WebrtcMediaSourceInterface*
rtcAdaptedVideoTrackSourceToWebrtcMediaSourceInterface(
    RtcAdaptedVideoTrackSource* source) {
  return rtc::ToC(
      static_cast<webrtc::MediaSourceInterface*>(rtc::ToCplusplus(source)));
}

extern "C" bool rtcAdaptedVideoTrackSourceAdaptFrame(
    RtcAdaptedVideoTrackSource* source,
    int width,
    int height,
    int64_t time_us,
    int* out_width,
    int* out_height,
    int* crop_width,
    int* crop_height,
    int* crop_x,
    int* crop_y) {
  return rtc::UnprotectedAdaptedVideoTrackSource::UnprotectedAdaptFrame(
      rtc::ToCplusplus(source), width, height, time_us, out_width, out_height,
      crop_width, crop_height, crop_x, crop_y);
}

extern "C" bool rtcAdaptedVideoTrackSource_apply_rotation(
    RtcAdaptedVideoTrackSource* source) {
  return rtc::UnprotectedAdaptedVideoTrackSource::unprotected_apply_rotation(
      rtc::ToCplusplus(source));
}

extern "C" CricketVideoAdapter* rtcAdaptedVideoTrackSource_video_adapter(
    RtcAdaptedVideoTrackSource* source) {
  return rtc::ToC(
      rtc::UnprotectedAdaptedVideoTrackSource::unprotected_video_adapter(
          rtc::ToCplusplus(source)));
}

extern "C" RtcAdaptedVideoTrackSource*
webrtcMediaSourceInterfaceToRtcAdaptedVideoTrackSource(
    WebrtcMediaSourceInterface* source) {
  return rtc::ToC(
      static_cast<rtc::AdaptedVideoTrackSource*>(rtc::ToCplusplus(source)));
}

extern "C" RtcAdaptedVideoTrackSource* rtcNewAdaptedVideoTrackSource(
    int required_alignment,
    void* context,
    const struct WebrtcVideoTrackSourceInterfaceFunctions* functions) {
  auto source =
      new rtc::RefCountedObject<rtc::UnprotectedAdaptedVideoTrackSource>(
          required_alignment, context, functions);

  source->AddRef();
  return rtc::ToC(static_cast<rtc::AdaptedVideoTrackSource*>(source));
}
