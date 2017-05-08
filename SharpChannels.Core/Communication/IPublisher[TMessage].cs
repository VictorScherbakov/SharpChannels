using System;
using System.Threading.Tasks;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IPublisher<in TMessage> : IDisposable
        where TMessage : IMessage
    {
        bool Active { get; }
        void Close();

        int SubscriberCount { get; }

        void Broadcast(TMessage message);
        void ParallelBroadcast(TMessage message, int parallelismDegree);

        Task BroadcastAsync(TMessage message);
        Task ParallelBroadcastAsync(TMessage message, int parallelismDegree);
    }
}