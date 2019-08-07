#include "api/rtp_transceiver_interface.h"

RTC_EXPORT extern "C" void webrtcRtpTransceiverInterfaceRelease(
    const void* transceiver) {
  static_cast<const webrtc::RtpTransceiverInterface*>(transceiver)->Release();
}

RTC_EXPORT extern "C" void* webrtcRtpTransceiverInterfaceReceiver(
    const void* transceiver) {
  return static_cast<const webrtc::RtpTransceiverInterface*>(transceiver)->receiver().release();
}
