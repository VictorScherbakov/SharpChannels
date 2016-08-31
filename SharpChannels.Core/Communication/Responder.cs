using System;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public class Responder : ResponderBase, IResponder
    {
        public Responder(IChannel channel)
        {
            _channel = channel;
        }

        public IChannel Channel => _channel;

        public event EventHandler<RequestReceivedArgs> RequestReceived;

        protected override IMessage OnRequestReceived(IMessage request)
        {
            var args = new RequestReceivedArgs(request);
            RequestReceived?.Invoke(this, args);
            return args.Response;
        }
    }
}