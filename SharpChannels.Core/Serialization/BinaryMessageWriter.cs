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
            _binaryWriter.Write((ushort)message.Type);
            _binaryWriter.Write(message.Data.Length);

            _binaryWriter.Write(message.Data);
            _binaryWriter.Flush();
        }
    }
}