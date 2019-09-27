/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_RTP_RECEIVER_INTERFACE_H_
#define SDK_C_API_RTP_RECEIVER_INTERFACE_H_

#include "sdk/c/api/media_stream_interface.h"

#ifdef __cplusplus
#include "api/rtp_receiver_interface.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::RtpReceiverInterface, WebrtcRtpReceiverInterface)

RTC_EXPORT void webrtcRtpReceiverInterfaceRelease(
    const WebrtcRtpReceiverInterface* receiver);

RTC_EXPORT WebrtcMediaStreamTrackInterface* webrtcRtpReceiverInterfaceTrack(
    const WebrtcRtpReceiverInterface* receiver);

#ifdef __cplusplus
}
#endif

#endif
