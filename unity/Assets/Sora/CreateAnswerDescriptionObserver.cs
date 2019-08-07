using System;
using Webrtc;

namespace Sora
{
    internal sealed class CreateAnswerDescriptionObserver : ICreateSessionDescriptionObserver
    {
        public Connection Connection;
        public string Type;

        public void OnSuccess(SessionDescriptionInterface desc)
        {
            try
            {
                var soraObserver = new SetLocalDescriptionObserver();

                soraObserver.Answer = new Signals.Answer();

                if (!desc.TryToString(out soraObserver.Answer.sdp))
                {
                    throw new ArgumentException(nameof(desc));
                }

                soraObserver.Answer.type = Type;
                soraObserver.Connection = Connection;

                using (var observer = new SetSessionDescriptionObserver(soraObserver))
                {
                    lock (Connection.SyncRoot)
                    {
                        Connection.PC.SetLocalDescription(observer, desc);
                    }
                }
            }
            catch (Exception exception)
            {
                Connection.OnException(exception);
            }
            finally
            {
                desc.Dispose();
            }
        }

        public void OnFailure(RtcError error)
        {
            Connection.Callbacks.OnFailure(error);
            Connection.Stop();
        }
    }
}
