/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_RTP_TRANSCEIVER_INTERFACE_H_
#define SDK_C_API_RTP_TRANSCEIVER_INTERFACE_H_

#include "sdk/c/api/rtp_receiver_interface.h"

#ifdef __cplusplus
#include "api/rtp_transceiver_interface.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::RtpTransceiverInterface, WebrtcRtpTransceiverInterface)

RTC_EXPORT void webrtcRtpTransceiverInterfaceRelease(
    const WebrtcRtpTransceiverInterface* transceiver);

RTC_EXPORT WebrtcRtpReceiverInterface* webrtcRtpTransceiverInterfaceReceiver(
    const WebrtcRtpTransceiverInterface* transceiver);

#ifdef __cplusplus
}
#endif

#endif
