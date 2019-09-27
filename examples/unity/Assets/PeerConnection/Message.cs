/*
 *  Copyright 2020 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using System.Runtime.Serialization;

namespace Pixiv.PeerConnection
{
    [DataContract]
    internal sealed class Message
    {
#pragma warning disable 0649
        [DataMember] public string type;
        [DataMember] public string sdp;
        [DataMember] public string sdpMid;
        [DataMember] public int sdpMLineIndex;
        [DataMember] public string candidate;
#pragma warning restore 0649
    }
}
