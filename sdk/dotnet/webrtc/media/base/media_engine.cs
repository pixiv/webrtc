/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;
using System;
using System.Runtime.InteropServices;

namespace Pixiv.Cricket
{
    public interface IDisposableMediaEngineInterface :
        Rtc.IDisposable, IMediaEngineInterface
    {
    }

    public interface IMediaEngineInterface
    {
        IntPtr Ptr { get; }
    }

    public sealed class DisposableMediaEngineInterface :
        Rtc.DisposablePtr, IDisposableMediaEngineInterface
    {
        IntPtr IMediaEngineInterface.Ptr => Ptr;

        public DisposableMediaEngineInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.MediaEngineInterface.Delete(Ptr);
        }
    }
}

namespace Pixiv.Cricket.Interop
{
    public static class MediaEngineInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cricketDeleteMediaEngineInterface")]
        public static extern void Delete(IntPtr ptr);
    }
}
