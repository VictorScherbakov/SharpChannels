using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    public interface IMessageSerializer
    {
        IMessage Deserialize(IBinaryMessageData messageData);
        IBinaryMessageData Serialize(IMessage message);
    }
}