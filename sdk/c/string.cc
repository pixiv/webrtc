#include <string>
#include "rtc_base/system/rtc_export.h"

RTC_EXPORT extern "C" void webrtcDeleteString(void* s) {
  delete static_cast<std::string*>(s);
}

RTC_EXPORT extern "C" const char* webrtcString_c_str(const void* s) {
  return static_cast<const std::string*>(s)->c_str();
}
