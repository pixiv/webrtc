/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/video/video_buffer.h"

extern "C" WebrtcVideoBufferInterface* webrtcCreateVideoBuffer() {
  return rtc::ToC(webrtc::CreateVideoBuffer().release());
}

extern "C" WebrtcVideoFrame* webrtcVideoBufferInterfaceMoveFrame(
    WebrtcVideoBufferInterface* buffer) {
  return rtc::ToC(rtc::ToCplusplus(buffer)->MoveFrame().release());
}

extern "C" RtcVideoSinkInterface*
webrtcVideoBufferInterfaceToRtcVideoSinkInterface(
    WebrtcVideoBufferInterface* buffer) {
  return rtc::ToC(static_cast<rtc::VideoSinkInterface<webrtc::VideoFrame>*>(
      rtc::ToCplusplus(buffer)));
}
