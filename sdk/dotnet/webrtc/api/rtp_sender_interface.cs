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
    public interface IDisposableRtpSenderInterface :
        IRtpSenderInterface, Rtc.IDisposable
    {
    }

    public interface IRtpSenderInterface
    {
        IntPtr Ptr { get; }
    }

    public sealed class DisposableRtpSenderInterface :
        Rtc.DisposablePtr, IDisposableRtpSenderInterface
    {
        IntPtr IRtpSenderInterface.Ptr => Ptr;

        public DisposableRtpSenderInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.RtpSenderInterface.Release(Ptr);
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class RtpSenderInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcRtpSenderInterfaceRelease")]
        public static extern void Release(IntPtr ptr);
    }
}
