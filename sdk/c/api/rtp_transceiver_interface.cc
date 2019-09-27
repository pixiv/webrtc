/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/rtp_transceiver_interface.h"

extern "C" void webrtcRtpTransceiverInterfaceRelease(
    const WebrtcRtpTransceiverInterface* transceiver) {
  rtc::ToCplusplus(transceiver)->Release();
}

extern "C" WebrtcRtpReceiverInterface* webrtcRtpTransceiverInterfaceReceiver(
    const WebrtcRtpTransceiverInterface* transceiver) {
  return rtc::ToC(rtc::ToCplusplus(transceiver)->receiver().release());
}
