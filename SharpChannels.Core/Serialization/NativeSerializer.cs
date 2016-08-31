using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    public class NativeSerializer<TMessage> : IMessageSerializer, IMessageSerializer<TMessage>
        where TMessage : IMessage
    {
        public IMessage Deserialize(IBinaryMessageData messageData)
        {
            var bf = new BinaryFormatter();

            using (var ms = new MemoryStream(messageData.Data))
            {
                return (IMessage) bf.Deserialize(ms);
            }
        }

        public IBinaryMessageData Serialize(IMessage message)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, message);
                ms.Flush();
                return new BinaryMessageData(ms.ToArray());
            }
        }

        public IBinaryMessageData Serialize(TMessage message)
        {
            return ((IMessageSerializer) this).Serialize(message);
        }

        TMessage IMessageSerializer<TMessage>.Deserialize(IBinaryMessageData messageData)
        {
            return (TMessage) ((IMessageSerializer) this).Deserialize(messageData);
        }
    }
}