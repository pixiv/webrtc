/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_VIDEO_BUFFER_H_
#define SDK_C_API_VIDEO_BUFFER_H_

#include "sdk/c/api/video/video_frame.h"
#include "sdk/c/api/video/video_sink_interface.h"

#ifdef __cplusplus
#include "api/video/video_buffer.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::VideoBufferInterface, WebrtcVideoBufferInterface)

RTC_EXPORT WebrtcVideoBufferInterface* webrtcCreateVideoBuffer(void);

RTC_EXPORT WebrtcVideoFrame* webrtcVideoBufferInterfaceMoveFrame(
    WebrtcVideoBufferInterface* buffer);

RTC_EXPORT RtcVideoSinkInterface*
webrtcVideoBufferInterfaceToRtcVideoSinkInterface(
    WebrtcVideoBufferInterface* buffer);

#ifdef __cplusplus
}
#endif

#endif
