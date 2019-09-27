/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_DATA_CHANNEL_INTERFACE_H_
#define SDK_C_API_DATA_CHANNEL_INTERFACE_H_

#include "sdk/c/interop.h"

#ifdef __cplusplus
#include "api/data_channel_interface.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::DataChannelInterface, WebrtcDataChannelInterface)

RTC_EXPORT void webrtcDataChannelInterfaceRelease(
    const WebrtcDataChannelInterface* channel);

#ifdef __cplusplus
}
#endif

#endif
