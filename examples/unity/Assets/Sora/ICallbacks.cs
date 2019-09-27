/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;

namespace Pixiv.Sora
{
    public interface ICallbacks
    {
        void Connect(
            IPeerConnectionFactoryInterface factory,
            IPeerConnectionInterface connection
        );

        void Disconnect();
        void Notify();
        void OnException(System.Exception exception);
        void OnFailure(RtcError error);
        void OnRemoveTrack(DisposableMediaStreamTrackInterface track);
        void OnTrack(DisposableMediaStreamTrackInterface track);
        void Push();
    }
}
