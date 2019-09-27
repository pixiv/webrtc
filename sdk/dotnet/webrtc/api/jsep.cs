/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */

using Pixiv.Rtc;
using System;
using System.Runtime.InteropServices;

namespace Pixiv.Webrtc
{
    public enum SdpType
    {
        Offer,
        PrAnswer,
        Answer
    }

    public interface ICreateSessionDescriptionObserver
    {
        IntPtr Ptr { get; }
    }

    public interface IDisposableCreateSessionDescriptionObserver :
        ICreateSessionDescriptionObserver, Rtc.IDisposable
    {
    }

    public interface IDisposableIceCandidateInterface :
        IIceCandidateInterface, Rtc.IDisposable
    {
    }

    public interface IDisposableSessionDescriptionInterface :
        ISessionDescriptionInterface, Rtc.IDisposable
    {
    }

    public interface IDisposableSetSessionDescriptionObserver :
        ISetSessionDescriptionObserver, Rtc.IDisposable
    {
    }

    public interface IIceCandidateInterface
    {
        IntPtr Ptr { get; }
    }

    public interface IManagedCreateSessionDescriptionObserver
    {
        void OnSuccess(DisposableSessionDescriptionInterface desc);
        void OnFailure(RtcError error);
    }

    public interface IManagedSetSessionDescriptionObserver
    {
        void OnSuccess();
        void OnFailure(RtcError error);
    }

    public interface ISessionDescriptionInterface
    {
        IntPtr Ptr { get; }
    }

    public interface ISetSessionDescriptionObserver
    {
        IntPtr Ptr { get; }
    }

    public sealed class DisposableCreateSessionDescriptionObserver :
        DisposablePtr, IDisposableCreateSessionDescriptionObserver
    {
        IntPtr ICreateSessionDescriptionObserver.Ptr => Ptr;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DestructionHandler(IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SuccessHandler(IntPtr context, IntPtr desc);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FailureHandler(IntPtr context, RtcError error);

        private static readonly FunctionPtrArray s_functions = new FunctionPtrArray(
            (DestructionHandler)OnDestruction,
            (SuccessHandler)OnSuccess,
            (FailureHandler)OnFailure
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcNewCreateSessionDescriptionObserver(
            IntPtr context,
            IntPtr functions
        );

        [MonoPInvokeCallback(typeof(DestructionHandler))]
        private static void OnDestruction(IntPtr context)
        {
            ((GCHandle)context).Free();
        }

        [MonoPInvokeCallback(typeof(SuccessHandler))]
        private static void OnSuccess(IntPtr context, IntPtr desc)
        {
            var handle = (GCHandle)context;

            ((IManagedCreateSessionDescriptionObserver)handle.Target).OnSuccess(
                new DisposableSessionDescriptionInterface(desc)
            );
        }

        [MonoPInvokeCallback(typeof(FailureHandler))]
        private static void OnFailure(IntPtr context, RtcError error)
        {
            var handle = (GCHandle)context;

            ((IManagedCreateSessionDescriptionObserver)handle.Target).OnFailure(
                error
            );
        }

        public DisposableCreateSessionDescriptionObserver(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public DisposableCreateSessionDescriptionObserver(
            IManagedCreateSessionDescriptionObserver implementation)
        {
            Ptr = webrtcNewCreateSessionDescriptionObserver(
                (IntPtr)GCHandle.Alloc(implementation),
                s_functions.Ptr
            );
        }

        private protected override void FreePtr()
        {
            Interop.CreateSessionDescriptionObserver.Release(Ptr);
        }
    }

    public sealed class DisposableSessionDescriptionInterface :
        DisposablePtr, IDisposableSessionDescriptionInterface
    {
        IntPtr ISessionDescriptionInterface.Ptr => Ptr;

        public DisposableSessionDescriptionInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.SessionDescriptionInterface.Delete(Ptr);
        }
    }

    public sealed class DisposableSetSessionDescriptionObserver :
        DisposablePtr, IDisposableSetSessionDescriptionObserver
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DestructionHandler(IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SuccessHandler(IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FailureHandler(IntPtr context, RtcError error);

        private static FunctionPtrArray s_functions = new FunctionPtrArray(
            (DestructionHandler)OnDestruction,
            (SuccessHandler)OnSuccess,
            (FailureHandler)OnFailure
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcNewSetSessionDescriptionObserver(
            IntPtr context,
            IntPtr functions
        );

        [MonoPInvokeCallback(typeof(DestructionHandler))]
        private static void OnDestruction(IntPtr context)
        {
            ((GCHandle)context).Free();
        }

        [MonoPInvokeCallback(typeof(SuccessHandler))]
        private static void OnSuccess(IntPtr context)
        {
            var handle = (GCHandle)context;
            ((IManagedSetSessionDescriptionObserver)handle.Target).OnSuccess();
        }

        [MonoPInvokeCallback(typeof(FailureHandler))]
        private static void OnFailure(IntPtr context, RtcError error)
        {
            var handle = (GCHandle)context;

            ((IManagedSetSessionDescriptionObserver)handle.Target).OnFailure(
                error
            );
        }

        IntPtr ISetSessionDescriptionObserver.Ptr => Ptr;

        public DisposableSetSessionDescriptionObserver(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public DisposableSetSessionDescriptionObserver(
            IManagedSetSessionDescriptionObserver implementation)
        {
            Ptr = webrtcNewSetSessionDescriptionObserver(
                (IntPtr)GCHandle.Alloc(implementation),
                s_functions.Ptr
            );
        }

        private protected override void FreePtr()
        {
            Interop.SetSessionDescriptionObserver.Release(Ptr);
        }
    }

    public sealed class IceCandidateInterface : IIceCandidateInterface
    {
        public IntPtr Ptr { get; }

        public IceCandidateInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }
    }

    public static class IceCandidateInterfaceExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool webrtcIceCandidateInterfaceToString(
            IntPtr ptr,
            out IntPtr result
        );

        public static bool TryToString(
            this IIceCandidateInterface candidate, out string s)
        {
            var result = webrtcIceCandidateInterfaceToString(
                candidate.Ptr, out var webrtcString);

            s = Rtc.Interop.String.MoveToString(webrtcString);

            return result;
        }
    }

    public static class SessionDescription
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcCreateSessionDescription(
            SdpType type,
            string sdp,
            IntPtr error
        );

        public static DisposableSessionDescriptionInterface Create(
            SdpType type,
            string sdp,
            IntPtr error)
        {
            var ptr = webrtcCreateSessionDescription(type, sdp, error);

            return ptr == IntPtr.Zero ?
                null : new DisposableSessionDescriptionInterface(ptr);
        }
    }

    public static class SessionDescriptionInterfaceExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool webrtcSessionDescriptionInterfaceToString(
            IntPtr ptr,
            out IntPtr result
        );

        public static bool TryToString(
            this ISessionDescriptionInterface desc,
            out string s)
        {
            var result = webrtcSessionDescriptionInterfaceToString(
                desc.Ptr, out var webrtcString);

            s = Rtc.Interop.String.MoveToString(webrtcString);

            return result;
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class CreateSessionDescriptionObserver
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcCreateSessionDescriptionObserverRelease")]
        public static extern void Release(IntPtr ptr);
    }

    public static class SessionDescriptionInterface
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDeleteSessionDescriptionInterface")]
        public static extern void Delete(IntPtr ptr);
    }

    public static class SetSessionDescriptionObserver
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcSetSessionDescriptionObserverRelease")]
        public static extern void Release(IntPtr ptr);
    }
}
