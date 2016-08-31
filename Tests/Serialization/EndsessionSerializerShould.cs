using SharpChannels.Core.Messages.System;
using SharpChannels.Core.Serialization.System;
using NUnit.Framework;

namespace Tests.Serialization
{
    public class EndsessionSerializerShould
    {
        [Test]
        public void SuccessfullySerializeMessage()
        {
            var message = new EndSessionMessage();
            var serializer = new EndSessionSerializer();

            var serializedMessage = serializer.Serialize(message);
            var deserializedMessage = serializer.Deserialize(serializedMessage);

            Assert.That(deserializedMessage, Is.InstanceOf<EndSessionMessage>());
        }
    }
}