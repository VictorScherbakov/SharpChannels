using System;
using System.Text;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    public class StringMessageSerializer : IMessageSerializer, IMessageSerializer<StringMessage>
    {
        private readonly Encoding _encoding;

        public StringMessage Deserialize(IBinaryMessageData messageData)
        {
            return new StringMessage(_encoding.GetString(messageData.Data));
        }

        public IBinaryMessageData Serialize(IMessage message)
        {
            var stringMessage = message as StringMessage;

            if (stringMessage == null)
                throw new ArgumentException("Should be StringMessage", nameof(message));

            return Serialize(stringMessage);
        }

        public IBinaryMessageData Serialize(StringMessage message)
        {
            return new BinaryMessageData(_encoding.GetBytes(message.Message));
        }

        public StringMessageSerializer() : this(Encoding.UTF8)
        {

        }

        public StringMessageSerializer(Encoding encoding)
        {
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));

            _encoding = encoding;
        }

        IMessage IMessageSerializer.Deserialize(IBinaryMessageData messageData)
        {
            return Deserialize(messageData);
        }
    }
}