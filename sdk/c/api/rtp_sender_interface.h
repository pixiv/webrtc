/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */

#ifndef SDK_C_API_RTP_SENDER_INTERFACE_H_
#define SDK_C_API_RTP_SENDER_INTERFACE_H_

#include "sdk/c/interop.h"

#ifdef __cplusplus
#include "api/rtp_sender_interface.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::RtpSenderInterface, WebrtcRtpSenderInterface)

RTC_EXPORT void webrtcRtpSenderInterfaceRelease(
    const WebrtcRtpSenderInterface* sender);

#ifdef __cplusplus
}
#endif

#endif
