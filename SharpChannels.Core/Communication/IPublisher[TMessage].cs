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

        int SubscriprionNumber { get; }

        void Broadcast(string topic, TMessage message);
        void ParallelBroadcast(string topic, TMessage message, int parallelismDegree);

        Task BroadcastAsync(string topic, TMessage message);
        Task ParallelBroadcastAsync(string topic, TMessage message, int parallelismDegree);
    }
}