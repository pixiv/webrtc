/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_RTP_SENDER_INTERFACE_H_
#define SDK_C_API_RTP_SENDER_INTERFACE_H_

#include "sdk/c/interop.h"
#include "sdk/c/api/media_stream_interface.h"

#ifdef __cplusplus
#include "api/rtp_sender_interface.h"

extern "C" {
#else
#include <stddef.h>
#endif

RTC_C_CLASS(std::vector<std::string>, WebrtcRtpSenderInterfaceStreamIds)
RTC_C_CLASS(webrtc::RtpSenderInterface, WebrtcRtpSenderInterface)

RTC_EXPORT void webrtcDeleteRtpSenderInterfaceStreamIds(
    WebrtcRtpSenderInterfaceStreamIds* ids);

RTC_EXPORT void webrtcRtpSenderInterfaceRelease(
    const WebrtcRtpSenderInterface* sender);

RTC_EXPORT WebrtcRtpSenderInterfaceStreamIds* webrtcRtpSenderInterfaceStream_ids(
    const WebrtcRtpSenderInterface* sender);

RTC_EXPORT WebrtcMediaStreamTrackInterface* webrtcRtpSenderInterfaceTrack(
    const WebrtcRtpSenderInterface* sender);

RTC_EXPORT void webrtcRtpSenderInterfaceStreamIdsData(
    WebrtcRtpSenderInterfaceStreamIds* ids,
    const char** data);

RTC_EXPORT size_t webrtcRtpSenderInterfaceStreamIdsSize(
    const WebrtcRtpSenderInterfaceStreamIds* ids);

#ifdef __cplusplus
}
#endif

#endif
