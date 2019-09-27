/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_MEDIA_BASE_MEDIA_ENGINE_H_
#define SDK_C_MEDIA_BASE_MEDIA_ENGINE_H_

#include "sdk/c/interop.h"

#ifdef __cplusplus
#include "media/base/media_engine.h"

extern "C" {
#endif

RTC_C_CLASS(cricket::MediaEngineInterface, CricketMediaEngineInterface)

RTC_EXPORT void cricketDeleteMediaEngineInterface(
    CricketMediaEngineInterface* engine);

#ifdef __cplusplus
}
#endif

#endif
