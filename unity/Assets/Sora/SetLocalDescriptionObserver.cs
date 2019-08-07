using Webrtc;

namespace Sora
{
    internal sealed class SetLocalDescriptionObserver : ISetSessionDescriptionObserver
    {
        public Connection Connection;
        public Signals.Answer Answer;

        public void OnSuccess()
        {
            Connection.SendSignalAsync(Answer);
        }

        public void OnFailure(RtcError error)
        {
            Connection.Callbacks.OnFailure(error);
            Connection.Stop();
        }
    }
}
