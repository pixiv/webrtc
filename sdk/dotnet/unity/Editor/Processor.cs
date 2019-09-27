/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#if UNITY_IOS

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.iOS.Xcode;

namespace Pixiv.Webrtc
{
    internal static class Processor
    {
        [UnityEditor.Callbacks.PostProcessBuild]
        private static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            var projectPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();

            project.ReadFromFile(projectPath);

            var unityTarget = project.TargetGuidByName(PBXProject.GetUnityTargetName());
            project.AddShellScriptBuildPhase(unityTarget, "WebRTC: Extract required code", "/bin/sh", @"FAT_EXECUTABLE=""$TARGET_BUILD_DIR/$WRAPPER_NAME/Frameworks/WebRTC.framework/WebRTC""
THIN_EXECUTABLES=()

for ARCH in $ARCHS
do
    lipo -extract ""$ARCH"" ""$FAT_EXECUTABLE"" -o ""$FAT_EXECUTABLE-$ARCH""
    THIN_EXECUTABLES+=(""$FAT_EXECUTABLE-$ARCH"")
done

lipo -o ""$FAT_EXECUTABLE"" -create ""${THIN_EXECUTABLES[@]}""
rm ""${THIN_EXECUTABLES[@]}""
");

            project.WriteToFile(projectPath);
        }
    }
}

#endif
