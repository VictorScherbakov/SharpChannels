using System;
using System.IO;

namespace SharpChannels.Core.Channels.Intradomain
{
    internal class IntradomainConnetcion
    {
        private readonly IntradomainConnectionSettings _settings;
        private readonly IntradomainStream _serverStream;
        private readonly IntradomainStream _clientStream;

        public IntradomainSocketState ClientSocketState { get; set; }
        public IntradomainSocketState ServerSocketState { get; set; }

        public string Name { get; set; }

        public IntradomainSocket ServerSocket { get; }
        public IntradomainSocket ClientSocket { get; }

        public bool IsCompletelyDisconnected()
        {
            return ClientSocketState == IntradomainSocketState.Disconnected &&
                   ServerSocketState == IntradomainSocketState.Disconnected;
        }

        public void DisconnectClient()
        {
            if(ClientSocketState == IntradomainSocketState.Connected)
                ClientSocketState = IntradomainSocketState.Disconnected;
        }

        public void DisconnectServer()
        {
            if (ServerSocketState == IntradomainSocketState.Connected)
                ServerSocketState = IntradomainSocketState.Disconnected;
        }

        public Stream GetStream(IntradomainSocket endpoint)
        {
            if (ServerSocket == endpoint)
                return _serverStream;

            if (ClientSocket == endpoint)
                return _clientStream;

            throw new ArgumentException(nameof(endpoint));
        }

        public IntradomainConnetcion(IntradomainSocket server, IntradomainSocket client, IntradomainConnectionSettings settings)
        {
            _settings = settings;
            if (server == null) throw new ArgumentNullException(nameof(server));
            if (client == null) throw new ArgumentNullException(nameof(client));

            if(server.Type != SocketType.Server) throw new ArgumentException(nameof(server));
            if(client.Type != SocketType.Client) throw new ArgumentException(nameof(client));

            if(client.ConnectionId != server.ConnectionId)
                throw new ArgumentException("Client and server sockets should have the same name", nameof(client));

            ServerSocket = server;
            ClientSocket = client;

            _serverStream = new IntradomainStream(_settings.ReceiveTimeout);
            _clientStream = new IntradomainStream(_settings.ReceiveTimeout);

            _serverStream.Partner = _clientStream;
            _clientStream.Partner = _serverStream;
        }
    }
}