#!/usr/bin/env python

# Copyright (c) 2020 pixiv Inc. All Rights Reserved.
#
# Use of this source code is governed by a BSD-style license
# that can be found in the LICENSE file in the root of the source
# tree.

import sys
from os import path

sys.path.append(path.join(path.dirname(path.realpath(__file__)), 'libs'))
from generate_licenses import LicenseBuilder

builder = LicenseBuilder([], [], None, None, sys.argv[2:])
print(sys.argv[1])
builder.GenerateLicenseText(sys.argv[1])
