using System;
using System.Collections.Generic;
using System.Linq;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    public class CompoundSerializer : IMessageSerializer
    {
        private readonly Dictionary<Type, IMessageSerializer> _serializersByType = new Dictionary<Type, IMessageSerializer>();
        private readonly Dictionary<ushort, IMessageSerializer> _serializersByCode = new Dictionary<ushort, IMessageSerializer>();
        private readonly Dictionary<IMessageSerializer, ushort> _codes = new Dictionary<IMessageSerializer, ushort>();

        public class SerializerInfo
        {
            public ushort Code { get; set; }
            public Type Type { get; set; }
            public IMessageSerializer Serializer { get; set; }
        }

        public IMessage Deserialize(IBinaryMessageData messageData)
        {
            var code = BitConverter.ToUInt16(messageData.Data, 0);

            if(!_serializersByCode.ContainsKey(code))
                throw new NotSupportedException($"Code {code} is not supported");

            var buffer = new byte[messageData.Data.Length - sizeof(ushort)];
            Buffer.BlockCopy(messageData.Data, sizeof(ushort), buffer, 0, messageData.Data.Length - sizeof(ushort));

            var serializer = _serializersByCode[code];
            return serializer.Deserialize(new BinaryMessageData(buffer));
        }

        public IBinaryMessageData Serialize(IMessage message)
        {
            var t = message.GetType();
            if (_serializersByType.ContainsKey(t))
            {
                var serializer = _serializersByType[t];
                var binaryData = serializer.Serialize(message);
                
                var buffer = new byte[binaryData.Data.Length + sizeof (ushort)];

                var code = BitConverter.GetBytes(_codes[serializer]);
                Buffer.BlockCopy(code, 0, buffer, 0, code.Length);
                Buffer.BlockCopy(binaryData.Data, 0, buffer, code.Length, binaryData.Data.Length);

                return new BinaryMessageData(buffer);
            }

            throw new NotSupportedException($"Messages of type {t.FullName} is not supported");
        }

        private bool CheckSerializers(IList<SerializerInfo> serializers)
        {
            var types = new HashSet<Type>(serializers.Select(s => s.Type));
            var codes = new HashSet<ushort>(serializers.Select(s => s.Code));

            if (!types.All(t => (typeof(IMessage).IsAssignableFrom(t))))
                return false;

            return serializers.Count == types.Count && serializers.Count == codes.Count;
        }

        public CompoundSerializer(IList<SerializerInfo> serializers)
        {
            if(!CheckSerializers(serializers))
                throw new ArgumentException("Serializers should have different codes and types, all types should be derived from IMessage", nameof(serializers));

            foreach (var serializerInfo in serializers)
            {
                _serializersByCode.Add(serializerInfo.Code, serializerInfo.Serializer);
                _serializersByType.Add(serializerInfo.Type, serializerInfo.Serializer);
                _codes.Add(serializerInfo.Serializer, serializerInfo.Code);
            }
        }
    }
}
