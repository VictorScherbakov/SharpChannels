using System.Collections.Generic;
using System.Linq;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;
using NUnit.Framework;

namespace Tests.Serialization
{
    public class CompoundSerializerShould
    {
        [Test]
        public void SuccessfullySerializeMessagesOfDifferentTypes()
        {
            var serializer = new CompoundSerializer(new List<CompoundSerializer.SerializerInfo>()
            {
                new CompoundSerializer.SerializerInfo
                {
                    Code = 1, Serializer = new StringMessageSerializer(), Type = typeof (StringMessage)
                },
                new CompoundSerializer.SerializerInfo
                {
                    Code = 2, Serializer = new NativeSerializer<RowBytesMessage>(), Type = typeof (RowBytesMessage)
                }

            });

            var stringMessage = new StringMessage("111111");

            var binaryMessage = serializer.Serialize(stringMessage);
            var deserializedMessage = (StringMessage) serializer.Deserialize(binaryMessage);

            Assert.That(stringMessage.Message, Is.EqualTo(deserializedMessage.Message));

            var rowBytesMessage = new RowBytesMessage(new byte[] {1, 2, 3, 4, 5, 155});

            binaryMessage = serializer.Serialize(rowBytesMessage);
            var deserializedRowBytesMessage = (RowBytesMessage)serializer.Deserialize(binaryMessage);

            Assert.That(rowBytesMessage.Message.SequenceEqual(deserializedRowBytesMessage.Message));
        }
    }
}