/*
 *  Copyright (c) 2015 The WebRTC project authors. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree. An additional intellectual property rights grant can be found
 *  in the file PATENTS.  All contributing project authors may
 *  be found in the AUTHORS file in the root of the source tree.
 */

#include "sdk/c/api/video/video_rotation.h"

#ifndef API_VIDEO_VIDEO_ROTATION_H_
#define API_VIDEO_VIDEO_ROTATION_H_

namespace webrtc {

// enum for clockwise rotation.
enum VideoRotation {
  kVideoRotation_0 = WEBRTC_VIDEO_RATION_0,
  kVideoRotation_90 = WEBRTC_VIDEO_RATION_90,
  kVideoRotation_180 = WEBRTC_VIDEO_RATION_180,
  kVideoRotation_270 = WEBRTC_VIDEO_RATION_270
};

}  // namespace webrtc

#endif  // API_VIDEO_VIDEO_ROTATION_H_
