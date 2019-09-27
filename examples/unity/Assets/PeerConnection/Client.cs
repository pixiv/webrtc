/*
 *  Copyright 2020 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Pixiv.PeerConnection
{
    internal sealed class Client : IDisposable
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly Uri _uri = new Uri("http://localhost:8888");

        public Task<HttpResponseMessage> PostMessageAsync(
            string peerId,
            string to,
            HttpContent content,
            CancellationToken token
        )
        {
            var uri = new Uri(_uri, $"message?peer_id={peerId}&to={to}");
            return _client.PostAsync(uri, content, token);
        }

        public Task<HttpResponseMessage> WaitAsync(
            string peerId,
            CancellationToken token
        )
        {
            var uri = new Uri(_uri, $"wait?peer_id={peerId}");
            return _client.GetAsync(uri, token);
        }

        public Task<HttpResponseMessage> SignInAsync(
            string user,
            string host,
            CancellationToken token
        )
        {
            var uri = new Uri(_uri, $"sign_in?{user}@{host}");
            return _client.GetAsync(uri, token);
        }

        public Task<HttpResponseMessage> SignOutAsync(
            string peerId,
            CancellationToken token
        )
        {
            var uri = new Uri(_uri, $"sign_out?peer_id={peerId}");
            return _client.GetAsync(uri, token);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
