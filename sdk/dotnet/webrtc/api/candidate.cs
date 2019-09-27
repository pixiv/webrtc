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
    public interface IDisposableCandidate : Rtc.IDisposable, ICandidate
    {
    }

    public interface ICandidate
    {
        IntPtr Ptr { get; }
    }

    public sealed class DisposableCandidate :
        Rtc.DisposablePtr, IDisposableCandidate
    {
        IntPtr ICandidate.Ptr => Ptr;

        public DisposableCandidate(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.Candidate.Delete(Ptr);
        }
    }
}

namespace Pixiv.Cricket.Interop
{
    public static class Candidate
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cricketDeleteCandidate")]
        public static extern void Delete(IntPtr ptr);
    }
}
