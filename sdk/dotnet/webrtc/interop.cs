/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;
using System;
using System.Runtime.InteropServices;

namespace Pixiv.Rtc.Interop
{
    public static class String
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void rtcDeleteString(IntPtr s);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr rtcString_c_str(IntPtr s);

        public static string MoveToString(IntPtr ptr)
        {
            try
            {
                return Marshal.PtrToStringAnsi(rtcString_c_str(ptr));
            }
            finally
            {
                rtcDeleteString(ptr);
            }
        }
    }
}
