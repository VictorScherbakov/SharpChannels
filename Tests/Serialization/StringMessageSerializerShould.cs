using System;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;
using NUnit.Framework;

namespace Tests.Serialization
{
    public class StringMessageSerializerShould
    {
        [Test]
        public void SuccessfulySerializeEmptyMessage()
        {
            var serializer = new StringMessageSerializer();
            var message = new StringMessage(string.Empty);

            var serializedMessage = serializer.Serialize(message);
            var deserializedMessage = serializer.Deserialize(serializedMessage);

            Assert.That(message.Message, Is.EqualTo(deserializedMessage.Message));
        }

        [Test]
        public void SuccessfulySerializeNonEmptyMessage()
        {
            var serializer = new StringMessageSerializer();
            var message = new StringMessage("123 asd asd");

            var serializedMessage = serializer.Serialize(message);
            var deserializedMessage = serializer.Deserialize(serializedMessage);

            Assert.That(message.Message, Is.EqualTo(deserializedMessage.Message));
        }

        [Test]
        public void ThrowExceptionWhenMessageIsNotStringMessage()
        {
            var serializer = new StringMessageSerializer();
            var message = new RowBytesMessage(null);

            Assert.Throws<ArgumentException>(() =>
            {
                serializer.Serialize(message);
            });
        }
    }
}
