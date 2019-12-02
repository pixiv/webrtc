/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include <AudioUnit/AudioUnit.h>

#import "RTCAudioDeviceModule+Private.h"
#include "rtc_base/ref_counted_object.h"

@implementation RTCAudioDeviceModule {
#if defined(WEBRTC_IOS)
  rtc::scoped_refptr<webrtc::ios_adm::AudioDeviceModuleIOS> _nativeModule;
#endif
}

- (instancetype)init {
#if defined(WEBRTC_IOS)
  self = [super init];
  _nativeModule = new rtc::RefCountedObject<webrtc::ios_adm::AudioDeviceModuleIOS>();
  return self;
#else
  return nullptr;
#endif
}

- (void)deliverRecordedData:(CMSampleBufferRef)sampleBuffer {
#if defined(WEBRTC_IOS)
  _nativeModule->OnDeliverRecordedExternalData(sampleBuffer);
#endif
}

- (void)setAudioUnitSubType:(OSType)audioUnitSubType {
#if defined(WEBRTC_IOS)
  _nativeModule->SetAudioUnitSubType(audioUnitSubType);
#endif
}

- (OSType)audioUnitSubType {
#if defined(WEBRTC_IOS)
  return _nativeModule->GetAudioUnitSubType();
#else
  return kAudioUnitSubType_VoiceProcessingIO;
#endif
}

#pragma mark - Private

#if defined(WEBRTC_IOS)
- (rtc::scoped_refptr<webrtc::ios_adm::AudioDeviceModuleIOS>)nativeModule {
  return _nativeModule;
}
#endif

@end
