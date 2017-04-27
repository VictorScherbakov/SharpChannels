using System;
using System.IO;
using SharpChannels.Core.Contracts;

namespace SharpChannels.Core.Channels.Intradomain
{
    internal class IntradomainConnetcion
    {
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
            Enforce.NotNull(server, nameof(server));
            Enforce.NotNull(client, nameof(client));
            Enforce.NotNull(settings, nameof(settings));

            Enforce.IsTrue(server.Type == SocketType.Server, nameof(server));
            Enforce.IsTrue(client.Type == SocketType.Client, nameof(client));
            Enforce.IsTrue(client.ConnectionId == server.ConnectionId, "Client and server sockets should have the same ConnectionId", nameof(client));

            ServerSocket = server;
            ClientSocket = client;

            _serverStream = new IntradomainStream(settings.ReceiveTimeout);
            _clientStream = new IntradomainStream(settings.ReceiveTimeout);

            _serverStream.Partner = _clientStream;
            _clientStream.Partner = _serverStream;
        }
    }
}