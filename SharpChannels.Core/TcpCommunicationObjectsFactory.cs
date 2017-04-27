using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core
{
    public class TcpCommunicationObjectsFactory<TMessage> : ICommunicationObjectsFactory<TMessage>
        where TMessage : IMessage
    {
        private readonly TcpEndpointData _endpointData;
        private readonly IMessageSerializer _serializer;
        private readonly ChannelSettings _channelSettings;
        private readonly TcpConnectionSettings _connetcionSettings;

        public IChannel<TMessage> CreateChannel()
        {
            return new TcpChannel<TMessage>(_endpointData, _serializer, _channelSettings, _connetcionSettings);
        }

        public IChannelAwaiter<IChannel<TMessage>> CreateChannelAwaiter()
        {
            return new TcpChannelAwaiter<TMessage>(_endpointData, _serializer, _channelSettings, _connetcionSettings);
        }

        public TcpCommunicationObjectsFactory(TcpEndpointData endpointData, IMessageSerializer serializer)
            : this(endpointData, serializer, ChannelSettings.GetDefaultSettings(), TcpConnectionSettings.GetDefault())
        {
        }

        public TcpCommunicationObjectsFactory(TcpEndpointData endpointData, 
                                              IMessageSerializer serializer, 
                                              ChannelSettings channelSettings, 
                                              TcpConnectionSettings connetcionSettings)
        {
            Enforce.NotNull(endpointData, nameof(endpointData));
            Enforce.NotNull(serializer, nameof(serializer));
            Enforce.NotNull(channelSettings, nameof(channelSettings));
            Enforce.NotNull(connetcionSettings, nameof(connetcionSettings));

            _endpointData = endpointData;
            _serializer = serializer;
            _channelSettings = channelSettings;
            _connetcionSettings = connetcionSettings;
        }
    }
}
