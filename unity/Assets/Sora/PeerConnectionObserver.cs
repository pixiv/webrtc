using System;
using Webrtc;

namespace Sora
{
    internal sealed class PeerConnectionObserver : IPeerConnectionObserver
    {
        public Connection Connection;

        public void OnSignalingChange(PeerConnectionInterface.SignalingState newState) { }

        public void OnAddStream(MediaStreamInterface stream)
        {
            stream.Dispose();
        }

        public void OnRemoveStream(MediaStreamInterface stream)
        {
            stream.Dispose();
        }

        public void OnDataChannel(DataChannelInterface dataChannel)
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

                var signal = new Signals.Candidate();
                signal.type = "candidate";
                signal.candidate = candidateString;

                Connection.SendSignalAsync(signal);
            }
            catch (Exception exception)
            {
                Connection.OnException(exception);
                Connection.Stop();
            }
        }

        public void OnIceCandidatesRemoved(IntPtr candidates) { }

        public void OnIceConnectionReceivingChange(bool receiving) { }

        public void OnAddTrack(RtpReceiverInterface receiver, MediaStreamInterface[] streams)
        {
            receiver.Dispose();

            foreach (var stream in streams)
            {
                stream.Dispose();
            }
        }

        public void OnTrack(RtpTransceiverInterface transceiver)
        {
            using (transceiver)
            {
                using (var receiver = transceiver.Receiver())
                {
                    Connection.Callbacks.OnTrack(receiver.Track());
                }
            }
        }

        public void OnRemoveTrack(RtpReceiverInterface receiver)
        {
            using (receiver)
            {
                Connection.Callbacks.OnRemoveTrack(receiver.Track());
            }
        }

        public void OnInterestingUsage(int usagePattern) { }
    }
}
