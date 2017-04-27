using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public class Publisher<TMessage> : Publisher, IPublisher<TMessage>
        where TMessage : IMessage
    {
        public Publisher(INewChannelRequestAcceptor newChannelRequestAcceptor, bool stopAcceptorOnClose) 
            : base(newChannelRequestAcceptor, stopAcceptorOnClose)
        {
        }

        public void Broadcast(TMessage message)
        {
            base.Broadcast(message);
        }
    }
}