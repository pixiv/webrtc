#!/bin/sh

# Copyright 2020 pixiv Inc. All Rights Reserved.
#
# Use of this source code is governed by a license that can be
# found in the LICENSE.pixiv file in the root of the source tree.

mkdir artifacts
mv src/sdk/dotnet/unity/obj/Release/desc artifacts
mv src/sdk/dotnet/unity/bin/Release artifacts/package
