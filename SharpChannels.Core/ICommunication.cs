﻿using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core
{
    public interface ICommunication<TMessage> 
        where TMessage : IMessage
    {
        IChannel<TMessage> CreateChannel();
        IChannelAwaiter<IChannel<TMessage>> CreateChannelAwaiter();
    }
}