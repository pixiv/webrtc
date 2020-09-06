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
    public interface IRTCDataBufferInterface
    {
        IntPtr Ptr { get; }
    }

    public sealed class RTCDataBufferInterface : IRTCDataBufferInterface
    {
        public IntPtr Ptr { get; }

        public RTCDataBufferInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }
    }

    public interface IManagedDataChannelObserver
    {
        DisposableDataChannelInterface DataChannel { get; }
        void OnStateChange();
        void OnMessage(bool binary, IntPtr data, int data_size);
        void OnBufferedAmountChange(UInt64 sent_data_size);
    }

    public interface IDataChannelObserver
    {
        IntPtr Ptr { get; }
    }

    public interface IDisposableDataChannelObserver :
        IDataChannelObserver, Rtc.IDisposable
    {
    }

    public sealed class DisposableDataChannelObserver : DisposablePtr, IDisposableDataChannelObserver
    {
        IntPtr IDataChannelObserver.Ptr => Ptr;



        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DestructionHandler(IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void OnBufferedAmountChangeHandler(IntPtr context, ulong sent_data_size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void OnMessageHandler(IntPtr context, bool binary, IntPtr data, int data_size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void OnStateChangeHandler(IntPtr context);

        private static readonly FunctionPtrArray s_functions = new FunctionPtrArray(
            (DestructionHandler)OnDestruction,
            (OnStateChangeHandler)OnStateChange,
            (OnMessageHandler)OnMessage,
            (OnBufferedAmountChangeHandler)OnBufferedAmountChange
        );

        [MonoPInvokeCallback(typeof(OnStateChangeHandler))]
        private static void OnStateChange(IntPtr context)
        {
            var handle = (GCHandle)context;

            ((IManagedDataChannelObserver)handle.Target).OnStateChange();
        }

        [MonoPInvokeCallback(typeof(OnMessageHandler))]
        private static void OnMessage(IntPtr context, bool binary, IntPtr data, int data_size)
        {
            var handle = (GCHandle)context;

            ((IManagedDataChannelObserver)handle.Target).OnMessage(binary, data, data_size);
        }

        [MonoPInvokeCallback(typeof(OnBufferedAmountChangeHandler))]
        private static void OnBufferedAmountChange(IntPtr context, ulong sent_data_size)
        {
            var handle = (GCHandle)context;

            ((IManagedDataChannelObserver)handle.Target).OnBufferedAmountChange(sent_data_size);
        }

        private protected override void FreePtr()
        {
            Interop.DataChannel.UnregisterObserver(DataChannel.GetPtr);
            DataChannel.Dispose();
        }

        [MonoPInvokeCallback(typeof(DestructionHandler))]
        private static void OnDestruction(IntPtr context)
        {
            ((GCHandle)context).Free();
        }

        public DisposableDataChannelObserver(
            IManagedDataChannelObserver implementation)
        {
            DataChannel = implementation.DataChannel;
            Ptr = Interop.DataChannel.RegisterObserver(
                (IntPtr)GCHandle.Alloc(implementation),
                implementation.DataChannel.GetPtr,
                s_functions.Ptr
            );
        }

        public DisposableDataChannelInterface DataChannel
        {
            get; private set;
        }

    }
}
namespace Pixiv.Webrtc.Interop
{
    public static class DataChannel
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDataChannelRegisterObserver")]
        public static extern IntPtr RegisterObserver(
            IntPtr context,
            IntPtr dataChannel,
            IntPtr functions
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDataChannelUnregisterObserver")]
        public static extern void UnregisterObserver(
            IntPtr context
        );
    }
}
