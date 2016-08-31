using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Intradomain
{
    public sealed class IntradomainChannelAwaiter : IntradomainChannelAwaiterBase
    {
        public IntradomainChannelAwaiter(IntradomainEndpoint endpoint, IMessageSerializer serializer,
            ChannelSettings channelSettings = null, IntradomainConnectionSettings connectionSettings = null)
            :base(endpoint, serializer, channelSettings, connectionSettings)
        {
        }
    }
}