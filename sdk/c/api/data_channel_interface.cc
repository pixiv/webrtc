/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/data_channel_interface.h"

RTC_EXPORT extern "C" void webrtcDataChannelInterfaceRelease(
    const WebrtcDataChannelInterface* channel) {
  rtc::ToCplusplus(channel)->Release();
}

RTC_EXPORT extern "C" RtcString* webrtcDataChannelLabel(
    const WebrtcDataChannelInterface* channel) {
  return rtc::ToC(new auto(rtc::ToCplusplus(channel)->label()));
}

RTC_EXPORT extern "C" RtcString* webrtcDataChannelStatus(
    const WebrtcDataChannelInterface* channel) {
      auto chan = rtc::ToCplusplus(channel);
      auto sState = chan->DataStateString(chan->state());
      return rtc::ToC(new std::string(sState));
}

RTC_EXPORT extern "C" bool webrtcDataChannelSendText(
    const WebrtcDataChannelInterface* channel,
    const char* text
    ) {
      auto chan = rtc::ToCplusplus(channel);
      auto db = webrtc::DataBuffer(text);
      return chan->Send(&db);
}

RTC_EXPORT extern "C" bool webrtcDataChannelSendData(
    const WebrtcDataChannelInterface* channel,
    const char* data,
    size_t len
    ) {
      auto chan = rtc::ToCplusplus(channel);
      auto writeBuffer(data, len);
      auto db = webrtc::DataBuffer(writeBuffer, true);
      return chan->Send(&db);
}

