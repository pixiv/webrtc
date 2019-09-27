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
    internal sealed class CreateOfferDescriptionObserver : IManagedCreateSessionDescriptionObserver
    {
        public Connection Connection;

        public async void OnSuccess(DisposableSessionDescriptionInterface desc)
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
