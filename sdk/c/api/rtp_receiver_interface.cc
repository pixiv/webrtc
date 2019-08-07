#include "api/rtp_receiver_interface.h"

RTC_EXPORT extern "C" void webrtcRtpReceiverInterfaceRelease(
    const void* receiver) {
  static_cast<const webrtc::RtpReceiverInterface*>(receiver)->Release();
}

RTC_EXPORT extern "C" void* webrtcRtpReceiverInterfaceTrack(
    const void* receiver) {
  return static_cast<const webrtc::RtpReceiverInterface*>(receiver)->track().release();
}
