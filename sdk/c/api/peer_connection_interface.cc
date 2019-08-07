#include "api/peer_connection_interface.h"

namespace webrtc {

extern "C" typedef void SignalingChangeHandler(
    void* context,
    PeerConnectionInterface::SignalingState new_state);
extern "C" typedef void AddStreamHandler(void* context, void* stream);
extern "C" typedef void RemoveStreamHandler(void* context, void* stream);
extern "C" typedef void DataChannelHandler(void* context, void* data_channel);
extern "C" typedef void RenegotiationNeededHandler(void* context);
extern "C" typedef void IceConnectionChangeHandler(
    void* context,
    PeerConnectionInterface::IceConnectionState new_state);
extern "C" typedef void StandardizedIceConnectionChangeHandler(
    void* context,
    PeerConnectionInterface::IceConnectionState new_state);
extern "C" typedef void ConnectionChangeHandler(
    void* context,
    PeerConnectionInterface::PeerConnectionState new_state);
extern "C" typedef void IceGatheringChangeHandler(
    void* context,
    PeerConnectionInterface::IceGatheringState new_state);
extern "C" typedef void IceCandidateHandler(
  void* context,
  const void* candidate);
extern "C" typedef void IceCandidatesRemovedHandler(void* context,
                                                    const void* candidates);
extern "C" typedef void IceConnectionReceivingChangeHandler(void* context,
                                                            bool receiving);
extern "C" typedef void AddTrackHandler(
    void* context,
    void* receiver,
    void** data,
    std::vector<rtc::scoped_refptr<MediaStreamInterface>>::size_type size);
extern "C" typedef void TrackHandler(void* context, void* transceiver);
extern "C" typedef void RemoveTrackHandler(void* context, void* receiver);
extern "C" typedef void InterestingUsageHandler(void* context,
                                                int usage_pattern);

class DelegatingPeerConnectionObserver : public PeerConnectionObserver {
  public:
    DelegatingPeerConnectionObserver(
        void* context,
        SignalingChangeHandler* onSignalingChange,
        AddStreamHandler* onAddStream,
        RemoveStreamHandler* onRemoveStream,
        DataChannelHandler* onDataChannel,
        RenegotiationNeededHandler* onRenegotiationNeeded,
        IceConnectionChangeHandler* onIceConnectionChange,
        StandardizedIceConnectionChangeHandler* onStandardizedIceConnectionChange,
        ConnectionChangeHandler* onConnectionChange,
        IceGatheringChangeHandler* onIceGatheringChange,
        IceCandidateHandler* onIceCandidate,
        IceCandidatesRemovedHandler* onIceCandidatesRemoved,
        IceConnectionReceivingChangeHandler* onIceConnectionReceivingChange,
        AddTrackHandler* onAddTrack,
        TrackHandler* onTrack,
        RemoveTrackHandler* onRemoveTrack,
        InterestingUsageHandler* onInterestingUsage) {
      context_ = context;
      onSignalingChange_ = onSignalingChange;
      onAddStream_ = onAddStream;
      onRemoveStream_ = onRemoveStream;
      onDataChannel_ = onDataChannel;
      onRenegotiationNeeded_ = onRenegotiationNeeded;
      onIceConnectionChange_ = onIceConnectionChange;
      onStandardizedIceConnectionChange_ = onStandardizedIceConnectionChange;
      onConnectionChange_ = onConnectionChange;
      onIceGatheringChange_ = onIceGatheringChange;
      onIceCandidate_ = onIceCandidate;
      onIceCandidatesRemoved_ = onIceCandidatesRemoved;
      onIceConnectionReceivingChange_ = onIceConnectionReceivingChange;
      onAddTrack_ = onAddTrack;
      onTrack_ = onTrack;
      onRemoveTrack_ = onRemoveTrack;
      onInterestingUsage_ = onInterestingUsage;
    }

    void OnSignalingChange(PeerConnectionInterface::SignalingState new_state)
        override {
      onSignalingChange_(context_, new_state);
    }

    void OnAddStream(rtc::scoped_refptr<MediaStreamInterface> stream)
        override {
      onAddStream_(context_, stream.release());
    }

    void OnRemoveStream(rtc::scoped_refptr<MediaStreamInterface> stream)
        override {
      onRemoveStream_(context_, stream.release());
    }

    void OnDataChannel(rtc::scoped_refptr<DataChannelInterface> data_channel)
        override {
      onDataChannel_(context_, data_channel.release());
    }

    void OnRenegotiationNeeded() override {
      onRenegotiationNeeded_(context_);
    }

    void OnIceConnectionChange(
        PeerConnectionInterface::IceConnectionState new_state) override {
      onIceConnectionChange_(context_, new_state);
    }

    void OnStandardizedIceConnectionChange(
        PeerConnectionInterface::IceConnectionState new_state) override {
      onStandardizedIceConnectionChange_(context_, new_state);
    }

    void OnConnectionChange(
        PeerConnectionInterface::PeerConnectionState new_state) override {
      onConnectionChange_(context_, new_state);
    }

    void OnIceGatheringChange(
        PeerConnectionInterface::IceGatheringState new_state) override {
      onIceGatheringChange_(context_, new_state);
    }

    void OnIceCandidate(const IceCandidateInterface* candidate) override {
      onIceCandidate_(context_, candidate);
    }

    void OnIceCandidatesRemoved(
        const std::vector<cricket::Candidate>& candidates) override {
      onIceCandidatesRemoved_(context_, &candidates);
    }

    void OnIceConnectionReceivingChange(bool receiving) override {
      onIceConnectionReceivingChange_(context_, receiving);
    }

    void OnAddTrack(
        rtc::scoped_refptr<RtpReceiverInterface> receiver,
        const std::vector<rtc::scoped_refptr<MediaStreamInterface>>& streams)
        override {
      auto streamsSize = streams.size();
      auto voidStreams = new void*[streamsSize];

      for (std::size_t index = 0; index < streamsSize; index++) {
        auto stream = streams[index].get();
        stream->AddRef();
        voidStreams[index] = stream;
      }

      onAddTrack_(context_, receiver.release(), voidStreams, streamsSize);
      delete[] voidStreams;
    }

    void OnTrack(rtc::scoped_refptr<RtpTransceiverInterface> transceiver)
        override {
      onTrack_(context_, transceiver.release());
    }

    void OnRemoveTrack(rtc::scoped_refptr<RtpReceiverInterface> receiver)
        override {
      onRemoveTrack_(context_, receiver.release());
    }

    void OnInterestingUsage(int usage_pattern) override {
      onInterestingUsage_(context_, usage_pattern);
    }

  private:
    void* context_;
    SignalingChangeHandler* onSignalingChange_;
    AddStreamHandler* onAddStream_;
    RemoveStreamHandler* onRemoveStream_;
    DataChannelHandler* onDataChannel_;
    RenegotiationNeededHandler* onRenegotiationNeeded_;
    IceConnectionChangeHandler* onIceConnectionChange_;
    StandardizedIceConnectionChangeHandler* onStandardizedIceConnectionChange_;
    ConnectionChangeHandler* onConnectionChange_;
    IceGatheringChangeHandler* onIceGatheringChange_;
    IceCandidateHandler* onIceCandidate_;
    IceCandidatesRemovedHandler* onIceCandidatesRemoved_;
    IceConnectionReceivingChangeHandler* onIceConnectionReceivingChange_;
    AddTrackHandler* onAddTrack_;
    TrackHandler* onTrack_;
    RemoveTrackHandler* onRemoveTrack_;
    InterestingUsageHandler* onInterestingUsage_;
};

}

enum {
  WEBRTC_RTC_CONFIGURATION_FLAG_DSCP = 1 << 1,
  WEBRTC_RTC_CONFIGURATION_FLAG_CPU_ADAPTATION = 1 << 2,
  WEBRTC_RTC_CONFIGURATION_FLAG_SUSPEND_BELOW_MIN_BITRATE = 1 << 3,
  WEBRTC_RTC_CONFIGURATION_FLAG_PRERENDERER_SMOOTHING = 1 << 4,
  WEBRTC_RTC_CONFIGURaTION_FLAG_EXPERIMENT_CPU_LOAD_ESTIMATOR = 1 << 5,
};

struct WebrtcAddTrackResult {
  void* error;
  void* value;
};

struct WebrtcIceServer {
  const char* uri;
  const char** urls;
  std::vector<std::string>::size_type urls_size;
  const char* username;
  const char* password;
  webrtc::PeerConnectionInterface::TlsCertPolicy tls_cert_policy;
  const char* hostname;
  const char** tls_alpn_protocols;
  std::vector<std::string>::size_type tls_alpn_protocols_size;
  const char** tls_elliptic_curves;
  std::vector<std::string>::size_type tls_elliptic_curves_size;
};

struct WebrtcPeerConnectionDependencies {
  void* observer;
  void* allocator;
  void* async_resolver_factory;
  void* cert_generator;
  void* tls_cert_verifier;
  void* video_bitrate_allocator_factory;
};

struct WebrtcRTCConfiguration {
  int flags;
  int audio_rtcp_report_interval_ms;
  int video_rtcp_report_interval_ms;
  WebrtcIceServer *servers;
  webrtc::PeerConnectionInterface::IceServers::size_type servers_size;
  webrtc::PeerConnectionInterface::IceTransportsType type;
  webrtc::PeerConnectionInterface::BundlePolicy bundle_policy;
  webrtc::PeerConnectionInterface::RtcpMuxPolicy rtcp_mux_policy;
  void** certificates;
  std::vector<rtc::scoped_refptr<rtc::RTCCertificate>>::size_type certificates_size;
  int ice_candidate_pool_size;
  webrtc::SdpSemantics sdp_semantics;
};

RTC_EXPORT extern "C" void webrtcDeletePeerConnectionObserver(
    void* observer) {
  delete static_cast<webrtc::PeerConnectionObserver*>(observer);
}

RTC_EXPORT extern "C" void* webrtcNewPeerConnectionObserver(
    void* context,
    webrtc::SignalingChangeHandler* onSignalingChange,
    webrtc::AddStreamHandler* onAddStream,
    webrtc::RemoveStreamHandler* onRemoveStream,
    webrtc::DataChannelHandler* onDataChannel,
    webrtc::RenegotiationNeededHandler* onRenegotiationNeeded,
    webrtc::IceConnectionChangeHandler* onIceConnectionChange,
    webrtc::StandardizedIceConnectionChangeHandler* onStandardizedIceConnectionChange,
    webrtc::ConnectionChangeHandler* onConnectionChange,
    webrtc::IceGatheringChangeHandler* onIceGatheringChange,
    webrtc::IceCandidateHandler* onIceCandidate,
    webrtc::IceCandidatesRemovedHandler* onIceCandidatesRemoved,
    webrtc::IceConnectionReceivingChangeHandler* onIceConnectionReceivingChange,
    webrtc::AddTrackHandler* onAddTrack,
    webrtc::TrackHandler* onTrack,
    webrtc::RemoveTrackHandler* onRemoveTrack,
    webrtc::InterestingUsageHandler* onInterestingUsage) {
  return static_cast<webrtc::PeerConnectionObserver*>(new webrtc::DelegatingPeerConnectionObserver(
    context, onSignalingChange, onAddStream, onRemoveStream,
    onDataChannel, onRenegotiationNeeded, onIceConnectionChange,
    onStandardizedIceConnectionChange, onConnectionChange, onIceGatheringChange,
    onIceCandidate, onIceCandidatesRemoved, onIceConnectionReceivingChange,
    onAddTrack, onTrack, onRemoveTrack, onInterestingUsage));
}

RTC_EXPORT extern "C" void* webrtcPeerConnectionFactoryInterfaceCreatePeerConnection(
    void* factory,
    const WebrtcRTCConfiguration* cconfiguration,
    const WebrtcPeerConnectionDependencies* cdependencies) {
  webrtc::PeerConnectionInterface::RTCConfiguration configuration;
  webrtc::PeerConnectionDependencies dependencies {
    static_cast<webrtc::PeerConnectionObserver*>(cdependencies->observer)
  };

  configuration.set_dscp(cconfiguration->flags & WEBRTC_RTC_CONFIGURATION_FLAG_DSCP);
  configuration.set_cpu_adaptation(cconfiguration->flags & WEBRTC_RTC_CONFIGURATION_FLAG_CPU_ADAPTATION);
  configuration.set_suspend_below_min_bitrate(cconfiguration->flags & WEBRTC_RTC_CONFIGURATION_FLAG_SUSPEND_BELOW_MIN_BITRATE);
  configuration.set_prerenderer_smoothing(cconfiguration->flags & WEBRTC_RTC_CONFIGURATION_FLAG_PRERENDERER_SMOOTHING);
  configuration.set_experiment_cpu_load_estimator(cconfiguration->flags & WEBRTC_RTC_CONFIGURaTION_FLAG_EXPERIMENT_CPU_LOAD_ESTIMATOR);
  configuration.set_audio_rtcp_report_interval_ms(cconfiguration->audio_rtcp_report_interval_ms);
  configuration.set_video_rtcp_report_interval_ms(cconfiguration->video_rtcp_report_interval_ms);
  configuration.servers.reserve(cconfiguration->servers_size);

  for (webrtc::PeerConnectionInterface::IceServers::size_type index = 0; index < cconfiguration->servers_size; index++) {
    webrtc::PeerConnectionInterface::IceServer server;
    auto cserver = cconfiguration->servers + index;

    if (cserver->uri) {
      server.uri.append(cserver->uri);
    }

    server.urls.reserve(cserver->urls_size);

    for (std::vector<std::string>::size_type url_index = 0; url_index < cserver->urls_size; url_index++) {
      server.urls.emplace_back(cserver->urls[url_index]);
    }

    if (cserver->username) {
      server.username.append(cserver->username);
    }

    if (cserver->password) {
      server.password.append(cserver->password);
    }

    server.tls_cert_policy = cserver->tls_cert_policy;

    if (cserver->hostname) {
      server.hostname.append(cserver->hostname);
    }

    server.tls_alpn_protocols.reserve(cserver->tls_alpn_protocols_size);

    for (std::vector<std::string>::size_type protocol_index = 0; protocol_index < cserver->tls_alpn_protocols_size; protocol_index++) {
      server.tls_alpn_protocols.emplace_back(cserver->tls_alpn_protocols[protocol_index]);
    }

    server.tls_elliptic_curves.reserve(cserver->tls_elliptic_curves_size);

    for (std::vector<std::string>::size_type curve_index = 0; curve_index < cserver->tls_elliptic_curves_size; curve_index++) {
      server.tls_elliptic_curves.emplace_back(cserver->tls_elliptic_curves[curve_index]);
    }

    configuration.servers.push_back(server);
  }

  configuration.type = cconfiguration->type;
  configuration.bundle_policy = cconfiguration->bundle_policy;
  configuration.rtcp_mux_policy = cconfiguration->rtcp_mux_policy;
  configuration.certificates.reserve(cconfiguration->certificates_size);

  for (std::vector<rtc::scoped_refptr<rtc::RTCCertificate>>::size_type index = 0; index < cconfiguration->certificates_size; index++) {
    configuration.certificates.emplace_back(static_cast<rtc::RTCCertificate*>(cconfiguration->certificates[index]));
  }

  configuration.ice_candidate_pool_size = cconfiguration->ice_candidate_pool_size;
  configuration.sdp_semantics = cconfiguration->sdp_semantics;
  dependencies.allocator.reset(static_cast<cricket::PortAllocator*>(cdependencies->allocator));
  dependencies.async_resolver_factory.reset(static_cast<webrtc::AsyncResolverFactory*>(cdependencies->async_resolver_factory));
  dependencies.cert_generator.reset(static_cast<rtc::RTCCertificateGeneratorInterface*>(cdependencies->cert_generator));
  dependencies.tls_cert_verifier.reset(static_cast<rtc::SSLCertificateVerifier*>(cdependencies->tls_cert_verifier));
  dependencies.video_bitrate_allocator_factory.reset(static_cast<webrtc::VideoBitrateAllocatorFactory*>(cdependencies->video_bitrate_allocator_factory));
  auto connection = static_cast<webrtc::PeerConnectionFactoryInterface*>(factory)->CreatePeerConnection(configuration, std::move(dependencies));

  return connection.release();
}

RTC_EXPORT extern "C" void* webrtcPeerConnectionFactoryInterfaceCreateVideoTrack(
    void* factory,
    const char* label,
    void* source) {
  return static_cast<webrtc::PeerConnectionFactoryInterface*>(factory)->CreateVideoTrack(
    label,
    static_cast<webrtc::VideoTrackSourceInterface*>(source)).release();
}

RTC_EXPORT extern "C" void webrtcPeerConnectionFactoryInterfaceRelease(
    const void* factory) {
  static_cast<const webrtc::PeerConnectionFactoryInterface*>(factory)->Release();
}

RTC_EXPORT extern "C" WebrtcAddTrackResult webrtcPeerConnectionInterfaceAddTrack(
    void* connection,
    void* track,
    const char* const* data,
    std::vector<std::string>::size_type size) {
  WebrtcAddTrackResult cresult;
  std::vector<std::string> stream_ids;

  stream_ids.reserve(size);

  while (size > 0) {
    stream_ids.emplace_back(*data);
    data++;
    size--;
  }

  auto result = static_cast<webrtc::PeerConnectionInterface*>(connection)->AddTrack(
    static_cast<webrtc::MediaStreamTrackInterface*>(track), stream_ids);

  cresult.error = new auto(result.MoveError());
  cresult.value = result.ok() ? result.value().release() : nullptr;

  return cresult;
}

RTC_EXPORT extern "C" void webrtcPeerConnectionInterfaceClose(void* connection) {
  static_cast<webrtc::PeerConnectionInterface*>(connection)->Close();
}

RTC_EXPORT extern "C" void webrtcPeerConnectionInterfaceCreateAnswer(
    void* connection,
    void* observer,
    const webrtc::PeerConnectionInterface::RTCOfferAnswerOptions* options) {
  static_cast<webrtc::PeerConnectionInterface*>(connection)->CreateAnswer(
    static_cast<webrtc::CreateSessionDescriptionObserver*>(observer), *options);
}

RTC_EXPORT extern "C" void webrtcPeerConnectionInterfaceCreateOffer(
    void* connection,
    void* observer,
    const webrtc::PeerConnectionInterface::RTCOfferAnswerOptions* options) {
  static_cast<webrtc::PeerConnectionInterface*>(connection)->CreateOffer(
    static_cast<webrtc::CreateSessionDescriptionObserver*>(observer), *options);
}

RTC_EXPORT extern "C" void webrtcPeerConnectionInterfaceRelease(
    const void* connection) {
  static_cast<const webrtc::PeerConnectionInterface*>(connection)->Release();
}

RTC_EXPORT extern "C" void webrtcPeerConnectionInterfaceSetLocalDescription(
    void* connection,
    void* observer,
    void* desc) {
  static_cast<webrtc::PeerConnectionInterface*>(connection)->SetLocalDescription(
    static_cast<webrtc::SetSessionDescriptionObserver*>(observer),
    static_cast<webrtc::SessionDescriptionInterface*>(desc));
}

RTC_EXPORT extern "C" void webrtcPeerConnectionInterfaceSetRemoteDescription(
    void* connection,
    void* observer,
    void* desc) {
  static_cast<webrtc::PeerConnectionInterface*>(connection)->SetRemoteDescription(
    static_cast<webrtc::SetSessionDescriptionObserver*>(observer),
    static_cast<webrtc::SessionDescriptionInterface*>(desc));
}
