/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree.
 */

using Pixiv.Sora;
using Pixiv.Webrtc;
using System;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
internal sealed class Main : MonoBehaviour
{
    private Connection _connection;
    private const string signalingUri = "";
    private const string channelId = "";
    private int _sampleRate;
    private readonly Callbacks _callbacks = new Callbacks();

#pragma warning disable 0649
    public Button downstream;
    public Button terminate;
    public Button upstream;
    public RawImage image;
    public Text text;
#pragma warning restore 0649

    private System.Collections.IEnumerator UpdateSora()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForEndOfFrame();
            _callbacks.UpdateVideoTrackSource();
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        _callbacks.UpdateAudioTrackSource(data, _sampleRate, channels);
    }

    private void OnEnable()
    {
        image.texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        downstream.interactable = true;
        terminate.interactable = false;
        upstream.interactable = true;
        _callbacks.Context = SynchronizationContext.Current;
        _callbacks.GameObject = gameObject;
        _sampleRate = AudioSettings.outputSampleRate;

        downstream.onClick.AddListener(() =>
        {
            _callbacks.Role = Role.Downstream;

            _connection = Connection.Start(
                Role.Downstream,
                new Uri(signalingUri),
                channelId,
                _callbacks
            );

            downstream.interactable = false;
            terminate.interactable = true;
            upstream.interactable = false;
        });

        terminate.onClick.AddListener(() =>
        {
            OnDisable();

            downstream.interactable = true;
            terminate.interactable = false;
            upstream.interactable = true;
        });

        upstream.onClick.AddListener(() =>
        {
            _callbacks.Role = Role.Upstream;

            _connection = Connection.Start(
                Role.Upstream,
                new Uri(signalingUri),
                channelId,
                _callbacks
            );

            downstream.interactable = false;
            terminate.interactable = true;
            upstream.interactable = false;
        });

        StartCoroutine(UpdateSora());
    }

    private void OnDisable()
    {
        if (_connection == null)
        {
            return;
        }

        _connection.Stop();
        _connection = null;
    }

    private unsafe void Update()
    {
        text.text = DateTime.Now.ToString();

        var frame = _callbacks.TryMoveNextVideoFrame();
        if (frame == null)
        {
            return;
        }

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
}
