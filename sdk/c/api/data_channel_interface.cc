/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/data_channel_interface.h"

namespace webrtc {

class DelegatingDataChannelObserver
    : public DataChannelObserver {
 public:
  DelegatingDataChannelObserver(
      void* context,
      const struct WebrtcDataChannelObserverFunctions* functions) {
    context_ = context;
    functions_ = functions;
  }

  ~DelegatingDataChannelObserver() {
    functions_->on_destruction(context_);
  }

  void OnStateChange() override {
    functions_->on_state_change(context_);
  }

  void OnMessage(const DataBuffer& buffer) override {
    functions_->on_message(context_, buffer.binary, buffer.data, buffer.size);
  }

  void OnBufferedAmountChange(uint64_t sent_data_size) override {
    functions_->on_buffered_amount_change(context_, sent_data_size);
  }

 private:
  void* context_;
  const struct WebrtcDataChannelObserverFunctions* functions_;
  };
}

struct WebrtcDataChannelObserverFunctions {
  void (*on_destruction)(void*);
  void (*on_state_change)(void*);
  void (*on_message)(void*, bool binary, void* data, size_t len);
  void (*on_buffered_amount_change)(void*, uint64_t);
};

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
    WebrtcDataChannelInterface* channel,
    const char* text
    ) {
      auto chan = rtc::ToCplusplus(channel);
      const auto db = webrtc::DataBuffer(text);
      return chan->Send(db);
}

RTC_EXPORT extern "C" bool webrtcDataChannelSendData(
    WebrtcDataChannelInterface* channel,
    const char* data,
    size_t len
    ) {
      auto chan = rtc::ToCplusplus(channel);
      rtc::CopyOnWriteBuffer writeBuffer(data, len);
      const auto db = webrtc::DataBuffer(writeBuffer, true);
      return chan->Send(db);
    }
    
RTC_EXPORT extern "C" WebrtcDataChannelObserver* webrtcDataChannelRegisterObserver(
    WebrtcDataChannelInterface* channel,
    const struct WebrtcDataChannelObserverFunctions* functions) {

      auto chan = rtc::ToCplusplus(channel);
      auto obs = new webrtc::DelegatingDataChannelObserver(channel, functions);
      chan->RegisterObserver(obs);
      return obs;
}

RTC_EXPORT extern "C" void webrtcDataChannelUnregisterObserver(
    WebrtcDataChannelInterface* channel
    ) {
      auto chan = rtc::ToCplusplus(channel);
      chan->UnregisterObserver();
}



