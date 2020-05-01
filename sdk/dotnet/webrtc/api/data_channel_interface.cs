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
    public interface IDataChannelInterface
    {
        IntPtr Ptr { get; }
    }

    public interface IDisposableDataChannelInterface :
        IDataChannelInterface, Rtc.IDisposable
    {
    }

    public sealed class DisposableDataChannelInterface :
        DisposablePtr, IDisposableDataChannelInterface
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
        
        public IntPtr GetPtr
        {
            get
            {
                return Ptr;
            }
        }
    }
    public static class DataChannelInterfaceExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcDataChannelRegisterObserver(IntPtr s, IntPtr o);
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void webrtcDataChannelUnRegisterObserver(IntPtr s);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcDataChannelLabel(
            IntPtr ptr
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern int webrtcDataChannelStatus(
           IntPtr ptr
       );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool webrtcDataChannelSendText(
           IntPtr ptr, string text 
       );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool webrtcDataChannelSendData(
           IntPtr ptr, IntPtr data, int len
       );

        public static string Label(this IDisposableDataChannelInterface channel)
        {
            return Rtc.Interop.String.MoveToString(
                webrtcDataChannelLabel(channel.Ptr));
        }

        public static int Status(this IDisposableDataChannelInterface channel)
        {
            return webrtcDataChannelStatus(channel.Ptr);
        }

        public static bool Send(this IDisposableDataChannelInterface channel, byte[] data)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                return webrtcDataChannelSendData(channel.Ptr, handle.AddrOfPinnedObject(), data.Length);
            }
            finally
            {
                handle.Free();
            }
        }

        public static bool Send(this IDisposableDataChannelInterface channel, string text)
        {
            return webrtcDataChannelSendText(channel.Ptr, text);
        }

        public static void UnRegisterObserver(this IDisposableDataChannelInterface channel)
        {
            webrtcDataChannelUnRegisterObserver(channel.Ptr);
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
