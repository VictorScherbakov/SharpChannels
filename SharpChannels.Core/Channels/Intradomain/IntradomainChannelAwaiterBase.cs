using System;
using System.Threading;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Intradomain
{
    public class IntradomainChannelAwaiterBase : IChannelAwaiter<IntradomainChannel>
    {
        private readonly IMessageSerializer _serializer;

        private readonly AutoResetEvent _stopEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _clientAccepted = new AutoResetEvent(false);
        protected readonly AutoResetEvent ChannelCreated = new AutoResetEvent(false);

        private void ThrowIfInactive()
        {
            if (!Active)
                throw new InvalidOperationException("IntradomainChannelAwaiter is inactive");
        }

        protected readonly ChannelSettings ChannelSettings;
        private readonly IntradomainConnectionManager _connectionManager;

        internal IntradomainSocket ServerSocket { get; private set; }

        protected readonly IntradomainConnectionSettings ConnectionSettings;

        protected virtual void OnErrorCreatingChannel(ErrorCreatingChannelEventArgs e)
        {
            ErrorCreatingChannel?.Invoke(this, e);
        }

        protected virtual IntradomainChannel CreateChannel(IMessageSerializer serializer)
        {
            return ChannelSettings != null
                ? IntradomainChannel.CreateAndOpen(ChannelCreated, ServerSocket, serializer, ChannelSettings, ConnectionSettings)
                : IntradomainChannel.CreateAndOpen(ChannelCreated, ServerSocket, serializer);
        }

        public event EventHandler<ErrorCreatingChannelEventArgs> ErrorCreatingChannel;

        public IntradomainChannel AwaitNewChannel()
        {
            ThrowIfInactive();

            while (true)
            {
                var waitResult = WaitHandle.WaitAny(new WaitHandle[] { _stopEvent, _clientAccepted });
                if (waitResult == 0)
                    return null;

                try
                {
                    return CreateChannel(_serializer);
                }
                catch (Exception ex)
                {
                    OnErrorCreatingChannel(new ErrorCreatingChannelEventArgs(ex));
                }
            }
        }

        public bool Active => _connectionManager.IsListening(ListeningEndpoint.Hub);

        public IntradomainEndpoint ListeningEndpoint { get; }

        public void Start()
        {
            if (Active)
                throw new InvalidOperationException("Already started");

            _connectionManager.Listen(ListeningEndpoint.Hub, client =>
            {
                ServerSocket = IntradomainSocket.ServerSocket(client.Hub, client.ConnectionId);

                _clientAccepted.Set();

                return ChannelCreated.WaitOne(ConnectionSettings?.ConnectTimeout ?? TimeSpan.FromMilliseconds(1000)) 
                    ? ServerSocket
                    : null;
            });
        }

        public void Stop()
        {
            ThrowIfInactive();

            _connectionManager.StopListening(ListeningEndpoint.Hub);
            _stopEvent.Set();
        }

        internal IntradomainChannelAwaiterBase(IntradomainEndpoint endpoint, IMessageSerializer serializer, ChannelSettings channelSettings = null, IntradomainConnectionSettings connectionSettings = null)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            ListeningEndpoint = endpoint;
            _serializer = serializer;
            ChannelSettings = channelSettings;
            ConnectionSettings = connectionSettings;

            _connectionManager = IntradomainConnectionManager.Instance;
        }
    }
}