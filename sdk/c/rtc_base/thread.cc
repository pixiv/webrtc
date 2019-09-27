/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/rtc_base/thread.h"

extern "C" RtcThread* rtcCreateThread() {
  return rtc::ToC(rtc::Thread::Create().release());
}

extern "C" RtcThread* rtcMessageQueueToRtcThread(RtcMessageQueue* queue) {
  return rtc::ToC(static_cast<rtc::Thread*>(rtc::ToCplusplus(queue)));
}

extern "C" void rtcThreadRun(RtcThread* thread) {
  rtc::ToCplusplus(thread)->Run();
}

extern "C" void rtcThreadStart(RtcThread* thread) {
  rtc::ToCplusplus(thread)->Start();
}

extern "C" RtcThreadManager* rtcThreadManagerInstance() {
  return rtc::ToC(rtc::ThreadManager::Instance());
}

extern "C" void rtcThreadManagerUnwrapCurrentThread(RtcThreadManager* manager) {
  rtc::ToCplusplus(manager)->UnwrapCurrentThread();
}

extern "C" RtcThread* rtcThreadManagerWrapCurrentThread(
    RtcThreadManager* manager) {
  return rtc::ToC(rtc::ToCplusplus(manager)->WrapCurrentThread());
}

extern "C" RtcMessageQueue* rtcThreadToRtcMessageQueue(RtcThread* thread) {
  return rtc::ToC(static_cast<rtc::MessageQueue*>(rtc::ToCplusplus(thread)));
}
