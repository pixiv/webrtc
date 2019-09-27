/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */

using System;
using System.Runtime.InteropServices;

namespace Pixiv.Webrtc
{
    public interface IDisposableRtpReceiverInterface
        : IRtpReceiverInterface, Rtc.IDisposable
    {
    }

    public interface IRtpReceiverInterface
    {
        IntPtr Ptr { get; }
    }

    public sealed class DisposableRtpReceiverInterface :
        Rtc.DisposablePtr, IDisposableRtpReceiverInterface
    {
        IntPtr IRtpReceiverInterface.Ptr => Ptr;

        public DisposableRtpReceiverInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.RtpReceiverInterface.Release(Ptr);
        }
    }

    public static class RtpReceiverInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcRtpReceiverInterfaceTrack(
            IntPtr ptr
        );

        public static DisposableMediaStreamTrackInterface Track(
            this IRtpReceiverInterface receiver)
        {
            return Interop.MediaStreamTrackInterface.WrapDisposable(
                webrtcRtpReceiverInterfaceTrack(receiver.Ptr));
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class RtpReceiverInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcRtpReceiverInterfaceRelease")]
        public static extern void Release(IntPtr ptr);
    }
}
