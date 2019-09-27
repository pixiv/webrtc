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

    public interface IDisposableMessageQueue : IDisposable, IMessageQueue
    {
    }

    public interface IDisposableRtcCertificateGeneratorInterface :
        IDisposable, IRtcCertificateGeneratorInterface
    {
    }

    public interface IDisposableSslCertificateVerifier :
        IDisposable, ISslCertificateVerifier
    {
    }

    public interface IDisposableThread : IDisposableMessageQueue, IThread
    {
    }

    public interface IMessageQueue
    {
        IntPtr Ptr { get; }
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

    public interface IThread : IMessageQueue
    {

        new IntPtr Ptr { get; }
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
        IntPtr IMessageQueue.Ptr =>
            Interop.Thread.ToRtcMessageQueue(((IThread)this).Ptr);

        IntPtr IThread.Ptr => Ptr;

        public DisposableThread(IntPtr ptr)
        {
            Ptr = ptr;
        }

        private protected override void FreePtr()
        {
            Interop.MessageQueue.Delete(Interop.Thread.ToRtcMessageQueue(Ptr));
        }
    }

    public sealed class Thread : IThread
    {
        IntPtr IMessageQueue.Ptr =>
            Interop.Thread.ToRtcMessageQueue(((IThread)this).Ptr);

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

    public static class MessageQueueExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void rtcMessageQueueQuit(IntPtr ptr);

        public static void Quit(this IMessageQueue queue)
        {
            rtcMessageQueueQuit(queue.Ptr);
        }
    }

    public static class ThreadExtension
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void rtcThreadRun(IntPtr ptr);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl)]
        private static extern void rtcThreadStart(IntPtr ptr);

        public static void Run(this IThread thread)
        {
            rtcThreadRun(thread.Ptr);
        }

        public static void Start(this IThread thread)
        {
            rtcThreadStart(thread.Ptr);
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
            rtcThreadManagerUnwrapCurrentThread(manager.Ptr);
        }

        public static Thread WrapCurrentThread(this IThreadManager manager)
        {
            return new Thread(rtcThreadManagerWrapCurrentThread(manager.Ptr));
        }
    }
}

namespace Pixiv.Rtc.Interop
{
    public static class MessageQueue
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "rtcDeleteMessageQueue")]
        public static extern void Delete(IntPtr ptr);
    }

    public static class Thread
    {
        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "rtcMessageQueueToRtcThread")]
        public static extern IntPtr FromMessageQueue(IntPtr ptr);

        [DllImport(Dll.Name, CallingConvention = CallingConvention.Cdecl, EntryPoint = "rtcThreadToRtcMessageQueue")]
        public static extern IntPtr ToRtcMessageQueue(IntPtr ptr);
    }
}
