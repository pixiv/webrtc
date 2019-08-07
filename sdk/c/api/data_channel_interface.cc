#include "api/data_channel_interface.h"
#include "rtc_base/system/rtc_export.h"

RTC_EXPORT extern "C" void webrtcDataChannelInterfaceRelease(
    const void* channel) {
  static_cast<const webrtc::DataChannelInterface*>(channel)->Release();
}
