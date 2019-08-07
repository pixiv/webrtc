using Webrtc;

namespace Sora
{
    internal sealed class SetRemoteDescriptionObserver : ISetSessionDescriptionObserver
    {
        public Connection Connection;

        public void OnSuccess()
        {
        }

        public void OnFailure(RtcError error)
        {
            Connection.Callbacks.OnFailure(error);
            Connection.Stop();
        }
    }
}
