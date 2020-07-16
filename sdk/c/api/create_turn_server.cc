/*
 *  Copyright 2012 The WebRTC Project Authors. All rights reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree. An additional intellectual property rights grant can be found
 *  in the file PATENTS.  All contributing project authors may
 *  be found in the AUTHORS file in the root of the source tree.
 */
#include <iostream>
#include <map>
#include <string>
#include <utility>

#include "p2p/base/basic_packet_socket_factory.h"
#include "p2p/base/port_interface.h"
#include "p2p/base/turn_server.h"
#include "rtc_base/async_udp_socket.h"
#include "rtc_base/ip_address.h"
#include "rtc_base/socket_address.h"
#include "rtc_base/socket_server.h"
#include "rtc_base/thread.h"
#include "rtc_base/string_encode.h"
#include "create_turn_server.h"


namespace {
class TurnFileAuth : public cricket::TurnAuthInterface {
 public:
  explicit TurnFileAuth(std::map<std::string, std::string> name_to_key)
      : name_to_key_(std::move(name_to_key)) {}

  virtual bool GetKey(const std::string& username,
                      const std::string& realm,
                      std::string* key) {
    auto it = name_to_key_.find(username);
    std::cerr << "TURN: Welcome " << username << std::endl;
    if (it == name_to_key_.end()) {
      return false;
    }
    *key = it->second;
    return true;
  }

 private:
  const std::map<std::string, std::string> name_to_key_;
};

}  // namespace

RTC_EXPORT extern "C" void webrtcCreateTURNServer(
	RtcThread* network_thread,
	const char* local_addr,
	const char* ip_addr,
	size_t min_port,
	size_t max_port) {

  rtc::Thread* thread = rtc::ToCplusplus(network_thread);
  rtc::SocketAddress int_addr;
  if (!int_addr.FromString(local_addr)) {
    std::cerr << "Unable to parse socket address: " << local_addr << std::endl;
    return;
  }

  rtc::IPAddress ext_addr;
  if (!IPFromString(ip_addr, &ext_addr)) {
    std::cerr << "Unable to parse IP address: " << ip_addr << std::endl;
    return;
  }

  rtc::AsyncUDPSocket* int_socket =
      rtc::AsyncUDPSocket::Create(thread->socketserver(), int_addr);
  if (!int_socket) {
    std::cerr << "Failed to create a UDP socket bound at " << int_addr.ToString()
              << std::endl;
    return;
  }

  cricket::TurnServer server(thread);
  std::string pwd = "a8a16ccd399c0a716e1bca6be0016d6f";
  char buf[32];
  size_t len = rtc::hex_decode(buf, sizeof(buf), pwd.data(),pwd.size());
  std::map<std::string, std::string> usrs = {
	{"user",std::string(buf, len)}
  };
  TurnFileAuth auth(usrs);
  server.set_realm("Agent");
  server.set_software("Agent");
  server.set_port_range(min_port,max_port);
  server.set_auth_hook(&auth);
  server.AddInternalSocket(int_socket, cricket::PROTO_UDP);
  server.SetExternalSocketFactory(new rtc::BasicPacketSocketFactory(),
                                  rtc::SocketAddress(ext_addr, 0));
  thread->Run();
}
