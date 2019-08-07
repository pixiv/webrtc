using System;
using System.Runtime.InteropServices;

namespace Webrtc
{
    public enum SdpSemantics
    {
        PlanB,
        UnifiedPlan
    }

    public interface IPeerConnectionObserver
    {
        void OnSignalingChange(PeerConnectionInterface.SignalingState newState);
        void OnAddStream(MediaStreamInterface stream);
        void OnRemoveStream(MediaStreamInterface stream);
        void OnDataChannel(DataChannelInterface dataChannel);
        void OnRenegotiationNeeded();
        void OnIceConnectionChange(PeerConnectionInterface.IceConnectionState newState);
        void OnStandardizedIceConnectionChange(PeerConnectionInterface.IceConnectionState newState);
        void OnConnectionChange();
        void OnIceGatheringChange(PeerConnectionInterface.IceGatheringState newState);
        void OnIceCandidate(IceCandidateInterface candidate);
        void OnIceCandidatesRemoved(IntPtr candidates);
        void OnIceConnectionReceivingChange(bool receiving);
        void OnAddTrack(RtpReceiverInterface receiver, MediaStreamInterface[] streams);
        void OnTrack(RtpTransceiverInterface transceiver);
        void OnRemoveTrack(RtpReceiverInterface receiver);
        void OnInterestingUsage(int usagePattern);
    }

    public sealed class PeerConnectionDependencies
    {
        public PeerConnectionObserver Observer { get; set; }
        public IntPtr Allocator { get; set; }
        public IntPtr AsyncResolverFactory { get; set; }
        public IntPtr CertGenerator { get; set; }
        public IntPtr TlsCertVerifier { get; set; }
        public IntPtr VideoBitrateAllocatorFactory { get; set; }
    }

    public sealed class PeerConnectionFactoryInterface : IDisposable
    {
        [Flags]
        private enum WebrtcRTCConfigurationFlags
        {
            Dscp = 1 << 1,
            CpuAdaptation = 1 << 2,
            SuspendBelowMinBitrate = 1 << 3,
            PrerendererSmoothing = 1 << 4,
            ExperimentCpuLoadEstimator = 1 << 5
        }

        private struct WebrtcIceServer
        {
            public IntPtr Uri;
            public IntPtr Urls;
            public UIntPtr UrlsSize;
            public IntPtr Username;
            public IntPtr Password;
            public PeerConnectionInterface.TlsCertPolicy TlsCertPolicy;
            public IntPtr Hostname;
            public IntPtr TlsAlpnProtocols;
            public UIntPtr TlsAlpnProtocolsSize;
            public IntPtr TlsEllipticCurves;
            public UIntPtr TlsEllipticCurvesSize;
        }

        private struct WebrtcPeerConnectionDependencies
        {
            public IntPtr Observer;
            public IntPtr Allocator;
            public IntPtr AsyncResolverFactory;
            public IntPtr CertGenerator;
            public IntPtr TlsCertVerifier;
            public IntPtr VideoBitrateAllocatorFactory;
        }

        private struct WebrtcRTCConfiguration
        {
            [MarshalAs(UnmanagedType.I4)]
            public WebrtcRTCConfigurationFlags Flags;
            public int AudioRtcpReportIntervalMs;
            public int VideoRtcpReportIntervalMs;
            public IntPtr Servers;
            public UIntPtr ServersSize;
            public PeerConnectionInterface.IceTransportsType Type;
            public PeerConnectionInterface.BundlePolicy BundlePolicy;
            public PeerConnectionInterface.RtcpMuxPolicy RtcpMuxPolicy;
            public IntPtr Certificates;
            public UIntPtr CertificatesSize;
            public int IceCandidatePoolSize;
            public SdpSemantics SdpSemantics;
        }

        internal IntPtr Ptr;

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcPeerConnectionFactoryInterfaceCreatePeerConnection(
            IntPtr factory,
            [MarshalAs(UnmanagedType.LPStruct)] WebrtcRTCConfiguration configuration,
            [MarshalAs(UnmanagedType.LPStruct)] WebrtcPeerConnectionDependencies dependencies);

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcPeerConnectionFactoryInterfaceCreateVideoTrack(
            IntPtr factory,
            string label,
            IntPtr source);

        [DllImport("webrtc_c")]
        private static extern void webrtcPeerConnectionFactoryInterfaceRelease(IntPtr factory);

        private static IntPtr StringArrayToHGlobalAnsiArrayPtr(string[] array)
        {
            var sizeOfIntPtr = Marshal.SizeOf<IntPtr>();
            var ptr = Marshal.AllocHGlobal(array.Length * sizeOfIntPtr);

            for (var index = 0; index < array.Length; index++)
            {
                Marshal.WriteIntPtr(
                    ptr,
                    index * sizeOfIntPtr,
                    Marshal.StringToHGlobalAnsi(array[index])
                );
            }

            return ptr;
        }

        private static void FreeHGlobalAnsiArrayPtr(IntPtr ptr, string[] array)
        {
            var sizeOfIntPtr = Marshal.SizeOf<IntPtr>();

            for (var index = 0; index < array.Length; index++)
            {
                Marshal.FreeHGlobal(Marshal.ReadIntPtr(
                    ptr,
                    index * sizeOfIntPtr
                ));
            }

            Marshal.FreeHGlobal(ptr);
        }

        internal PeerConnectionFactoryInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~PeerConnectionFactoryInterface()
        {
            webrtcPeerConnectionFactoryInterfaceRelease(Ptr);
        }

        public PeerConnectionInterface CreatePeerConnection(PeerConnectionInterface.RtcConfiguration configuration, PeerConnectionDependencies dependencies)
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            var servers = new WebrtcIceServer[configuration.Servers.Length];
            var unmanagedConfiguration = new WebrtcRTCConfiguration();
            var unmanagedDependencies = new WebrtcPeerConnectionDependencies();

            if (configuration.Dscp)
            {
                unmanagedConfiguration.Flags |= WebrtcRTCConfigurationFlags.Dscp;
            }

            if (configuration.CpuAdaptation)
            {
                unmanagedConfiguration.Flags |= WebrtcRTCConfigurationFlags.CpuAdaptation;
            }

            if (configuration.SuspendBelowMinBitrate)
            {
                unmanagedConfiguration.Flags |= WebrtcRTCConfigurationFlags.SuspendBelowMinBitrate;
            }

            if (configuration.PrerendererSmoothing)
            {
                unmanagedConfiguration.Flags |= WebrtcRTCConfigurationFlags.PrerendererSmoothing;
            }

            if (configuration.ExperimentCpuLoadEstimator)
            {
                unmanagedConfiguration.Flags |= WebrtcRTCConfigurationFlags.ExperimentCpuLoadEstimator;
            }

            unmanagedConfiguration.AudioRtcpReportIntervalMs = configuration.AudioRtcpReportIntervalMs;
            unmanagedConfiguration.VideoRtcpReportIntervalMs = configuration.VideoRtcpReportIntervalMs;
            unmanagedConfiguration.ServersSize = new UIntPtr((uint)configuration.Servers.Length);
            unmanagedConfiguration.Type = configuration.Type;
            unmanagedConfiguration.BundlePolicy = configuration.BundlePolicy;
            unmanagedConfiguration.RtcpMuxPolicy = configuration.RtcpMuxPolicy;
            unmanagedConfiguration.CertificatesSize = new UIntPtr((uint)configuration.Certificates.Length);
            unmanagedConfiguration.IceCandidatePoolSize = configuration.IceCandidatePoolSize;
            unmanagedConfiguration.SdpSemantics = configuration.SdpSemantics;

            unmanagedDependencies.Observer = dependencies.Observer.Ptr;
            unmanagedDependencies.Allocator = dependencies.Allocator;
            unmanagedDependencies.AsyncResolverFactory = dependencies.AsyncResolverFactory;
            unmanagedDependencies.CertGenerator = dependencies.CertGenerator;
            unmanagedDependencies.TlsCertVerifier = dependencies.TlsCertVerifier;
            unmanagedDependencies.VideoBitrateAllocatorFactory = dependencies.VideoBitrateAllocatorFactory;

            var serversHandle = GCHandle.Alloc(servers, GCHandleType.Pinned);
            var certificatesHandle = GCHandle.Alloc(configuration.Certificates, GCHandleType.Pinned);

            for (var index = 0; index < configuration.Servers.Length; index++)
            {
                var server = configuration.Servers[index];
                ref var unmanagedServer = ref servers[index];

                unmanagedServer.UrlsSize = new UIntPtr((uint)server.Urls.Length);
                unmanagedServer.TlsCertPolicy = server.TlsCertPolicy;
                unmanagedServer.TlsAlpnProtocolsSize = new UIntPtr((uint)server.TlsAlpnProtocols.Length);
                unmanagedServer.TlsEllipticCurvesSize = new UIntPtr((uint)server.TlsEllipticCurves.Length);

                unmanagedServer.Uri = Marshal.StringToHGlobalAnsi(server.Uri);
                unmanagedServer.Urls = StringArrayToHGlobalAnsiArrayPtr(server.Urls);
                unmanagedServer.Username = Marshal.StringToHGlobalAnsi(server.Username);
                unmanagedServer.Password = Marshal.StringToHGlobalAnsi(server.Password);
                unmanagedServer.Hostname = Marshal.StringToHGlobalAnsi(server.Hostname);
                unmanagedServer.TlsAlpnProtocols = StringArrayToHGlobalAnsiArrayPtr(server.TlsAlpnProtocols);
                unmanagedServer.TlsEllipticCurves = StringArrayToHGlobalAnsiArrayPtr(server.TlsEllipticCurves);
            }

            try
            {
                unmanagedConfiguration.Servers = serversHandle.AddrOfPinnedObject();
                unmanagedConfiguration.Certificates = certificatesHandle.AddrOfPinnedObject();

                return new PeerConnectionInterface(
                    webrtcPeerConnectionFactoryInterfaceCreatePeerConnection(
                        Ptr,
                        unmanagedConfiguration,
                        unmanagedDependencies
                    )
                );
            }
            finally
            {
                for (var index = 0; index < configuration.Servers.Length; index++)
                {
                    var server = configuration.Servers[index];
                    ref var unmanagedServer = ref servers[index];

                    FreeHGlobalAnsiArrayPtr(
                        unmanagedServer.TlsEllipticCurves,
                        server.TlsEllipticCurves
                    );

                    FreeHGlobalAnsiArrayPtr(
                        unmanagedServer.TlsAlpnProtocols,
                        server.TlsAlpnProtocols
                    );

                    FreeHGlobalAnsiArrayPtr(
                        unmanagedServer.Urls,
                        server.Urls
                    );

                    Marshal.FreeHGlobal(unmanagedServer.Hostname);
                    Marshal.FreeHGlobal(unmanagedServer.Password);
                    Marshal.FreeHGlobal(unmanagedServer.Username);
                    Marshal.FreeHGlobal(unmanagedServer.Uri);
                }

                certificatesHandle.Free();
                serversHandle.Free();
            }
        }

        public MediaStreamTrackInterface CreateVideoTrack(
            string label,
            VideoTrackSourceInterface source)
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            return MediaStreamTrackInterface.Wrap(
                webrtcPeerConnectionFactoryInterfaceCreateVideoTrack(
                    Ptr,
                    label,
                    VideoTrackSourceInterface.webrtcMediaSourceInterfaceToWebrtcVideoTrackSourceInterface(
                        source.Ptr
                    )
                )
            );
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcPeerConnectionFactoryInterfaceRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }

    public sealed class PeerConnectionInterface : IDisposable
    {
        private readonly struct WebrtcAddTrackResult
        {
#pragma warning disable 0649
            public readonly IntPtr Error;
            public readonly IntPtr Value;
#pragma warning restore 0649
        }

        internal IntPtr Ptr;

        [DllImport("webrtc_c")]
        private static extern void webrtcDeleteRTCError(IntPtr ptr);

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcRTCErrorMessage(IntPtr ptr);

        [DllImport("webrtc_c")]
        private static extern RtcErrorType webrtcRTCErrorType(IntPtr ptr);

        [DllImport("webrtc_c")]
        private static extern WebrtcAddTrackResult webrtcPeerConnectionInterfaceAddTrack(
            IntPtr connection,
            IntPtr track,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] string[] data,
            [MarshalAs(UnmanagedType.SysUInt)] int size
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcPeerConnectionInterfaceCreateAnswer(
            IntPtr connection,
            IntPtr observer,
            RtcOfferAnswerOptions options
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcPeerConnectionInterfaceCreateOffer(
            IntPtr connection,
            IntPtr observer,
            RtcOfferAnswerOptions options
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcPeerConnectionInterfaceClose(
            IntPtr connection
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcPeerConnectionInterfaceRelease(IntPtr ptr);

        [DllImport("webrtc_c")]
        private static extern void webrtcPeerConnectionInterfaceSetLocalDescription(
            IntPtr connection,
            IntPtr observer,
            IntPtr desc
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcPeerConnectionInterfaceSetRemoteDescription(
            IntPtr connection,
            IntPtr observer,
            IntPtr desc
        );

        public enum SignalingState
        {
            Stable,
            HaveLocalOffer,
            HaveLocalPrAnswer,
            HaveRemoteOffer,
            HaveRemotePrAnswer,
            Closed,
        }

        public enum IceGatheringState
        {
            New,
            Gathering,
            Complete
        }

        public enum IceConnectionState
        {
            New,
            Checking,
            Connected,
            Completed,
            Failed,
            Disconnected,
            Closed,
            Max,
        }

        public enum IceTransportsType
        {
            None,
            Relay,
            NoHost,
            All
        }

        public enum BundlePolicy
        {
            Balanced,
            MaxBundle,
            MaxCompat
        }

        public enum RtcpMuxPolicy
        {
            Negotiate,
            Require,
        }

        public enum TlsCertPolicy
        {
            Secure,
            InsecureNoCheck
        }

        public sealed class IceServer
        {
            public string Uri { get; set; }
            public string[] Urls { get; set; } = new string[0];
            public string Username { get; set; }
            public string Password { get; set; }
            public TlsCertPolicy TlsCertPolicy { get; set; } =
                TlsCertPolicy.Secure;
            public string Hostname { get; set; }
            public string[] TlsAlpnProtocols { get; set; } = new string[0];
            public string[] TlsEllipticCurves { get; set; } = new string[0];
        }

        public sealed class RtcConfiguration
        {
            public bool Dscp { get; set; }
            public bool CpuAdaptation { get; set; }
            public bool SuspendBelowMinBitrate { get; set; }
            public bool PrerendererSmoothing { get; set; }
            public bool ExperimentCpuLoadEstimator { get; set; }
            public int AudioRtcpReportIntervalMs { get; set; } = 5000;
            public int VideoRtcpReportIntervalMs { get; set; } = 1000;
            public IceServer[] Servers { get; set; } = new IceServer[0];
            public IceTransportsType Type { get; set; } = IceTransportsType.All;
            public BundlePolicy BundlePolicy { get; set; } =
                BundlePolicy.Balanced;
            public RtcpMuxPolicy RtcpMuxPolicy { get; set; } =
                RtcpMuxPolicy.Require;
            public IntPtr[] Certificates { get; set; } = new IntPtr[0];
            public int IceCandidatePoolSize { get; set; } = 0;
            public SdpSemantics SdpSemantics;
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class RtcOfferAnswerOptions
        {
            public const int Undefined = -1;
            public const int MaxOfferToReceiveMedia = 1;
            public const int OfferToReceiveMediaTrue = 1;

            public int OfferToReceiveVideo { get; set; } = Undefined;
            public int OfferToReceiveAudio { get; set; } = Undefined;

            [field: MarshalAs(UnmanagedType.I1)]
            public bool VoiceActivityDetection { get; set; } = true;

            [field: MarshalAs(UnmanagedType.I1)]
            public bool IceRestart { get; set; } = false;

            [field: MarshalAs(UnmanagedType.I1)]
            public bool UseRtpMux { get; set; } = true;

            [field: MarshalAs(UnmanagedType.I1)]
            public bool RawPacketizationForVideo { get; set; } = false;

            public int NumSimulcastLayers { get; set; } = 1;

            [field: MarshalAs(UnmanagedType.I1)]
            public bool UseObsoleteSctpSdp = false;
        }

        internal PeerConnectionInterface(IntPtr ptr)
        {
            Ptr = ptr;
        }

        ~PeerConnectionInterface()
        {
            webrtcPeerConnectionInterfaceRelease(Ptr);
        }

        public RtcErrorOr<RtpSenderInterface> AddTrack(
            MediaStreamTrackInterface track, string[] streamIds)
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            RtcErrorType errorType;
            string errorMessage;

            var result = webrtcPeerConnectionInterfaceAddTrack(
                Ptr, track.Ptr, streamIds, streamIds.Length);

            try
            {
                var errorMessagePtr = webrtcRTCErrorMessage(result.Error);
                errorType = webrtcRTCErrorType(result.Error);
                errorMessage = Marshal.PtrToStringAnsi(errorMessagePtr);
            }
            finally
            {
                webrtcDeleteRTCError(result.Error);
            }

            var error = new RtcError(errorType, errorMessage);
            var value = errorType == RtcErrorType.None ?
                new RtpSenderInterface(result.Value) : null;

            return new RtcErrorOr<RtpSenderInterface>(error, value);
        }

        public void Close()
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            webrtcPeerConnectionInterfaceClose(Ptr);
        }

        public void CreateAnswer(
            CreateSessionDescriptionObserver observer,
            RtcOfferAnswerOptions options)
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            webrtcPeerConnectionInterfaceCreateAnswer(
                Ptr, observer.Ptr, options);
        }

        public void CreateOffer(
            CreateSessionDescriptionObserver observer,
            RtcOfferAnswerOptions options)
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            webrtcPeerConnectionInterfaceCreateOffer(
                Ptr, observer.Ptr, options);
        }

        public void SetLocalDescription(
            SetSessionDescriptionObserver observer,
            SessionDescriptionInterface desc)
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            webrtcPeerConnectionInterfaceSetLocalDescription(
                Ptr, observer.Ptr, desc.Ptr);

            desc.Ptr = IntPtr.Zero;
            GC.SuppressFinalize(desc);
        }

        public void SetRemoteDescription(
            SetSessionDescriptionObserver observer,
            SessionDescriptionInterface desc)
        {
            if (Ptr == IntPtr.Zero)
            {
                throw new ObjectDisposedException(null);
            }

            webrtcPeerConnectionInterfaceSetRemoteDescription(
                Ptr, observer.Ptr, desc.Ptr);

            desc.Ptr = IntPtr.Zero;
            GC.SuppressFinalize(desc);
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcPeerConnectionInterfaceRelease(Ptr);
            Ptr = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }
    }

    public sealed class PeerConnectionObserver : IDisposable
    {
        private delegate void SignalingChangeHandler(
            IntPtr context,
            PeerConnectionInterface.SignalingState newState
        );
        private delegate void AddStreamHandler(
            IntPtr context,
            IntPtr stream
        );
        private delegate void RemoveStreamHandler(
            IntPtr context,
            IntPtr stream
        );
        private delegate void DataChannelHandler(
            IntPtr context,
            IntPtr dataChannel
        );
        private delegate void RenegotiationNeededHandler(IntPtr context);
        private delegate void IceConnectionChangeHandler(
            IntPtr context,
            PeerConnectionInterface.IceConnectionState newState
        );
        private delegate void StandardizedIceConnectionChangeHandler(
            IntPtr context,
            PeerConnectionInterface.IceConnectionState newState
        );
        private delegate void ConnectionChangeHandler(IntPtr context);
        private delegate void IceGatheringChangeHandler(
            IntPtr context,
            PeerConnectionInterface.IceGatheringState newState
        );
        private delegate void IceCandidateHandler(
            IntPtr context,
            IceCandidateInterface candidate
        );
        private delegate void IceCandidatesRemovedHandler(
            IntPtr context,
            IntPtr candidates
        );
        private delegate void IceConnectionReceivingChangeHandler(
            IntPtr context,
            bool receiving
        );
        private delegate void AddTrackHandler(
            IntPtr context,
            IntPtr receiver,
            IntPtr data,
            [MarshalAs(UnmanagedType.SysUInt)] int size
        );
        private delegate void TrackHandler(
            IntPtr context,
            IntPtr transceiver
        );
        private delegate void RemoveTrackHandler(
            IntPtr context,
            IntPtr receiver
        );
        private delegate void InterestingUsageHandler(
            IntPtr context,
            int usagePattern
        );

        private readonly GCHandle _handle;
        internal IntPtr Ptr;

        private static readonly SignalingChangeHandler s_onSignalingChange =
            (context, newState) =>
                GetContextTarget(context).OnSignalingChange(newState);

        private static readonly AddStreamHandler s_onAddStream =
            (context, stream) => GetContextTarget(context).OnAddStream(
                new MediaStreamInterface(stream));

        private static readonly RemoveStreamHandler s_onRemoveStream =
            (context, stream) => GetContextTarget(context).OnRemoveStream(
                new MediaStreamInterface(stream));

        private static readonly DataChannelHandler s_onDataChannel =
            (context, dataChannel) =>
                GetContextTarget(context).OnDataChannel(
                    new DataChannelInterface(dataChannel));

        private static readonly RenegotiationNeededHandler s_onRenegotiationNeeded =
            context => GetContextTarget(context).OnRenegotiationNeeded();

        private static readonly IceConnectionChangeHandler s_onIceConnectionChange =
            (context, newState) =>
                GetContextTarget(context).OnIceConnectionChange(newState);

        private static readonly StandardizedIceConnectionChangeHandler s_onStandardizedIceConnectionChange =
            (context, newState) =>
                GetContextTarget(context).OnStandardizedIceConnectionChange(
                    newState);

        private static readonly ConnectionChangeHandler s_onConnectionChange =
            context => GetContextTarget(context).OnConnectionChange();

        private static readonly IceGatheringChangeHandler s_onIceGatheringChange =
            (context, newState) =>
                GetContextTarget(context).OnIceGatheringChange(newState);

        private static readonly IceCandidateHandler s_onIceCandidate =
            (context, candidate) =>
                GetContextTarget(context).OnIceCandidate(candidate);

        private static readonly IceCandidatesRemovedHandler s_onIceCandidatesRemoved =
            (context, candidates) =>
                GetContextTarget(context).OnIceCandidatesRemoved(candidates);

        private static readonly IceConnectionReceivingChangeHandler s_onIceConnectionReceivingChange =
            (context, receiving) =>
                GetContextTarget(context).OnIceConnectionReceivingChange(
                    receiving);

        private static readonly AddTrackHandler s_onAddTrack =
            (context, receiver, data, size) =>
            {
                var sizeOfIntPtr = Marshal.SizeOf<IntPtr>();
                var streams = new MediaStreamInterface[size];

                for (var index = 0; index < size; index++)
                {
                    var ptr = Marshal.PtrToStructure<IntPtr>(data);
                    streams[index] = new MediaStreamInterface(ptr);
                    data += sizeOfIntPtr;
                }

                GetContextTarget(context).OnAddTrack(
                    new RtpReceiverInterface(receiver), streams);
            };

        private static readonly TrackHandler s_onTrack =
            (context, transceiver) => GetContextTarget(context).OnTrack(
                new RtpTransceiverInterface(transceiver));

        private static readonly RemoveTrackHandler s_onRemoveTrack =
            (context, receiver) => GetContextTarget(context).OnRemoveTrack(
                new RtpReceiverInterface(receiver));

        private static readonly InterestingUsageHandler s_onInterestingUsage =
            (context, usagePattern) =>
                GetContextTarget(context).OnInterestingUsage(usagePattern);

        private static IPeerConnectionObserver GetContextTarget(IntPtr context)
        {
            return (IPeerConnectionObserver)((GCHandle)context).Target;
        }

        [DllImport("webrtc_c")]
        private static extern IntPtr webrtcNewPeerConnectionObserver(
            IntPtr context,
            SignalingChangeHandler onSignalingChange,
            AddStreamHandler onAddStream,
            RemoveStreamHandler onRemoveStream,
            DataChannelHandler onDataChannel,
            RenegotiationNeededHandler onRenegotiationNeeded,
            IceConnectionChangeHandler onIceConnectionChange,
            StandardizedIceConnectionChangeHandler onStandardizedIceConnectionChange,
            ConnectionChangeHandler onConnectionChange,
            IceGatheringChangeHandler onIceGatheringChange,
            IceCandidateHandler onIceCandidate,
            IceCandidatesRemovedHandler onIceCandidatesRemoved,
            IceConnectionReceivingChangeHandler onIceConnectionReceivingChange,
            AddTrackHandler onAddTrack,
            TrackHandler onTrack,
            RemoveTrackHandler onRemoveTrack,
            InterestingUsageHandler onInterestingUsage
        );

        [DllImport("webrtc_c")]
        private static extern void webrtcDeletePeerConnectionObserver(
            IntPtr observer
        );

        public PeerConnectionObserver(IPeerConnectionObserver implementation)
        {
            _handle = GCHandle.Alloc(implementation);

            Ptr = webrtcNewPeerConnectionObserver(
                (IntPtr)_handle,
                s_onSignalingChange,
                s_onAddStream,
                s_onRemoveStream,
                s_onDataChannel,
                s_onRenegotiationNeeded,
                s_onIceConnectionChange,
                s_onStandardizedIceConnectionChange,
                s_onConnectionChange,
                s_onIceGatheringChange,
                s_onIceCandidate,
                s_onIceCandidatesRemoved,
                s_onIceConnectionReceivingChange,
                s_onAddTrack,
                s_onTrack,
                s_onRemoveTrack,
                s_onInterestingUsage
            );
        }

        ~PeerConnectionObserver()
        {
            webrtcDeletePeerConnectionObserver(Ptr);
            _handle.Free();
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero)
            {
                return;
            }

            webrtcDeletePeerConnectionObserver(Ptr);
            Ptr = IntPtr.Zero;
            _handle.Free();
            GC.SuppressFinalize(this);
        }
    }
}
