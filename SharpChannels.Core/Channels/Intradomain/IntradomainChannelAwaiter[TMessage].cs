using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Intradomain
{
    public sealed class IntradomainChannelAwaiter<TMessage> : IntradomainChannelAwaiterBase, IChannelAwaiter<IntradomainChannel<TMessage>>
        where TMessage : IMessage
    {
        public IntradomainChannelAwaiter(IntradomainEndpoint endpoint, IMessageSerializer serializer, ChannelSettings channelSettings = null, IntradomainConnectionSettings connectionSettings = null) 
            : base(endpoint, serializer, channelSettings, connectionSettings)
        {
        }

        protected override IntradomainChannel CreateChannel(IMessageSerializer serializer)
        {
            return ChannelSettings != null
                ? IntradomainChannel.CreateAndOpen<TMessage>(ChannelCreated, ServerSocket, serializer, ChannelSettings, ConnectionSettings)
                : IntradomainChannel.CreateAndOpen<TMessage>(ChannelCreated, ServerSocket, serializer);
        }

        IntradomainChannel<TMessage> IChannelAwaiter<IntradomainChannel<TMessage>>.AwaitNewChannel()
        {
            return (IntradomainChannel<TMessage>)AwaitNewChannel();
        }
    }
}