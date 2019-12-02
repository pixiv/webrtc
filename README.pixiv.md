This is a fork of WebRTC made by [pixiv Inc](https://www.pixiv.co.jp/).

# Changes
- `rtc_symbol_enable_export` is compatible with Windows build.
- `tools_webrtc/libs/generate_licenses.py` is compatible with Windows build.
- Partial C bindings are introduced at `sdk/c`
- .NET bindings based on C bindings are introduced at `sdk/dotnet`
  - `sdk/dotnet/unity` is a Unity package. Build with MSBuild.
  - `example/unity` is an example to connect to
    [WebRTC SFU Sora](https://sora.shiguredo.jp/) on Unity.
- Native APIs are availble for Android library.
- `webrtc::VideoBuffer`, a simple `webrtc::VideoSinkInterface` which allows you
  to retrieve the last frame at an arbitrary time is introduced.
- APIs necessary to deliver recorded audio data on iOS's broadcast extension is
  added.

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

# .NET bindings

## Features

There are a few .NET binding libraries for WebRTC. Such examples include:

- [WebRTC for the Universal Windows Platform (webrtc-uwp)](https://webrtc-uwp.github.io/)
- [MixedReality-WebRTC](https://github.com/microsoft/MixedReality-WebRTC), based on webrtc-uwp
- [Unity Render Streaming](https://github.com/Unity-Technologies/UnityRenderStreaming)
- The unity plugin located at `sdk/unityplugin`

The bindings provided by this fork has following *unique* features:

- Portable
  - .NET Standard 2.0
  - Satisfies additional requirements of [Mono's full AOT](https://www.mono-project.com/docs/advanced/aot/#full-aot)
    and [IL2CPP](https://docs.unity3d.com/Manual/IL2CPP.html).
  - No Unity or other runtime dependency.
  - No operating system requirements other than WebRTC requirements.
- Up-to-date
  - [Unified Plan](https://webrtc.org/web-apis/chrome/unified-plan/)
  - Available as a Unity [package](https://docs.unity3d.com/Manual/Packages.html),
    whose format is an enhancement of traditional Unity asset packages.
- Extensible
  - Its interface-based API design allows to create bindings for C++ code using
    native API.
  - As low-level as native API.
    - No assumption for inputs/outputs. Use [Unity's RenderTexture](https://docs.unity3d.com/Manual/class-RenderTexture.html)
      or whatever.
    - No assumption for signaling. You can use existing signaling server
      implementations such as [Sora](https://sora.shiguredo.jp/) and SaaS such
      as [ImageFlux Live Streaming](https://www.sakura.ad.jp/services/imageflux/livestreaming/).
    - This also means you need more code to integrate it to an application.
- Provides comprehensive license file
  - Although it may sound ridiculous to say this as a *unique* feature, it is
    almost impossible to create one without
    `tools_webrtc/libs/generate_licenses.py`.

## Tested platforms

- Unity on Android
- Unity on Linux/X11
- Unity on Windows
- Unity on iOS
- Unity on macOS

## Why fork instead of implementing C/.NET bindings completely out-of-tree?

By forking and extending the build infrastructure for C/.NET bindings, the
build system of those bindings are automatically aligned with the one of the
library itself.

The build system of WebRTC also contributes to the portability because it is of
course capable to build for any platforms WebRTC supports.

The changes made in existing code for the bindings are minimal, and it is
expected to be easy to maintain them, comparing with possible tons of build
scripts and their messes. (Note that it supports multiple platforms.)

## Using the bindings

The bindings are simple mappings for native APIs. See
https://webrtc.org/native-code/native-apis/ for details of native APIs.

You may also refer to `examples/unity` for .NET/Unity specifics.

## Coding style

C# coding style is conforming to
[.NET Core coding style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md).

## Release build procedure

Currently unity package is built manually because of build system complications.
The release build procedure is as follows:

1. Prepare a computer with Linux installed for Android, Linux, and Windows
   builds according to https://webrtc.org/native-code/development/.

2. Check out this code base at `src`.

3. Build `unity.msbuildproj` with:
  * `src/sdk/dotnet/unity` as the working directory,
  * `Android;LinuxX64;WinX64` as `Targets` property, and
  * `Release` as `Configuration` property

4. Prepare a computer with macOS installed for iOS and macOS builds according to
   https://webrtc.org/native-code/development/.

5. Check out this code base at `src`.

6. Build `src/sdk/dotnet/unity/unity.msbuildproj` the following properties:
  * `src/sdk/dotnet/unity` as the working directory,
  * `Ios;MacX64` as `Targets` property, and
  * `Release` as `Configuration` property

7. Copy `Editor` and `Editor.meta` in `src/sdk/dotnet/unity/bin/Release`
   produced by 6 to `src/sdk/dotnet/unity/bin/Release` produced by 3.

8. Copy the contents of `src/sdk/dotnet/unity/bin/Release/Runtime`
   produced by 6 to `src/sdk/dotnet/unity/bin/Release/Runtime` produced by 3.
   Do not overwrite exist files.

9. Add license notices included in `src/sdk/dotnet/unity/bin/Release/LICENSE.md`
   produced by 6 to one produced by 3 if they are missing.

9. Make a tarball containing `src/sdk/dotnet/unity/bin/Release/Runtime`
   produced by 3 as `package` directory.

10. Gzip the tarball produced by 9.

# Further enhancements

The following is something nice to have, but it is not currently planned to
implement them:

- Automated tests
- Examples for .NET Core, .NET Framework/Mono and Xamarin
- Examples independent of Sora
- NuGet package

# Additional copyright notice

`examples/unity/Assets/aint_we_got_fun_billy_jones1921.mp3` is a recording of
_Ain't We Got Fun_, performed by Billy Jones. It is distributed by Digital
History.
    Mintz, S., & McNeil, S. (2016). Digital History.
    http://www.digitalhistory.uh.edu/music/music.cfm

The copyrights of the recording has expired and it is in the public domain.
