/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Cricket;
using Pixiv.Webrtc;
using System;
using System.Runtime.Serialization.Json;

namespace Pixiv.Sora
{
    internal sealed class PeerConnectionObserver : IManagedPeerConnectionObserver
    {
        public Connection Connection;

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

        public void OnIceCandidate(IceCandidateInterface candidate)
        {
            try
            {
                if (!candidate.TryToString(out var candidateString))
                {
                    throw new ArgumentException(nameof(candidate));
                }

                var stream = new System.IO.MemoryStream();

                using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
                    stream,
                    System.Text.Encoding.UTF8,
                    false))
                {
                    writer.WriteStartElement("root");
                    writer.WriteAttributeString("type", "object");
                    writer.WriteElementString("type", "candidate");
                    writer.WriteElementString("candidate", candidateString);
                    writer.WriteEndElement();
                }

                Connection.SendSignalAsync(stream);
            }
            catch (Exception exception)
            {
                Connection.OnException(exception);
                Connection.Stop();
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

        public void OnAddTrack(DisposableRtpReceiverInterface receiver, DisposableMediaStreamInterface[] streams)
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
                    Connection.Callbacks.OnTrack(receiver.Track());
                }
            }
        }

        public void OnRemoveTrack(DisposableRtpReceiverInterface receiver)
        {
            using (receiver)
            {
                Connection.Callbacks.OnRemoveTrack(receiver.Track());
            }
        }

        public void OnInterestingUsage(int usagePattern) { }
    }
}
