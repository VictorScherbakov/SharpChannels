using System;
using System.Threading.Tasks;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IPublisher : IDisposable
    {
        bool Active { get; }
        void Close();

        int SubscriprionNumber { get; }

        void Broadcast(string topic, IMessage message);
        void ParallelBroadcast(string topic, IMessage message, int parallelismDegree);

        Task BroadcastAsync(string topic, IMessage message);
        Task ParallelBroadcastAsync(string topic, IMessage message, int parallelismDegree);
    }
}