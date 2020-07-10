/*
 *  Copyright 2012 The WebRTC Project Authors. All rights reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree. An additional intellectual property rights grant can be found
 *  in the file PATENTS.  All contributing project authors may
 *  be found in the AUTHORS file in the root of the source tree.
 */

#include <fstream>
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

extern "C" void
webrtcCreateTURNServer(
	const char* local_addr,
	const char* ip_addr) {

  rtc::SocketAddress int_addr;
  if (!int_addr.FromString(local_addr)) {
	  //local_addr = localhost:8091
    std::cerr << "Unable to parse socket address: " << local_addr << std::endl;
    return;
  }

  rtc::IPAddress ext_addr;
  if (!IPFromString(ip_addr, &ext_addr)) {
	  //ip_addr = localhost
    std::cerr << "Unable to parse IP address: " << ip_addr << std::endl;
    return;
  }

  rtc::Thread* main = rtc::Thread::Current();
  rtc::AsyncUDPSocket* int_socket =
      rtc::AsyncUDPSocket::Create(main->socketserver(), int_addr);
  if (!int_socket) {
    std::cerr << "Failed to create a UDP socket bound at" << int_addr.ToString()
              << std::endl;
    return;
  }

  cricket::TurnServer server(main);

  TurnFileAuth auth(std::map<std::string, std::string>());
  server.set_realm("Agent");
  server.set_software("Agent");
  server.set_auth_hook(&auth);
  server.AddInternalSocket(int_socket, cricket::PROTO_UDP);
  server.SetExternalSocketFactory(new rtc::BasicPacketSocketFactory(),
                                  rtc::SocketAddress(ext_addr, 0));

  std::cout << "Listening internally at " << int_addr.ToString() << std::endl;

  main->Run();
}
