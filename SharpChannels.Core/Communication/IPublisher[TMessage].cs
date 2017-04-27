using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IPublisher<in TMessage>
        where TMessage : IMessage
    {
        bool Active { get; }
        void Broadcast(TMessage message);
        void Close();
        int SubscriberCount { get; }
    }
}