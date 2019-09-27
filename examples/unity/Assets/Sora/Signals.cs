/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using System.Runtime.Serialization;

namespace Pixiv.Sora.Signals
{
    [DataContract]
    internal sealed class Config
    {
#pragma warning disable 0649
        [DataMember]
        public IceServer[] iceServers;

        [DataMember]
        public string iceTransportPolicy;
#pragma warning restore 0649
    }

    [DataContract]
    internal sealed class IceServer
    {
#pragma warning disable 0649
        [DataMember]
        public string credential;

        [DataMember]
        public string[] urls;

        [DataMember]
        public string username;
#pragma warning restore 0649
    }

    [DataContract]
    internal sealed class Offer
    {
#pragma warning disable 0649
        [DataMember]
        public string type;

        [DataMember]
        public string sdp;

        [DataMember]
        public Config config;
#pragma warning restore 0649
    }
}
