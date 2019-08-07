using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    public enum SdpType
    {
        Offer,
        PrAnswer,
        Answer
    }

    public interface ICreateSessionDescriptionObserver
    {
        void OnSuccess(SessionDescriptionInterface desc);
        void OnFailure(RtcError error);
    }

    public interface ISetSessionDescriptionObserver
    {
        void OnSuccess();
        void OnFailure(RtcError error);
    }

    public sealed class CreateSessionDescriptionObserver : IDisposable
    {

        private delegate void DestructionHandler(IntPtr context);
        private delegate void SuccessHandler(IntPtr context, IntPtr desc);
        private delegate void FailureHandler(IntPtr context, RtcError error);

        internal IntPtr Ptr;

        [DllImport("webrtc_c")]
        private static extern void webrtcCreateSessionDescriptionObserverRelease(
            IntPtr ptr
        );

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcNewCreateSessionDescriptionObserver(
            IntPtr context,
            DestructionHandler onDestruction,
            SuccessHandler onSuccess,
            FailureHandler onFailure
        );

        private static readonly DestructionHandler s_onDestruction = context =>
        {
            ((GCHandle)context).Free();
        };

        private static readonly SuccessHandler s_onSuccess = (context, desc) =>
        {
            var handle = (GCHandle)context;
            var target = (ICreateSessionDescriptionObserver)handle.Target;
            target.OnSuccess(new SessionDescriptionInterface(desc));
        };

        private static readonly FailureHandler s_onFailure = (context, error) =>
        {
            var handle = (GCHandle)context;
            ((ICreateSessionDescriptionObserver)handle.Target).OnFailure(error);
        };

        public CreateSessionDescriptionObserver(
            ICreateSessionDescriptionObserver implementation)
        {
            Ptr = webrtcNewCreateSessionDescriptionObserver(
                (IntPtr)GCHandle.Alloc(implementation),
                s_onDestruction,
                s_onSuccess,
                s_onFailure
            );
        }

        ~CreateSessionDescriptionObserver()
        {
            webrtcCreateSessionDescriptionObserverRelease(Ptr);
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcCreateSessionDescriptionObserverRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }

    public readonly struct IceCandidateInterface
    {
#pragma warning disable 0649
        internal readonly IntPtr Ptr;
#pragma warning restore 0649

        [DllImport("webrtc_c")]
        private static extern bool webrtcIceCandidateInterfaceToString(
            IntPtr ptr,
            out String result
        );

        public bool IsDefault => Ptr == IntPtr.Zero;

        public bool TryToString(out string s)
        {
            if (IsDefault)
            {
                s = null;
                return false;
            }

            var result = webrtcIceCandidateInterfaceToString(
                Ptr, out var webrtcString);

            using (webrtcString)
            {
                s = webrtcString.ToString();
            }

            return result;
        }

        public override string ToString()
        {
            return TryToString(out var result) ?
                result : "Invalid Ice Candidate";
        }
    }

    public sealed class SessionDescriptionInterface : IDisposable
    {
        internal IntPtr Ptr;

        [DllImport("webrtc_c")]
        private static extern void webrtcDeleteSessionDescriptionInterface(
            IntPtr ptr
        );

        [DllImport("webrtc_c")]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool webrtcSessionDescriptionInterfaceToString(
            IntPtr ptr,
            out String result
        );

        internal SessionDescriptionInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~SessionDescriptionInterface()
        {
            webrtcDeleteSessionDescriptionInterface(Ptr);
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcDeleteSessionDescriptionInterface(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        public bool TryToString(out string s)
        {
            if (Ptr == IntPtr.Zero)
            {
                s = null;
                return false;
            }

            var result = webrtcSessionDescriptionInterfaceToString(Ptr, out var webrtcString);

            using (webrtcString)
            {
                s = webrtcString.ToString();
            }

            return result;
        }

        public override string ToString()
        {
            return TryToString(out var result) ? result : "Invalid Session Description";
        }
    }

    public sealed class SetSessionDescriptionObserver : IDisposable
    {
        private delegate void DestructionHandler(IntPtr context);
        private delegate void SuccessHandler(IntPtr context);
        private delegate void FailureHandler(IntPtr context, RtcError error);

        internal IntPtr Ptr;

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcNewSetSessionDescriptionObserver(
            IntPtr context,
            DestructionHandler onDestruction,
            SuccessHandler onSuccess,
            FailureHandler onFailure
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcSetSessionDescriptionObserverRelease(
            IntPtr ptr
        );

        private static readonly DestructionHandler s_onDestruction = context =>
        {
            ((GCHandle)context).Free();
        };

        private static readonly SuccessHandler s_onSuccess = context =>
        {
            var handle = (GCHandle)context;
            ((ISetSessionDescriptionObserver)handle.Target).OnSuccess();
        };

        private static readonly FailureHandler s_onFailure = (context, error) =>
        {
            var handle = (GCHandle)context;
            ((ISetSessionDescriptionObserver)handle.Target).OnFailure(error);
        };

        public SetSessionDescriptionObserver(
            ISetSessionDescriptionObserver implementation)
        {
            Ptr = webrtcNewSetSessionDescriptionObserver(
                (IntPtr)GCHandle.Alloc(implementation),
                s_onDestruction,
                s_onSuccess,
                s_onFailure
            );
        }

        ~SetSessionDescriptionObserver()
        {
            webrtcSetSessionDescriptionObserverRelease(Ptr);
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcSetSessionDescriptionObserverRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }

    public static class SessionDescription
    {
        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcCreateSessionDescription(
            SdpType type,
            string sdp,
            IntPtr error
        );

        public static SessionDescriptionInterface Create(
            SdpType type,
            string sdp,
            IntPtr error)
        {
            var ptr = webrtcCreateSessionDescription(type, sdp, error);

            return ptr == IntPtr.Zero ?
                null : new SessionDescriptionInterface(ptr);
        }
    }
}
