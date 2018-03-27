/*
 *  Copyright 2017 The WebRTC project authors. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree. An additional intellectual property rights grant can be found
 *  in the file PATENTS.  All contributing project authors may
 *  be found in the AUTHORS file in the root of the source tree.
 */

#ifndef PC_DTLSSRTPTRANSPORT_H_
#define PC_DTLSSRTPTRANSPORT_H_

#include <memory>
#include <string>
#include <vector>

#include "p2p/base/dtlstransportinternal.h"
#include "pc/srtptransport.h"
#include "rtc_base/buffer.h"

namespace webrtc {

// The subclass of SrtpTransport is used for DTLS-SRTP. When the DTLS handshake
// is finished, it extracts the keying materials from DtlsTransport and
// configures the SrtpSessions in the base class.
class DtlsSrtpTransport : public SrtpTransport {
 public:
  explicit DtlsSrtpTransport(bool rtcp_mux_enabled);

  // Set P2P layer RTP/RTCP DtlsTransports. When using RTCP-muxing,
  // |rtcp_dtls_transport| is null.
  void SetDtlsTransports(cricket::DtlsTransportInternal* rtp_dtls_transport,
                         cricket::DtlsTransportInternal* rtcp_dtls_transport);

  void SetRtcpMuxEnabled(bool enable) override;

  // Set the header extension ids that should be encrypted.
  void UpdateSendEncryptedHeaderExtensionIds(
      const std::vector<int>& send_extension_ids);

  void UpdateRecvEncryptedHeaderExtensionIds(
      const std::vector<int>& recv_extension_ids);

  // TODO(zhihuang): Remove this when we remove RtpTransportAdapter.
  RtpTransportAdapter* GetInternal() override { return nullptr; }

  sigslot::signal2<DtlsSrtpTransport*, bool> SignalDtlsSrtpSetupFailure;

 private:
  bool IsDtlsActive();
  bool IsDtlsConnected();
  bool IsDtlsWritable();
  bool DtlsHandshakeCompleted();
  void MaybeSetupDtlsSrtp();
  void SetupRtpDtlsSrtp();
  void SetupRtcpDtlsSrtp();
  bool ExtractParams(cricket::DtlsTransportInternal* dtls_transport,
                     int* selected_crypto_suite,
                     rtc::ZeroOnFreeBuffer<unsigned char>* send_key,
                     rtc::ZeroOnFreeBuffer<unsigned char>* recv_key);
  void SetDtlsTransport(cricket::DtlsTransportInternal* new_dtls_transport,
                        cricket::DtlsTransportInternal** old_dtls_transport);
  void SetRtpDtlsTransport(cricket::DtlsTransportInternal* rtp_dtls_transport);
  void SetRtcpDtlsTransport(
      cricket::DtlsTransportInternal* rtcp_dtls_transport);
  void UpdateWritableStateAndMaybeSetupDtlsSrtp();
  // Set the writability and fire the SignalWritableState if the writability
  // changes.
  void SetWritable(bool writable);

  void OnDtlsState(cricket::DtlsTransportInternal* dtls_transport,
                   cricket::DtlsTransportState state);

  // Override the RtpTransport::OnWritableState.
  void OnWritableState(rtc::PacketTransportInternal* packet_transport) override;

  bool writable_ = false;
  // Owned by the TransportController.
  cricket::DtlsTransportInternal* rtp_dtls_transport_ = nullptr;
  cricket::DtlsTransportInternal* rtcp_dtls_transport_ = nullptr;

  // The encrypted header extension IDs.
  rtc::Optional<std::vector<int>> send_extension_ids_;
  rtc::Optional<std::vector<int>> recv_extension_ids_;
};

}  // namespace webrtc

#endif  // PC_DTLSSRTPTRANSPORT_H_
