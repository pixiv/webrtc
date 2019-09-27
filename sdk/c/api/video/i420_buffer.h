/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_VIDEO_I420_BUFFER_H_
#define SDK_C_API_VIDEO_I420_BUFFER_H_

#include "sdk/c/api/video/video_frame_buffer.h"

#ifdef __cplusplus
#include "api/video/i420_buffer.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::I420Buffer, WebrtcI420Buffer)

RTC_EXPORT WebrtcI420Buffer* webrtcCreateI420Buffer(int width,
                                                    int height,
                                                    int stride_y,
                                                    int stride_u,
                                                    int stride_v);

RTC_EXPORT void webrtcI420BufferScaleFrom(WebrtcI420Buffer* buffer,
                                          const WebrtcI420BufferInterface* src);

RTC_EXPORT WebrtcVideoFrameBuffer* webrtcI420BufferToWebrtcVideoFrameBuffer(
    WebrtcI420Buffer* buffer);

RTC_EXPORT WebrtcI420Buffer* webrtcVideoFrameBufferToWebrtcI420Buffer(
    WebrtcVideoFrameBuffer* buffer);

#ifdef __cplusplus
}
#endif

#endif
