/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/video/video_frame_buffer.h"

extern "C" void webrtcVideoFrameBufferRelease(
    const WebrtcVideoFrameBuffer* buffer) {
  rtc::ToCplusplus(buffer)->Release();
}

extern "C" WebrtcI420BufferInterface*
webrtcVideoFrameBufferToWebrtcI420BufferInterface(
    WebrtcVideoFrameBuffer* buffer) {
  return rtc::ToC(
      static_cast<webrtc::I420BufferInterface*>(rtc::ToCplusplus(buffer)));
}
