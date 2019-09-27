/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;
using System;
using System.Runtime.InteropServices;

namespace Pixiv.Rtc
{
    internal sealed class FunctionPtrArray
    {
        public IntPtr Ptr { get; }
        private readonly Delegate[] delegates_;

        public FunctionPtrArray(params Delegate[] unmarshalleds)
        {
            delegates_ = unmarshalleds;

            var ptrSize = Marshal.SizeOf<IntPtr>();
            var ptr = Marshal.AllocHGlobal(unmarshalleds.Length * ptrSize);

            Ptr = ptr;

            foreach (var unmarshalled in unmarshalleds)
            {
                var marshalled = Marshal.GetFunctionPointerForDelegate(unmarshalled);
                Marshal.WriteIntPtr(ptr, marshalled);
                ptr += ptrSize;
            }
        }

        ~FunctionPtrArray()
        {
            Marshal.FreeHGlobal(Ptr);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class MonoPInvokeCallbackAttribute : Attribute
    {
        public MonoPInvokeCallbackAttribute(Type type)
        {
        }
    }

    public interface IDisposable : System.IDisposable
    {
        void ReleasePtr();
    }

    public interface IDisposableRtcCertificateGeneratorInterface :
        IDisposable, IRtcCertificateGeneratorInterface
    {
    }

    public interface IDisposableSslCertificateVerifier :
        IDisposable, ISslCertificateVerifier
    {
    }

    public interface IDisposableThread : IDisposable, IThread
    {
    }

    public interface IRtcCertificate
    {
        IntPtr Ptr { get; }
    }

    public interface IRtcCertificateGeneratorInterface
    {
        IntPtr Ptr { get; }
    }

    public interface ISslCertificateVerifier
    {
        IntPtr Ptr { get; }
    }

    public interface IThread
    {
        IntPtr Ptr { get; }
    }

    public interface IThreadManager
    {
        IntPtr Ptr { get; }
    }

    public abstract class DisposablePtr : IDisposable
    {
        private IntPtr _ptr;

        private protected IntPtr Ptr
        {
            get
            {
                if (_ptr == IntPtr.Zero)
                {
                    throw new ObjectDisposedException(null);
                }

                return _ptr;
            }

            set
            {
                if (value == IntPtr.Zero)
                {
                    throw new ArgumentException(nameof(value));
                }

                _ptr = value;
                GC.ReRegisterForFinalize(this);
            }
        }

        private protected bool IsInvalid => Ptr == IntPtr.Zero;

        internal DisposablePtr()
        {
        }

        ~DisposablePtr()
        {
            FreePtr();
        }

        public void Dispose()
        {
            if (_ptr == IntPtr.Zero)
            {
                return;
            }

            FreePtr();
            ReleasePtr();
        }

        private protected abstract void FreePtr();

        public virtual void ReleasePtr()
        {
            _ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }

    public sealed class DisposableThread : DisposablePtr, IDisposableThread
    {
        IntPtr IThread.Ptr => Ptr;

        public DisposableThread(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.Thread.Delete(Ptr);
        }
    }

    public sealed class Thread : IThread
    {
        IntPtr IThread.Ptr => _ptr;

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr rtcCreateThread();

        private IntPtr _ptr;

        public Thread(IntPtr ptr)
        {
            _ptr = ptr;
        }

        public static DisposableThread Create()
        {
            return new DisposableThread(rtcCreateThread());
        }
    }

    public sealed class ThreadManager : IThreadManager
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr rtcThreadManagerInstance();

        public IntPtr Ptr { get; }

        public ThreadManager(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public static ThreadManager Instance { get; } =
            new ThreadManager(rtcThreadManagerInstance());
    }

    public static class ThreadExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void rtcThreadQuit(IntPtr ptr);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void rtcThreadRun(IntPtr ptr);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void rtcThreadStart(IntPtr ptr);

        public static void Quit(this IThread thread)
        {
            if (thread == null)
            {
                throw new ArgumentNullException(nameof(thread));
            }

            rtcThreadQuit(thread.Ptr);
            GC.KeepAlive(thread);
        }

        public static void Run(this IThread thread)
        {
            if (thread == null)
            {
                throw new ArgumentNullException(nameof(thread));
            }

            rtcThreadRun(thread.Ptr);
            GC.KeepAlive(thread);
        }

        public static void Start(this IThread thread)
        {
            if (thread == null)
            {
                throw new ArgumentNullException(nameof(thread));
            }

            rtcThreadStart(thread.Ptr);
            GC.KeepAlive(thread);
        }
    }

    public static class ThreadManagerExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void rtcThreadManagerUnwrapCurrentThread(
            IntPtr ptr
        );

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr rtcThreadManagerWrapCurrentThread(
            IntPtr ptr
        );

        public static void UnwrapCurrentThread(this IThreadManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            rtcThreadManagerUnwrapCurrentThread(manager.Ptr);
            GC.KeepAlive(manager);
        }

        public static Thread WrapCurrentThread(this IThreadManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            var thread = rtcThreadManagerWrapCurrentThread(manager.Ptr);
            GC.KeepAlive(manager);

            return new Thread(thread);
        }
    }
}

namespace Pixiv.Rtc.Interop
{
    public static class Thread
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "rtcDeleteThread")]
        public static extern void Delete(IntPtr ptr);
    }
}
