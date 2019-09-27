/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/video_codecs.h"
#include "api/video_codecs/builtin_video_decoder_factory.h"
#include "api/video_codecs/builtin_video_encoder_factory.h"

extern "C" WebrtcVideoDecoderFactory* webrtcCreateBuiltinVideoDecoderFactory() {
  return rtc::ToC(webrtc::CreateBuiltinVideoDecoderFactory().release());
}

extern "C" WebrtcVideoEncoderFactory* webrtcCreateBuiltinVideoEncoderFactory() {
  return rtc::ToC(webrtc::CreateBuiltinVideoEncoderFactory().release());
}

extern "C" void webrtcDeleteVideoDecoderFactory(
    WebrtcVideoDecoderFactory* factory) {
  delete rtc::ToCplusplus(factory);
}

extern "C" void webrtcDeleteVideoEncoderFactory(
    WebrtcVideoEncoderFactory* factory) {
  delete rtc::ToCplusplus(factory);
}
