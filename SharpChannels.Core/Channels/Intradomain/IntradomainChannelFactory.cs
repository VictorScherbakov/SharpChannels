using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Intradomain
{
    public class IntradomainChannelFactory : IChannelFactory
    {
        private readonly IntradomainEndpoint _endpointData;
        private readonly IMessageSerializer _serializer;

        public IChannel CreateInstance()
        {
            return new IntradomainChannel(_endpointData, _serializer);
        }

        public IntradomainChannelFactory(IntradomainEndpoint endpointData, IMessageSerializer serializer)
        {
            _endpointData = endpointData;
            _serializer = serializer;
        }
    }
}