/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */

#include "api/create_peerconnection_factory.h"
#include "api/video_codecs/video_decoder_factory.h"
#include "api/video_codecs/video_encoder_factory.h"
#include "sdk/c/api/create_peerconnection_factory.h"

extern "C" WebrtcPeerConnectionFactoryInterface*
webrtcCreatePeerConnectionFactory(
    RtcThread* network_thread,
    RtcThread* worker_thread,
    RtcThread* signaling_thread,
    WebrtcAudioDeviceModule* default_adm,
    WebrtcAudioEncoderFactory* audio_encoder_factory,
    WebrtcAudioDecoderFactory* audio_decoder_factory,
    WebrtcVideoEncoderFactory* video_encoder_factory,
    WebrtcVideoDecoderFactory* video_decoder_factory,
    WebrtcAudioMixer* audio_mixer,
    WebrtcAudioProcessing* audio_processing) {
  return rtc::ToC(
      webrtc::CreatePeerConnectionFactory(
          rtc::ToCplusplus(network_thread), rtc::ToCplusplus(worker_thread),
          rtc::ToCplusplus(signaling_thread), rtc::ToCplusplus(default_adm),
          rtc::ToCplusplus(audio_encoder_factory),
          rtc::ToCplusplus(audio_decoder_factory),
          std::unique_ptr<webrtc::VideoEncoderFactory>(
              rtc::ToCplusplus(video_encoder_factory)),
          std::unique_ptr<webrtc::VideoDecoderFactory>(
              rtc::ToCplusplus(video_decoder_factory)),
          rtc::ToCplusplus(audio_mixer), rtc::ToCplusplus(audio_processing))
          .release());
}
