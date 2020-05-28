/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Cricket;
using Pixiv.Rtc;
using Pixiv.Sora.Signals;
using Pixiv.Webrtc;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Pixiv.Sora
{
    public sealed class Connection
    {
        private Connection(
            Role role,
            Uri signalingUri,
            string channelId,
            ICallbacks callbacks)
        {
            var observer = new PeerConnectionObserver();
            observer.Connection = this;
            Callbacks = callbacks;
            _role = role;
            _signalingUri = signalingUri;
            _channelId = channelId;
            _pcObserver = new DisposablePeerConnectionObserver(observer);
            _webSocket.Options.SetRequestHeader("User-Agent", "sora-dotnet-sdk");
        }

        internal readonly ICallbacks Callbacks;

        private readonly ClientWebSocket _webSocket = new ClientWebSocket();
        private readonly Role _role;
        private readonly Uri _signalingUri;
        private readonly DisposablePeerConnectionObserver _pcObserver;
        private readonly object _syncRoot = new object();
        private readonly string _channelId;
        private readonly Thread[] _threads = new Thread[3];
        private Task _webSocketReceiveTask;
        private Task _webSocketSendTask;
        private DisposablePeerConnectionFactoryInterface _pcFactory;
        private DisposablePeerConnectionInterface _pc;
        private bool _stopping;
        private int _startedThreads;

        internal async Task Standby(string sdp)
        {
            lock (_syncRoot)
            {
                if (_stopping)
                {
                    return;
                }

                using (_pc)
                {
                    _pc.Close();
                }

                _pc = null;
            }

            var config = new PeerConnectionInterface.RtcConfiguration();
            var dependencies = new PeerConnectionDependencies();
            var soraRemoteObserver = new SetRemoteDescriptionObserver();

            config.SdpSemantics = SdpSemantics.UnifiedPlan;
            dependencies.Observer = _pcObserver;
            soraRemoteObserver.Connection = this;

            var connect = new MemoryStream();

            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
                connect,
                Encoding.UTF8,
                false))
            {
                writer.WriteStartElement("root");
                writer.WriteAttributeString("type", "object");
                writer.WriteElementString("type", "connect");
                writer.WriteElementString("channel_id", _channelId);
                writer.WriteElementString("sdp", sdp);
                writer.WriteElementString("user_agent", "sora-dotnet-sdk");
                writer.WriteStartElement("audio");
                writer.WriteAttributeString("type", "boolean");
                writer.WriteString("false");
                writer.WriteEndElement();
                writer.WriteStartElement("video");
                writer.WriteAttributeString("type", "object");
                writer.WriteElementString("codec_type", "VP8");
                writer.WriteEndElement();

                switch (_role)
                {
                    case Role.Downstream:
                        writer.WriteElementString("role", "downstream");
                        break;

                    case Role.Upstream:
                        writer.WriteElementString("role", "upstream");
                        break;
                }

                writer.WriteEndElement();
            }

            SendSignalAsync(connect);

            using (var remoteObserver = new DisposableSetSessionDescriptionObserver(soraRemoteObserver))
            while (true)
            {
                var offer = await ReceiveSignalAsync<Offer>();
                if (offer == null)
                {
                    break;
                }

                switch (offer.type)
                {
                    case "notify":
                        Callbacks.Notify();
                        break;

                    case "offer":
                        config.Servers = new PeerConnectionInterface.IceServer[offer.config.iceServers.Length];

                        for (var index = 0; index < config.Servers.Length; index++)
                        {
                            var offerServer = offer.config.iceServers[index];
                            var server = new PeerConnectionInterface.IceServer();

                            server.Urls = offerServer.urls;
                            server.Username = offerServer.username;
                            server.Password = offerServer.credential;

                            config.Servers[index] = server;
                        }

                        switch (offer.config.iceTransportPolicy)
                        {
                            case null:
                            case "all":
                                config.Type = PeerConnectionInterface.IceTransportsType.All;
                                break;

                            case "relay":
                                config.Type = PeerConnectionInterface.IceTransportsType.Relay;
                                break;

                            default:
                                throw new Exception();
                        }

                        lock (_syncRoot)
                        {
                            if (_stopping)
                            {
                                return;
                            }

                            _pc = _pcFactory.CreatePeerConnection(config, dependencies);
                            _pc.SetAudioRecording(false);
                            Callbacks.Connect(_pcFactory, _pc);
                            AnswerOffer(offer.sdp, "answer", remoteObserver);
                        }
                        break;

                    case "update":
                        AnswerOffer(offer.sdp, "update", remoteObserver);
                        break;

                    case "ping":
                        Ping();
                        break;

                    case "push":
                        Callbacks.Push();
                        break;
                }
            }
        }

        internal void SendSignalAsync(MemoryStream stream)
        {
            lock (_syncRoot)
            {
                _webSocketSendTask = SendSignalAsyncClosure(_webSocketSendTask, stream);
            }
        }

        internal void SetLocalDescription(ISetSessionDescriptionObserver observer, IDisposableSessionDescriptionInterface desc)
        {
            lock (_syncRoot)
            {
                if (_stopping)
                {
                    desc.Dispose();
                    return;
                }

                _pc.SetLocalDescription(observer, desc);
            }
        }

        internal void OnException(Exception exception)
        {
            while (true)
            {
                try
                {
                    Callbacks.OnException(exception);
                    break;
                }
                catch (Exception another)
                {
                    exception = another;
                }
            }
        }

        private void Ping()
        {
            var stream = new MemoryStream();

            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
                stream,
                Encoding.UTF8,
                false))
            {
                writer.WriteStartElement("root");
                writer.WriteAttributeString("type", "object");
                writer.WriteElementString("type", "pong");
                writer.WriteEndElement();
            }

            SendSignalAsync(stream);
        }

        private void StartThread()
        {
            try
            {
                var thread = Rtc.ThreadManager.Instance.WrapCurrentThread();

                try
                {
                    int startedThreads;

                    lock (_syncRoot)
                    {
                        if (_stopping)
                        {
                            return;
                        }

                        startedThreads = _startedThreads;
                        _threads[startedThreads] = thread;
                        startedThreads++;
                        _startedThreads = startedThreads;
                    }

                    if (startedThreads == _threads.Length)
                    {
                        var config = new PeerConnectionInterface.RtcConfiguration();
                        var dependencies = new PeerConnectionDependencies();
                        var soraObserver = new CreateOfferDescriptionObserver();

                        config.SdpSemantics = SdpSemantics.UnifiedPlan;
                        dependencies.Observer = _pcObserver;
                        soraObserver.Connection = this;

                        lock (_syncRoot)
                        {
                            if (_stopping)
                            {
                                return;
                            }

                            _webSocketReceiveTask = _webSocket.ConnectAsync(
                                _signalingUri,
                                System.Threading.CancellationToken.None);

                            _webSocketSendTask = _webSocketReceiveTask;

                            using (var audioEncoderFactory = AudioEncoderFactory.CreateBuiltin())
                            using (var audioDecoderFactory = AudioDecoderFactory.CreateBuiltin())
                            {
                                _pcFactory = PeerConnectionFactory.Create(
                                    _threads[0],
                                    _threads[1],
                                    thread,
                                    null,
                                    audioEncoderFactory,
                                    audioDecoderFactory,
                                    VideoEncoderFactory.CreateBuiltin(),
                                    VideoDecoderFactory.CreateBuiltin(),
                                    null,
                                    null
                                );
                            }

                            _pc = _pcFactory.CreatePeerConnection(config, dependencies);

                            using (var observer = new DisposableCreateSessionDescriptionObserver(
                                soraObserver))
                            {
                                _pc.CreateOffer(
                                    observer,
                                    new PeerConnectionInterface.RtcOfferAnswerOptions()
                                );
                            }
                        }
                    }

                    thread.Run();
                }
                finally
                {
                    Rtc.ThreadManager.Instance.UnwrapCurrentThread();
                }
            }
            catch (Exception exception)
            {
                OnException(exception);
                Stop();
            }
        }

        private Task<T> ReceiveSignalAsync<T>()
        {
            Task<T> task;

            lock (_syncRoot)
            {
                task = ReceiveSignalClosure<T>(_webSocketReceiveTask);
                _webSocketReceiveTask = task;
            }

            return task;
        }

        private async Task<T> ReceiveSignalClosure<T>(Task previous)
        {
            var buffer = new byte[4096];
            var serializer = new DataContractJsonSerializer(typeof(T));

            await previous;

            using (var stream = new MemoryStream())
            {
                WebSocketReceiveResult result;

                do
                {
                    Task<WebSocketReceiveResult> task;

                    lock (_syncRoot)
                    {
                        if (_stopping)
                        {
                            return default(T);
                        }

                        task = _webSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            System.Threading.CancellationToken.None
                        );
                    }

                    result = await task;
                    if (result.CloseStatus.HasValue)
                    {
                        return default(T);
                    }

                    await stream.WriteAsync(
                        buffer,
                        0,
                        result.Count,
                        System.Threading.CancellationToken.None);
                } while (!result.EndOfMessage);

                stream.Position = 0;

                return (T)serializer.ReadObject(stream);
            }
        }

        private async Task SendSignalAsyncClosure(Task previous, MemoryStream stream)
        {
            try
            {
                using (stream)
                {
                    Task task;

                    await previous;

                    lock (_syncRoot)
                    {
                        if (_stopping)
                        {
                            return;
                        }

                        task = _webSocket.SendAsync(
                            new ArraySegment<byte>(
                                stream.GetBuffer(),
                                0,
                                (int)stream.Length
                            ),
                            WebSocketMessageType.Text,
                            true,
                            System.Threading.CancellationToken.None
                        );
                    }

                    await task;
                }
            }
            catch (Exception exception)
            {
                OnException(exception);
                Stop();
            }
        }

        private void AnswerOffer(string sdp, string type, ISetSessionDescriptionObserver remoteObserver)
        {
            var soraLocalObserver = new CreateAnswerDescriptionObserver();

            soraLocalObserver.Connection = this;
            soraLocalObserver.Type = type;

            using (var desc = SessionDescription.Create(
                SdpType.Offer, sdp, IntPtr.Zero))
            {
                if (desc == null)
                {
                    throw new Exception();
                }

                lock (_syncRoot)
                {
                    if (_stopping)
                    {
                        return;
                    }

                    _pc.SetRemoteDescription(remoteObserver, desc);

                    using (var answerObserver = new DisposableCreateSessionDescriptionObserver(soraLocalObserver))
                    {
                        _pc.CreateAnswer(
                            answerObserver,
                            new PeerConnectionInterface.RtcOfferAnswerOptions());
                    }
                }
            }
        }

        public static Connection Start(
            Role role,
            Uri signalingUri,
            string channelId,
            ICallbacks callbacks)
        {
            var connection = new Connection(
                role, signalingUri, channelId, callbacks);

            // Do not use Rtc.Thread.Start because one of the threads may invoke
            // delegates, and Mono requires a thread interacting with managed
            // objects to be tracked by the runtime.
            //
            // api/DESIGN.md
            // > At the moment, the API does not give any guarantee on which
            // > thread* the callbacks and events are called on.
            //
            // Generational GC | Mono
            // https://www.mono-project.com/docs/advanced/garbage-collector/sgen/
            // > The Mono runtime will automatically register all threads that
            // > are created from the managed world with the garbage collector.
            // > For developers embedding Mono it is important that they
            // > register with the runtime any additional thread they create
            // > that manipulates managed objects with mono_thread_attach.
            System.Threading.ThreadStart startThread = connection.StartThread;

            for (var count = 0; count < connection._threads.Length; count++)
            {
                var thread = new System.Threading.Thread(startThread);
                thread.Name = "Sora";
                thread.Start();
            }

            return connection;
        }

        public void Stop()
        {
            lock (_syncRoot)
            {
                if (_stopping)
                {
                    return;
                }

                _stopping = true;
            }

            System.Threading.ThreadPool.QueueUserWorkItem(async state =>
            {
                try
                {
                    using (_webSocket)
                    using (_pcObserver)
                    using (_pcFactory)
                    using (_pc)
                    {
                        if (_webSocket.State == WebSocketState.Open)
                        {
                            await _webSocket.CloseAsync(
                                WebSocketCloseStatus.Empty,
                                null,
                                System.Threading.CancellationToken.None);
                        }

                        _pc?.Close();
                        Callbacks.Disconnect();
                    }

                    foreach (var thread in _threads)
                    {
                        thread.Quit();
                    }
                }
                catch (Exception exception)
                {
                    OnException(exception);
                }
            }, null);
        }
    }
}
