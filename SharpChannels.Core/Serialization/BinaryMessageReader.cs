using System;
using System.IO;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    internal class BinaryMessageReader
    {
        private readonly BinaryReader _binaryReader;

        public BinaryMessageReader(Stream stream)
        {
            Enforce.NotNull(stream, nameof(stream));

            _binaryReader = new BinaryReader(stream);
        }

        public IBinaryMessageData Read(Action<int> checkMessageLength)
        {
            var type = _binaryReader.ReadUInt16();
            var length = _binaryReader.ReadInt32();

            if (!BitConverter.IsLittleEndian)
            {
                Endianness.Swap(ref type);
                Endianness.Swap(ref length);
            }

            checkMessageLength?.Invoke(length);

            var buffer = _binaryReader.ReadBytes(length);

            if (buffer.Length != length)
                throw new EndOfStreamException();

            return new BinaryMessageData(buffer, (MessageType)type);
        }
    }
}