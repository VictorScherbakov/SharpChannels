using SharpChannels.Core.Messages.System;
using SharpChannels.Core.Serialization.System;
using NUnit.Framework;

namespace Tests.Serialization
{
    public class HandshakeRequestSerializerShould
    {
        [Test]
        public void SuccessfullySerializeMessage()
        {
            var request = new HandshakeRequest(100);
            var serializer = new HandshakeRequestSerializer();

            var serializedMessage = serializer.Serialize(request);
            var deserializedMessage = (HandshakeRequest)serializer.Deserialize(serializedMessage);

            Assert.That(request.RequestMarker, Is.EqualTo(deserializedMessage.RequestMarker));
        }
    }
}