using System;

namespace SharpChannels.Core.Messages
{
    public class MessageEventArgs : EventArgs
    {
        public IMessage Message { get; private set; }

        public MessageEventArgs(IMessage message)
        {
            Message = message;
        }
    }
}