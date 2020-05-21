/*
 *  Copyright 2020 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

package org.webrtc;

import static org.webrtc.MediaCodecUtils.EXYNOS_PREFIX;
import static org.webrtc.MediaCodecUtils.QCOM_PREFIX;

import android.media.MediaFormat;
import android.os.Build;
import java.util.Map;

class HardwareVideoEncoderUtils {
  private static final String TAG = "HardwareVideoEncoderUtils";

  // Forced key frame interval - used to reduce color distortions on Qualcomm platforms.
  private static final int QCOM_VP8_KEY_FRAME_INTERVAL_ANDROID_L_MS = 15000;
  private static final int QCOM_VP8_KEY_FRAME_INTERVAL_ANDROID_M_MS = 20000;
  private static final int QCOM_VP8_KEY_FRAME_INTERVAL_ANDROID_N_MS = 15000;

  private static final int VIDEO_AVC_PROFILE_HIGH = 8;
  private static final int VIDEO_AVC_LEVEL_3 = 0x100;

  public static int getKeyFrameIntervalSec(VideoCodecType type) {
    switch (type) {
      case VP8: // Fallthrough intended.
      case VP9:
        return 100;
      case H264:
        return 20;
    }
    throw new IllegalArgumentException("Unsupported VideoCodecType " + type);
  }

  public static int getForcedKeyFrameIntervalMs(VideoCodecType type, String codecName) {
    if (type == VideoCodecType.VP8 && codecName.startsWith(QCOM_PREFIX)) {
      if (Build.VERSION.SDK_INT == Build.VERSION_CODES.LOLLIPOP
          || Build.VERSION.SDK_INT == Build.VERSION_CODES.LOLLIPOP_MR1) {
        return QCOM_VP8_KEY_FRAME_INTERVAL_ANDROID_L_MS;
      } else if (Build.VERSION.SDK_INT == Build.VERSION_CODES.M) {
        return QCOM_VP8_KEY_FRAME_INTERVAL_ANDROID_M_MS;
      } else if (Build.VERSION.SDK_INT > Build.VERSION_CODES.M) {
        return QCOM_VP8_KEY_FRAME_INTERVAL_ANDROID_N_MS;
      }
    }
    // Other codecs don't need key frame forcing.
    return 0;
  }

  public static BitrateAdjuster createBitrateAdjuster(VideoCodecType type, String codecName) {
    if (codecName.startsWith(EXYNOS_PREFIX)) {
      if (type == VideoCodecType.VP8) {
        // Exynos VP8 encoders need dynamic bitrate adjustment.
        return new DynamicBitrateAdjuster();
      } else {
        // Exynos VP9 and H264 encoders need framerate-based bitrate adjustment.
        return new FramerateBitrateAdjuster();
      }
    }
    // Other codecs don't need bitrate adjustment.
    return new BaseBitrateAdjuster();
  }

  public static MediaFormat createH264HighProfileMediaFormat() {
    MediaFormat format = createMediaFormat(VideoCodecType.H264);
    format.setInteger("profile", VIDEO_AVC_PROFILE_HIGH);
    format.setInteger("level", VIDEO_AVC_LEVEL_3);
    return format;
  }

  public static MediaFormat createMediaFormat(VideoCodecType codecType) {
    MediaFormat format = new MediaFormat();
    format.setString(MediaFormat.KEY_MIME, codecType.mimeType());
    return format;
  }

  public static MediaFormat createMediaFormat(VideoCodecType codecType, Map<String, String> params) {
    MediaFormat format = new MediaFormat();

    if (codecType == VideoCodecType.H264) {
      String profileLevelId = params.get(VideoCodecInfo.H264_FMTP_PROFILE_LEVEL_ID);
      if (profileLevelId == null) {
        profileLevelId = VideoCodecInfo.H264_CONSTRAINED_BASELINE_3_1;
      }
      switch (profileLevelId) {
        case VideoCodecInfo.H264_CONSTRAINED_HIGH_3_1:
          return createH264HighProfileMediaFormat();
        case VideoCodecInfo.H264_CONSTRAINED_BASELINE_3_1:
          break;
        default:
          Logging.w(TAG, "Unknown profile level id: " + profileLevelId);
      }
    }

    return createMediaFormat(codecType);
  }
}
