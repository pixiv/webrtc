#include "api/video_codecs/builtin_video_decoder_factory.h"
#include "api/video_codecs/builtin_video_encoder_factory.h"

RTC_EXPORT extern "C" void* webrtcCreateBuiltinVideoDecoderFactory() {
    return webrtc::CreateBuiltinVideoDecoderFactory().release();
}

RTC_EXPORT extern "C" void* webrtcCreateBuiltinVideoEncoderFactory() {
    return webrtc::CreateBuiltinVideoEncoderFactory().release();
}
