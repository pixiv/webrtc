/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/rtp_sender_interface.h"

extern "C" void webrtcDeleteRtpSenderInterfaceStreamIds(
    WebrtcRtpSenderInterfaceStreamIds* ids) {
  delete rtc::ToCplusplus(ids);
}

extern "C" void webrtcRtpSenderInterfaceRelease(
    const WebrtcRtpSenderInterface* sender) {
  rtc::ToCplusplus(sender)->Release();
}

extern "C" WebrtcRtpSenderInterfaceStreamIds* webrtcRtpSenderInterfaceStream_ids(
    const WebrtcRtpSenderInterface* sender) {
  return rtc::ToC(new auto(rtc::ToCplusplus(sender)->stream_ids()));
}

extern "C" WebrtcMediaStreamTrackInterface* webrtcRtpSenderInterfaceTrack(
    const WebrtcRtpSenderInterface* sender) {
  return rtc::ToC(rtc::ToCplusplus(sender)->track());
}

extern "C" void webrtcRtpSenderInterfaceStreamIdsData(
    WebrtcRtpSenderInterfaceStreamIds* ids,
    const char** data) {
  auto cplusplusRaw = rtc::ToCplusplus(ids);
  auto cplusplus = std::unique_ptr<std::vector<std::string>>(cplusplusRaw);

  for (size_t index = 0; index < cplusplus->size(); index++) {
    data[index] = (*cplusplus)[index].c_str();
  }
}

extern "C" size_t webrtcRtpSenderInterfaceStreamIdsSize(
    const WebrtcRtpSenderInterfaceStreamIds* ids) {
  return rtc::ToCplusplus(ids)->size();
}
