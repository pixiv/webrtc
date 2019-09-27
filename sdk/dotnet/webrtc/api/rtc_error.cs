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
    public enum RtcErrorType
    {
        None,
        UnsupportedOperation,
        UnsupportedParameter,
        InvalidParameter,
        InvalidRange,
        SyntaxError,
        InvalidState,
        InvalidModification,
        NetworkError,
        ResourceExhausted,
        InternalError,
    }

    public readonly struct RtcError
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr webrtcRTCErrorMessage(IntPtr ptr);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern RtcErrorType webrtcRTCErrorType(IntPtr ptr);

        public bool OK => Type == RtcErrorType.None;
        public RtcErrorType Type { get; }
        public string Message { get; }

        public RtcError(IntPtr ptr)
        {
            var messagePtr = webrtcRTCErrorMessage(ptr);
            Type = webrtcRTCErrorType(ptr);
            Message = Marshal.PtrToStringAnsi(messagePtr);
        }

        public RtcError(RtcErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }

    public readonly struct RtcErrorOr<T>
    {
        public RtcError Error { get; }

        private readonly T _value;

        public T Value
        {
            get
            {
                System.Diagnostics.Debug.Assert(Error.OK);
                return _value;
            }
        }

        internal RtcErrorOr(RtcError error, T value)
        {
            Error = error;
            _value = value;
        }
    }
}

namespace Pixiv.Webrtc.Interop
{
    public static class RtcError
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "webrtcDeleteRTCError")]
        public static extern void Delete(IntPtr ptr);
    }
}
