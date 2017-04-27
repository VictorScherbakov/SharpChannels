using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IPublisher
    {
        bool Active { get; }
        void Broadcast(IMessage message);
        void Close();
        int SubscriberCount { get; }
    }
}