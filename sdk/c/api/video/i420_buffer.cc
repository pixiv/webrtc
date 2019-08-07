#include "api/video/i420_buffer.h"

RTC_EXPORT extern "C" void *webrtcCreateI420Buffer(int width,
                                                   int height,
                                                   int stride_y,
                                                   int stride_u,
                                                   int stride_v) {
  return webrtc::I420Buffer::Create(
    width, height, stride_y, stride_u, stride_v).release();
}

RTC_EXPORT extern "C" void webrtcI420BufferScaleFrom(void* buffer, void* src) {
  static_cast<webrtc::I420Buffer*>(buffer)->ScaleFrom(
    *static_cast<webrtc::I420BufferInterface*>(src));
}

RTC_EXPORT extern "C" void* webrtcI420BufferToWebrtcVideoFrameBuffer(
    void* buffer) {
  return static_cast<webrtc::VideoFrameBuffer*>(
    static_cast<webrtc::I420Buffer*>(buffer));
}

RTC_EXPORT extern "C" void* webrtcVideoFrameBufferToI420Buffer(void* buffer) {
  return static_cast<webrtc::I420Buffer*>(
    static_cast<webrtc::VideoFrameBuffer*>(buffer));
}
