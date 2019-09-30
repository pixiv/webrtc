/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_VIDEO_CODECS_H_
#define SDK_C_API_VIDEO_CODECS_H_

#include "sdk/c/interop.h"

#ifdef __cplusplus
#include "api/video_codecs/video_decoder_factory.h"
#include "api/video_codecs/video_encoder_factory.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::VideoDecoderFactory, WebrtcVideoDecoderFactory)
RTC_C_CLASS(webrtc::VideoEncoderFactory, WebrtcVideoEncoderFactory)

RTC_EXPORT WebrtcVideoDecoderFactory* webrtcCreateBuiltinVideoDecoderFactory(
    void);

RTC_EXPORT WebrtcVideoEncoderFactory* webrtcCreateBuiltinVideoEncoderFactory(
    void);

RTC_EXPORT void webrtcDeleteVideoDecoderFactory(
    WebrtcVideoDecoderFactory* factory);

RTC_EXPORT void webrtcDeleteVideoEncoderFactory(
    WebrtcVideoEncoderFactory* factory);

#ifdef __cplusplus
}
#endif

#endif
