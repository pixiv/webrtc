using System.Runtime.Serialization;

namespace Sora.Signals
{
    [DataContract]
    internal sealed class Answer
    {
        [DataMember]
        public string type;

        [DataMember]
        public string sdp;
    }

    [DataContract]
    internal sealed class Candidate
    {
        [DataMember]
        public string type;

        [DataMember]
        public string candidate;
    }

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
    internal sealed class Connect
    {
        [DataMember]
        public string type;

        [DataMember]
        public string role;

        [DataMember]
        public string channel_id;

        [DataMember]
        public string sdp;

        [DataMember]
        public string user_agent;

        [DataMember]
        public bool audio;

        [DataMember]
        public Video video;
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

    [DataContract]
    internal sealed class Pong
    {
#pragma warning disable 0649
        [DataMember]
        public string type;
#pragma warning restore 0649
    }

    [DataContract]
    internal sealed class Video
    {
        [DataMember]
        public string codec_type;
    }
}
