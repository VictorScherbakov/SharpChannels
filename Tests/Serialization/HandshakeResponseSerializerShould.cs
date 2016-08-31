using SharpChannels.Core.Messages.System;
using SharpChannels.Core.Serialization.System;
using NUnit.Framework;

namespace Tests.Serialization
{
    public class HandshakeResponseSerializerShould
    {
        [Test]
        public void SuccessfullySerializeMessage()
        {
            var response = new HandshakeResponse(100);
            var serializer = new HandshakeResponseSerializer();

            var serializedMessage = serializer.Serialize(response);
            var deserializedMessage = (HandshakeResponse)serializer.Deserialize(serializedMessage);

            Assert.That(response.ResponseMarker, Is.EqualTo(deserializedMessage.ResponseMarker));
        }
    }
}