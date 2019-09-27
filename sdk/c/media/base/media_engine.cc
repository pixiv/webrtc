/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/media/base/media_engine.h"

extern "C" void cricketDeleteMediaEngineInterface(
    CricketMediaEngineInterface* engine) {
  delete rtc::ToCplusplus(engine);
}
