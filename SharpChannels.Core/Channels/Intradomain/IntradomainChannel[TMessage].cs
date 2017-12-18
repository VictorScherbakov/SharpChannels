using SharpChannels.Core.Messages;
using SharpChannels.Core.Security;
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
            : this(endpoint, serializer, null, null, null)
        {
        }

        public IntradomainChannel(IntradomainEndpoint endpoint, 
                                  IMessageSerializer serializer, 
                                  ChannelSettings channelSettings = null, 
                                  IntradomainConnectionSettings connectionSettings = null,
                                  ISecurityWrapper clientSecurityWrapper = null) 
            : base(endpoint, serializer, channelSettings, connectionSettings, clientSecurityWrapper)
        {
        }

        internal IntradomainChannel(IntradomainSocket socket, 
                                    IMessageSerializer serializer, 
                                    ChannelSettings channelSettings = null, 
                                    IntradomainConnectionSettings connectionSettings = null,
                                    ISecurityWrapper serverSecurityWrapper = null)
            : base(socket, serializer, channelSettings, connectionSettings, serverSecurityWrapper)
        {
        }
    }
}