/*
 *  Copyright 2019 pixiv Inc. All Rights Reserved.
 *
 *  Use of this source code is governed by a license that can be
 *  found in the LICENSE.pixiv file in the root of the source tree.
 */

using Pixiv.Webrtc;
using System.Runtime.Serialization.Json;

namespace Pixiv.Sora
{
    internal sealed class SetLocalDescriptionObserver : IManagedSetSessionDescriptionObserver
    {
        public Connection Connection;
        public string Type;
        public string Sdp;

        public void OnSuccess()
        {
            var stream = new System.IO.MemoryStream();

            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
                stream,
                System.Text.Encoding.UTF8,
                false))
            {
                writer.WriteStartElement("root");
                writer.WriteAttributeString("type", "object");
                writer.WriteElementString("type", Type);
                writer.WriteElementString("sdp", Sdp);
                writer.WriteEndElement();
            }

            Connection.SendSignalAsync(stream);
        }

        public void OnFailure(RtcError error)
        {
            Connection.Callbacks.OnFailure(error);
            Connection.Stop();
        }
    }
}
