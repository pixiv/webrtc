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

    public static class RtpSenderInterfaceExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcDeleteRtpSenderInterfaceStreamIds(
            IntPtr ids
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcRtpSenderInterfaceStream_ids(
            IntPtr sender
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcRtpSenderInterfaceTrack(
            IntPtr sender
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcRtpSenderInterfaceStreamIdsData(
            IntPtr ids,
            IntPtr data
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern UIntPtr webrtcRtpSenderInterfaceStreamIdsSize(
            IntPtr ids
        );

        public static string[] StreamIds(this IRtpSenderInterface sender)
        {
            var ids = webrtcRtpSenderInterfaceStream_ids(sender.Ptr);
            try
            {
                GC.KeepAlive(sender);

                var left = (long)webrtcRtpSenderInterfaceStreamIdsSize(ids);
                var data = new IntPtr[left];
                var results = new string[left];

                var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                try
                {
                    var addr = handle.AddrOfPinnedObject();
                    webrtcRtpSenderInterfaceStreamIdsData(ids, addr);
                }
                finally
                {
                    handle.Free();
                }

                while (left > 0)
                {
                    left--;
                    results[left] = Marshal.PtrToStringAnsi(data[left]);
                }

                return results;
            }
            finally
            {
                webrtcDeleteRtpSenderInterfaceStreamIds(ids);
            }
        }

        public static DisposableMediaStreamTrackInterface Track(
            this IRtpSenderInterface sender
        )
        {
            var ptr = webrtcRtpSenderInterfaceTrack(sender.Ptr);
            GC.KeepAlive(sender);
            return Interop.MediaStreamTrackInterface.WrapDisposable(ptr);
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
