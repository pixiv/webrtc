using Sora;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
internal sealed class Main : MonoBehaviour
{
    private sealed class LoggingCallbacks : ICallbacks
    {
        private readonly Rtc.VideoSinkInterface _sink;

        private readonly Dictionary<string, Webrtc.VideoTrackInterface> _pendings =
            new Dictionary<string, Webrtc.VideoTrackInterface>();

        private KeyValuePair<string, Webrtc.VideoTrackInterface> _current;

        public LoggingCallbacks(Rtc.IVideoSink sink)
        {
            _sink = new Rtc.VideoSinkInterface(sink);
        }

        public Webrtc.VideoTrackSourceInterface CreateVideoTrackSource()
        {
            return new VideoTrackSource();
        }

        public void Disconnect()
        {
            _sink.Dispose();
            Debug.Log("Disconnected");
        }

        public void Notify()
        {
            Debug.Log("Notified");
        }

        public void OnException(Exception exception)
        {
            Debug.LogException(exception);
        }

        public void OnFailure(Webrtc.RtcError error)
        {
            Debug.LogError(error.Message);
        }

        public void OnTrack(Webrtc.MediaStreamTrackInterface track)
        {
            if (!(track is Webrtc.VideoTrackInterface video))
            {
                Debug.Log($"Track {track} is ignored because it is not a video track");
                track.Dispose();
                return;
            }

            var pair = new KeyValuePair<string, Webrtc.VideoTrackInterface>(
                video.ID,
                video
            );

            if (_current.Key == null)
            {
                _current = pair;
                Debug.Log($"Streaming track {_current}");
                video.AddOrUpdateSink(_sink, new Rtc.VideoSinkWants());
            }
            else
            {
                Debug.Log($"Track {pair} is pending because track {_current} is being streamed.");
                _pendings.Add(pair.Key, pair.Value);
            }
        }

        public void OnRemoveTrack(Webrtc.MediaStreamTrackInterface track)
        {
            string id;

            using (track)
            {
                id = track.ID;
            }

            Debug.Log($"Track {id} is removed");

            if (_current.Key == id)
            {
                var enumerator = _pendings.GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    return;
                }

                _current = enumerator.Current;
                Debug.Log($"Streaming track ${_current.Key}");
                _current.Value.AddOrUpdateSink(_sink, new Rtc.VideoSinkWants());
            }
            else
            {
                _pendings.Remove(id);
            }
        }

        public void Push()
        {
            Debug.Log("Pushed");
        }
    }

    private sealed class VideoSink : Rtc.IVideoSink
    {
        public volatile RawImage Image;
        private volatile byte[] _buffer;
        private volatile int _width;
        private volatile int _height;
        private readonly object _syncRoot = new object();

        public void OnFrame(Webrtc.ConstVideoFrame frame)
        {
            try
            {
                var buffer = new byte[frame.Size * 3];
                var bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                try
                {
                    if (!Webrtc.Libyuv.ConvertFromI420(
                        frame,
                        Webrtc.Libyuv.VideoType.RGB24,
                        0,
                        bufferHandle.AddrOfPinnedObject()
                    )) {
                        Debug.LogError("Failed to convert from I420");
                        return;
                    }
                }
                finally
                {
                    bufferHandle.Free();
                }

                lock (_syncRoot)
                {
                    _buffer = buffer;
                    _width = frame.Width;
                    _height = frame.Height;
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        public void OnDiscardedFrame()
        {
        }

        public void Update()
        {
            Texture2D newTexture;
            byte[] buffer;
            int width;
            int height;
            var oldTexture = Image.texture;

            lock (_syncRoot)
            {
                buffer = _buffer;

                if (buffer == null)
                {
                    return;
                }

                width = _width;
                height = _height;
                _buffer = null;
            }

            if (oldTexture.width == width && oldTexture.height == height)
            {
                newTexture = (Texture2D)oldTexture;
            }
            else
            {
                newTexture = new Texture2D(
                    width,
                    height,
                    TextureFormat.RGB24,
                    false
                );

                Image.texture = newTexture;
                Texture.Destroy(oldTexture);
            }

            var data = newTexture.GetRawTextureData<byte>();
            var sourceStride = width * 3;
            var sourceRow = sourceStride * height;
            var destination = 0;

            while (sourceRow > 0)
            {
                for (var source = sourceRow - sourceStride; source < sourceRow; source += 3)
                {
                    data[destination] = buffer[source + 2];
                    destination++;
                    data[destination] = buffer[source + 1];
                    destination++;
                    data[destination] = buffer[source];
                    destination++;
                }

                sourceRow -= sourceStride;
            }

            newTexture.Apply();
        }
    }

    private sealed class VideoTrackSource : Rtc.AdaptedVideoTrackSource
    {
        public void Update()
        {
            var width = Screen.width;
            var height = Screen.height;
            var time = (long)(Time.unscaledTime * 1e6);

            if (!AdaptFrame(
                width,
                height,
                time,
                out var outWidth,
                out var outHeight,
                out var cropWidth,
                out var cropHeight,
                out var cropX,
                out var cropY))
            {
                return;
            }

            var texture = ScreenCapture.CaptureScreenshotAsTexture(1);

            try
            {
                var data = texture.GetRawTextureData<Color32>();
                var greater = width * height;
                var rowLesser = width;

                while (rowLesser < greater)
                {
                    var lesser = rowLesser;

                    while (lesser > rowLesser - width)
                    {
                        lesser--;
                        greater--;
                        var temporary = data[greater];
                        data[greater] = data[lesser];
                        data[lesser] = temporary;
                    }

                    rowLesser += width;
                }

                using (var i420 = Webrtc.I420Buffer.Create(cropWidth, cropHeight))
                {
                    unsafe
                    {
                        if (!Webrtc.Libyuv.ConvertToI420(
                            Webrtc.Libyuv.VideoType.ABGR,
                            data.Length,
                            width,
                            height,
                            (IntPtr)NativeArrayUnsafeUtility.GetUnsafePtr(data),
                            i420,
                            cropX,
                            cropY))
                        {
                            Debug.LogError("Failed to convert to I420");
                            return;
                        }
                    }

                    using (var builder = new Webrtc.VideoFrame.Builder())
                    {
                        using (var scaled = Webrtc.I420Buffer.Create(outWidth, outHeight))
                        {
                            scaled.ScaleFrom(i420);
                            builder.VideoFrameBuffer = scaled;
                        }

                        builder.Rotation = 0;
                        builder.TimestampUs = time;

                        using (var frame = builder.Build())
                        {
                            OnFrame(frame);
                        }
                    }
                }
            }
            finally
            {
                Texture2D.Destroy(texture);
            }
        }

        public override SourceState State => SourceState.Live;
        public override bool Remote => false;
        public override bool IsScreencast => true;
        public override bool? NeedsDenoising => null;
    }

    private Connection _connection;
    private readonly VideoSink _sink = new VideoSink();

#pragma warning disable 0649
    public RawImage image;
    public Text text;
#pragma warning restore 0649

    private System.Collections.IEnumerator UpdateSora()
    {
        while (isActiveAndEnabled)
        {
            yield return new WaitForEndOfFrame();
            ((VideoTrackSource)_connection.VideoTrackSource)?.Update();
        }
    }

    private void OnEnable()
    {
        image.texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        _sink.Image = image;

        _connection = Connection.Start(
            Role.Upstream,
            new LoggingCallbacks(_sink)
        );

        StartCoroutine(UpdateSora());
    }

    private void OnDisable()
    {
        _connection.Stop();
    }

    private void Update()
    {
        text.text = DateTime.Now.ToString();
        _sink.Update();
    }
}
