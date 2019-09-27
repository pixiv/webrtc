/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/rtp_receiver_interface.h"

extern "C" void webrtcRtpReceiverInterfaceRelease(
    const WebrtcRtpReceiverInterface* receiver) {
  rtc::ToCplusplus(receiver)->Release();
}

extern "C" WebrtcMediaStreamTrackInterface* webrtcRtpReceiverInterfaceTrack(
    const WebrtcRtpReceiverInterface* receiver) {
  return rtc::ToC(rtc::ToCplusplus(receiver)->track().release());
}
