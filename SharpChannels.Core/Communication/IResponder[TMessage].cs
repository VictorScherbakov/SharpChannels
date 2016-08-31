using System;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IResponder<TMessage>
        where TMessage : IMessage
    {
        IChannel<TMessage> Channel { get; }
        void StartResponding();

        event EventHandler<RequestReceivedArgs<TMessage>> RequestReceived;
    }
}