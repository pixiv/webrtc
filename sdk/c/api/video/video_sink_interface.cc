/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/video/video_sink_interface.h"

namespace rtc {

class DelegatingVideoSinkInterface
    : public VideoSinkInterface<webrtc::VideoFrame> {
 public:
  DelegatingVideoSinkInterface(
      void* context,
      const struct RtcVideoSinkInterfaceFunctions* functions) {
    context_ = context;
    functions_ = functions;
  }

  ~DelegatingVideoSinkInterface() { functions_->on_destruction(context_); }

  void OnFrame(const webrtc::VideoFrame& frame) override {
    functions_->on_frame(context_, rtc::ToC(&frame));
  }

  void OnDiscardedFrame() override { functions_->on_discarded_frame(context_); }

 private:
  void* context_;
  const struct RtcVideoSinkInterfaceFunctions* functions_;
};
}

extern "C" RtcVideoSinkInterface* rtcNewVideoSinkInterface(
    void* context,
    const struct RtcVideoSinkInterfaceFunctions* functions) {
  return rtc::ToC(static_cast<rtc::VideoSinkInterface<webrtc::VideoFrame>*>(
      new rtc::DelegatingVideoSinkInterface(context, functions)));
}

extern "C" void rtcDeleteVideoSinkInterface(RtcVideoSinkInterface* sink) {
  delete rtc::ToCplusplus(sink);
}
