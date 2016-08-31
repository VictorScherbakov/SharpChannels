using System.IO;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    internal class BinaryMessageReader
    {
        public static IBinaryMessageData Read(Stream stream, int maxLength)
        {
            var br = new BinaryReader(stream);

            var type = br.ReadUInt16();
            var length = br.ReadInt32();
            if(length > maxLength)
                throw new ProtocolException($"Message is too long. Allowed {maxLength} bytes but {length} found", ProtocolErrorCode.MessageTooLong);

            var buffer = br.ReadBytes(length);

            if (buffer.Length != length)
                throw new EndOfStreamException();

            return new BinaryMessageData(buffer, (MessageType)type);
        }
    }
}