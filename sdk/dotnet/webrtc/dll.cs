/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

namespace Pixiv.Webrtc
{
    internal static class Dll
    {
#if WEBRTC_INTERNAL
        public const string Name = "__Internal";
#else
        public const string Name = "jingle_peerconnection_so";
#endif
    }
}
