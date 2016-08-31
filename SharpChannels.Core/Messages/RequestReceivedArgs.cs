using System;

namespace SharpChannels.Core.Messages
{
    public class RequestReceivedArgs : EventArgs
    {
        public IMessage Request { get; private set; }
        public IMessage Response { get; set; }

        public RequestReceivedArgs(IMessage request)
        {
            Request = request;
        }
    }
}