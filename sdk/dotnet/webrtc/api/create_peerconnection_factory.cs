/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Rtc;
using System;
using System.Runtime.InteropServices;

namespace Pixiv.Webrtc
{
    public static class PeerConnectionFactory
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
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

        public static DisposablePeerConnectionFactoryInterface Create(
            IThread networkThread,
            IThread workerThread,
            IThread signalingThread,
            IAudioDeviceModule defaultAdm,
            IAudioEncoderFactory audioEncoderFactory,
            IAudioDecoderFactory audioDecoderFactory,
            IDisposableVideoEncoderFactory videoEncoderFactory,
            IDisposableVideoDecoderFactory videoDecoderFactory,
            IAudioMixer audioMixer,
            IAudioProcessing audioProcessing)
        {
            IntPtr videoEncoderFactoryPtr;
            IntPtr videoDecoderFactoryPtr;

            if (videoEncoderFactory == null)
            {
                videoEncoderFactoryPtr = IntPtr.Zero;
            }
            else
            {
                videoEncoderFactoryPtr = videoEncoderFactory.Ptr;
                videoEncoderFactory.ReleasePtr();
            }

            if (videoDecoderFactory == null)
            {
                videoDecoderFactoryPtr = IntPtr.Zero;
            }
            else
            {
                videoDecoderFactoryPtr = videoDecoderFactory.Ptr;
                videoDecoderFactory.ReleasePtr();
            }

            var factory = webrtcCreatePeerConnectionFactory(
                networkThread == null ? IntPtr.Zero : networkThread.Ptr,
                workerThread == null ? IntPtr.Zero : workerThread.Ptr,
                signalingThread == null ? IntPtr.Zero : signalingThread.Ptr,
                defaultAdm == null ? IntPtr.Zero : defaultAdm.Ptr,
                audioEncoderFactory == null ?
                    IntPtr.Zero : audioEncoderFactory.Ptr,
                audioDecoderFactory == null ?
                    IntPtr.Zero : audioDecoderFactory.Ptr,
                videoEncoderFactoryPtr,
                videoDecoderFactoryPtr,
                audioMixer == null ? IntPtr.Zero : audioMixer.Ptr,
                audioProcessing == null ? IntPtr.Zero : audioProcessing.Ptr
            );

            GC.KeepAlive(networkThread);
            GC.KeepAlive(workerThread);
            GC.KeepAlive(signalingThread);
            GC.KeepAlive(defaultAdm);
            GC.KeepAlive(audioEncoderFactory);
            GC.KeepAlive(audioDecoderFactory);
            GC.KeepAlive(videoEncoderFactory);
            GC.KeepAlive(videoDecoderFactory);
            GC.KeepAlive(audioMixer);
            GC.KeepAlive(audioProcessing);

            return new DisposablePeerConnectionFactoryInterface(factory);
        }
    }
}
