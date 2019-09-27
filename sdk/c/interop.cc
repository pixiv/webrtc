/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/interop.h"

extern "C" void rtcDeleteString(RtcString* s) {
  delete rtc::ToCplusplus(s);
}

extern "C" const char* rtcString_c_str(const RtcString* s) {
  return rtc::ToCplusplus(s)->c_str();
}
