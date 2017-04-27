using SharpChannels.Core.Channels;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public class Requester<TMessage> : IRequester<TMessage>
        where TMessage : IMessage
    {
        public IChannel<TMessage> Channel { get; }

        public TMessage Request(TMessage request)
        {
            if (!Channel.IsOpened)
                return default(TMessage);

            Channel.Send(request);
            return Channel.Receive();
        }

        public Requester(IChannel<TMessage> channel)
        {
            Enforce.NotNull(channel, nameof(channel));

            Channel = channel;
        }
    }
}
