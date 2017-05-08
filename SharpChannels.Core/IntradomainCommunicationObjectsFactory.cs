using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Intradomain;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core
{
    public class IntradomainCommunicationObjectsFactory<TMessage> : ICommunicationObjectsFactory<TMessage>
        where TMessage : IMessage
    {
        private readonly IntradomainEndpoint _endpoint;
        private readonly IMessageSerializer _serializer;
        private readonly ChannelSettings _channelSettings;
        private readonly IntradomainConnectionSettings _connetcionSettings;

        public IChannel<TMessage> CreateChannel()
        {
            return new IntradomainChannel<TMessage>(_endpoint, _serializer, _channelSettings, _connetcionSettings);
        }

        public IChannelAwaiter<IChannel<TMessage>> CreateChannelAwaiter()
        {
            return new IntradomainChannelAwaiter<TMessage>(_endpoint, _serializer, _channelSettings, _connetcionSettings);
        }

        public IntradomainCommunicationObjectsFactory(IntradomainEndpoint endpoint, IMessageSerializer serializer)
            : this(endpoint, serializer, ChannelSettings.GetDefault(), IntradomainConnectionSettingsBuilder.GetDefaultSettings())
        {
        }

        public IntradomainCommunicationObjectsFactory(IntradomainEndpoint endpoint,
                                                      IMessageSerializer serializer,
                                                      ChannelSettings channelSettings,
                                                      IntradomainConnectionSettings connetcionSettings)
        {
            Enforce.NotNull(endpoint, nameof(endpoint));
            Enforce.NotNull(serializer, nameof(serializer));
            Enforce.NotNull(channelSettings, nameof(channelSettings));
            Enforce.NotNull(connetcionSettings, nameof(connetcionSettings));

            _endpoint = endpoint;
            _serializer = serializer;
            _channelSettings = channelSettings;
            _connetcionSettings = connetcionSettings;
        }
    }
}