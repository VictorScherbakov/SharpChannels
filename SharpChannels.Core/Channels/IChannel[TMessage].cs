using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Channels
{
    public interface IChannel<TMessage> : IChannel
    where TMessage : IMessage
    {
        void Send(TMessage message);
        new TMessage Receive();
    }
}