#include "api/rtc_error.h"

RTC_EXPORT extern "C" void webrtcDeleteRTCError(void* error) {
  delete static_cast<webrtc::RTCError*>(error);
}

RTC_EXPORT extern "C" const char* webrtcRTCErrorMessage(const void* error) {
  return static_cast<const webrtc::RTCError*>(error)->message();
}

RTC_EXPORT extern "C" webrtc::RTCErrorType webrtcRTCErrorType(
    const void* error) {
  return static_cast<const webrtc::RTCError*>(error)->type();
}
