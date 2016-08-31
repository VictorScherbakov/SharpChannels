using System;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;
namespace SharpChannels.Core.Communication
{
    public class Responder<TMessage> : ResponderBase, IResponder<TMessage>
        where TMessage : IMessage
    {
        public Responder(IChannel<TMessage> channel)
        {
            _channel = channel;
        }

        public IChannel<TMessage> Channel => (IChannel<TMessage>) _channel;

        public event EventHandler<RequestReceivedArgs<TMessage>> RequestReceived;

        protected virtual void OnRequestReceived(RequestReceivedArgs<TMessage> e)
        {
            RequestReceived?.Invoke(this, e);
        }

        protected override IMessage OnRequestReceived(IMessage request)
        {
            var args = new RequestReceivedArgs<TMessage>((TMessage)request);
            RequestReceived?.Invoke(this, args);
            return args.Response;
        }
    }
}