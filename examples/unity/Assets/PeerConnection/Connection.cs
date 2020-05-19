/*
 *  Copyright 2020 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Rtc;
using Pixiv.Webrtc;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Pixiv.PeerConnection
{
    public sealed class Connection
    {
        internal readonly ICallbacks Callbacks;
        private DisposablePeerConnectionInterface _connection;
        private DisposablePeerConnectionObserver _observer;
        private DisposablePeerConnectionFactoryInterface _factory;
        private readonly System.Threading.CancellationToken _haltToken;
        private readonly System.Threading.CancellationToken _signOutToken;
        private readonly Client _client;
        private readonly Thread[] _threads = new Thread[3];
        private readonly object _lock = new object();
        private readonly string _id;
        private readonly string _serverUri;
        private volatile string _peerId = null;
        private bool _loopback = false;
        private int _startedThreads;

        internal async Task ProvideIceCandidate(
            DisposablePeerConnectionInterface connection,
            IIceCandidateInterface candidate)
        {
            if (!candidate.TryToString(out var candidateString))
            {
                throw new ArgumentException(nameof(candidate));
            }

            lock (_lock)
            {
                if (_connection != connection)
                {
                    return;
                }

                if (_loopback)
                {
                    connection.AddIceCandidate(candidate);
                    return;
                }
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
                    stream,
                    Encoding.UTF8,
                    false))
                {
                    writer.WriteStartElement("root");
                    writer.WriteAttributeString("type", "object");
                    writer.WriteElementString("sdpMid", candidate.SdpMid());
                    writer.WriteStartElement("sdpMLineIndex");
                    writer.WriteAttributeString("type", "number");
                    writer.WriteValue(candidate.SdpMlineIndex());
                    writer.WriteEndElement();
                    writer.WriteElementString("candidate", candidateString);
                    writer.WriteEndElement();
                }

                stream.Position = 0;

                var peerId = _peerId;

                if (peerId == null)
                {
                    return;
                }

                try
                {
                    var message = await _client.PostMessageAsync(
                        _id,
                        peerId,
                        new StreamContent(stream),
                        _signOutToken
                    );

                    message.Dispose();
                }
                catch (ObjectDisposedException)
                {
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        internal async Task SetLocalSessionDescriptionAsync(
            DisposablePeerConnectionInterface connection,
            IDisposableSessionDescriptionInterface desc)
        {
            var managedObserver = new DummySetSessionDescriptionObserver(this);

            if (!desc.TryToString(out var sdp))
            {
                throw new ArgumentException(nameof(sdp));
            }

            var type = desc.GetSdpType();

            lock (_lock)
            {
                if (_connection != connection)
                {
                    return;
                }

                using (var observer = new DisposableSetSessionDescriptionObserver(managedObserver))
                {
                    _connection.SetLocalDescription(observer, desc);

                    if (_loopback)
                    {
                        var description = SessionDescription.Create(
                            SdpType.Answer,
                            sdp,
                            IntPtr.Zero
                        );

                        _connection.SetRemoteDescription(
                            observer,
                            description);
                        return;
                    }
                }
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
                    stream,
                    Encoding.UTF8,
                    false))
                {
                    writer.WriteStartElement("root");
                    writer.WriteAttributeString("type", "object");
                    writer.WriteElementString("type", type.ToSdpString());
                    writer.WriteElementString("sdp", sdp);
                    writer.WriteEndElement();
                }

                stream.Position = 0;

                var peerId = _peerId;

                if (peerId == null)
                {
                    return;
                }

                try
                {
                    var message = await _client.PostMessageAsync(
                        _id,
                        peerId,
                        new StreamContent(stream),
                        _signOutToken
                    );

                    message.Dispose();
                }
                catch (ObjectDisposedException)
                {
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private Connection(
            string id,
            string serverUri,
            ICallbacks callbacks,
            Client client,
            System.Threading.CancellationToken signOutToken,
            System.Threading.CancellationToken haltToken
        )
        {
            var signOutTokenSource =
                System.Threading.CancellationTokenSource.CreateLinkedTokenSource(
                    signOutToken,
                    haltToken
                );

            Callbacks = callbacks;
            _client = client;
            _haltToken = haltToken;
            _id = id;
            _serverUri = serverUri;
            _signOutToken = signOutTokenSource.Token;
        }

        private Message ReadMessage(byte[] content)
        {
            var serializer = new DataContractJsonSerializer(typeof(Message));

            using (var stream = new MemoryStream(content, false))
            {
                return (Message)serializer.ReadObject(stream);
            }
        }

        private void ReaderAddIceCandidate(
            string sdpMid,
            int sdpMlineIndex,
            string sdp
        )
        {
            using (var candidate = IceCandidate.Create(sdpMid, sdpMlineIndex, sdp, IntPtr.Zero))
            {
                if (candidate == null)
                {
                    return;
                }

                _connection.AddIceCandidate(candidate);
            }
        }

        private void ReaderConsumeSdp(string typeString, string sdp)
        {
            var type = SdpTypeExtension.FromString(typeString);
            if (!type.HasValue)
            {
                return;
            }

            var managedObserver = new DummySetSessionDescriptionObserver(this);

            using (var description = SessionDescription.Create(
                type.Value, sdp, IntPtr.Zero
            ))
            {
                if (description == null)
                {
                    return;
                }
                using (var observer = new DisposableSetSessionDescriptionObserver(managedObserver))
                {
                    _connection.SetRemoteDescription(
                        observer,
                        description);
                }
                if (type == SdpType.Offer)
                {
                    var managedAnswerObserver =
                        new CreateSessionDescriptionObserver(this, _connection);

                    using (var answerObserver = new DisposableCreateSessionDescriptionObserver(managedAnswerObserver))
                    {
                        _connection.CreateAnswer(
                            answerObserver,
                            new PeerConnectionInterface.RtcOfferAnswerOptions()
                        );
                    }
                }
            }
        }

        private void ReaderLoopback()
        {
            var managedObserver = new CreateSessionDescriptionObserver(this, _connection);

            var senders = _connection.GetSenders();
            try
            {
                lock (_lock)
                {
                    WriterDisconnect();
                    _loopback = true;
                    WriterConnect(/*dtls=*/false);
                }
                if (_connection == null)
                {
                    return;
                }
                foreach (var sender in senders)
                {
                    _connection.AddTrack(sender.Track(), sender.StreamIds());
                }
                using (var observer = new DisposableCreateSessionDescriptionObserver(managedObserver))
                {
                    _connection.CreateOffer(
                        observer,
                        new PeerConnectionInterface.RtcOfferAnswerOptions()
                    );
                }
            }
            finally
            {
                foreach (var sender in senders)
                {
                    sender.Dispose();
                }
            }
        }

        private async Task ReaderConsumeResponse(HttpResponseMessage response)
        {
            byte[] content;

            using (response)
            {
                var peerId = response.Headers.GetValues("Pragma").First();

                if (_id == peerId)
                {
                    return;
                }

                if (_peerId == null)
                {
                    lock (_lock)
                    {
                        WriterConnect(true);
                    }

                    _peerId = peerId;
                }
                else if (_peerId != peerId)
                {
                    return;
                }

                content = await response.Content.ReadAsByteArrayAsync();
            }

            if (content.Length == 3 || content[0] == 66 || content[1] == 89 || content[2] == 69)
            {
                lock (_lock)
                {
                    WriterDisconnect();
                    _loopback = false;
                }

                _peerId = null;
            }
            else
            {
                var message = ReadMessage(content);
                if (message.type == null)
                {
                    ReaderAddIceCandidate(
                        message.sdpMid,
                        message.sdpMLineIndex,
                        message.candidate
                    );
                }
                else if (message.type == "offer-loopback")
                {
                    ReaderLoopback();
                }
                else
                {
                    ReaderConsumeSdp(message.type, message.sdp);
                }
            }
        }

        private async void ReaderRun()
        {
            try
            {
                try
                {
                    _factory = PeerConnectionFactory.Create(
                        _threads[0] /* network_thread */,
                        _threads[1] /* worker_thread */,
                        _threads[2] /* signaling_thread */,
                        null /* default_adm */,
                        AudioEncoderFactory.CreateBuiltin(),
                        AudioDecoderFactory.CreateBuiltin(),
                        VideoEncoderFactory.CreateBuiltin(),
                        VideoDecoderFactory.CreateBuiltin(),
                        null /* audio_mixer */,
                        null /* audio_processing */
                    );

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                HttpResponseMessage response;

                                try
                                {
                                    response = await _client.WaitAsync(
                                        _id,
                                        _signOutToken
                                    );
                                }
                                catch (OperationCanceledException)
                                {
                                    break;
                                }

                                await ReaderConsumeResponse(response);
                            }
                            catch (Exception exception)
                            {
                                Callbacks.OnException(exception);
                            }
                        }

                        try
                        {
                            await _client.SignOutAsync(_id, _haltToken);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }
                    finally
                    {
                        if (_connection != null)
                        {
                            WriterDisconnect();
                        }

                        _factory.Dispose();
                    }
                }
                finally
                {
                    foreach (var thread in _threads)
                    {
                        thread.Quit();
                    }

                    _client.Dispose();
                }
            }
            catch (Exception exception)
            {
                Callbacks.OnException(exception);
            }
        }

        private void WriterConnect(bool dtls)
        {
            var configuration = new PeerConnectionInterface.RtcConfiguration();
            var dependencies = new PeerConnectionDependencies();
            var server = new PeerConnectionInterface.IceServer();

            configuration.SdpSemantics = SdpSemantics.UnifiedPlan;
            configuration.EnableDtlsSrtp = dtls;
            server.Uri = _serverUri;
            configuration.Servers = new[] { server };

            var observer = new PeerConnectionObserver(this);

            _observer = new DisposablePeerConnectionObserver(observer);
            dependencies.Observer = _observer;

            _connection = _factory.CreatePeerConnection(
                configuration,
                dependencies
            );

            observer.PeerConnection = _connection;
            _connection.SetAudioRecording(false);
            Callbacks.Connect(_factory, _connection);
        }

        private void WriterDisconnect()
        {
            using (var oldConnection = _connection)
            {
                _connection = null;
            }

            using (var oldObserver = _observer)
            {
                _observer = null;
            }

            Callbacks.Disconnect();
        }

        private void StartThread()
        {
            try
            {
                var thread = ThreadManager.Instance.WrapCurrentThread();

                try
                {
                    int startedThreads;

                    lock (_lock)
                    {
                        startedThreads = _startedThreads;
                        _threads[startedThreads] = thread;
                        startedThreads++;
                        _startedThreads = startedThreads;
                    }

                    if (startedThreads == _threads.Length)
                    {
                        ReaderRun();
                    }

                    thread.Run();
                }
                finally
                {
                    ThreadManager.Instance.UnwrapCurrentThread();
                }
            }
            catch (Exception exception)
            {
                Callbacks.OnException(exception);
            }
        }

        public static async Task Start(
            string user,
            string host,
            string serverUri,
            ICallbacks callbacks,
            System.Threading.CancellationToken signOutToken,
            System.Threading.CancellationToken haltToken
        )
        {
            var client = new Client();
            try
            {
                string id;

                using (var response = await client.SignInAsync(user, host, haltToken))
                {
                    id = response.Headers.GetValues("Pragma").First();
                }

                var connection = new Connection(
                    id,
                    serverUri,
                    callbacks,
                    client,
                    signOutToken,
                    haltToken
                );

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
            }
            catch (Exception)
            {
                client.Dispose();
                throw;
            }
        }
    }
}
