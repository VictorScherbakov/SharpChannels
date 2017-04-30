using System.Threading.Tasks;
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

        public void ParallelBroadcast(TMessage message, int parallelismDegree)
        {
            base.ParallelBroadcast(message, parallelismDegree);
        }

        public async Task BroadcastAsync(TMessage message)
        {
            await base.BroadcastAsync(message);
        }

        public async Task ParallelBroadcastAsync(TMessage message, int parallelismDegree)
        {
            await base.ParallelBroadcastAsync(message, parallelismDegree);
        }
    }
}