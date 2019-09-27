/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */

#include "api/peer_connection_interface.h"
#include "sdk/c/api/peer_connection_interface.h"

namespace webrtc {

class DelegatingPeerConnectionObserver : public PeerConnectionObserver {
  public:
   DelegatingPeerConnectionObserver(
       void* context,
       const struct WebrtcPeerConnectionObserverFunctions* functions) {
     context_ = context;
     functions_ = functions;
   }

   ~DelegatingPeerConnectionObserver() { functions_->on_destruction(context_); }

   void OnSignalingChange(
       PeerConnectionInterface::SignalingState new_state) override {
     functions_->on_signaling_change(
         context_,
         static_cast<WebrtcPeerConnectionInterfaceSignalingState>(new_state));
   }

    void OnAddStream(rtc::scoped_refptr<MediaStreamInterface> stream)
        override {
      functions_->on_add_stream(context_, rtc::ToC(stream.release()));
    }

    void OnRemoveStream(rtc::scoped_refptr<MediaStreamInterface> stream)
        override {
      functions_->on_remove_stream(context_, rtc::ToC(stream.release()));
    }

    void OnDataChannel(rtc::scoped_refptr<DataChannelInterface> data_channel)
        override {
      functions_->on_data_channel(context_, rtc::ToC(data_channel.release()));
    }

    void OnRenegotiationNeeded() override {
      functions_->on_renegotiation_needed(context_);
    }

    void OnIceConnectionChange(
        PeerConnectionInterface::IceConnectionState new_state) override {
      auto c_new_state =
          static_cast<WebrtcPeerConnectionInterfaceIceConnectionState>(
              new_state);

      functions_->on_ice_connection_change(context_, c_new_state);
    }

    void OnStandardizedIceConnectionChange(
        PeerConnectionInterface::IceConnectionState new_state) override {
      auto c_new_state =
          static_cast<WebrtcPeerConnectionInterfaceIceConnectionState>(
              new_state);

      functions_->on_standardized_ice_connection_change(context_, c_new_state);
    }

    void OnConnectionChange(
        PeerConnectionInterface::PeerConnectionState new_state) override {
      auto c_new_state =
          static_cast<WebrtcPeerConnectionInterfacePeerConnectionState>(
              new_state);

      functions_->on_connection_change(context_, c_new_state);
    }

    void OnIceGatheringChange(
        PeerConnectionInterface::IceGatheringState new_state) override {
      auto c_new_state =
          static_cast<WebrtcPeerConnectionInterfaceIceGatheringState>(
              new_state);

      functions_->on_ice_gathering_change(context_, c_new_state);
    }

    void OnIceCandidate(
        const webrtc::IceCandidateInterface* candidate) override {
      functions_->on_ice_candidate(context_, rtc::ToC(candidate));
    }

    void OnIceCandidatesRemoved(
        const std::vector<cricket::Candidate>& candidates) override {
      auto size = candidates.size();
      auto ccandidates = new const CricketCandidate*[size];

      for (size_t index = 0; index < size; index++) {
        ccandidates[index] = rtc::ToC(&candidates[index]);
      }

      functions_->on_ice_candidates_removed(context_, ccandidates, size);
      delete[] ccandidates;
    }

    void OnIceConnectionReceivingChange(bool receiving) override {
      functions_->on_ice_connection_receiving_change(context_, receiving);
    }

    void OnAddTrack(
        rtc::scoped_refptr<RtpReceiverInterface> receiver,
        const std::vector<rtc::scoped_refptr<MediaStreamInterface>>& streams)
        override {
      auto streamsSize = streams.size();
      auto voidStreams = new WebrtcMediaStreamInterface*[streamsSize];

      for (std::size_t index = 0; index < streamsSize; index++) {
        auto stream = streams[index].get();
        stream->AddRef();
        voidStreams[index] = rtc::ToC(stream);
      }

      functions_->on_add_track(context_, rtc::ToC(receiver.release()),
                               voidStreams, streamsSize);

      delete[] voidStreams;
    }

    void OnTrack(rtc::scoped_refptr<RtpTransceiverInterface> transceiver)
        override {
      functions_->on_track(context_, rtc::ToC(transceiver.release()));
    }

    void OnRemoveTrack(rtc::scoped_refptr<RtpReceiverInterface> receiver)
        override {
      functions_->on_remove_track(context_, rtc::ToC(receiver.release()));
    }

    void OnInterestingUsage(int usage_pattern) override {
      functions_->on_interesting_usage(context_, usage_pattern);
    }

  private:
    void* context_;
    const struct WebrtcPeerConnectionObserverFunctions* functions_;
};

}

RTC_EXPORT extern "C" void webrtcDeletePeerConnectionObserver(
    WebrtcPeerConnectionObserver* observer) {
  delete rtc::ToCplusplus(observer);
}

RTC_EXPORT extern "C" WebrtcPeerConnectionObserver*
webrtcNewPeerConnectionObserver(
    void* context,
    const struct WebrtcPeerConnectionObserverFunctions* functions) {
  return rtc::ToC(static_cast<webrtc::PeerConnectionObserver*>(
      new webrtc::DelegatingPeerConnectionObserver(context, functions)));
}

RTC_EXPORT extern "C" WebrtcPeerConnectionInterface*
webrtcPeerConnectionFactoryInterfaceCreatePeerConnection(
    WebrtcPeerConnectionFactoryInterface* factory,
    const struct WebrtcPeerConnectionInterfaceRTCConfiguration* cconfiguration,
    const struct WebrtcPeerConnectionDependencies* cdependencies) {
  webrtc::PeerConnectionInterface::RTCConfiguration configuration;
  webrtc::PeerConnectionDependencies dependencies{
      rtc::ToCplusplus(cdependencies->observer)};

  configuration.set_dscp(cconfiguration->flags &
                         WEBRTC_RTC_CONFIGURATION_FLAG_DSCP);

  configuration.set_cpu_adaptation(
      cconfiguration->flags & WEBRTC_RTC_CONFIGURATION_FLAG_CPU_ADAPTATION);

  configuration.set_suspend_below_min_bitrate(
      cconfiguration->flags &
      WEBRTC_RTC_CONFIGURATION_FLAG_SUSPEND_BELOW_MIN_BITRATE);

  configuration.set_prerenderer_smoothing(
      cconfiguration->flags &
      WEBRTC_RTC_CONFIGURATION_FLAG_PRERENDERER_SMOOTHING);

  configuration.set_experiment_cpu_load_estimator(
      cconfiguration->flags &
      WEBRTC_RTC_CONFIGURaTION_FLAG_EXPERIMENT_CPU_LOAD_ESTIMATOR);

  configuration.set_audio_rtcp_report_interval_ms(
      cconfiguration->audio_rtcp_report_interval_ms);

  configuration.set_video_rtcp_report_interval_ms(
      cconfiguration->video_rtcp_report_interval_ms);

  configuration.servers.reserve(cconfiguration->servers_size);

  for (size_t index = 0; index < cconfiguration->servers_size; index++) {
    webrtc::PeerConnectionInterface::IceServer server;
    auto cserver = cconfiguration->servers + index;

    if (cserver->uri) {
      server.uri.append(cserver->uri);
    }

    server.urls.reserve(cserver->urls_size);

    for (size_t url_index = 0; url_index < cserver->urls_size; url_index++) {
      server.urls.emplace_back(cserver->urls[url_index]);
    }

    if (cserver->username) {
      server.username.append(cserver->username);
    }

    if (cserver->password) {
      server.password.append(cserver->password);
    }

    server.tls_cert_policy =
        static_cast<webrtc::PeerConnectionInterface::TlsCertPolicy>(
            cserver->tls_cert_policy);

    if (cserver->hostname) {
      server.hostname.append(cserver->hostname);
    }

    server.tls_alpn_protocols.reserve(cserver->tls_alpn_protocols_size);

    for (size_t protocol_index = 0;
         protocol_index < cserver->tls_alpn_protocols_size; protocol_index++) {
      server.tls_alpn_protocols.emplace_back(cserver->tls_alpn_protocols[protocol_index]);
    }

    server.tls_elliptic_curves.reserve(cserver->tls_elliptic_curves_size);

    for (size_t curve_index = 0;
         curve_index < cserver->tls_elliptic_curves_size; curve_index++) {
      server.tls_elliptic_curves.emplace_back(cserver->tls_elliptic_curves[curve_index]);
    }

    configuration.servers.push_back(server);
  }

  configuration.type =
      static_cast<webrtc::PeerConnectionInterface::IceTransportsType>(
          cconfiguration->type);
  configuration.bundle_policy =
      static_cast<webrtc::PeerConnectionInterface::BundlePolicy>(
          cconfiguration->bundle_policy);
  configuration.rtcp_mux_policy =
      static_cast<webrtc::PeerConnectionInterface::RtcpMuxPolicy>(
          cconfiguration->rtcp_mux_policy);
  configuration.certificates.reserve(cconfiguration->certificates_size);

  for (size_t index = 0; index < cconfiguration->certificates_size; index++) {
    configuration.certificates.emplace_back(
        rtc::ToCplusplus(cconfiguration->certificates[index]));
  }

  configuration.ice_candidate_pool_size = cconfiguration->ice_candidate_pool_size;
  configuration.sdp_semantics =
      static_cast<webrtc::SdpSemantics>(cconfiguration->sdp_semantics);
  dependencies.allocator.reset(rtc::ToCplusplus(cdependencies->allocator));
  dependencies.async_resolver_factory.reset(
      rtc::ToCplusplus(cdependencies->async_resolver_factory));
  dependencies.cert_generator.reset(
      rtc::ToCplusplus(cdependencies->cert_generator));
  dependencies.tls_cert_verifier.reset(
      rtc::ToCplusplus(cdependencies->tls_cert_verifier));
  dependencies.video_bitrate_allocator_factory.reset(
      rtc::ToCplusplus(cdependencies->video_bitrate_allocator_factory));
  auto connection = rtc::ToCplusplus(factory)->CreatePeerConnection(
      configuration, std::move(dependencies));

  return rtc::ToC(connection.release());
}

RTC_EXPORT extern "C" WebrtcAudioTrackInterface*
webrtcPeerConnectionFactoryInterfaceCreateAudioTrack(
    WebrtcPeerConnectionFactoryInterface* factory,
    const char* label,
    WebrtcAudioSourceInterface* source) {
  return rtc::ToC(rtc::ToCplusplus(factory)
                      ->CreateAudioTrack(label, rtc::ToCplusplus(source))
                      .release());
}

RTC_EXPORT extern "C" WebrtcVideoTrackInterface*
webrtcPeerConnectionFactoryInterfaceCreateVideoTrack(
    WebrtcPeerConnectionFactoryInterface* factory,
    const char* label,
    WebrtcVideoTrackSourceInterface* source) {
  return rtc::ToC(rtc::ToCplusplus(factory)
                      ->CreateVideoTrack(label, rtc::ToCplusplus(source))
                      .release());
}

RTC_EXPORT extern "C" void webrtcPeerConnectionFactoryInterfaceRelease(
    const WebrtcPeerConnectionFactoryInterface* factory) {
  rtc::ToCplusplus(factory)->Release();
}

RTC_EXPORT extern "C" WebrtcPeerConnectionInterfaceAddTrackResult
webrtcPeerConnectionInterfaceAddTrack(WebrtcPeerConnectionInterface* connection,
                                      WebrtcMediaStreamTrackInterface* track,
                                      const char* const* data,
                                      size_t size) {
  WebrtcPeerConnectionInterfaceAddTrackResult cresult;
  std::vector<std::string> stream_ids;

  stream_ids.reserve(size);

  while (size > 0) {
    stream_ids.emplace_back(*data);
    data++;
    size--;
  }

  auto result = rtc::ToCplusplus(connection)
                    ->AddTrack(rtc::ToCplusplus(track), stream_ids);

  cresult.error = rtc::ToC(new auto(result.MoveError()));
  cresult.value = result.ok() ? rtc::ToC(result.value().release()) : nullptr;

  return cresult;
}

RTC_EXPORT extern "C" void webrtcPeerConnectionInterfaceClose(
    WebrtcPeerConnectionInterface* connection) {
  rtc::ToCplusplus(connection)->Close();
}

RTC_EXPORT extern "C" void webrtcPeerConnectionInterfaceCreateAnswer(
    WebrtcPeerConnectionInterface* connection,
    WebrtcCreateSessionDescriptionObserver* observer,
    const struct WebrtcPeerConnectionInterfaceRTCOfferAnswerOptions* options) {
  rtc::ToCplusplus(connection)
      ->CreateAnswer(rtc::ToCplusplus(observer), *options);
}

extern "C" void webrtcPeerConnectionInterfaceCreateOffer(
    WebrtcPeerConnectionInterface* connection,
    WebrtcCreateSessionDescriptionObserver* observer,
    const struct WebrtcPeerConnectionInterfaceRTCOfferAnswerOptions* options) {
  rtc::ToCplusplus(connection)
      ->CreateOffer(rtc::ToCplusplus(observer), *options);
}

extern "C" void webrtcPeerConnectionInterfaceRelease(
    const WebrtcPeerConnectionInterface* connection) {
  rtc::ToCplusplus(connection)->Release();
}

extern "C" void webrtcPeerConnectionInterfaceSetAudioRecording(
    WebrtcPeerConnectionInterface* connection,
    bool recording) {
  rtc::ToCplusplus(connection)->SetAudioRecording(recording);
}

extern "C" void webrtcPeerConnectionInterfaceSetLocalDescription(
    WebrtcPeerConnectionInterface* connection,
    WebrtcSetSessionDescriptionObserver* observer,
    WebrtcSessionDescriptionInterface* desc) {
  rtc::ToCplusplus(connection)
      ->SetLocalDescription(rtc::ToCplusplus(observer), rtc::ToCplusplus(desc));
}

extern "C" void webrtcPeerConnectionInterfaceSetRemoteDescription(
    WebrtcPeerConnectionInterface* connection,
    WebrtcSetSessionDescriptionObserver* observer,
    WebrtcSessionDescriptionInterface* desc) {
  rtc::ToCplusplus(connection)
      ->SetRemoteDescription(rtc::ToCplusplus(observer),
                             rtc::ToCplusplus(desc));
}
