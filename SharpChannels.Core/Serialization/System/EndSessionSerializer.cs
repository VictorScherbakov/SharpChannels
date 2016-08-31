using System;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Messages.System;

namespace SharpChannels.Core.Serialization.System
{
    internal class EndSessionSerializer : IMessageSerializer
    {
        private static readonly byte[] _emptyByteArray = new byte[0];

        public IMessage Deserialize(IBinaryMessageData messageData)
        {
            if (messageData.Type != MessageType.EndSession)
                throw new ArgumentException("Message data has wrong type", nameof(messageData));

            return EndSessionMessage.Instance;
        }

        public IBinaryMessageData Serialize(IMessage message)
        {
            if (!(message is EndSessionMessage))
                throw new ArgumentException(nameof(message));

            return new BinaryMessageData(_emptyByteArray, MessageType.EndSession);
        }
    }
}