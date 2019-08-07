using Rtc;
using System;

namespace Webrtc
{
    public static class PeerConnectionFactory
    {
        [System.Runtime.InteropServices.DllImport("webrtc_c")]
        private static extern IntPtr webrtcCreatePeerConnectionFactory(
            IntPtr networkThread,
            IntPtr workerThread,
            IntPtr signalingThread,
            IntPtr defaultAdm,
            IntPtr audioEncoderFactory,
            IntPtr audioDecoderFactory,
            IntPtr videoEncoderFactory,
            IntPtr videoDecoderFactory,
            IntPtr audioMixer,
            IntPtr audioProcessing);

        public static PeerConnectionFactoryInterface Create(
            Thread networkThread,
            Thread workerThread,
            Thread signalingThread,
            IntPtr defaultAdm,
            AudioEncoderFactory audioEncoderFactory,
            AudioDecoderFactory audioDecoderFactory,
            VideoEncoderFactory videoEncoderFactory,
            VideoDecoderFactory videoDecoderFactory,
            IntPtr audioMixer,
            IntPtr audioProcessing)
        {
            return new PeerConnectionFactoryInterface(
                webrtcCreatePeerConnectionFactory(
                    networkThread.Ptr,
                    workerThread.Ptr,
                    signalingThread.Ptr,
                    defaultAdm,
                    audioEncoderFactory.Ptr,
                    audioDecoderFactory.Ptr,
                    videoEncoderFactory.Ptr,
                    videoDecoderFactory.Ptr,
                    audioMixer,
                    audioProcessing
                )
            );
        }
    }
}
