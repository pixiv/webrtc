/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/video/i420_buffer.h"

extern "C" WebrtcI420Buffer* webrtcCreateI420Buffer(int width,
                                                    int height,
                                                    int stride_y,
                                                    int stride_u,
                                                    int stride_v) {
  return rtc::ToC(
      webrtc::I420Buffer::Create(width, height, stride_y, stride_u, stride_v)
          .release());
}

extern "C" void webrtcI420BufferScaleFrom(
    WebrtcI420Buffer* buffer,
    const WebrtcI420BufferInterface* src) {
  rtc::ToCplusplus(buffer)->ScaleFrom(*rtc::ToCplusplus(src));
}

extern "C" WebrtcVideoFrameBuffer* webrtcI420BufferToWebrtcVideoFrameBuffer(
    WebrtcI420Buffer* buffer) {
  return rtc::ToC(
      static_cast<webrtc::VideoFrameBuffer*>(rtc::ToCplusplus(buffer)));
}

extern "C" WebrtcI420Buffer* webrtcVideoFrameBufferToWebrtcI420Buffer(
    WebrtcVideoFrameBuffer* buffer) {
  return rtc::ToC(static_cast<webrtc::I420Buffer*>(rtc::ToCplusplus(buffer)));
}
