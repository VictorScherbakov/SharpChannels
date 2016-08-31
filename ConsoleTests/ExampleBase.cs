using System;
using System.Net;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Intradomain;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace ConsoleTests
{
    public abstract class ExampleBase
    {
        public Transport Transport { get; protected set; }

        protected IChannelAwaiter<IChannel> CreateAwaiter(Transport transport, IEndpointData endpoint, IMessageSerializer serializer)
        {
            switch (transport)
            {
                case Transport.Tcp:
                    return new TcpChannelAwaiter<StringMessage>((TcpEndpointData)endpoint, serializer);
                case Transport.Intradomain:
                    return new IntradomainChannelAwaiter<StringMessage>((IntradomainEndpoint)endpoint, serializer);
                default:
                    throw new NotSupportedException();
            }
        }

        protected IChannel<StringMessage> CreateChannel(Transport transport, IEndpointData endpoint, IMessageSerializer serializer)
        {
            switch (transport)
            {
                case Transport.Tcp:
                    return new TcpChannel<StringMessage>((TcpEndpointData)endpoint, serializer);
                case Transport.Intradomain:
                    return new IntradomainChannel<StringMessage>((IntradomainEndpoint)endpoint, serializer);
                default:
                    throw new NotSupportedException();
            }
        }

        public abstract void Do();

        protected IEndpointData CreateEndpoint(Transport transport)
        {
            switch (transport)
            {
                case Transport.Tcp:
                    return new TcpEndpointData(IPAddress.Loopback, 2000);
                case Transport.Intradomain:
                    return new IntradomainEndpoint("test");
                default:
                    throw new NotSupportedException();
            }
        }
    }
}