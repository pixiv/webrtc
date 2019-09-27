/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_VIDEO_VIDEO_FRAME_BUFFER_H_
#define SDK_C_API_VIDEO_VIDEO_FRAME_BUFFER_H_

#include "sdk/c/interop.h"

#ifdef __cplusplus
#include "api/video/video_frame_buffer.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::VideoFrameBuffer, WebrtcVideoFrameBuffer)
RTC_C_CLASS(webrtc::I420BufferInterface, WebrtcI420BufferInterface)

RTC_EXPORT void webrtcVideoFrameBufferRelease(
    const WebrtcVideoFrameBuffer* buffer);

RTC_EXPORT WebrtcI420BufferInterface*
webrtcVideoFrameBufferToWebrtcI420BufferInterface(
    WebrtcVideoFrameBuffer* buffer);

#ifdef __cplusplus
}
#endif

#endif
