#!/bin/sh

# Copyright 2020 pixiv Inc. All Rights Reserved.
#
# Use of this source code is governed by a license that can be
# found in the LICENSE.pixiv file in the root of the source tree.

set -eux
mkdir depot_tools
cd depot_tools
git init
git fetch --depth=1 https://chromium.googlesource.com/chromium/tools/depot_tools.git 624bf6e42543d2b381410baabcb80c0c4d3072b3
git checkout FETCH_HEAD
cd ..
gclient config --name=src --unmanaged "https://github.com/$GITHUB_REPOSITORY.git"
