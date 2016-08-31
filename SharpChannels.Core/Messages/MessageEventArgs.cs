using System;
using SharpChannels.Core.Channels;

namespace SharpChannels.Core.Messages
{
    public class MessageEventArgs : EventArgs
    {
        public IMessage Message { get; private set; }
        public IChannel Channel { get; private set; }

        public MessageEventArgs(IMessage message, IChannel channel)
        {
            Message = message;
            Channel = channel;
        }
    }
}