using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Messages.PubSub;

namespace SharpChannels.Core.Serialization.PubSub
{
    internal class SubscribeMessageSerializer : IMessageSerializer, IMessageSerializer<SubscribeMessage>
    {
        IMessage IMessageSerializer.Deserialize(IBinaryMessageData messageData)
        {
            return Deserialize(messageData);
        }

        public IBinaryMessageData Serialize(SubscribeMessage message)
        {
            using (var ms = new MemoryStream())
            {
                var binaryWriter = new BinaryWriter(ms);
                binaryWriter.Write(message.Topics.Length);

                foreach (var topic in message.Topics)
                {
                    var topicBytes = Encoding.UTF8.GetBytes(topic);
                    binaryWriter.Write(topicBytes.Length);
                    binaryWriter.Write(topicBytes);
                }

                ms.Flush();
                return new BinaryMessageData(ms.ToArray(), MessageType.User);
            }
        }

        public IBinaryMessageData Serialize(IMessage message)
        {
            Enforce.Is<SubscribeMessage>(message, nameof(message));

            return Serialize((SubscribeMessage)message);
        }

        public SubscribeMessage Deserialize(IBinaryMessageData messageData)
        {
            using (var ms = new MemoryStream(messageData.Data))
            {
                ms.Position = 0;
                var br = new BinaryReader(ms);
                var topicCount = br.ReadInt32();

                if(topicCount < 0) throw new ProtocolException("Negative topic number", ProtocolErrorCode.InvalidTopicNumber);
                if (topicCount > 100) throw new ProtocolException("Too many topics", ProtocolErrorCode.TooManyTopics);

                var topicList = new List<string>();

                for (int i = 0; i < topicCount; i++)
                {
                    var length = br.ReadInt32();
                    topicList.Add(Encoding.UTF8.GetString(br.ReadBytes(length)));
                }
                return new SubscribeMessage(topicList);
            }
        }
    }
}