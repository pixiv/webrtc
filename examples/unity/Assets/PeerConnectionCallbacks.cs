/*
 *  Copyright 2020 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Rtc;
using Pixiv.Webrtc;
using System.Collections.Generic;
using UnityEngine;

internal sealed class PeerConnectionCallbacks : Pixiv.PeerConnection.ICallbacks
{
    private readonly Dictionary<string, DisposableVideoTrackInterface> _pendingVideos =
        new Dictionary<string, DisposableVideoTrackInterface>();

    private DisposableVideoBufferInterface _videoBuffer;
    private KeyValuePair<string, DisposableVideoTrackInterface> _currentVideo;
    private VideoTrackSource _videoTrackSource;
    private readonly AudioSource _audioSource = new AudioSource();

    public void Connect(
        IPeerConnectionFactoryInterface factory,
        IPeerConnectionInterface connection)
    {
        _videoTrackSource = new VideoTrackSource();

        using (var track = factory.CreateVideoTrack(
            "screen_video", _videoTrackSource))
        {
            var result = connection.AddTrack(track, new[] { "screen_stream" });
            if (result.Error.OK)
            {
                result.Value.Dispose();
            }
            else
            {
                OnFailure(result.Error);
            }
        }

        using (var source = new DisposableAudioSourceInterface(_audioSource))
        using (var track = factory.CreateAudioTrack("system_audio", source))
        {
            var result = connection.AddTrack(track, new[] { "audio_stream" });
            if (result.Error.OK)
            {
                result.Value.Dispose();
            }
            else
            {
                OnFailure(result.Error);
            }
        }
    }

    public void Disconnect()
    {
        if (_currentVideo.Value != null)
        {
            _currentVideo.Value.Dispose();

            _currentVideo =
                new KeyValuePair<string, DisposableVideoTrackInterface>(
                    null,
                    null
                );
        }

        if (_videoBuffer != null)
        {
            _videoBuffer.Dispose();
            _videoBuffer = null;
        }

        if (_videoTrackSource != null)
        {
            _videoTrackSource.Dispose();
            _videoTrackSource = null;
        }

        foreach (var pending in _pendingVideos.Values)
        {
            pending.Dispose();
        }

        _pendingVideos.Clear();

        Debug.Log("Disconnected");
    }

    public void OnException(System.Exception exception)
    {
        Debug.LogException(exception);
    }

    public void OnFailure(RtcError error)
    {
        Debug.LogError(error.Message);
    }

    public void OnTrack(DisposableMediaStreamTrackInterface track)
    {
        if (track is DisposableVideoTrackInterface video)
        {
            var pair = new KeyValuePair<string, DisposableVideoTrackInterface>(
                video.ID(),
                video
            );

            if (_currentVideo.Key == null)
            {
                _currentVideo = pair;
                Debug.Log($"Streaming track {_currentVideo}");

                if (_videoBuffer == null)
                {
                    _videoBuffer = VideoBuffer.Create();
                }

                video.AddOrUpdateSink(_videoBuffer, new VideoSinkWants());
            }
            else
            {
                Debug.Log($"Track {pair} is pending because track {_currentVideo} is being streamed.");
                _pendingVideos.Add(pair.Key, pair.Value);
            }
        }
        else
        {
            Debug.Log($"Track {track} is ignored because it is not a video.");
            track.Dispose();
        }
    }

    public void OnRemoveTrack(DisposableMediaStreamTrackInterface track)
    {
        string id;

        using (track)
        {
            id = track.ID();
        }

        Debug.Log($"Track {id} is removed");

        if (_currentVideo.Key == id)
        {
            var enumerator = _pendingVideos.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return;
            }

            _currentVideo = enumerator.Current;
            Debug.Log($"Streaming track ${_currentVideo.Key}");
            _currentVideo.Value.AddOrUpdateSink(
                _videoBuffer,
                new VideoSinkWants()
            );
        }
        else
        {
            _pendingVideos.Remove(id);
        }
    }

    public DisposableVideoFrame TryMoveNextVideoFrame()
    {
        return _videoBuffer?.MoveFrame();
    }

    public void UpdateVideoTrackSource()
    {
        _videoTrackSource?.Update();
    }

    public void UpdateAudioTrackSource(
        float[] data,
        int sampleRate,
        int channels)
    {
        _audioSource.Configure(sampleRate, channels);
        _audioSource.OnData(data);
    }
}
