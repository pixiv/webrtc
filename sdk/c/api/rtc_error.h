/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_RTC_ERROR_H_
#define SDK_C_API_RTC_ERROR_H_

#include "sdk/c/interop.h"

enum WebrtcRTCErrorType {
  WEBRTC_RTC_ERROR_TYPE_NONE,
  WEBRTC_RTC_ERROR_TYPE_UNSUPPORTED_OPERATION,
  WEBRTC_RTC_ERROR_TYPE_UNSUPPORTED_PARAMETER,
  WEBRTC_RTC_ERROR_TYPE_INVALID_PARAMETER,
  WEBRTC_RTC_ERROR_TYPE_INVALID_RANGE,
  WEBRTC_RTC_ERROR_TYPE_SYNTAX_ERROR,
  WEBRTC_RTC_ERROR_TYPE_INVALID_STATE,
  WEBRTC_RTC_ERROR_TYPE_INVALID_MODIFICATION,
  WEBRTC_RTC_ERROR_TYPE_NETWORK_ERROR,
  WEBRTC_RTC_ERROR_TYPE_RESOURCE_EXHAUSTED,
  WEBRTC_RTC_ERROR_TYPE_INTERNAL_ERROR,
};

#ifdef __cplusplus
#include "api/rtc_error.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::RTCError, WebrtcRTCError)

RTC_EXPORT void webrtcDeleteRTCError(WebrtcRTCError* error);

RTC_EXPORT const char* webrtcRTCErrorMessage(const WebrtcRTCError* error);

RTC_EXPORT enum WebrtcRTCErrorType webrtcRTCErrorType(
    const WebrtcRTCError* error);

#ifdef __cplusplus
}
#endif

#endif
