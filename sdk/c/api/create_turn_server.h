/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/interop.h"
#include "sdk/c/rtc_base/thread.h"

#ifdef __cplusplus
extern "C" {
#endif

RTC_EXPORT void webrtcCreateTURNServer(
    RtcThread* network_thread,
    const char* local_addr,
    const char* ip_addr,
    size_t min_port,
    size_t max_port);

#ifdef __cplusplus
}
#endif

