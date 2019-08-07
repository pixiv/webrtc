#include "api/rtp_sender_interface.h"

RTC_EXPORT extern "C" void webrtcRtpSenderInterfaceRelease(const void* sender) {
  static_cast<const webrtc::RtpSenderInterface*>(sender)->Release();
}
