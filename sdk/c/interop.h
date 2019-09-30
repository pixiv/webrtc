/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_INTEROP_H_
#define SDK_C_INTEROP_H_

#include "rtc_base/system/rtc_export.h"

#ifdef __cplusplus
#include <string>

#if __has_attribute(unused)
#define RTC_C_CLASS_HELPER __attribute__((unused))
#else
#define RTC_C_CLASS_HELPER
#endif

#define RTC_C_CLASS(Cplusplus, C)                                             \
  union C {                                                                   \
   private:                                                                   \
    char value;                                                               \
  };                                                                          \
  namespace rtc {                                                             \
  RTC_C_CLASS_HELPER static inline C* ToC(Cplusplus* cplusplus) {             \
    return reinterpret_cast<C*>(reinterpret_cast<char*>((cplusplus)));        \
  }                                                                           \
  RTC_C_CLASS_HELPER static inline Cplusplus* ToCplusplus(C* c) {             \
    return reinterpret_cast<Cplusplus*>(reinterpret_cast<char*>(c));          \
  }                                                                           \
  RTC_C_CLASS_HELPER static inline const C* ToC(const Cplusplus* cplusplus) { \
    auto charptr = reinterpret_cast<const char*>(cplusplus);                  \
    return reinterpret_cast<const C*>(charptr);                               \
  }                                                                           \
  RTC_C_CLASS_HELPER static inline const Cplusplus* ToCplusplus(const C* c) { \
    auto charptr = reinterpret_cast<const char*>(c);                          \
    return reinterpret_cast<const Cplusplus*>(charptr);                       \
  }                                                                           \
  }

#define RTC_C_CLASS_FORWARD(C) union C;

extern "C" {
#else
#define RTC_C_CLASS(Cplusplus, C) typedef union C C;
#define RTC_C_CLASS_FORWARD(C) typedef union C C;
#endif

RTC_C_CLASS(std::string, RtcString)

RTC_EXPORT void rtcDeleteString(RtcString* s);
RTC_EXPORT const char* rtcString_c_str(const RtcString* s);

#ifdef __cplusplus
}
#endif

#endif
