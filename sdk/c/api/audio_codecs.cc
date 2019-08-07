#include "api/audio_codecs/builtin_audio_decoder_factory.h"
#include "api/audio_codecs/builtin_audio_encoder_factory.h"

RTC_EXPORT extern "C" void webrtcAudioDecoderFactoryRelease(
    const void* factory) {
  static_cast<const webrtc::AudioDecoderFactory*>(factory)->Release();
}

RTC_EXPORT extern "C" void webrtcAudioEncoderFactoryRelease(
    const void* factory) {
  static_cast<const webrtc::AudioEncoderFactory*>(factory)->Release();
}

RTC_EXPORT extern "C" void* webrtcCreateBuiltinAudioDecoderFactory() {
  return webrtc::CreateBuiltinAudioDecoderFactory().release();
}

RTC_EXPORT extern "C" void* webrtcCreateBuiltinAudioEncoderFactory() {
  return webrtc::CreateBuiltinAudioEncoderFactory().release();
}
