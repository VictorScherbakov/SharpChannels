using System.Net.Sockets;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Security;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Tcp
{
    public class TcpChannelAwaiter<TMessage> : TcpChannelAwaiter, IChannelAwaiter<TcpChannel<TMessage>>
        where TMessage : IMessage
    {
        protected override TcpChannel CreateChannel(TcpClient client, IMessageSerializer serializer)
        {
            return new TcpChannel<TMessage>(client, 
                                            serializer, 
                                            ChannelSettings ?? ChannelSettings.GetDefault(), 
                                            ConnectionSettings ?? TcpConnectionSettingsBuilder.GetDefaultSettings(),
                                            ServerSecurityWrapper);
        }

        TcpChannel<TMessage> IChannelAwaiter<TcpChannel<TMessage>>.AwaitNewChannel()
        {
            return (TcpChannel<TMessage>)AwaitNewChannel();
        }

        public TcpChannelAwaiter(TcpEndpointData endpointData, 
                                 IMessageSerializer serializer, 
                                 ChannelSettings channelSettings = null, 
                                 TcpConnectionSettings connectionSettings = null,
                                 ISecurityWrapper serverSecurityWrapper = null)
            : base(endpointData, 
                  serializer, 
                  channelSettings, 
                  connectionSettings, 
                  serverSecurityWrapper)
        {
        }
    }
}