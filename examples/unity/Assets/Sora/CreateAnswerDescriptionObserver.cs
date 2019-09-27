/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;
using System;

namespace Pixiv.Sora
{
    internal sealed class CreateAnswerDescriptionObserver : IManagedCreateSessionDescriptionObserver
    {
        public Connection Connection;
        public string Type;

        public void OnSuccess(DisposableSessionDescriptionInterface desc)
        {
            try
            {
                var soraObserver = new SetLocalDescriptionObserver();

                soraObserver.Connection = Connection;
                soraObserver.Type = Type;

                if (!desc.TryToString(out soraObserver.Sdp))
                {
                    throw new ArgumentException(nameof(desc));
                }

                using (var observer = new DisposableSetSessionDescriptionObserver(soraObserver))
                {
                    Connection.SetLocalDescription(observer, desc);
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
