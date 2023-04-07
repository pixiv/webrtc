これは [ピクシブ株式会社](https://www.pixiv.co.jp/) による WebRTC のフォークです。

# 変更
- iOS の broadcast extension で録音した音声を配信するために必要な API が追加されています。
- iOS で録音するときに Voice-Processing I/O unit 以外の audio unit component が
  使えるようになっています。
- **破壊的変更** iOSでの暗黙のマイク権限要求を取り除くパッチが適用されています。オリジナルの
  パッチは次のリンクを参照してください。
  https://github.com/shiguredo-webrtc-build/webrtc-build/blob/m86.4240.10.0/patches/ios_manual_audio_input.patch

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
