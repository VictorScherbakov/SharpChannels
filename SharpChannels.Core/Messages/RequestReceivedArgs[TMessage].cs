using System;

namespace SharpChannels.Core.Messages
{
    public class RequestReceivedArgs<TMessage> : EventArgs
        where TMessage : IMessage
    {
        public TMessage Request { get; private set; }
        public TMessage Response { get; set; }

        public RequestReceivedArgs(TMessage request)
        {
            Request = request;
        }
    }
}