#include "api/create_peerconnection_factory.h"
#include "api/video_codecs/video_decoder_factory.h"
#include "api/video_codecs/video_encoder_factory.h"

RTC_EXPORT extern "C" void* webrtcCreatePeerConnectionFactory(
    void* network_thread,
    void* worker_thread,
    void* signaling_thread,
    void* default_adm,
    void* audio_encoder_factory,
    void* audio_decoder_factory,
    void* video_encoder_factory,
    void* video_decoder_factory,
    void* audio_mixer,
    void* audio_processing) {
  return webrtc::CreatePeerConnectionFactory(
    static_cast<rtc::Thread*>(network_thread),
    static_cast<rtc::Thread*>(worker_thread),
    static_cast<rtc::Thread*>(signaling_thread),
    static_cast<webrtc::AudioDeviceModule*>(default_adm),
    static_cast<webrtc::AudioEncoderFactory*>(audio_encoder_factory),
    static_cast<webrtc::AudioDecoderFactory*>(audio_decoder_factory),
    std::unique_ptr<webrtc::VideoEncoderFactory>(
      static_cast<webrtc::VideoEncoderFactory*>(video_encoder_factory)
    ),
    std::unique_ptr<webrtc::VideoDecoderFactory>(
      static_cast<webrtc::VideoDecoderFactory*>(video_decoder_factory)
    ),
    static_cast<webrtc::AudioMixer*>(audio_mixer),
    static_cast<webrtc::AudioProcessing*>(audio_processing)).release();
}
