This is a fork of WebRTC made by [pixiv Inc](https://www.pixiv.co.jp/).

# Changes

- `org.webrtc.AutomaticHardwareVideoEncoderFactory` is added to Android SDK.

# Using unsupported hardware video encoder on Android

`org.webrtc.VideoEncoderFactory` implementations provided by the upstream only
accept hardware encoders of a limited set of vendors.
`AutomaticHardwareVideoEncoderFactory` uses hardware video encoder
"automatically" chosen by the platform. It has pros and cons due to the nature.

Pros:
- Potentially supports more video encodings like H.264, which WebRTC's embedded
  encoder for is not enabled by default and requires license fee.
- Enables potentially efficient hardware video encoder.

Cons:
- The encoder chosen by the factory is unsupported by the upstream and may have
  quirks. (`OMX.hisi.video.encoder.avc` is known to have some quirk and is
  explicitly replaced by `OMX.google.h264.encoder`, for example.)
- Available only on API level 21+.

Therefore, this encoder factory implementation is preferred only in some special
situations. An example is a case employing HLS transcoding feature provided by
[ImageFlux Live Streaming](https://www.sakura.ad.jp/services/imageflux/livestreaming/).
