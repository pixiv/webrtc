/*
 *  Copyright 2020 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;

namespace Pixiv.PeerConnection
{
    internal sealed class DummySetSessionDescriptionObserver : IManagedSetSessionDescriptionObserver
    {
        private readonly ICallbacks _callbacks;

        public DummySetSessionDescriptionObserver(Connection connection)
        {
            _callbacks = connection.Callbacks;
        }

        public void OnSuccess()
        {
        }

        public void OnFailure(RtcError error)
        {
            _callbacks.OnFailure(error);
        }
    }
}
