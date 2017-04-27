using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Intradomain
{
    public class IntradomainChannelAwaiterFactory<TMessage> : IChannelAwaiterFactory<IChannel<TMessage>>
        where TMessage : IMessage
    {
        public IChannelAwaiter<IChannel<TMessage>> CreateInstance(IEndpointData endpointData, IMessageSerializer serializer)
        {
            Enforce.NotNull(endpointData, nameof(endpointData));
            Enforce.NotNull(serializer, nameof(serializer));

            Enforce.Is<IntradomainEndpoint>(endpointData, nameof(endpointData));

            return new IntradomainChannelAwaiter<TMessage>((IntradomainEndpoint)endpointData, serializer);
        }
    }
}