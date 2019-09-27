/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Sora;
using Pixiv.Webrtc;
using System;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

internal sealed class Sora
{
    private Connection _connection;
    private const string signalingUri = "";
    private const string channelId = "";
    public readonly SoraCallbacks Callbacks = new SoraCallbacks();

    public void Start()
    {
        Stop();

        _connection = Connection.Start(
            Callbacks.Role,
            new Uri(signalingUri),
            channelId,
            Callbacks
        );
    }

    public void Stop()
    {
        if (_connection == null)
        {
            return;
        }

        _connection.Stop();
        _connection = null;
    }
}

internal sealed class PeerConnection
{
    private CancellationTokenSource _source;

    public readonly PeerConnectionCallbacks Callbacks =
        new PeerConnectionCallbacks();

    public async void SignIn()
    {
        var user = Environment.GetEnvironmentVariable("USERNAME") ?? "user";
        string host;

        try
        {
            host = System.Net.Dns.GetHostName();
        }
        catch (System.Net.Sockets.SocketException)
        {
            host = "host";
        }

        _source = new CancellationTokenSource();

        try
        {
            await Pixiv.PeerConnection.Connection.Start(
                user,
                host,
                Environment.GetEnvironmentVariable("WEBRTC_CONNECT") ?? "stun:stun.l.google.com:19302",
                Callbacks,
                _source.Token,
                CancellationToken.None
            );
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    public void SignOut()
    {
        _source?.Cancel();
    }
}

[RequireComponent(typeof(Text))]
internal sealed class Main : MonoBehaviour
{
    private readonly PeerConnection _peerConnection = new PeerConnection();
    private readonly Sora _sora = new Sora();
    private int _sampleRate;

#pragma warning disable 0649
    public Button peerConnectionSignIn;
    public Button peerConnectionSignOut;
    public Button soraDownstream;
    public Button soraTerminate;
    public Button soraUpstream;
    public RawImage peerConnectionImage;
    public RawImage soraImage;
    public Text text;
#pragma warning restore 0649

    private static unsafe void MoveFrame(DisposableVideoFrame frame, RawImage image)
    {
        Texture2D newTexture;
        var oldTexture = image.texture;

        using (frame)
        {
            var width = frame.Width();
            var height = frame.Height();

            if (oldTexture.width == width && oldTexture.height == height)
            {
                newTexture = (Texture2D)oldTexture;
            }
            else
            {
                newTexture = new Texture2D(
                    width,
                    height,
                    TextureFormat.RGBA32,
                    false
                );

                image.texture = newTexture;
                Texture.Destroy(oldTexture);
            }

            var data = newTexture.GetRawTextureData<Color32>();
            if (!Libyuv.ConvertFromI420(
                frame,
                Libyuv.VideoType.ABGR,
                0,
                (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr(data)
            )) {
                Debug.LogError("Failed to convert from I420");
                return;
            }
        }

        newTexture.Apply();
    }

    private System.Collections.IEnumerator UpdateVideo()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForEndOfFrame();
            _peerConnection.Callbacks.UpdateVideoTrackSource();
            _sora.Callbacks.UpdateVideoTrackSource();
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        _peerConnection.Callbacks.UpdateAudioTrackSource(data, _sampleRate, channels);
        _sora.Callbacks.UpdateAudioTrackSource(data, _sampleRate, channels);
    }

    private void OnEnable()
    {
        peerConnectionImage.texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        soraImage.texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        peerConnectionSignIn.interactable = true;
        peerConnectionSignOut.interactable = false;
        soraDownstream.interactable = true;
        soraTerminate.interactable = false;
        soraUpstream.interactable = true;
        _sora.Callbacks.Context = SynchronizationContext.Current;
        _sora.Callbacks.GameObject = gameObject;
        _sampleRate = AudioSettings.outputSampleRate;

        peerConnectionSignIn.onClick.AddListener(() =>
        {
            _peerConnection.SignIn();
            peerConnectionSignIn.interactable = false;
            peerConnectionSignOut.interactable = true;
        });

        peerConnectionSignOut.onClick.AddListener(() =>
        {
            _peerConnection.SignOut();
            peerConnectionSignIn.interactable = true;
            peerConnectionSignOut.interactable = false;
        });

        soraDownstream.onClick.AddListener(() =>
        {
            _sora.Callbacks.Role = Role.Downstream;
            _sora.Start();

            soraDownstream.interactable = false;
            soraTerminate.interactable = true;
            soraUpstream.interactable = false;
        });

        soraTerminate.onClick.AddListener(() =>
        {
            _sora.Stop();

            soraDownstream.interactable = true;
            soraTerminate.interactable = false;
            soraUpstream.interactable = true;
        });

        soraUpstream.onClick.AddListener(() =>
        {
            _sora.Callbacks.Role = Role.Upstream;
            _sora.Start();

            soraDownstream.interactable = false;
            soraTerminate.interactable = true;
            soraUpstream.interactable = false;
        });

        StartCoroutine(UpdateVideo());
    }

    private void OnDisable()
    {
        _peerConnection.SignOut();
        _sora.Stop();
    }

    private void Update()
    {
        text.text = DateTime.Now.ToString();

        var soraFrame = _sora.Callbacks.TryMoveNextVideoFrame();
        if (soraFrame != null)
        {
            MoveFrame(soraFrame, soraImage);
        }


        var peerConnectionFrame = _peerConnection.Callbacks.TryMoveNextVideoFrame();
        if (peerConnectionFrame != null)
        {
            MoveFrame(peerConnectionFrame, peerConnectionImage);
        }
    }
}
