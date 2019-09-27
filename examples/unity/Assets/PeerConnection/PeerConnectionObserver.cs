/*
 *  Copyright 2020 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Cricket;
using Pixiv.Webrtc;

namespace Pixiv.PeerConnection
{
    internal sealed class PeerConnectionObserver : IManagedPeerConnectionObserver
    {
        private readonly Connection _connection;
        public DisposablePeerConnectionInterface PeerConnection;

        public PeerConnectionObserver(Connection connection)
        {
            _connection = connection;
        }

        public void OnSignalingChange(PeerConnectionInterface.SignalingState newState) { }

        public void OnAddStream(DisposableMediaStreamInterface stream)
        {
            stream.Dispose();
        }

        public void OnRemoveStream(DisposableMediaStreamInterface stream)
        {
            stream.Dispose();
        }

        public void OnDataChannel(DisposableDataChannelInterface dataChannel)
        {
            dataChannel.Dispose();
        }

        public void OnRenegotiationNeeded() { }

        public void OnIceConnectionChange(PeerConnectionInterface.IceConnectionState newState) { }

        public void OnStandardizedIceConnectionChange(PeerConnectionInterface.IceConnectionState newState) { }

        public void OnConnectionChange() { }

        public void OnIceGatheringChange(PeerConnectionInterface.IceGatheringState newState) { }

        public async void OnIceCandidate(IceCandidateInterface candidate)
        {
            try
            {
                await _connection.ProvideIceCandidate(
                    PeerConnection,
                    candidate
                );
            }
            catch (System.Exception exception)
            {
                _connection.Callbacks.OnException(exception);
            }
        }

        public void OnIceCandidatesRemoved(DisposableCandidate[] candidates)
        {
            foreach (var candidate in candidates)
            {
                candidate.Dispose();
            }
        }

        public void OnIceConnectionReceivingChange(bool receiving) { }

        public void OnAddTrack(
            DisposableRtpReceiverInterface receiver,
            DisposableMediaStreamInterface[] streams
        )
        {
            receiver.Dispose();

            foreach (var stream in streams)
            {
                stream.Dispose();
            }
        }

        public void OnTrack(DisposableRtpTransceiverInterface transceiver)
        {
            using (transceiver)
            {
                using (var receiver = transceiver.Receiver())
                {
                    _connection.Callbacks.OnTrack(receiver.Track());
                }
            }
        }

        public void OnRemoveTrack(DisposableRtpReceiverInterface receiver)
        {
            using (receiver)
            {
                _connection.Callbacks.OnRemoveTrack(receiver.Track());
            }
        }

        public void OnInterestingUsage(int usagePattern) { }
    }
}
