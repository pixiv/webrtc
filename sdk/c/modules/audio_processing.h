/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_MODULES_AUDIO_PROCESSING_H_
#define SDK_C_MODULES_AUDIO_PROCESSING_H_

#include "sdk/c/interop.h"

#ifdef __cplusplus
#include "modules/audio_processing/include/audio_processing.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::AudioProcessing, WebrtcAudioProcessing)
RTC_C_CLASS(webrtc::AudioProcessingBuilder, WebrtcAudioProcessingBuilder)

RTC_EXPORT WebrtcAudioProcessing* webrtcAudioProcessingBuilderCreate(
    WebrtcAudioProcessingBuilder* builder);

RTC_EXPORT void webrtcReleaseAudioProcessing(WebrtcAudioProcessing* processing);

RTC_EXPORT void webrtcDeleteAudioProcessingBuilder(
    WebrtcAudioProcessingBuilder* builder);

RTC_EXPORT WebrtcAudioProcessingBuilder* webrtcNewAudioProcessingBuilder(void);

#ifdef __cplusplus
}
#endif

#endif
