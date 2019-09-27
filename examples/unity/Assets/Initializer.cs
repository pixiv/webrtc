/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

#if UNITY_ANDROID && !UNITY_EDITOR

using UnityEngine;

internal static class Initializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoadRuntimeMethod()
    {
        using (var playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var factoryClass = new AndroidJavaClass("org.webrtc.PeerConnectionFactory"))
        using (var optionsClass = new AndroidJavaClass("org.webrtc.PeerConnectionFactory$InitializationOptions"))
        using (var activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var builder = optionsClass.CallStatic<AndroidJavaObject>("builder", activity))
        using (var options = builder.Call<AndroidJavaObject>("createInitializationOptions"))
        {
            factoryClass.CallStatic("initialize", options);
        }
    }
}

#endif
