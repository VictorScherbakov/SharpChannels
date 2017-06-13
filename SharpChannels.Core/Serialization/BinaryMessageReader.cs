using System;
using System.IO;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    internal class BinaryMessageReader
    {
        public static IBinaryMessageData Read(Stream stream, Action<int> checkMessageLength)
        {
            var br = new BinaryReader(stream);

            var type = br.ReadUInt16();
            var length = br.ReadInt32();
            checkMessageLength?.Invoke(length);

            var buffer = br.ReadBytes(length);

            if (buffer.Length != length)
                throw new EndOfStreamException();

            return new BinaryMessageData(buffer, (MessageType)type);
        }
    }
}