/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#include "sdk/c/api/candidate.h"

extern "C" void cricketDeleteCandidate(CricketCandidate* candidate) {
  delete rtc::ToCplusplus(candidate);
}
