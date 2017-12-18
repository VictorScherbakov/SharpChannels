using System.Net.Sockets;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Security;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Tcp
{
    public class TcpChannel<TMessage> : TcpChannel, IChannel<TMessage>
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

        internal TcpChannel(TcpClient client, 
                            IMessageSerializer serializer, 
                            ChannelSettings channelSettings, 
                            TcpConnectionSettings connetcionSettings,
                            ISecurityWrapper serverSecurityWrapper) 
            : base(client, serializer, channelSettings, connetcionSettings, serverSecurityWrapper)
        {
        }

        public TcpChannel(TcpEndpointData endpointData, IMessageSerializer serializer) 
            : base(endpointData, serializer)
        {
        }

        public TcpChannel(TcpEndpointData endpointData, 
                          IMessageSerializer serializer, 
                          ChannelSettings channelSettings, 
                          TcpConnectionSettings connetcionSettings,
                          ISecurityWrapper clientSecurityWrapper) 
            : base(endpointData, serializer, channelSettings, connetcionSettings, clientSecurityWrapper)
        {
        }
    }
}