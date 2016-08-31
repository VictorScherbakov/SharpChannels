using System;
using System.Collections;
using System.IO;
using SharpChannels.Core.Messages;
using ProtoBuf;

namespace SharpChannels.Core.Serialization.Protobuf
{
    public class ProtobufSerializer : IMessageSerializer
    {
        private readonly Type _type;

        public IMessage Deserialize(IBinaryMessageData messageData)
        {
            if(messageData.Type != MessageType.User)
                throw new ArgumentException(nameof(messageData));

            using (var ms = new MemoryStream(messageData.Data))
            {
                var result = Serializer.Deserialize(_type, ms);
                return (IMessage)result;
            }
        }

        public IBinaryMessageData Serialize(IMessage message)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, message);
                return new BinaryMessageData(ms.ToArray());
            }
        }

        public ProtobufSerializer(Type type)
        {
            if (!((IList) type.GetInterfaces()).Contains(typeof (IMessage))
                || (!type.IsClass && !type.IsValueType))
            {
                throw new ArgumentException(nameof(type));
            }

            _type = type;
        }
    }
}
