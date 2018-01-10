using System;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IBusServer<TMessage> where TMessage : IMessage
    {
        int SubscriprionNumber { get; }
        event EventHandler<ExceptionEventArgs> CommunicationFailed;
        event EventHandler<ClientAcceptedEventArgs> ClientConnected;
    }
}