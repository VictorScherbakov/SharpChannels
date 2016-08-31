using System;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Messages.System;

namespace SharpChannels.Core.Serialization.System
{
    internal class HandshakeRequestSerializer : IMessageSerializer
    {
        public IMessage Deserialize(IBinaryMessageData messageData)
        {
            if (messageData.Type != MessageType.HandshakeRequest)
                throw new ArgumentException("Message data has wrong type", nameof(messageData));

            var value = BitConverter.ToUInt16(messageData.Data, 0);
            return new HandshakeRequest(value);
        }

        public IBinaryMessageData Serialize(IMessage message)
        {
            var handshakeRequest = (HandshakeRequest) message;
            return new BinaryMessageData(BitConverter.GetBytes(handshakeRequest.RequestMarker), MessageType.HandshakeRequest);
        }
    }
}