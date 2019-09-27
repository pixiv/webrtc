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
    public interface IDataChannelInterface
    {
        IntPtr Ptr { get; }
    }

    public interface IDisposableDataChannelInterface :
        IDataChannelInterface, Rtc.IDisposable
    {
    }

    public sealed class DisposableDataChannelInterface :
        Rtc.DisposablePtr, IDisposableDataChannelInterface
    {
        IntPtr IDataChannelInterface.Ptr => Ptr;

        internal DisposableDataChannelInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.DataChannelInterface.Release(Ptr);
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class DataChannelInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDataChannelInterfaceRelease")]
        public static extern void Release(IntPtr ptr);
    }
}
