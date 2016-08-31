using System.Net;

namespace SharpChannels.Core.Channels.Tcp
{
    public class TcpEndpointData : IEndpointData
    {
        public IPAddress Address { get; }

        public int Port { get; }

        public TcpEndpointData(IPAddress address, int port)
        {
            Address = address;
            Port = port;
        }
    }
}