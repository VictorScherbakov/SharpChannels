using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public class Requester : IRequester
    {
        public IChannel Channel { get; }

        public IMessage Request(IMessage request)
        {
            if (!Channel.IsOpened)
                return null;

            Channel.Send(request);
            return Channel.Receive();
        }

        public Requester(IChannel channel)
        {
            Channel = channel;
        }
    }
}