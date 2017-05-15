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

        public void Broadcast(string topic, TMessage message)
        {
            base.Broadcast(topic, message);
        }

        public void ParallelBroadcast(string topic, TMessage message, int parallelismDegree)
        {
            base.ParallelBroadcast(topic, message, parallelismDegree);
        }

        public async Task BroadcastAsync(string topic, TMessage message)
        {
            await base.BroadcastAsync(topic, message);
        }

        public async Task ParallelBroadcastAsync(string topic, TMessage message, int parallelismDegree)
        {
            await base.ParallelBroadcastAsync(topic, message, parallelismDegree);
        }
    }
}