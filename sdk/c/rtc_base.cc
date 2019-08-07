#include "rtc_base/ref_count.h"
#include "rtc_base/system/rtc_export.h"
#include "rtc_base/thread.h"

RTC_EXPORT extern "C" void* rtcCreateThread() {
  return rtc::Thread::Create().release();
}

RTC_EXPORT extern "C" void rtcDeleteThread(void* thread) {
  delete static_cast<rtc::Thread*>(thread);
}

RTC_EXPORT extern "C" void rtcMessageQueueQuit(void* queue) {
  static_cast<rtc::MessageQueue*>(queue)->Quit();
}

RTC_EXPORT extern "C" void rtcThreadRun(void* thread) {
  static_cast<rtc::Thread*>(thread)->Run();
}

RTC_EXPORT extern "C" void rtcThreadStart(void* thread) {
  static_cast<rtc::Thread*>(thread)->Start();
}

RTC_EXPORT extern "C" void* rtcThreadManagerInstance() {
  return rtc::ThreadManager::Instance();
}

RTC_EXPORT extern "C" void* rtcThreadManagerWrapCurrentThread(void* instance) {
  return static_cast<rtc::ThreadManager*>(instance)->WrapCurrentThread();
}
