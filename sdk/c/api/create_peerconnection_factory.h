/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_CREATE_PEERCONNECTION_FACTORY_H_
#define SDK_C_CREATE_PEERCONNECTION_FACTORY_H_

#include "sdk/c/api/audio.h"
#include "sdk/c/api/audio_codecs.h"
#include "sdk/c/api/peer_connection_interface.h"
#include "sdk/c/api/video_codecs.h"
#include "sdk/c/modules/audio_device.h"
#include "sdk/c/modules/audio_processing.h"
#include "sdk/c/rtc_base/thread.h"

#ifdef __cplusplus
extern "C" {
#endif

RTC_EXPORT WebrtcPeerConnectionFactoryInterface*
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
    WebrtcAudioProcessing* audio_processing);

#ifdef __cplusplus
}
#endif

#endif
