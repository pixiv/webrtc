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
  rtc::scoped_refptr<webrtc::ios_adm::AudioDeviceModuleIOS> _nativeModule;
}

- (instancetype)init {
  self = [super init];
  _nativeModule = new rtc::RefCountedObject<webrtc::ios_adm::AudioDeviceModuleIOS>();
  return self;
}

- (void)deliverRecordedData:(CMSampleBufferRef)sampleBuffer {
  _nativeModule->OnDeliverRecordedExternalData(sampleBuffer);
}

#pragma mark - Private

- (rtc::scoped_refptr<webrtc::ios_adm::AudioDeviceModuleIOS>)nativeModule {
  return _nativeModule;
}

@end
