/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_CANDIDATE_H_
#define SDK_C_API_CANDIDATE_H_

#include "sdk/c/interop.h"

#ifdef __cplusplus
#include "api/candidate.h"

extern "C" {
#endif

RTC_C_CLASS(cricket::Candidate, CricketCandidate)
RTC_EXPORT void cricketDeleteCandidate(CricketCandidate* candidate);

#ifdef __cplusplus
}
#endif

#endif
