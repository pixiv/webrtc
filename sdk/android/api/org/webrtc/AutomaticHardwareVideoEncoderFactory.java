/*
 *  Copyright 2020 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

package org.webrtc;

import android.annotation.TargetApi;
import android.media.MediaCodecInfo;
import android.media.MediaCodecList;
import android.media.MediaFormat;
import android.support.annotation.Nullable;
import java.util.ArrayList;
import java.util.List;

/** Factory for android hardware video encoders automatically chosen by the platform. */
@TargetApi(21)
public class AutomaticHardwareVideoEncoderFactory implements VideoEncoderFactory {
  private static final String TAG = "AutomaticHardwareVideoEncoderFactory";

  @Nullable private final EglBase14.Context sharedContext;

  /**
   * Creates a HardwareVideoEncoderFactory that supports surface texture encoding.
   *
   * @param sharedContext The textures generated will be accessible from this context. May be null,
   *                      this disables texture support.
   */
  public AutomaticHardwareVideoEncoderFactory(EglBase.Context sharedContext) {
    // Texture mode requires EglBase14.
    if (sharedContext instanceof EglBase14.Context) {
      this.sharedContext = (EglBase14.Context) sharedContext;
    } else {
      Logging.w(TAG, "No shared EglBase.Context.  Encoders will not use texture mode.");
      this.sharedContext = null;
    }
  }

  @Nullable
  @Override
  public VideoEncoder createEncoder(VideoCodecInfo info) {
    MediaCodecList list = new MediaCodecList(MediaCodecList.REGULAR_CODECS);
    VideoCodecType type = VideoCodecType.valueOf(info.name);
    MediaFormat format = HardwareVideoEncoderUtils.createMediaFormat(type, info.params);

    String codecName = list.findEncoderForFormat(format);
    if (codecName == null) {
      return null;
    }

    MediaCodecInfo[] mediaCodecInfos = list.getCodecInfos();

    // OMX.hisi.video.encoder.avc implemented in Kirin 970 and 980 occasionally
    // fails, which makes getOutputBuffers() continuously timeout. Fortunately,
    // those SoCs are so new that devices with them are likely to have modern,
    // reliable OMX.google.h264.encoder.
    if (codecName.equals("OMX.hisi.video.encoder.avc")) {
      for (MediaCodecInfo mediaCodecInfo : mediaCodecInfos) {
        if ("OMX.google.h264.encoder".equals(mediaCodecInfo.getName())) {
          codecName = "OMX.google.h264.encoder";
          break;
        }
      }
    }

    int keyFrameIntervalSec = HardwareVideoEncoderUtils.getKeyFrameIntervalSec(type);
    int forcedKeyFrameIntervalMs = HardwareVideoEncoderUtils.getForcedKeyFrameIntervalMs(type, codecName);
    BitrateAdjuster bitrateAdjuster = HardwareVideoEncoderUtils.createBitrateAdjuster(type, codecName);

    for (MediaCodecInfo mediaCodecInfo : mediaCodecInfos) {
      if (codecName.equals(mediaCodecInfo.getName())) {
        MediaCodecInfo.CodecCapabilities capabilities =
          mediaCodecInfo.getCapabilitiesForType(type.mimeType());
        Integer surfaceColorFormat = MediaCodecUtils.selectColorFormat(
            MediaCodecUtils.TEXTURE_COLOR_FORMATS, capabilities);
        Integer yuvColorFormat = MediaCodecUtils.selectColorFormat(
            MediaCodecUtils.ENCODER_COLOR_FORMATS, capabilities);

        return new HardwareVideoEncoder(new MediaCodecWrapperFactoryImpl(), codecName, type,
          surfaceColorFormat, yuvColorFormat, info.params, keyFrameIntervalSec,
          forcedKeyFrameIntervalMs, bitrateAdjuster, sharedContext);
      }
    }

    return null;
  }

  @Override
  public VideoCodecInfo[] getSupportedCodecs() {
    MediaCodecList list = new MediaCodecList(MediaCodecList.REGULAR_CODECS);
    List<VideoCodecInfo> supportedCodecInfos = new ArrayList<VideoCodecInfo>();
    // Generate a list of supported codecs in order of preference:
    // VP8, VP9, H264 (high profile), and H264 (baseline profile).
    for (VideoCodecType type :
        new VideoCodecType[] {VideoCodecType.VP8, VideoCodecType.VP9, VideoCodecType.H264}) {
      if (type == VideoCodecType.H264) {
        // TODO(sakal): Always add H264 HP once WebRTC correctly removes codecs that are not
        // supported by the decoder.
        MediaFormat highProfileFormat = HardwareVideoEncoderUtils.createH264HighProfileMediaFormat();
        if (list.findEncoderForFormat(highProfileFormat) != null) {
          supportedCodecInfos.add(new VideoCodecInfo(
              type.name(), MediaCodecUtils.getCodecProperties(type, /* highProfile= */ true)));
        }
      }
      MediaFormat format = HardwareVideoEncoderUtils.createMediaFormat(type);
      if (list.findEncoderForFormat(format) != null) {
        supportedCodecInfos.add(new VideoCodecInfo(
            type.name(), MediaCodecUtils.getCodecProperties(type, /* highProfile= */ false)));
      }
    }

    return supportedCodecInfos.toArray(new VideoCodecInfo[supportedCodecInfos.size()]);
  }
}
