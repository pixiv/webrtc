/*
 *  Copyright 2020 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;

namespace Pixiv.PeerConnection
{
    internal sealed class CreateSessionDescriptionObserver : IManagedCreateSessionDescriptionObserver
    {
        private readonly Connection _connection;
        private readonly DisposablePeerConnectionInterface _peerConnection;

        public CreateSessionDescriptionObserver(
            Connection connection,
            DisposablePeerConnectionInterface peerConnection
        )
        {
            _connection = connection;
            _peerConnection = peerConnection;
        }

        public async void OnSuccess(DisposableSessionDescriptionInterface desc)
        {
            try
            {
                await _connection.SetLocalSessionDescriptionAsync(
                    _peerConnection,
                    desc
                );
            }
            catch (System.Exception exception)
            {
                _connection.Callbacks.OnException(exception);
            }
            finally
            {
                desc.Dispose();
            }
        }

        public void OnFailure(RtcError error)
        {
            _connection.Callbacks.OnFailure(error);
        }
    }
}
