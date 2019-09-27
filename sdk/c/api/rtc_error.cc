/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/rtc_error.h"

extern "C" void webrtcDeleteRTCError(WebrtcRTCError* error) {
  delete rtc::ToCplusplus(error);
}

extern "C" const char* webrtcRTCErrorMessage(const WebrtcRTCError* error) {
  return rtc::ToCplusplus(error)->message();
}

extern "C" WebrtcRTCErrorType webrtcRTCErrorType(const WebrtcRTCError* error) {
  return static_cast<WebrtcRTCErrorType>(rtc::ToCplusplus(error)->type());
}
