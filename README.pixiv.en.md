This is a fork of WebRTC made by [pixiv Inc](https://www.pixiv.co.jp/).

# Changes
- APIs necessary to deliver recorded audio data on iOS's broadcast extension is
  added.
- An audio unit component other than the Voice-Processing I/O unit can be used
  to record audio on iOS.
- **BREAKING CHANGE** A patch to remove the implicit microphone permission
  requirement for iOS is applied. The original patch is:
  https://github.com/shiguredo-webrtc-build/webrtc-build/blob/m86.4240.10.0/patches/ios_manual_audio_input.patch

# Delivering audio data on iOS's broadcast extension

## The problem and solution

A broadcast extension allows to broadcast your screen from iOS. However, an
extension is an environment restricted compared to normal applications, and
lacks the Audio Unit framework, which is required by the audio device module
for iOS of WebRTC.

A broadcast extension can still receive sounds of applications and one recorded
with microphone via [`RPBroadcastSampleHandler`](https://developer.apple.com/documentation/replaykit/rpbroadcastsamplehandler)
object. This fork fixes the audio device module on environments without the
Audio Unit framework and adds interfaces to deliver data received via
[`RPBroadcastSampleHandler`](https://developer.apple.com/documentation/replaykit/rpbroadcastsamplehandler).

## Relevant interfaces

Use interfaces described here to deliver audio with your broadcast extension.

### `[RTCPeerConnectionFactory initWithEncoderFactory: decoderFactory: audioDeviceModule:]`

This method is same to `[RTCPeerConnectionFactory initWithEncoderFactory: decoderFactory:]`,
but has one more parameter, `audioDeviceModule`, whose type is `RTCAudioDeviceModule`.

Use this method to provide `RTCAudioDeviceModule` delivering audio data.

### `RTCAudioDeviceModule`

This class is defined in `RTCAudioDeviceModule.h`. It wraps the native
implementation delivering audio data. The implementation is extended to
receive data not from the Audio Unit framework.

### `[RTCAudioDeviceModule deliverRecordedData:]`

This method actually delivers the data your provide. The type of the parameter
is [`CMSampleBuffer`](https://developer.apple.com/documentation/coremedia/cmsamplebuffer-u71).

Provide [`CMSampleBuffer`](https://developer.apple.com/documentation/coremedia/cmsamplebuffer-u71)
acquired with [`[RPBroadcastSampleHandler processSampleBuffer: with:]`](https://developer.apple.com/documentation/replaykit/rpbroadcastsamplehandler/2123045-processsamplebuffer).

You must keep calling this method once you added an audio stream. However,
broadcast extension does not provide audio buffer with
`RPSampleBufferType.audioMic` while the microphone is disabled. Therefore,
you have to provide dummy samples before the microphone is enabled if you
are broadcasting the audio recorded with the microphone.

Note that `NSTimer` and `DispatchSourceTimer` cannot be used to implement
such a mechanism, at least on iOS 12.2. `DispatchQueue.asyncAfter` is
a nice alternative.

The audio unit must be uninitialized and disabled to use this method.

### `RTCAudioSession.useManualAudio`

Set `YES` on broadcast extension. That prevents initializing the audio unit.

### `RTCAudioSession.isAudioEnabled`

Set `NO` on broadcast extension. That disables the audio unit.

# Using an audio unit component other than the Voice-Processing I/O unit on iOS

The Voice-Processing I/O unit has features favorable for two-way communication
such as echo suppression. These features may not be needed for other scenarios,
and even cause problems like reduced volume.

Use another audio unit component to cover such scenarios.

## `RTCAudioDeviceModule.audioUnitSubType`

This is a property holding the audio unit subtype. The type is `OSType`. Refer to:

https://developer.apple.com/documentation/audiotoolbox/audio_unit_v2_c_api/1584139-input_output_audio_unit_subtypes
