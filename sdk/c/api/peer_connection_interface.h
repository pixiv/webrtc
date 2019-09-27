/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#ifndef SDK_C_API_PEER_CONNECTION_INTERFACE_H_
#define SDK_C_API_PEER_CONNECTION_INTERFACE_H_

#include <stddef.h>
#include "sdk/c/api/async_resolver_factory.h"
#include "sdk/c/api/candidate.h"
#include "sdk/c/api/data_channel_interface.h"
#include "sdk/c/api/jsep.h"
#include "sdk/c/api/media_stream_interface.h"
#include "sdk/c/api/rtc_error.h"
#include "sdk/c/api/rtp_receiver_interface.h"
#include "sdk/c/api/rtp_sender_interface.h"
#include "sdk/c/api/rtp_transceiver_interface.h"
#include "sdk/c/api/video/video_bitrate_allocator_factory.h"
#include "sdk/c/p2p/base.h"
#include "sdk/c/rtc_base/rtc_certificate.h"
#include "sdk/c/rtc_base/rtc_certificate_generator.h"
#include "sdk/c/rtc_base/ssl_certificate.h"

#ifndef __cplusplus
#include <stdbool.h>
#endif

enum {
  WEBRTC_RTC_CONFIGURATION_FLAG_DSCP = 1 << 1,
  WEBRTC_RTC_CONFIGURATION_FLAG_CPU_ADAPTATION = 1 << 2,
  WEBRTC_RTC_CONFIGURATION_FLAG_SUSPEND_BELOW_MIN_BITRATE = 1 << 3,
  WEBRTC_RTC_CONFIGURATION_FLAG_PRERENDERER_SMOOTHING = 1 << 4,
  WEBRTC_RTC_CONFIGURaTION_FLAG_EXPERIMENT_CPU_LOAD_ESTIMATOR = 1 << 5,
};

enum WebrtcSdpSemantics {
  WEBRTC_SDP_SEMANTICS_PLAN_B,
  WEBRTC_SDP_SEMANTICS_UNIFIED_PLAN
};

enum WebrtcPeerConnectionInterfaceSignalingState {
  WEBRTC_PEER_CONNECTION_INTERFACE_SIGNALING_STATE_STABLE,
  WEBRTC_PEER_CONNECTION_INTERFACE_SIGNALING_STATE_HAVE_LOCAL_OFFER,
  WEBRTC_PEER_CONNECTION_INTERFACE_SIGNALING_STATE_HAVE_LOCAL_PR_ANSWER,
  WEBRTC_PEER_CONNECTION_INTERFACE_SIGNALING_STATE_HAVE_REMOTE_OFFER,
  WEBRTC_PEER_CONNECTION_INTERFACE_SIGNALING_STATE_HAVE_REMOTE_PR_ANSWER,
  WEBRTC_PEER_CONNECTION_INTERFACE_SIGNALING_STATE_CLOSED,
};

enum WebrtcPeerConnectionInterfaceIceGatheringState {
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_GATHERING_STATE_NEW,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_GATHERING_STATE_GATHERING,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_GATHERING_STATE_COMPLETE
};

enum WebrtcPeerConnectionInterfacePeerConnectionState {
  WEBRTC_PEER_CONNECTION_INTERFACE_PEER_CONNECTION_STATE_NEW,
  WEBRTC_PEER_CONNECTION_INTERFACE_PEER_CONNECTION_STATE_CONNECTING,
  WEBRTC_PEER_CONNECTION_INTERFACE_PEER_CONNECTION_STATE_CONNECTED,
  WEBRTC_PEER_CONNECTION_INTERFACE_PEER_CONNECTION_STATE_DISCONNECTED,
  WEBRTC_PEER_CONNECTION_INTERFACE_PEER_CONNECTION_STATE_FAILED,
  WEBRTC_PEER_CONNECTION_INTERFACE_PEER_CONNECTION_STATE_CLOSED,
};

enum WebrtcPeerConnectionInterfaceIceConnectionState {
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_CONNECTION_STATE_NEW,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_CONNECTION_STATE_CHECKING,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_CONNECTION_STATE_CONNECTED,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_CONNECTION_STATE_COMPLETED,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_CONNECTION_STATE_FAILED,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_CONNECTION_STATE_DISCONNECTED,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_CONNECTION_STATE_CLOSED,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_CONNECTION_STATE_MAX,
};

enum WebrtcPeerConnectionInterfaceTlsCertPolicy {
  WEBRTC_PEER_CONNECTION_INTERFACE_TLS_CERT_POLICY_SECURE,
  WEBRTC_PEER_CONNECTION_INTERFACE_TLS_CERT_POLICY_INSECURE_NO_CHECK,
};

enum WebrtcPeerConnectionInterfaceIceTransportsType {
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_TRANSPORTS_TYPE_NONE,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_TRANSPORTS_TYPE_RELAY,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_TRANSPORTS_TYPE_NO_HOST,
  WEBRTC_PEER_CONNECTION_INTERFACE_ICE_TRANSPORTS_TYPE_ALL
};

enum WebrtcPeerConnectionInterfaceBundlePolicy {
  WEBRTC_PEER_CONNECTION_INTERFACE_BUNDLE_POLICY_BALANCED,
  WEBRTC_PEER_CONNECTION_INTERFACE_BUNDLE_POLICY_MAX_BUNDLE,
  WEBRTC_PEER_CONNECTION_INTERFACE_BUNDLE_POLICY_MAX_COMPAT
};

enum WebrtcPeerConnectionInterfaceRtcpMuxPolicy {
  WEBRTC_PEER_CONNECTION_INTERFACE_RTCP_MUX_POLICY_NEGOTIATE,
  WEBRTC_PEER_CONNECTION_INTERFACE_RTCP_MUX_POLICY_REQUIRE,
};

// See: https://www.w3.org/TR/webrtc/#idl-def-rtcofferansweroptions
struct WebrtcPeerConnectionInterfaceRTCOfferAnswerOptions {
#ifdef __cplusplus
  static const int kUndefined = -1;
  static const int kMaxOfferToReceiveMedia = 1;

  // The default value for constraint offerToReceiveX:true.
  static const int kOfferToReceiveMediaTrue = 1;

  WebrtcPeerConnectionInterfaceRTCOfferAnswerOptions()
      : WebrtcPeerConnectionInterfaceRTCOfferAnswerOptions(kUndefined,
                                                           kUndefined,
                                                           true,
                                                           false,
                                                           true) {}

  WebrtcPeerConnectionInterfaceRTCOfferAnswerOptions(
      int offer_to_receive_video,
      int offer_to_receive_audio,
      bool voice_activity_detection,
      bool ice_restart,
      bool use_rtp_mux)
      : offer_to_receive_video(offer_to_receive_video),
        offer_to_receive_audio(offer_to_receive_audio),
        voice_activity_detection(voice_activity_detection),
        ice_restart(ice_restart),
        use_rtp_mux(use_rtp_mux),
        raw_packetization_for_video(false),
        num_simulcast_layers(1),
        use_obsolete_sctp_sdp(false) {}
#endif

  // These options are left as backwards compatibility for clients who need
  // "Plan B" semantics. Clients who have switched to "Unified Plan" semantics
  // should use the RtpTransceiver API (AddTransceiver) instead.
  //
  // offer_to_receive_X set to 1 will cause a media description to be
  // generated in the offer, even if no tracks of that type have been added.
  // Values greater than 1 are treated the same.
  //
  // If set to 0, the generated directional attribute will not include the
  // "recv" direction (meaning it will be "sendonly" or "inactive".
  int offer_to_receive_video;
  int offer_to_receive_audio;

  bool voice_activity_detection;
  bool ice_restart;

  // If true, will offer to BUNDLE audio/video/data together. Not to be
  // confused with RTCP mux (multiplexing RTP and RTCP together).
  bool use_rtp_mux;

  // If true, "a=packetization:<payload_type> raw" attribute will be offered
  // in the SDP for all video payload and accepted in the answer if offered.
  bool raw_packetization_for_video;

  // This will apply to all video tracks with a Plan B SDP offer/answer.
  int num_simulcast_layers;

  // If true: Use SDP format from draft-ietf-mmusic-scdp-sdp-03
  // If false: Use SDP format from draft-ietf-mmusic-sdp-sdp-26 or later
  bool use_obsolete_sctp_sdp;
};

#ifdef __cplusplus
#include "api/peer_connection_interface.h"

extern "C" {
#endif

RTC_C_CLASS(webrtc::PeerConnectionObserver, WebrtcPeerConnectionObserver)

RTC_C_CLASS(webrtc::PeerConnectionInterface, WebrtcPeerConnectionInterface)

RTC_C_CLASS(webrtc::PeerConnectionFactoryInterface,
            WebrtcPeerConnectionFactoryInterface)

struct WebrtcPeerConnectionInterfaceAddTrackResult {
  WebrtcRTCError* error;
  WebrtcRtpSenderInterface* value;
};

struct WebrtcPeerConnectionInterfaceIceServer {
  const char* uri;
  const char** urls;
  size_t urls_size;
  const char* username;
  const char* password;
  enum WebrtcPeerConnectionInterfaceTlsCertPolicy tls_cert_policy;
  const char* hostname;
  const char** tls_alpn_protocols;
  size_t tls_alpn_protocols_size;
  const char** tls_elliptic_curves;
  size_t tls_elliptic_curves_size;
};

struct WebrtcPeerConnectionDependencies {
  WebrtcPeerConnectionObserver* observer;
  CricketPortAllocator* allocator;
  WebrtcAsyncResolverFactory* async_resolver_factory;
  RtcRTCCertificateGeneratorInterface* cert_generator;
  RtcSSLCertificateVerifier* tls_cert_verifier;
  WebrtcVideoBitrateAllocatorFactory* video_bitrate_allocator_factory;
};

struct WebrtcPeerConnectionInterfaceRTCConfiguration {
  int flags;
  int audio_rtcp_report_interval_ms;
  int video_rtcp_report_interval_ms;
  struct WebrtcPeerConnectionInterfaceIceServer* servers;
  size_t servers_size;
  enum WebrtcPeerConnectionInterfaceIceTransportsType type;
  enum WebrtcPeerConnectionInterfaceBundlePolicy bundle_policy;
  enum WebrtcPeerConnectionInterfaceRtcpMuxPolicy rtcp_mux_policy;
  RtcRTCCertificate** certificates;
  size_t certificates_size;
  int ice_candidate_pool_size;
  enum WebrtcSdpSemantics sdp_semantics;
};

struct WebrtcPeerConnectionObserverFunctions {
  void (*on_destruction)(void*);

  void (*on_signaling_change)(void*,
                              enum WebrtcPeerConnectionInterfaceSignalingState);

  void (*on_add_stream)(void*, WebrtcMediaStreamInterface*);

  void (*on_remove_stream)(void*, WebrtcMediaStreamInterface*);

  void (*on_data_channel)(void*, WebrtcDataChannelInterface*);

  void (*on_renegotiation_needed)(void*);

  void (*on_ice_connection_change)(
      void*,
      enum WebrtcPeerConnectionInterfaceIceConnectionState);

  void (*on_standardized_ice_connection_change)(
      void*,
      enum WebrtcPeerConnectionInterfaceIceConnectionState);

  void (*on_connection_change)(
      void*,
      enum WebrtcPeerConnectionInterfacePeerConnectionState);

  void (*on_ice_gathering_change)(
      void*,
      enum WebrtcPeerConnectionInterfaceIceGatheringState);

  void (*on_ice_candidate)(void*, const WebrtcIceCandidateInterface*);

  void (*on_ice_candidates_removed)(void*, const CricketCandidate**, size_t);

  void (*on_ice_connection_receiving_change)(void*, bool);

  void (*on_add_track)(void*,
                       WebrtcRtpReceiverInterface*,
                       WebrtcMediaStreamInterface**,
                       size_t);

  void (*on_track)(void*, WebrtcRtpTransceiverInterface*);

  void (*on_remove_track)(void*, WebrtcRtpReceiverInterface*);

  void (*on_interesting_usage)(void*, int usage_pattern);
};

RTC_EXPORT void webrtcDeletePeerConnectionObserver(
    WebrtcPeerConnectionObserver* observer);

RTC_EXPORT WebrtcPeerConnectionObserver* webrtcNewPeerConnectionObserver(
    void* context,
    const struct WebrtcPeerConnectionObserverFunctions* functions);

RTC_EXPORT WebrtcPeerConnectionInterface*
webrtcPeerConnectionFactoryInterfaceCreatePeerConnection(
    WebrtcPeerConnectionFactoryInterface* factory,
    const struct WebrtcPeerConnectionInterfaceRTCConfiguration* cconfiguration,
    const struct WebrtcPeerConnectionDependencies* cdependencies);

RTC_EXPORT WebrtcAudioTrackInterface*
webrtcPeerConnectionFactoryInterfaceCreateAudioTrack(
    WebrtcPeerConnectionFactoryInterface* factory,
    const char* label,
    WebrtcAudioSourceInterface* source);

RTC_EXPORT WebrtcVideoTrackInterface*
webrtcPeerConnectionFactoryInterfaceCreateVideoTrack(
    WebrtcPeerConnectionFactoryInterface* factory,
    const char* label,
    WebrtcVideoTrackSourceInterface* source);

RTC_EXPORT void webrtcPeerConnectionFactoryInterfaceRelease(
    const WebrtcPeerConnectionFactoryInterface* factory);

RTC_EXPORT struct WebrtcPeerConnectionInterfaceAddTrackResult
webrtcPeerConnectionInterfaceAddTrack(WebrtcPeerConnectionInterface* connection,
                                      WebrtcMediaStreamTrackInterface* track,
                                      const char* const* data,
                                      size_t size);

RTC_EXPORT void webrtcPeerConnectionInterfaceClose(
    WebrtcPeerConnectionInterface* connection);

RTC_EXPORT void webrtcPeerConnectionInterfaceCreateAnswer(
    WebrtcPeerConnectionInterface* connection,
    WebrtcCreateSessionDescriptionObserver* observer,
    const struct WebrtcPeerConnectionInterfaceRTCOfferAnswerOptions* options);

RTC_EXPORT void webrtcPeerConnectionInterfaceCreateOffer(
    WebrtcPeerConnectionInterface* connection,
    WebrtcCreateSessionDescriptionObserver* observer,
    const struct WebrtcPeerConnectionInterfaceRTCOfferAnswerOptions* options);

RTC_EXPORT void webrtcPeerConnectionInterfaceRelease(
    const WebrtcPeerConnectionInterface* connection);

RTC_EXPORT void webrtcPeerConnectionInterfaceSetAudioRecording(
    WebrtcPeerConnectionInterface* connection,
    bool recording);

RTC_EXPORT void webrtcPeerConnectionInterfaceSetLocalDescription(
    WebrtcPeerConnectionInterface* connection,
    WebrtcSetSessionDescriptionObserver* observer,
    WebrtcSessionDescriptionInterface* desc);

RTC_EXPORT void webrtcPeerConnectionInterfaceSetRemoteDescription(
    WebrtcPeerConnectionInterface* connection,
    WebrtcSetSessionDescriptionObserver* observer,
    WebrtcSessionDescriptionInterface* desc);

#ifdef __cplusplus
}
#endif

#endif
