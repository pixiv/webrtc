/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_AUDIO_CODECS_H_
#define SDK_C_API_AUDIO_CODECS_H_

#include "sdk/c/interop.h"

#ifdef __cplusplus
#include "api/audio_codecs/builtin_audio_decoder_factory.h"
#include "api/audio_codecs/builtin_audio_encoder_factory.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::AudioDecoderFactory, WebrtcAudioDecoderFactory)
RTC_C_CLASS(webrtc::AudioEncoderFactory, WebrtcAudioEncoderFactory)

RTC_EXPORT void webrtcAudioDecoderFactoryRelease(
    const WebrtcAudioDecoderFactory* factory);

RTC_EXPORT void webrtcAudioEncoderFactoryRelease(
    const WebrtcAudioEncoderFactory* factory);

RTC_EXPORT WebrtcAudioDecoderFactory* webrtcCreateBuiltinAudioDecoderFactory(
    void);

RTC_EXPORT WebrtcAudioEncoderFactory* webrtcCreateBuiltinAudioEncoderFactory(
    void);

#ifdef __cplusplus
}
#endif

#endif
