using System;
using System.IO;
using SharpChannels.Core.Messages;
using ProtoBuf;

namespace SharpChannels.Core.Serialization.Protobuf
{
    public class ProtobufSerializer<TMessage> : IMessageSerializer<TMessage>, IMessageSerializer
        where TMessage : IMessage
    {
        public TMessage Deserialize(IBinaryMessageData messageData)
        {
            if (messageData.Type != MessageType.User)
                throw new ArgumentException(nameof(messageData));

            using (var ms = new MemoryStream(messageData.Data))
            {
                return Serializer.Deserialize<TMessage>(ms);
            }
        }

        public IBinaryMessageData Serialize(IMessage message)
        {
            if(!(message is TMessage))
                throw new ArgumentException(nameof(message));

            return Serialize((TMessage) message);
        }

        public IBinaryMessageData Serialize(TMessage message)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, message);
                return new BinaryMessageData(ms.ToArray());
            }
        }

        IMessage IMessageSerializer.Deserialize(IBinaryMessageData messageData)
        {
            return Deserialize(messageData);
        }
    }
}
