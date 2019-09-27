/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_RTC_BASE_MESSAGE_QUEUE_H_
#define SDK_C_RTC_BASE_MESSAGE_QUEUE_H_

#include "sdk/c/interop.h"

#ifdef __cplusplus
#include "rtc_base/message_queue.h"

extern "C" {
#endif

RTC_C_CLASS(rtc::MessageQueue, RtcMessageQueue)

RTC_EXPORT void rtcDeleteMessageQueue(RtcMessageQueue* queue);

RTC_EXPORT void rtcMessageQueueQuit(RtcMessageQueue* queue);

#ifdef __cplusplus
}
#endif

#endif
