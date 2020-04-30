/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using System;
using System.Runtime.InteropServices;

namespace Pixiv.Webrtc
{
    public interface IDisposableRtpTransceiverInterface :
        IRtpTransceiverInterface, Rtc.IDisposable
    {
    }

    public interface IRtpTransceiverInterface
    {
        IntPtr Ptr { get; }
    }

    public sealed class DisposableRtpTransceiverInterface :
        Rtc.DisposablePtr, IDisposableRtpTransceiverInterface
    {
        IntPtr IRtpTransceiverInterface.Ptr => Ptr;

        internal DisposableRtpTransceiverInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.RtpTransceiverInterface.Release(Ptr);
        }
    }

    public static class RtpTransceiverInterfaceExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcRtpTransceiverInterfaceReceiver(
            IntPtr ptr
        );

        public static DisposableRtpReceiverInterface Receiver(
            this IRtpTransceiverInterface transceiver)
        {
            if (transceiver == null)
            {
                throw new ArgumentNullException(nameof(transceiver));
            }

            var receiver = webrtcRtpTransceiverInterfaceReceiver(transceiver.Ptr);
            GC.KeepAlive(receiver);

            return new DisposableRtpReceiverInterface(receiver);
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class RtpTransceiverInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcRtpTransceiverInterfaceRelease")]
        public static extern void Release(IntPtr ptr);
    }
}
