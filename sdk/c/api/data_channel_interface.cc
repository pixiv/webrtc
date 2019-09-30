/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/data_channel_interface.h"

RTC_EXPORT extern "C" void webrtcDataChannelInterfaceRelease(
    const WebrtcDataChannelInterface* channel) {
  rtc::ToCplusplus(channel)->Release();
}
