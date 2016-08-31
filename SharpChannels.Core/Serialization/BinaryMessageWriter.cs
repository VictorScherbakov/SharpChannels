using System.IO;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    internal class BinaryMessageWriter
    {
        public static void Write(Stream stream, IBinaryMessageData message)
        {
            var bw = new BinaryWriter(stream);

            bw.Write((ushort)message.Type);
            bw.Write(message.Data.Length);

            bw.Write(message.Data);
        }
    }
}