using System;
using System.IO;
using System.Threading;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Security;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Intradomain
{
    public class IntradomainChannel :  ChannelBase
    {
        private readonly IntradomainConnectionSettings _connectionSettings;
        private readonly IntradomainEndpoint _endpoint;

        private readonly IntradomainSocket _socket;
        private readonly IntradomainConnectionManager _connectionManager;

        protected override void CloseInternal()
        {
            Enforce.State.FitsTo(IsOpened, "Already closed");

            _connectionManager.Disconnect(_socket);
        }

        protected override Stream GetStream() => _connectionManager.GetStream(_socket);
        protected override ISecurityWrapper SecurityWrapper { get; }

        public override IEndpointData EndpointData => _endpoint;


        protected override void OpenTransport()
        {
            if (!IntradomainConnectionManager.Instance.Connect(_socket, _connectionSettings))
                throw new Exception("Error establish intradomain connection");
        }

        public override bool IsOpened => _connectionManager.Connected(_socket);

        ~IntradomainChannel()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (IsOpened)
            {
                _connectionManager.Disconnect(_socket);
               base.Dispose(true);
            }
        }

        private static void EstablishConnection(IntradomainChannel channel, 
                                             AutoResetEvent channelCreatedEvent, 
                                             IntradomainSocket socket, 
                                             TimeSpan connectTimeout)
        {
            // release waiting client
            channelCreatedEvent.Set();

            channel._connectionManager.WaitForConnection(socket.ConnectionId, (int)connectTimeout.TotalMilliseconds);

            channel.ResponseHandshake();
        }

        internal static IntradomainChannel CreateAndOpen(AutoResetEvent channelCreatedEvent, 
                                                         IntradomainSocket socket, 
                                                         IMessageSerializer serializer, 
                                                         ChannelSettings channelSettings = null, 
                                                         IntradomainConnectionSettings connectionSettings = null,
                                                         ISecurityWrapper serverSecurityWrapper = null)
        {
            var result = new IntradomainChannel(socket, serializer, channelSettings, connectionSettings, serverSecurityWrapper);

            EstablishConnection(result, channelCreatedEvent, socket, connectionSettings?.ConnectTimeout ?? IntradomainConnectionSettingsBuilder.GetDefaultSettings().ConnectTimeout);
            return result;
        }

        internal static IntradomainChannel<TMessage> CreateAndOpen<TMessage>(AutoResetEvent channelCreatedEvent, 
                                                                             IntradomainSocket socket, 
                                                                             IMessageSerializer serializer, 
                                                                             ChannelSettings channelSettings = null, 
                                                                             IntradomainConnectionSettings connectionSettings = null,
                                                                             ISecurityWrapper serverSecurityWrapper = null)
            where TMessage : IMessage
        {
            var result = new IntradomainChannel<TMessage>(socket, serializer, channelSettings, connectionSettings, serverSecurityWrapper);

            EstablishConnection(result, channelCreatedEvent, socket, connectionSettings?.ConnectTimeout ?? IntradomainConnectionSettingsBuilder.GetDefaultSettings().ConnectTimeout);
            return result;
        }

        internal IntradomainChannel(IntradomainSocket socket, IMessageSerializer serializer)
        {
            Serializer = serializer;

            _socket = socket;
            _connectionManager = IntradomainConnectionManager.Instance;
        }

        internal IntradomainChannel(IntradomainEndpoint endpoint, SocketType socketType, IMessageSerializer serializer)
        {
            Serializer = serializer;
            _endpoint = endpoint;

            _socket = new IntradomainSocket(socketType, endpoint.Hub);
            _connectionManager = IntradomainConnectionManager.Instance;
        }

        public IntradomainChannel(IntradomainEndpoint endpoint, 
                                  IMessageSerializer serializer, 
                                  ChannelSettings channelSettings = null, 
                                  IntradomainConnectionSettings connectionSettings = null,
                                  ISecurityWrapper clientSecurityWrapper = null)
            : this(endpoint, SocketType.Client, serializer)
        {
            _connectionSettings = connectionSettings ?? IntradomainConnectionSettingsBuilder.GetDefaultSettings();
            MaxMessageLength = channelSettings?.MaxMessageLength ?? ChannelSettings.GetDefault().MaxMessageLength;

            SecurityWrapper = clientSecurityWrapper;
        }

        internal IntradomainChannel(IntradomainSocket socket, 
                                    IMessageSerializer serializer, 
                                    ChannelSettings channelSettings = null, 
                                    IntradomainConnectionSettings connectionSettings = null,
                                    ISecurityWrapper serverSecurityWrapper = null)
            : this(socket, serializer)
        {
            _connectionSettings = connectionSettings ?? IntradomainConnectionSettingsBuilder.GetDefaultSettings();
            MaxMessageLength = channelSettings?.MaxMessageLength ?? ChannelSettings.GetDefault().MaxMessageLength;

            SecurityWrapper = serverSecurityWrapper;
        }
    }
}