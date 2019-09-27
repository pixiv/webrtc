/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/rtc_base/message_queue.h"

extern "C" void rtcDeleteMessageQueue(RtcMessageQueue* queue) {
  delete rtc::ToCplusplus(queue);
}

extern "C" void rtcMessageQueueQuit(RtcMessageQueue* queue) {
  rtc::ToCplusplus(queue)->Quit();
}
