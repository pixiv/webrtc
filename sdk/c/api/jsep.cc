#include "api/jsep.h"
#include "rtc_base/ref_counted_object.h"

namespace webrtc {

extern "C" typedef void CreateSessionDescriptionSuccessHandler(void*, void*);
extern "C" typedef void DestructionHandler(void*);
extern "C" typedef void FailureHandler(
  void*,
  webrtc::RTCErrorType,
  const char*);
extern "C" typedef void SetSessionDescriptionSuccessHandler(void*);

class DelegatingCreateSessionDescriptionObserver : public CreateSessionDescriptionObserver {
  public:
    DelegatingCreateSessionDescriptionObserver(
        void* context,
        DestructionHandler* onDestruction,
        CreateSessionDescriptionSuccessHandler* onSuccess,
        FailureHandler* onFailure) {
      context_ = context;
      onDestruction_ = onDestruction;
      onSuccess_ = onSuccess;
      onFailure_ = onFailure;
    }

    ~DelegatingCreateSessionDescriptionObserver() {
      onDestruction_(context_);
    }

  protected:
    void OnSuccess(SessionDescriptionInterface* desc) override {
      onSuccess_(context_, desc);
    }

    void OnFailure(RTCError error) override {
      onFailure_(context_, error.type(), error.message());
    }

  private:
    void* context_;
    DestructionHandler* onDestruction_;
    CreateSessionDescriptionSuccessHandler* onSuccess_;
    FailureHandler* onFailure_;
};

class DelegatingSetSessionDescriptionObserver : public SetSessionDescriptionObserver {
  public:
    DelegatingSetSessionDescriptionObserver(
        void* context,
        DestructionHandler* onDestruction,
        SetSessionDescriptionSuccessHandler* onSuccess,
        FailureHandler* onFailure) {
      context_ = context;
      onDestruction_ = onDestruction;
      onSuccess_ = onSuccess;
      onFailure_ = onFailure;
    }

    ~DelegatingSetSessionDescriptionObserver() {
      onDestruction_(context_);
    }

  protected:
    void OnSuccess() override {
      onSuccess_(context_);
    }

    void OnFailure(RTCError error) override {
      onFailure_(context_, error.type(), error.message());
    }

  private:
    void* context_;
    DestructionHandler* onDestruction_;
    SetSessionDescriptionSuccessHandler* onSuccess_;
    FailureHandler* onFailure_;
};

}

RTC_EXPORT extern "C" void* webrtcCreateSessionDescription(
    webrtc::SdpType type,
    const char* sdp,
    void* error)
{
  return webrtc::CreateSessionDescription(
    type, sdp, static_cast<webrtc::SdpParseError*>(error)).release();
}

RTC_EXPORT extern "C" void webrtcCreateSessionDescriptionObserverRelease(const void* observer) {
  static_cast<const webrtc::CreateSessionDescriptionObserver*>(observer)->Release();
}

RTC_EXPORT extern "C" void webrtcDeleteSessionDescriptionInterface(void* desc) {
  delete static_cast<webrtc::SessionDescriptionInterface*>(desc);
}

RTC_EXPORT extern "C" bool webrtcIceCandidateInterfaceToString(const void* candidate, void** out) {
  if (!out) {
    return false;
  }

  auto s = new std::string();
  *out = s;
  return static_cast<const webrtc::IceCandidateInterface*>(candidate)->ToString(s);
}

RTC_EXPORT extern "C" void* webrtcNewCreateSessionDescriptionObserver(
  void* context,
  webrtc::DestructionHandler* onDestruction,
  webrtc::CreateSessionDescriptionSuccessHandler* onSuccess,
  webrtc::FailureHandler* onFailure
) {
  auto observer = new rtc::RefCountedObject<webrtc::DelegatingCreateSessionDescriptionObserver>(
    context, onDestruction, onSuccess, onFailure);

  observer->AddRef();

  return static_cast<webrtc::CreateSessionDescriptionObserver*>(observer);
}

RTC_EXPORT extern "C" void* webrtcNewSetSessionDescriptionObserver(
  void* context,
  webrtc::DestructionHandler* onDestruction,
  webrtc::SetSessionDescriptionSuccessHandler* onSuccess,
  webrtc::FailureHandler* onFailure
) {
  auto observer = new rtc::RefCountedObject<webrtc::DelegatingSetSessionDescriptionObserver>(
    context, onDestruction, onSuccess, onFailure);

  observer->AddRef();

  return static_cast<webrtc::SetSessionDescriptionObserver*>(observer);
}

RTC_EXPORT extern "C" bool webrtcSessionDescriptionInterfaceToString(
    const void* desc, void** out) {
  if (!out) {
    return false;
  }

  auto s = new std::string();
  *out = s;
  return static_cast<const webrtc::SessionDescriptionInterface*>(desc)->ToString(s);
}

RTC_EXPORT extern "C" void webrtcSetSessionDescriptionObserverRelease(
    const void* observer) {
  static_cast<const webrtc::SetSessionDescriptionObserver*>(observer)->Release();
}
