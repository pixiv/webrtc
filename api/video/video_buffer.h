/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef API_VIDEO_VIDEO_BUFFER_H_
#define API_VIDEO_VIDEO_BUFFER_H_

#include "api/video/video_frame.h"
#include "api/video/video_sink_interface.h"
#include "rtc_base/system/rtc_export.h"

namespace webrtc {

class RTC_EXPORT VideoBufferInterface :
    public rtc::VideoSinkInterface<VideoFrame> {
  public:
    virtual std::unique_ptr<VideoFrame> MoveFrame() = 0;
};

RTC_EXPORT std::unique_ptr<VideoBufferInterface> CreateVideoBuffer();

}

#endif  // API_VIDEO_VIDEO_BUFFER_H_
