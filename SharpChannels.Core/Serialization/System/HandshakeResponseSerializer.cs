using System;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Messages.System;

namespace SharpChannels.Core.Serialization.System
{
    internal class HandshakeResponseSerializer : IMessageSerializer
    {
        public IMessage Deserialize(IBinaryMessageData messageData)
        {
            if (messageData.Type != MessageType.HandshakeResponse)
                throw new ArgumentException("Message data has wrong type", nameof(messageData));

            var value = BitConverter.ToUInt16(messageData.Data, 0);
            return new HandshakeResponse(value);
        }

        public IBinaryMessageData Serialize(IMessage message)
        {
            var handshakeRequest = (HandshakeResponse)message;
            return new BinaryMessageData(BitConverter.GetBytes(handshakeRequest.ResponseMarker), MessageType.HandshakeResponse);
        }
    }
}