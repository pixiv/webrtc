/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */

#include "sdk/c/api/jsep.h"
#include "rtc_base/ref_counted_object.h"

namespace webrtc {

class DelegatingCreateSessionDescriptionObserver
    : public CreateSessionDescriptionObserver {
 public:
  DelegatingCreateSessionDescriptionObserver(
      void* context,
      const struct WebrtcCreateSessionDescriptionObserverFunctions* functions) {
    context_ = context;
    functions_ = functions;
  }

  ~DelegatingCreateSessionDescriptionObserver() {
    functions_->on_destruction(context_);
  }

 protected:
  void OnSuccess(SessionDescriptionInterface* desc) override {
    functions_->on_success(context_, rtc::ToC(desc));
  }

  void OnFailure(RTCError error) override {
    auto ctype = static_cast<WebrtcRTCErrorType>(error.type());
    functions_->on_failure(context_, ctype, error.message());
  }

 private:
  void* context_;
  const struct WebrtcCreateSessionDescriptionObserverFunctions* functions_;
};

class DelegatingSetSessionDescriptionObserver
    : public SetSessionDescriptionObserver {
 public:
  DelegatingSetSessionDescriptionObserver(
      void* context,
      const struct WebrtcSetSessionDescriptionObserverFunctions* functions) {
    context_ = context;
    functions_ = functions;
  }

  ~DelegatingSetSessionDescriptionObserver() {
    functions_->on_destruction(context_);
  }

 protected:
  void OnSuccess() override { functions_->on_success(context_); }

  void OnFailure(RTCError error) override {
    auto ctype = static_cast<WebrtcRTCErrorType>(error.type());
    functions_->on_failure(context_, ctype, error.message());
  }

 private:
  void* context_;
  const struct WebrtcSetSessionDescriptionObserverFunctions* functions_;
};
}

extern "C" WebrtcSessionDescriptionInterface* webrtcCreateSessionDescription(
    enum WebrtcSdpType ctype,
    const char* sdp,
    WebrtcSdpParseError** cerror) {
  auto type = static_cast<webrtc::SdpType>(ctype);
  webrtc::SdpParseError* error = nullptr;
  if (cerror) {
    error = new webrtc::SdpParseError();
    *cerror = rtc::ToC(error);
  }

  return rtc::ToC(webrtc::CreateSessionDescription(type, sdp, error).release());
}

extern "C" void webrtcCreateSessionDescriptionObserverRelease(
    const WebrtcCreateSessionDescriptionObserver* observer) {
  rtc::ToCplusplus(observer)->Release();
}

extern "C" void webrtcDeleteSessionDescriptionInterface(
    WebrtcSessionDescriptionInterface* desc) {
  delete rtc::ToCplusplus(desc);
}

extern "C" bool webrtcIceCandidateInterfaceToString(
    const WebrtcIceCandidateInterface* candidate,
    RtcString** out) {
  if (!out) {
    return false;
  }

  auto s = new std::string();
  *out = rtc::ToC(s);
  return rtc::ToCplusplus(candidate)->ToString(s);
}

extern "C" WebrtcCreateSessionDescriptionObserver*
webrtcNewCreateSessionDescriptionObserver(
    void* context,
    const struct WebrtcCreateSessionDescriptionObserverFunctions* functions) {
  auto observer = new rtc::RefCountedObject<
      webrtc::DelegatingCreateSessionDescriptionObserver>(context, functions);

  observer->AddRef();

  return rtc::ToC(observer);
}

extern "C" WebrtcSetSessionDescriptionObserver*
webrtcNewSetSessionDescriptionObserver(
    void* context,
    const struct WebrtcSetSessionDescriptionObserverFunctions* functions) {
  auto observer = new rtc::RefCountedObject<
      webrtc::DelegatingSetSessionDescriptionObserver>(context, functions);

  observer->AddRef();

  return rtc::ToC(
      static_cast<webrtc::SetSessionDescriptionObserver*>(observer));
}

extern "C" bool webrtcSessionDescriptionInterfaceToString(
    const WebrtcSessionDescriptionInterface* desc,
    RtcString** out) {
  if (!out) {
    return false;
  }

  auto s = new std::string();
  *out = rtc::ToC(s);
  return rtc::ToCplusplus(desc)->ToString(s);
}

extern "C" void webrtcSetSessionDescriptionObserverRelease(
    const WebrtcSetSessionDescriptionObserver* observer) {
  rtc::ToCplusplus(observer)->Release();
}
