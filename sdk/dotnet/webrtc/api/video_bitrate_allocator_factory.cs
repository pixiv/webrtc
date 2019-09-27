/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

namespace Pixiv.Webrtc
{
    public interface IDisposableVideoBitrateAllocatorFactory :
        Rtc.IDisposable, IVideoBitrateAllocatorFactory
    {
    }

    public interface IVideoBitrateAllocatorFactory
    {
        System.IntPtr Ptr { get; }
    }
}
