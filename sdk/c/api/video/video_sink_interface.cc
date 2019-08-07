#include "api/video/video_frame.h"
#include "api/video/video_sink_interface.h"

namespace rtc {

extern "C" typedef void FrameHandler(void* context, const void* frame);
extern "C" typedef void DiscardedFrameHandler(void* context);

class DelegatingVideoSinkInterface : public VideoSinkInterface<webrtc::VideoFrame> {
  public:
    DelegatingVideoSinkInterface(
        void* context,
        FrameHandler* onFrame,
        DiscardedFrameHandler* onDiscardedFrame) {
      context_ = context;
      onFrame_ = onFrame;
      onDiscardedFrame_ = onDiscardedFrame;
    }

    void OnFrame(const webrtc::VideoFrame& frame) override {
      onFrame_(context_, &frame);
    }

    void OnDiscardedFrame() override {
      onDiscardedFrame_(context_);
    }

  private:
    void* context_;
    FrameHandler* onFrame_;
    DiscardedFrameHandler* onDiscardedFrame_;
};

}

RTC_EXPORT extern "C" void* rtcNewVideoSinkInterface(
    void* context,
    rtc::FrameHandler* onFrame,
    rtc::DiscardedFrameHandler* onDiscardedFrame) {
  return static_cast<rtc::VideoSinkInterface<webrtc::VideoFrame>*>(
    new rtc::DelegatingVideoSinkInterface(context, onFrame, onDiscardedFrame));
}

RTC_EXPORT extern "C" void rtcDeleteVideoSinkInterface(void* sink) {
  delete static_cast<rtc::VideoSinkInterface<webrtc::VideoFrame>*>(sink);
}
