using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    public class BinaryMessageData : IBinaryMessageData
    {
        public MessageType Type { get; }
        public byte[] Data { get; }

        public BinaryMessageData(byte[] data)
            : this(data, MessageType.User)
        {
        }

        internal BinaryMessageData(byte[] data, MessageType type)
        {
            Data = data;
            Type = type;
        }
    }
}