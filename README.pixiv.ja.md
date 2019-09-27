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
- Android ライブラリでネイティブAPIが利用可能になっています。
- 最後のフレームを任意のタイミングで取得できる、簡単な `webrtc::VideoSinkInterface` が
  `webrtc::VideoBuffer` として追加されています。

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
