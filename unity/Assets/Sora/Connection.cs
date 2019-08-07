using Sora.Signals;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sora
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
            _pcObserver = new Webrtc.PeerConnectionObserver(observer);
            _webSocket.Options.SetRequestHeader("User-Agent", "sora-dotnet-sdk");
        }

        internal readonly ICallbacks Callbacks;
        internal readonly object SyncRoot = new object();
        internal Webrtc.PeerConnectionInterface PC;

        private readonly ClientWebSocket _webSocket = new ClientWebSocket();
        private readonly Role _role;
        private readonly Uri _signalingUri;
        private readonly Webrtc.PeerConnectionObserver _pcObserver;
        private readonly string _channelId;
        private volatile int _startedThreads;
        private Rtc.Thread _networkThread;
        private Rtc.Thread _workerThread;
        private Rtc.Thread _signalingThread;
        private Task _webSocketReceiveTask;
        private Task _webSocketSendTask;
        private Webrtc.PeerConnectionFactoryInterface _pcFactory;
        private Webrtc.VideoTrackSourceInterface _videoTrackSource;
        private bool _stopping;

        internal async Task Standby(string sdp)
        {
            var config = new Webrtc.PeerConnectionInterface.RtcConfiguration();
            var connect = new Connect();
            var dependencies = new Webrtc.PeerConnectionDependencies();
            var soraRemoteObserver = new SetRemoteDescriptionObserver();

            config.SdpSemantics = Webrtc.SdpSemantics.UnifiedPlan;
            connect.type = "connect";
            connect.channel_id = _channelId;
            connect.sdp = sdp;
            connect.user_agent = "sora-dotnet-sdk";
            connect.audio = false;
            connect.video = new Video { codec_type = "VP8" };
            dependencies.Observer = _pcObserver;
            soraRemoteObserver.Connection = this;

            switch (_role)
            {
                case Role.Downstream:
                    connect.role = "downstream";
                    break;

                case Role.Upstream:
                    connect.role = "upstream";
                    break;
            }

            SendSignalAsync(connect);

            using (var remoteObserver = new Webrtc.SetSessionDescriptionObserver(soraRemoteObserver))
            while (true)
            {
                var (offer, closeStatus) = await ReceiveSignalAsync<Offer>();
                if (closeStatus.HasValue)
                {
                    break;
                }

                switch (offer.type)
                {
                    case "notify":
                        Callbacks.Notify();
                        break;

                    case "offer":
                        config.Servers = new Webrtc.PeerConnectionInterface.IceServer[offer.config.iceServers.Length];

                        for (var index = 0; index < config.Servers.Length; index++)
                        {
                            var offerServer = offer.config.iceServers[index];
                            var server = new Webrtc.PeerConnectionInterface.IceServer();

                            server.Urls = offerServer.urls;
                            server.Username = offerServer.username;
                            server.Password = offerServer.credential;

                            config.Servers[index] = server;
                        }

                        switch (offer.config.iceTransportPolicy)
                        {
                            case null:
                            case "all":
                                config.Type = Webrtc.PeerConnectionInterface.IceTransportsType.All;
                                break;

                            case "relay":
                                config.Type = Webrtc.PeerConnectionInterface.IceTransportsType.Relay;
                                break;

                            default:
                                throw new Exception();
                        }

                        lock (SyncRoot)
                        {
                            PC = _pcFactory.CreatePeerConnection(config, dependencies);

                            if (_role == Role.Upstream)
                            {
                                _videoTrackSource = Callbacks.CreateVideoTrackSource();

                                using (var track = _pcFactory.CreateVideoTrack(
                                    "screen_video", _videoTrackSource))
                                {
                                    var result = PC.AddTrack(track, new[] { "screen_stream" });
                                    if (result.Error.OK)
                                    {
                                        result.Value.Dispose();
                                    }
                                    else
                                    {
                                        Callbacks.OnFailure(result.Error);
                                    }
                                }
                            }

                            AnswerOffer(offer.sdp, "answer", remoteObserver);
                        }
                        break;

                    case "update":
                        AnswerOffer(offer.sdp, "update", remoteObserver);
                        break;

                    case "ping":
                        SendSignalAsync(new Pong { type = "pong" });
                        break;

                    case "push":
                        Callbacks.Push();
                        break;
                }
            }
        }

        internal void SendSignalAsync(object value)
        {
            lock (SyncRoot)
            {
                _webSocketSendTask = SendSignalAsyncClosure(_webSocketSendTask, value);
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

        private void RunThread(ref Rtc.Thread field)
        {
            try
            {
                var thread = Rtc.ThreadManager.Instance.WrapCurrentThread();

                field = thread;

                if (Interlocked.Increment(ref _startedThreads) == 3)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        var config = new Webrtc.PeerConnectionInterface.RtcConfiguration();
                        var dependencies = new Webrtc.PeerConnectionDependencies();
                        var soraObserver = new CreateOfferDescriptionObserver();

                        config.SdpSemantics = Webrtc.SdpSemantics.UnifiedPlan;
                        dependencies.Observer = _pcObserver;
                        soraObserver.Connection = this;

                        lock (SyncRoot)
                        {
                            _webSocketReceiveTask = _webSocket.ConnectAsync(
                                _signalingUri,
                                CancellationToken.None);

                            _webSocketSendTask = _webSocketReceiveTask;

                            using (var audioEncoderFactory = Webrtc.AudioEncoderFactory.CreateBuiltin())
                            using (var audioDecoderFactory = Webrtc.AudioDecoderFactory.CreateBuiltin())
                            {
                                _pcFactory = Webrtc.PeerConnectionFactory.Create(
                                    _networkThread,
                                    _workerThread,
                                    _signalingThread,
                                    IntPtr.Zero,
                                    audioEncoderFactory,
                                    audioDecoderFactory,
                                    Webrtc.VideoEncoderFactory.CreateBuiltin(),
                                    Webrtc.VideoDecoderFactory.CreateBuiltin(),
                                    IntPtr.Zero,
                                    IntPtr.Zero
                                );
                            }

                            PC = _pcFactory.CreatePeerConnection(config, dependencies);

                            using (var observer = new Webrtc.CreateSessionDescriptionObserver(
                                soraObserver))
                            {
                                PC.CreateOffer(
                                    observer,
                                    new Webrtc.PeerConnectionInterface.RtcOfferAnswerOptions()
                                );
                            }
                        }
                    });
                }

                thread.Run();
            }
            catch (Exception exception)
            {
                OnException(exception);
                Stop();
            }
        }

        private Task<(T, WebSocketCloseStatus?)> ReceiveSignalAsync<T>()
        {
            Task<(T, WebSocketCloseStatus?)> task;

            lock (SyncRoot)
            {
                task = ReceiveSignalClosure<T>(_webSocketReceiveTask);
                _webSocketReceiveTask = task;
            }

            return task;
        }

        private async Task<(T, WebSocketCloseStatus?)> ReceiveSignalClosure<T>(Task previous)
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

                    lock (SyncRoot)
                    {
                        task = _webSocket.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            CancellationToken.None
                        );
                    }

                    result = await task;
                    if (result.CloseStatus.HasValue)
                    {
                        UnityEngine.Debug.Log(result.CloseStatusDescription);
                        return (default(T), result.CloseStatus);
                    }

                    await stream.WriteAsync(
                        buffer, 0, result.Count, CancellationToken.None);
                } while (!result.EndOfMessage);

                stream.Position = 0;
                UnityEngine.Debug.Log(stream.Length);

                return ((T)serializer.ReadObject(stream), null);
            }
        }

        private async Task SendSignalAsyncClosure(Task previous, object value)
        {
            try
            {
                Task task;
                var serializer = new DataContractJsonSerializer(value.GetType());

                await previous;

                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, value);

                    lock (SyncRoot)
                    {
                        task = _webSocket.SendAsync(
                            new ArraySegment<byte>(
                                stream.GetBuffer(),
                                0,
                                (int)stream.Length
                            ),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );
                    }
                }

                await task;
            }
            catch (Exception exception)
            {
                OnException(exception);
                Stop();
            }
        }

        private void AnswerOffer(string sdp, string type, Webrtc.SetSessionDescriptionObserver remoteObserver)
        {
            var soraLocalObserver = new CreateAnswerDescriptionObserver();

            soraLocalObserver.Connection = this;
            soraLocalObserver.Type = type;

            using (var desc = Webrtc.SessionDescription.Create(
                Webrtc.SdpType.Offer, sdp, IntPtr.Zero))
            {
                if (desc == null)
                {
                    throw new Exception();
                }

                lock (SyncRoot)
                {
                    PC.SetRemoteDescription(remoteObserver, desc);

                    using (var answerObserver = new Webrtc.CreateSessionDescriptionObserver(soraLocalObserver))
                    {
                        PC.CreateAnswer(
                            answerObserver,
                            new Webrtc.PeerConnectionInterface.RtcOfferAnswerOptions());
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

            var networkThread = new Thread(
                state => connection.RunThread(ref connection._networkThread));

            var workerThread = new Thread(
                state => connection.RunThread(ref connection._workerThread));

            var signalingThread = new Thread(
                state => connection.RunThread(ref connection._signalingThread));

            networkThread.Start();
            workerThread.Start();
            signalingThread.Start();

            return connection;
        }

        public void Stop()
        {
            if (_stopping)
            {
                return;
            }

            _stopping = true;

            ThreadPool.QueueUserWorkItem(async state =>
            {
                try
                {
                    Task task;

                    lock (SyncRoot)
                    using (_pcObserver)
                    using (_networkThread)
                    using (_workerThread)
                    using (_signalingThread)
                    using (_pcFactory)
                    using (PC)
                    {
                        if (_videoTrackSource != null)
                        {
                            _videoTrackSource.Dispose();
                            _videoTrackSource = null;
                        }

                        task = _webSocket.State == WebSocketState.Open ?
                            _webSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None) :
                            Task.CompletedTask;

                        PC?.Close();
                    }

                    await task;

                    lock (SyncRoot)
                    {
                        _webSocket.Dispose();
                    }

                    Callbacks.Disconnect();
                }
                catch (Exception exception)
                {
                    OnException(exception);
                }
            }, null);
        }

        public Webrtc.VideoTrackSourceInterface VideoTrackSource
        {
            get
            {
                lock (SyncRoot)
                {
                    return _videoTrackSource;
                }
            }
        }
    }
}
