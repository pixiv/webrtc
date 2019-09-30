/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/modules/audio_processing.h"

extern "C" WebrtcAudioProcessing* webrtcAudioProcessingBuilderCreate(
    WebrtcAudioProcessingBuilder* builder) {
  auto processing = rtc::ToCplusplus(builder)->Create();
  processing->AddRef();
  return rtc::ToC(processing);
}

extern "C" void webrtcReleaseAudioProcessing(
    WebrtcAudioProcessing* processing) {
  rtc::ToCplusplus(processing)->Release();
}

extern "C" void webrtcDeleteAudioProcessingBuilder(
    WebrtcAudioProcessingBuilder* builder) {
  delete rtc::ToCplusplus(builder);
}

extern "C" WebrtcAudioProcessingBuilder* webrtcNewAudioProcessingBuilder() {
  return rtc::ToC(new webrtc::AudioProcessingBuilder());
}
