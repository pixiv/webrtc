#include "media/base/adapted_video_track_source.h"
#include "rtc_base/ref_counted_object.h"

enum WebrtcVideoTrackSourceInterfaceNeedsDenoising {
  WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_DEFAULT,
  WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_FALSE,
  WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_TRUE,
};

namespace rtc {

extern "C" typedef AdaptedVideoTrackSource::SourceState SourceStateGetter(void*);
extern "C" typedef WebrtcVideoTrackSourceInterfaceNeedsDenoising NeedsDenoisingGetter(void*);
extern "C" typedef bool BoolGetter(void*);
extern "C" typedef void DestructionHandler(void*);

class UnprotectedAdaptedVideoTrackSource : public AdaptedVideoTrackSource {
  public:
    explicit UnprotectedAdaptedVideoTrackSource(
        int required_alignment, void* context, DestructionHandler onDestruction,
        SourceStateGetter state, BoolGetter remote, BoolGetter is_screencast,
        NeedsDenoisingGetter needs_denoising) :
          AdaptedVideoTrackSource(required_alignment) {
      context_ = context;
      onDestruction_ = onDestruction;
      state_ = state;
      remote_ = remote;
      is_screencast_ = is_screencast;
      needs_denoising_ = needs_denoising;
    }

    void OnFrame(const webrtc::VideoFrame& frame) {
      AdaptedVideoTrackSource::OnFrame(frame);
    }

    bool AdaptFrame(int width,
                    int height,
                    int64_t time_us,
                    int* out_width,
                    int* out_height,
                    int* crop_width,
                    int* crop_height,
                    int* crop_x,
                    int* crop_y) {
      return AdaptedVideoTrackSource::AdaptFrame(
        width, height, time_us, out_width, out_height,
        crop_width, crop_height, crop_x, crop_y);
    }

    SourceState state() const override {
      return state_(context_);
    }

    bool remote() const override {
      return remote_(context_);
    }

    bool is_screencast() const override {
      return is_screencast_(context_);
    }

    absl::optional<bool> needs_denoising() const override {
      switch (needs_denoising_(context_))
      {
        case WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_DEFAULT:
          return absl::optional<bool>();

        case WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_FALSE:
          return absl::optional<bool>(false);

        case WEBRTC_VIDEO_TRACK_SOURCE_INTERFACE_NEEDS_DENOISING_TRUE:
          return absl::optional<bool>(true);
      }
    }

    bool apply_rotation() {
      return AdaptedVideoTrackSource::apply_rotation();
    }

    cricket::VideoAdapter* video_adapter() {
      return AdaptedVideoTrackSource::video_adapter();
    }

  protected:
    ~UnprotectedAdaptedVideoTrackSource() {
      onDestruction_(context_);
    }

  private:
    void* context_;
    DestructionHandler* onDestruction_;
    SourceStateGetter* state_;
    BoolGetter* remote_;
    BoolGetter* is_screencast_;
    NeedsDenoisingGetter* needs_denoising_;
};

}

RTC_EXPORT extern "C" void rtcUnprotectedAdaptedVideoTrackSourceOnFrame(
    void* source,
    const void* frame) {
  return static_cast<rtc::UnprotectedAdaptedVideoTrackSource*>(source)->OnFrame(
    *static_cast<const webrtc::VideoFrame*>(frame));
}

RTC_EXPORT extern "C" void* rtcUnprotectedAdaptedVideoTrackSourceToWebrtcMediaSourceInterface(
    void* source) {
  return static_cast<webrtc::MediaSourceInterface*>(
    static_cast<rtc::UnprotectedAdaptedVideoTrackSource*>(source));
}

RTC_EXPORT extern "C" bool rtcUnprotectedAdaptedVideoTrackSourceAdaptFrame(
    void* source,
    int width,
    int height,
    int64_t time_us,
    int* out_width,
    int* out_height,
    int* crop_width,
    int* crop_height,
    int* crop_x,
    int* crop_y) {
  return static_cast<rtc::UnprotectedAdaptedVideoTrackSource*>(source)->
    AdaptFrame(width, height, time_us, out_width, out_height,
               crop_width, crop_height, crop_x, crop_y);
}

RTC_EXPORT extern "C" bool rtcUnprotectedAdaptedVideoTrackSource_apply_rotation(
    void* source) {
  return static_cast<rtc::UnprotectedAdaptedVideoTrackSource*>(source)->
    apply_rotation();
}

RTC_EXPORT extern "C" void* rtcUnprotectedAdaptedVideoTrackSource_video_adapter(
    void* source) {
  return static_cast<rtc::UnprotectedAdaptedVideoTrackSource*>(source)->
    video_adapter();
}

RTC_EXPORT extern "C" void* webrtcMediaSourceInterfaceToRtcUnprotectedAdaptedVideoTrackSource(
    void* source) {
  return static_cast<rtc::UnprotectedAdaptedVideoTrackSource*>(
    static_cast<webrtc::MediaSourceInterface*>(source));
}

RTC_EXPORT extern "C" void* rtcNewUnprotectedAdaptedVideoTrackSource(
    int required_alignment, void* context,
    rtc::DestructionHandler* onDestruction, rtc::SourceStateGetter* state,
    rtc::BoolGetter* remote, rtc::BoolGetter* is_screencast,
    rtc::NeedsDenoisingGetter* needs_denoising) {
  auto source =
    new rtc::RefCountedObject<rtc::UnprotectedAdaptedVideoTrackSource>(
      required_alignment, context, onDestruction,
      state, remote, is_screencast, needs_denoising);

  source->AddRef();
  return source;
}
