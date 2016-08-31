using System;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels
{
    public interface IChannel : IChannelEssentials, IDisposable
    {
        void Send(IMessage message);
        IMessage Receive();

        void Send(IMessage message, IMessageSerializer serializer);
        IMessage Receive(IMessageSerializer serializer);
    }
}