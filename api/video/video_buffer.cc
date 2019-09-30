/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "api/video/video_buffer.h"
#include "rtc_base/critical_section.h"

namespace webrtc {

class VideoBuffer : public VideoBufferInterface {
  public:
    std::unique_ptr<VideoFrame> MoveFrame() override {
      auto ptr = frame_.exchange(nullptr, std::memory_order_acquire);
      return std::unique_ptr<VideoFrame>(ptr);
    }

    void OnFrame(const VideoFrame& source) override {
      auto ptr = new VideoFrame(source);
      ptr = frame_.exchange(ptr, std::memory_order_acq_rel);
      if (ptr) {
        delete ptr;
      }
    }

  private:
    std::atomic<VideoFrame*> frame_;
};

std::unique_ptr<VideoBufferInterface> CreateVideoBuffer() {
  return std::unique_ptr<VideoBufferInterface>(new VideoBuffer());
}

}
