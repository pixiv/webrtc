これは [ピクシブ株式会社](https://www.pixiv.co.jp/) による WebRTC のフォークです。

# 変更

- `rtc_symbol_enable_export` と Windows ビルドの互換性が追加されています。
- `tools_webrtc/libs/generate_licenses.py` と Windows ビルドの互換性が追加されています。
- 部分的なC言語バインディングが `sdk/c` に追加されています。
- C言語バインディングに基づく .NET バインディングが `sdk/dotnet` に追加されています。
  - `sdk/dotnet/unity` は Unity パッケージになっています。
    - MSBuild でビルドしてください。
    - ビルド時に Mono を使わないでください。詳細は https://github.com/dotnet/msbuild/issues/3468
      を参照してください。
  - `example/unity` は Unity で [WebRTC SFU Sora](https://sora.shiguredo.jp/) と
    `examples/peerconnection` にあるプログラムに接続する例になっています。
- ライブラリでネイティブAPIが利用可能になっています。
- 最後のフレームを任意のタイミングで取得できる、簡単な `webrtc::VideoSinkInterface` が
  `webrtc::VideoBuffer` として追加されています。
- iOS の broadcast extension で録音した音声を配信するために必要な API が追加されています。
- iOS で録音するときに Voice-Processing I/O unit 以外の audio unit component が
  使えるようになっています。

# iOS の broadcast extension で録音した音声を配信する

## 傾向と対策

Broadcast extension により、iOSから画面を配信することができます。しかし、 extension は
通常のアプリケーションに比べて制約があり、 WebRTC の iOS 向けの audio device module で
必要とされる Audio Unit framework を欠いています。

その代わり、 broadcast extension は [`RPBroadcastSampleHandler`](https://developer.apple.com/documentation/replaykit/rpbroadcastsamplehandler)
オブジェクトを用いてアプリケーションの音声とマイクによる録音を受け取ることができます。この
フォークは Audio Unit framework がない環境での audio device module の動作を修正し、
[`RPBroadcastSampleHandler`](https://developer.apple.com/documentation/replaykit/rpbroadcastsamplehandler)
経由で受け取った音声を配信するインターフェイスを追加しています。

## 関連するインターフェイス

Broadcast extension で音声を配信するには以下で説明するインターフェイスを利用してください。

### `[RTCPeerConnectionFactory initWithEncoderFactory: decoderFactory: audioDeviceModule:]`

このメソッドは `[RTCPeerConnectionFactory initWithEncoderFactory: decoderFactory:]`
と同一ですが、 `RTCAudioDeviceModule` を型にもつ、  `audioDeviceModule`
パラメータが追加されています。

このメソッドを用いて音声を配信する `RTCAudioDeviceModule` を提供してください。

### `RTCAudioDeviceModule`

このクラスは `RTCAudioDeviceModule.h` に定義されています。音声を転送するネイティブ実装を
ラップしています。内部実装は Audio Unit framework 以外から音声を受け取れるように拡張されて
います。

### `[RTCAudioDeviceModule deliverRecordedData:]`

This method actually delivers the data your provide. The type of the parameter
is
このメソッドは提供された音声を実際に転送します。パラメータの型は [`CMSampleBuffer`](https://developer.apple.com/documentation/coremedia/cmsamplebuffer-u71)
です。

[`[RPBroadcastSampleHandler processSampleBuffer: with:]`](https://developer.apple.com/documentation/replaykit/rpbroadcastsamplehandler/2123045-processsamplebuffer)
で得られた [`CMSampleBuffer`](https://developer.apple.com/documentation/coremedia/cmsamplebuffer-u71)
を提供してください。

一度音声ストリームを追加したらこのメソッドを呼び続けなければなりませんが、 broadcast
extension はマイクが無効の間は `RPSampleBufferType.audioMic` を持つ audio buffer
を提供しません。そのため、マイクで録音した音声を配信している場合は、マイクが有効になるまで
ダミーのサンプルを提供する必要があります。

少なくとも iOS 12.2 においては、そのような仕組みを実装するために `NSTimer` や
`DispatchSourceTimer` を使えないことに注意してください。 `DispatchQueue.asyncAfter`
が良い代替となります。

このメソッドを利用するためには audio unit が未初期化で無効になっている必要があります。

### `RTCAudioSession.useManualAudio`

Broadcast extension では `YES` に設定してください。これは audio unit の初期化を防ぎます。

### `RTCAudioSession.isAudioEnabled`

Broadcast extension では `NO` に設定してください。これは audio unit を無効にします。

# iOS 上で Voice-Processing I/O 以外の audio unit component を利用する

Voice-Processing I/O unit にはエコーキャンセリングのような双方向のコミュニケーションに
適した機能があります。これらの機能は場合によっては不要であり、音量の低下などの問題を引き起こし
さえします。

そのような場合を網羅するためには他の audio unit component を利用してください。

## `RTCAudioDeviceModule.audioUnitSubType`

これは audio unit subtype を保持しているプロパティです。型は `OSType` です。次の
ドキュメンテーションを参照してください。

https://developer.apple.com/documentation/audiotoolbox/audio_unit_v2_c_api/1584139-input_output_audio_unit_subtypes

# .NET バインディング

## 機能

WebRTC の .NET バインディングはいくつかあります。例えば次のようなものがあります。

- [WebRTC for the Universal Windows Platform (webrtc-uwp)](https://webrtc-uwp.github.io/)
- webrtc-uwp ベースの [MixedReality-WebRTC](https://github.com/microsoft/MixedReality-WebRTC)
- [Unity Render Streaming](https://github.com/Unity-Technologies/UnityRenderStreaming)
- `sdk/unityplugin` にある unity plugin

このフォークで提供されているバインディングは次のような *独自の* 機能があります。

- ポータブル
  - .NET Standard 2.0
  - [Mono の full AOT](https://www.mono-project.com/docs/advanced/aot/#full-aot)
    と [IL2CPP](https://docs.unity3d.com/Manual/IL2CPP.html) の追加要件を満たしています。
  - Unity やその他の実行時の依存がありません。
  - WebRTC の要件以外の追加のOSの要件がありません。
- 最新
  - [Unified Plan](https://webrtc.org/web-apis/chrome/unified-plan/)
  - 伝統的な Unity アセットパッケージから改良された Unity [package](https://docs.unity3d.com/Manual/Packages.html)
    として利用可能です。
- 拡張可能
  - インターフェイスに基づいた API 設計により、ネイティブ API を利用している C++ コード
    のバインディングを作成することを可能にします。
  - ネイティブ API と同程度の低レベルになっています。
    - 入出力に関する想定がありません。 [Unity の RenderTexture](https://docs.unity3d.com/Manual/class-RenderTexture.html)
      でもなんでも使えます。
    - シグナリングに関する想定がありません。 [Sora](https://sora.shiguredo.jp/) のような
      既存のシグナリングサーバーや [ImageFlux Live Streaming](https://www.sakura.ad.jp/services/imageflux/livestreaming/)
      のような SaaS を利用できます。
    - これはアプリケーションに統合するためにより多くのコードが必要になることも同時に意味します。
- 包括的なライセンスファイルを提供します。
  - これを *独自の* 機能と呼ぶのは馬鹿馬鹿しく聞こえるかもしれませんが、
    `tools_webrtc/libs/generate_licenses.py` なしではそのようなファイルを作るのは
    ほぼ不可能です。

## テスト済みの環境

- Android 上の Unity
- Linux/X11 上の Unity
- Windows 上の Unity
- iOS 上の Unity
- macOS 上の Unity

## リポジトリの外に C/.NET バインディングを配置せずにフォークしている理由

フォークしてビルド基盤を C/.NET バインディング向けに拡張することで、これらのバインディングの
ビルドシステムはライブラリ自体のものとの一貫性を自動的に保つことができます。

また、 WebRTC のビルドシステムは当然 WebRTC が対応するすべてのプラットフォーム向けにビルドする
ことができるので、移植性に貢献します。

バインディングのための既存のコードの変更は最小限にとどめられており、大量のビルドスクリプトと
それによる困難に比べればより簡単に維持できることが期待されています。 (複数のプラットフォーム
に対応していることを留意してください。)

## バインディングを利用する

バインディングはネイティブ API と単純な対応関係を持っています。ネイティブ API の詳細は
https://webrtc.org/native-code/native-apis/ を参照してください。

.NET/Unity 固有の事柄については `examples/unity` も参照してください。

## コーディングスタイル

C# のコーディングスタイルは [.NET Core のコーディングスタイル](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md)
に準拠しています。

## Unity パッケージのビルド手順

1. 次の記事で説明されている前準備として必要なソフトウェアをインストールしてください。
   https://webrtc.github.io/webrtc-org/native-code/development/prerequisite-sw/

2. チェックアウトのディレクトリを作って作業ディレクトリとして利用してください。

3. 次のコマンドを実行してください。

```shell
gclient config --name=src --unmanaged https://github.com/pixiv/webrtc.git
```

4. 次のコマンドを実行してください。

```shell
gclient sync
```

ホストとは異なるOSをターゲットとするには `--deps` オプションを指定してください。

5. 次のような構成で `unity.msbuildproj` をビルドしてください。
  * 作業ディレクトリは `src/sdk/dotnet/unity`
  * `Android`, `Ios`, `LinuxX64`, `MacX64`, `WinX64` のうちのいくつかを `Targets` プロパティとして指定
  * `Release` を `Configuration` プロパティとして指定

複数の `Targets` を指定する場合はセミコロン区切り (`;`) にしてください。

6. `src/sdk/dotnet/unity/bin/Release/Runtime` を `package` ディレクトリとして含む
   tarball を作成してください。

7. 6で得られた tarball を gzip してください。

# 更なる改良

あったほうが良いものの、現在実装が予定されていないものを次に列挙します。

- 自動テスト
- .NET Core や .NET Framework/Mono, Xamarin の例
- NuGet パッケージ

# 追加の著作権表示

`examples/unity/Assets/aint_we_got_fun_billy_jones1921.mp3` は Billy Jones に
よって演奏された _Ain't We Got Fun_ の録音です。Digital History によって配布されています。
    Mintz, S., & McNeil, S. (2016). Digital History.
    http://www.digitalhistory.uh.edu/music/music.cfm

この録音の著作権は失効しており、パブリックドメインにあります。
