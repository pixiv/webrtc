/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

namespace Pixiv.Rtc
{
    public sealed class VideoSinkWants
    {
        public bool RotationApplied { get; set; } = false;
        public bool BlackFrames { get; set; } = false;
        public int MaxPixelCount { get; set; } = int.MaxValue;
        public int? TargetPixelCount { get; set; }
        public int MaxFramerateFps { get; set; } = int.MaxValue;
    }
}
