/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/audio_codecs.h"

extern "C" void webrtcAudioDecoderFactoryRelease(
    const WebrtcAudioDecoderFactory* factory) {
  rtc::ToCplusplus(factory)->Release();
}

extern "C" void webrtcAudioEncoderFactoryRelease(
    const WebrtcAudioEncoderFactory* factory) {
  rtc::ToCplusplus(factory)->Release();
}

extern "C" WebrtcAudioDecoderFactory* webrtcCreateBuiltinAudioDecoderFactory() {
  return rtc::ToC(webrtc::CreateBuiltinAudioDecoderFactory().release());
}

extern "C" WebrtcAudioEncoderFactory* webrtcCreateBuiltinAudioEncoderFactory() {
  return rtc::ToC(webrtc::CreateBuiltinAudioEncoderFactory().release());
}
