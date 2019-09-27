/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;
using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

internal sealed class VideoTrackSource : Pixiv.Rtc.AdaptedVideoTrackSource
{
    public void Update()
    {
        var width = Screen.width;
        var height = Screen.height;
        var time = (long)(Time.unscaledTime * 1e6);

        if (!AdaptFrame(
            width,
            height,
            time,
            out var outWidth,
            out var outHeight,
            out var cropWidth,
            out var cropHeight,
            out var cropX,
            out var cropY))
        {
            return;
        }

        var texture = ScreenCapture.CaptureScreenshotAsTexture(1);

        try
        {
            var data = texture.GetRawTextureData<Color32>();
            var greater = width * height;
            var rowLesser = width;

            while (rowLesser < greater)
            {
                var lesser = rowLesser;

                while (lesser > rowLesser - width)
                {
                    lesser--;
                    greater--;
                    var temporary = data[greater];
                    data[greater] = data[lesser];
                    data[lesser] = temporary;
                }

                rowLesser += width;
            }

            using (var i420 = I420Buffer.Create(cropWidth, cropHeight))
            {
                unsafe
                {
                    if (!Libyuv.ConvertToI420(
                        Libyuv.VideoType.ABGR,
                        data.Length,
                        width,
                        height,
                        (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr(data),
                        i420,
                        cropX,
                        cropY))
                    {
                        Debug.LogError("Failed to convert to I420");
                        return;
                    }
                }

                using (var builder = new VideoFrame.Builder())
                {
                    using (var scaled = I420Buffer.Create(outWidth, outHeight))
                    {
                        scaled.ScaleFrom(i420);
                        builder.SetVideoFrameBuffer(scaled);
                    }

                    builder.SetRotation(0);
                    builder.SetTimestampMs(time);

                    using (var frame = builder.Build())
                    {
                        OnFrame(frame);
                    }
                }
            }
        }
        finally
        {
            Texture2D.Destroy(texture);
        }
    }

    public override MediaSourceInterface.SourceState State =>
        MediaSourceInterface.SourceState.Live;

    public override bool Remote => false;
    public override bool IsScreencast => true;
    public override bool? NeedsDenoising => null;
}
