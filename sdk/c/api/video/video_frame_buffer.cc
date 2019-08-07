#include "api/video/video_frame_buffer.h"
#include "rtc_base/system/rtc_export.h"

RTC_EXPORT extern "C" void webrtcVideoFrameBufferRelease(const void* buffer) {
  static_cast<const webrtc::VideoFrameBuffer*>(buffer)->Release();
}

RTC_EXPORT extern "C" void* webrtcVideoFrameBufferToWebrtcI420BufferInterface(
    void* buffer) {
  return static_cast<webrtc::I420BufferInterface*>(
    static_cast<webrtc::VideoFrameBuffer*>(buffer));
}
