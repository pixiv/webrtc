using Webrtc;

namespace Sora
{
    public interface ICallbacks
    {
        VideoTrackSourceInterface CreateVideoTrackSource();
        void Disconnect();
        void Notify();
        void OnException(System.Exception exception);
        void OnFailure(RtcError error);
        void OnRemoveTrack(MediaStreamTrackInterface track);
        void OnTrack(MediaStreamTrackInterface track);
        void Push();
    }
}
