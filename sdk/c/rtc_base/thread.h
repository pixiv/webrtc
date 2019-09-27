/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_RTC_BASE_THREAD_H_
#define SDK_C_RTC_BASE_THREAD_H_

#include "sdk/c/rtc_base/message_queue.h"

#ifdef __cplusplus
#include "rtc_base/thread.h"

extern "C" {
#endif

RTC_C_CLASS(rtc::Thread, RtcThread)
RTC_C_CLASS(rtc::ThreadManager, RtcThreadManager)

RTC_EXPORT RtcThread* rtcCreateThread(void);

RTC_EXPORT RtcThread* rtcMessageQueueToRtcThread(RtcMessageQueue* queue);

RTC_EXPORT void rtcThreadRun(RtcThread* thread);

RTC_EXPORT void rtcThreadStart(RtcThread* thread);

RTC_EXPORT RtcThreadManager* rtcThreadManagerInstance(void);

RTC_EXPORT void rtcThreadManagerUnwrapCurrentThread(RtcThreadManager* manager);

RTC_EXPORT RtcThread* rtcThreadManagerWrapCurrentThread(
    RtcThreadManager* manager);

RTC_EXPORT RtcMessageQueue* rtcThreadToRtcMessageQueue(RtcThread* thread);

#ifdef __cplusplus
}
#endif

#endif
