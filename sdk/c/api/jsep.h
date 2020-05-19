/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_JSEP_H_
#define SDK_C_API_JSEP_H_

#include "sdk/c/api/rtc_error.h"

enum WebrtcSdpType {
  WEBRTC_SDP_TYPE_OFFER,
  WEBRTC_SDP_TYPE_PR_ANSWER,
  WEBRTC_SDP_TYPE_ANSWER
};

enum WebrtcOptionalSdpType {
  WEBRTC_OPTIONAL_SDP_TYPE_OFFER = WEBRTC_SDP_TYPE_OFFER,
  WEBRTC_OPTIONAL_SDP_TYPE_PR_ANSWER = WEBRTC_SDP_TYPE_PR_ANSWER,
  WEBRTC_OPTIONAL_SDP_TYPE_ANSWER = WEBRTC_SDP_TYPE_ANSWER,
  WEBRTC_OPTIONAL_SDP_TYPE_NULLOPT
};

#ifdef __cplusplus
#include "api/jsep.h"

extern "C" {
#else
#include <stdbool.h>
#endif

RTC_C_CLASS(webrtc::SdpParseError, WebrtcSdpParseError)

RTC_C_CLASS(webrtc::IceCandidateInterface, WebrtcIceCandidateInterface)

RTC_C_CLASS(webrtc::SessionDescriptionInterface,
            WebrtcSessionDescriptionInterface)

RTC_C_CLASS(webrtc::CreateSessionDescriptionObserver,
            WebrtcCreateSessionDescriptionObserver)

RTC_C_CLASS(webrtc::SetSessionDescriptionObserver,
            WebrtcSetSessionDescriptionObserver)

struct WebrtcCreateSessionDescriptionObserverFunctions {
  void (*on_destruction)(void*);
  void (*on_success)(void*, WebrtcSessionDescriptionInterface*);
  void (*on_failure)(void*, enum WebrtcRTCErrorType, const char*);
};

struct WebrtcSetSessionDescriptionObserverFunctions {
  void (*on_destruction)(void*);
  void (*on_success)(void*);
  void (*on_failure)(void*, enum WebrtcRTCErrorType, const char*);
};

RTC_EXPORT WebrtcIceCandidateInterface* webrtcCreateIceCandidate(
    const char* sdp_mid,
    int sdp_mline_index,
    const char* sdp,
    WebrtcSdpParseError** error);

RTC_EXPORT WebrtcSessionDescriptionInterface* webrtcCreateSessionDescription(
    enum WebrtcSdpType type,
    const char* sdp,
    WebrtcSdpParseError** error);

RTC_EXPORT void webrtcCreateSessionDescriptionObserverRelease(
    const WebrtcCreateSessionDescriptionObserver* observer);

RTC_EXPORT void webrtcDeleteIceCandidateInterface(
    WebrtcIceCandidateInterface* candidate);

RTC_EXPORT void webrtcDeleteSessionDescriptionInterface(
    WebrtcSessionDescriptionInterface* desc);

RTC_EXPORT RtcString* webrtcIceCandidateInterfaceSdp_mid(
    const WebrtcIceCandidateInterface* candidate);

RTC_EXPORT int webrtcIceCandidateInterfaceSdp_mline_index(
    const WebrtcIceCandidateInterface* candidate);

RTC_EXPORT bool webrtcIceCandidateInterfaceToString(
    const WebrtcIceCandidateInterface* candidate,
    RtcString** out);

RTC_EXPORT WebrtcCreateSessionDescriptionObserver*
webrtcNewCreateSessionDescriptionObserver(
    void* context,
    const struct WebrtcCreateSessionDescriptionObserverFunctions* functions);

RTC_EXPORT WebrtcSetSessionDescriptionObserver*
webrtcNewSetSessionDescriptionObserver(
    void* context,
    const struct WebrtcSetSessionDescriptionObserverFunctions* functions);

RTC_EXPORT enum WebrtcOptionalSdpType
webrtcSdpTypeFromString(const char* type_str);

RTC_EXPORT const char* webrtcSdpTypeToString(enum WebrtcSdpType type);

RTC_EXPORT enum WebrtcSdpType webrtcSessionDescriptionInterfaceGetType(
    const WebrtcSessionDescriptionInterface* desc);

RTC_EXPORT bool webrtcSessionDescriptionInterfaceToString(
    const WebrtcSessionDescriptionInterface* desc,
    RtcString** out);

RTC_EXPORT void webrtcSetSessionDescriptionObserverRelease(
    const WebrtcSetSessionDescriptionObserver* observer);

#ifdef __cplusplus
}
#endif

#endif
