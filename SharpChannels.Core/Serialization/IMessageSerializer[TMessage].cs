using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Serialization
{
    public interface IMessageSerializer<TMessage>
        where TMessage : IMessage
    {
        TMessage Deserialize(IBinaryMessageData messageData);
        IBinaryMessageData Serialize(TMessage message);
    }
}