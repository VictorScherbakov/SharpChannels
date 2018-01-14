using System;
using System.IO;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    internal class BinaryMessageWriter
    {
        private readonly BinaryWriter _binaryWriter;

        public BinaryMessageWriter(Stream stream)
        {
            _binaryWriter = new BinaryWriter(stream);
        }

        public void Write(IBinaryMessageData message)
        {
            var type = (ushort)message.Type;
            var length = message.Data.Length;

            if (!BitConverter.IsLittleEndian)
            {
                Endianness.Swap(ref type);
                Endianness.Swap(ref length);
            }

            _binaryWriter.Write(type);
            _binaryWriter.Write(length);

            _binaryWriter.Write(message.Data);
            _binaryWriter.Flush();
        }
    }
}