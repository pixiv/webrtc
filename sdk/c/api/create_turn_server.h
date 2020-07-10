/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/interop.h"

#ifdef __cplusplus
extern "C" {
#endif

RTC_EXPORT void webrtcCreateTURNServer(
    const char* local_addr,
	const char* ip_addr);

#ifdef __cplusplus
}
#endif

#endif
