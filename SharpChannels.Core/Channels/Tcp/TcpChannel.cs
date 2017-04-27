using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Tcp
{
    public class TcpChannel : ChannelBase
    {
        private readonly TcpEndpointData _endpointData;
        private TcpClient _client;

        public override IEndpointData EndpointData => _endpointData;

        protected override void OpenTransport()
        {
            _client.Connect(_endpointData.Address, _endpointData.Port);
        }

        protected override Stream Stream => _client.GetStream();

        protected override void CloseInternal()
        {
            Enforce.State.FitsTo(IsOpened, "Already closed");

            _client.Close();
        }

        public override bool IsOpened => _client.Connected;

        internal TcpChannel(TcpClient client, IMessageSerializer serializer, ChannelSettings channelSettings, TcpConnectionSettings connetcionSettings)
        {
            Enforce.NotNull(client, nameof(client));
            Enforce.NotNull(serializer, nameof(serializer));

            var ipEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            _endpointData = new TcpEndpointData(ipEndPoint.Address, ipEndPoint.Port);
            Serializer = serializer;
            _client = client;
            _client.NoDelay = true;

            connetcionSettings.SetupClient(_client);
            MaxMessageLength = channelSettings.MaxMessageLength;

            ResponseHandshake();
        }

        public TcpChannel(TcpEndpointData endpointData, IMessageSerializer serializer)
        {
            Enforce.NotNull(endpointData, nameof(endpointData));
            Enforce.NotNull(serializer, nameof(serializer));

            _endpointData = endpointData;
            Serializer = serializer;
            _client = new TcpClient(endpointData.Address.AddressFamily)
            {
                NoDelay = true
            };

            MaxMessageLength = ChannelSettings.GetDefaultSettings().MaxMessageLength;
        }

        public TcpChannel(TcpEndpointData endpointData, IMessageSerializer serializer, ChannelSettings channelSettings, TcpConnectionSettings connetcionSettings)
            : this(endpointData, serializer)
        {
            connetcionSettings.SetupClient(_client);

            MaxMessageLength = channelSettings.MaxMessageLength;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    ((IDisposable) _client).Dispose();
                    _client = null;
                }
            }
        }

        ~TcpChannel()
        {
            Dispose(false);
        }
    }
}