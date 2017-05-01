using System;
using System.Threading.Tasks;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IPublisher : IDisposable
    {
        bool Active { get; }
        void Close();

        int SubscriberCount { get; }

        void Broadcast(IMessage message);
        void ParallelBroadcast(IMessage message, int parallelismDegree);

        Task BroadcastAsync(IMessage message);
        Task ParallelBroadcastAsync(IMessage message, int parallelismDegree);
    }
}