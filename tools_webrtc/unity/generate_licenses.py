#!/usr/bin/env python

# Copyright (c) 2019 pixiv Inc. All Rights Reserved.
#
# Use of this source code is governed by a BSD-style license
# that can be found in the LICENSE file in the root of the source
# tree.

from imp import find_module, load_module
from sys import argv
from os import path

libs = [path.join(path.dirname(__file__), '../libs')]
found_generate_licenses = find_module('generate_licenses', libs)
generate_licenses = load_module('generate_licenses', *found_generate_licenses)
android = False
ios = False
generics = []

for arg in argv[3:]:
  if arg == 'Android':
    android = True
  elif arg == 'Ios':
    ios = True
  else:
    generics.append(path.join(argv[1], {
      'LinuxX64': 'linux_x64',
      'MacX64': 'mac_x64',
      'WinX64': 'win_x64',
      'WinX86': 'win_x86'
    }[arg]))

builder = \
  generate_licenses.LicenseBuilder(generics, [':jingle_peerconnection_so'])

if android:
  builder.AddBuild([
    path.join(argv[1], 'android/armeabi-v7a'),
    path.join(argv[1], 'android/arm64-v8a')
  ], [
    'sdk/android:libwebrtc',
    'sdk/android:libjingle_peerconnection_so'
  ])

if ios:
  builder.AddBuild([
    path.join(argv[1], 'ios_libs/arm_libs'),
    path.join(argv[1], 'ios_libs/arm64_libs'),
    path.join(argv[1], 'ios_libs/x64_libs')
  ], ['sdk:framework_objc'])

builder.GenerateLicenseText(argv[2])
