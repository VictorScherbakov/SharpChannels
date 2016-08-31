using System.Net.Sockets;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Tcp
{
    public class TcpChannelAwaiter<TMessage> : TcpChannelAwaiter, IChannelAwaiter<TcpChannel<TMessage>>
        where TMessage : IMessage
    {
        protected override TcpChannel CreateChannel(TcpClient client, IMessageSerializer serializer)
        {
            return new TcpChannel<TMessage>(client, serializer, ChannelSettings ?? ChannelSettings.GetDefaultSettings(), ConnectionSettings ?? TcpConnectionSettings.GetDefault());
        }

        TcpChannel<TMessage> IChannelAwaiter<TcpChannel<TMessage>>.AwaitNewChannel()
        {
            return (TcpChannel<TMessage>)AwaitNewChannel();
        }

        public TcpChannelAwaiter(TcpEndpointData endpointData, IMessageSerializer serializer, ChannelSettings channelSettings = null, TcpConnectionSettings connectionSettings = null)
            : base(endpointData, serializer, channelSettings, connectionSettings)
        {
        }
    }
}