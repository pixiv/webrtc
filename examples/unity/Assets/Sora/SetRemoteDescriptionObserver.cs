/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;

namespace Pixiv.Sora
{
    internal sealed class SetRemoteDescriptionObserver : IManagedSetSessionDescriptionObserver
    {
        public Connection Connection;

        public void OnSuccess()
        {
        }

        public void OnFailure(RtcError error)
        {
            Connection.Callbacks.OnFailure(error);
            Connection.Stop();
        }
    }
}
