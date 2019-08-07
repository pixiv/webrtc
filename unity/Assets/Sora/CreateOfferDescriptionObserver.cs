using System;
using Webrtc;

namespace Sora
{
    internal sealed class CreateOfferDescriptionObserver : ICreateSessionDescriptionObserver
    {
        public Connection Connection;

        public async void OnSuccess(SessionDescriptionInterface desc)
        {
            try
            {
                string descString;

                using (desc)
                {
                    if (!desc.TryToString(out descString))
                    {
                        throw new ArgumentException(nameof(desc));
                    }
                }

                lock (Connection.SyncRoot)
                {
                    using (Connection.PC)
                    {
                        Connection.PC.Close();
                    }

                    Connection.PC = null;
                }

                await Connection.Standby(descString);
            }
            catch (Exception exception)
            {
                Connection.OnException(exception);
            }

            Connection.Stop();
        }

        public void OnFailure(RtcError error)
        {
            Connection.Callbacks.OnFailure(error);
            Connection.Stop();
        }
    }
}
