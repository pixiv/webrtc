/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

internal sealed class AudioSource : IManagedAudioSourceInterface
{
    private readonly Dictionary<IntPtr, AudioTrackSinkInterface> _sinks =
        new Dictionary<IntPtr, AudioTrackSinkInterface>();

    public MediaSourceInterface.SourceState State =>
        MediaSourceInterface.SourceState.Live;

    private short[] _data = new short[0];
    private int _channels;
    private int _sampleRate;
    private int _bufferreds;

    public bool Remote => false;

    public void RegisterObserver(ObserverInterface observer)
    {
    }

    public void UnregisterObserver(ObserverInterface observer)
    {
    }

    public void AddSink(AudioTrackSinkInterface sink)
    {
        lock (((ICollection)_sinks).SyncRoot)
        {
            _sinks.Add(sink.Ptr, sink);
        }
    }

    public void RemoveSink(AudioTrackSinkInterface sink)
    {
        lock (((ICollection)_sinks).SyncRoot)
        {
            _sinks.Remove(sink.Ptr);
        }
    }

    public void Configure(int sampleRate, int channels)
    {
        if (sampleRate != _sampleRate || channels != _channels)
        {
            _sampleRate = sampleRate;
            _channels = channels;
            _bufferreds = 0;
            _data = new short[sampleRate * channels / 100];
        }
    }

    public void OnData(float[] data)
    {
        var index = 0;
        while (data.Length - index > _data.Length - _bufferreds)
        {
            while (_bufferreds < _data.Length)
            {
                _data[_bufferreds] = Sample(data[index]);
                _bufferreds++;
                index++;
            }

            var handle = GCHandle.Alloc(_data, GCHandleType.Pinned);
            try
            {
                lock (((ICollection)_sinks).SyncRoot)
                {
                    foreach (var sink in _sinks.Values)
                    {
                        sink.OnData(
                            handle.AddrOfPinnedObject(),
                            16,
                            _sampleRate,
                            _channels,
                            _data.Length / _channels);
                    }
                }
            }
            finally
            {
                handle.Free();
            }

            _bufferreds = 0;
        }

        while (index < data.Length)
        {
            _data[_bufferreds] = Sample(data[index]);
            _bufferreds++;
            index++;
        }
    }

    private static short Sample(float sample)
    {
        var scaled = sample < 0 ?
            -sample * short.MinValue : sample * short.MaxValue;

        return (short)UnityEngine.Mathf.Round(scaled);
    }
}
