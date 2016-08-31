using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Intradomain
{
    public class IntradomainChannel<TMessage> : IntradomainChannel, IChannel<TMessage>
        where TMessage : IMessage
    {
        public void Send(TMessage message)
        {
            base.Send(message);
        }

        TMessage IChannel<TMessage>.Receive()
        {
            return (TMessage)Receive();
        }

        public IntradomainChannel(IntradomainEndpoint endpoint, IMessageSerializer serializer) 
            : this(endpoint, serializer, null, null)
        {
        }

        public IntradomainChannel(IntradomainEndpoint endpoint, IMessageSerializer serializer, ChannelSettings channelSettings = null, IntradomainConnectionSettings connectionSettings = null) 
            : base(endpoint, serializer, channelSettings, connectionSettings)
        {
        }

        internal IntradomainChannel(IntradomainSocket socket, IMessageSerializer serializer, ChannelSettings channelSettings = null, IntradomainConnectionSettings connectionSettings = null)
            : base(socket, serializer, channelSettings, connectionSettings)
        {
        }
    }
}