using System;
using System.Net.Sockets;
using System.Threading;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels.Tcp
{
    public class TcpChannelAwaiter : IChannelAwaiter<TcpChannel>
    {
        private readonly IMessageSerializer _serializer;
        private readonly TcpListenerEx _listener;

        private readonly AutoResetEvent _stopEvent = new AutoResetEvent(false);

        private void ThrowIfInactive()
        {
            if (!Active)
                throw new InvalidOperationException("TcpChannelAwaiter is inactive");
        }

        protected readonly ChannelSettings ChannelSettings;
        protected readonly TcpConnectionSettings ConnectionSettings;

        protected virtual void OnErrorCreatingChannel(ErrorCreatingChannelEventArgs e)
        {
            ErrorCreatingChannel?.Invoke(this, e);
        }

        protected virtual TcpChannel CreateChannel(TcpClient client, IMessageSerializer serializer)
        {
            return new TcpChannel(client, serializer, 
                ChannelSettings ?? ChannelSettings.GetDefaultSettings(), 
                ConnectionSettings ?? TcpConnectionSettings.GetDefault());
        }

        public event EventHandler<ErrorCreatingChannelEventArgs> ErrorCreatingChannel;

        public TcpChannel AwaitNewChannel()
        {
            ThrowIfInactive();

            try
            {
                while (true)
                {
                    var asyncResult = _listener.BeginAcceptTcpClient(null, null);

                    var waitResult = WaitHandle.WaitAny(new[] { _stopEvent, asyncResult.AsyncWaitHandle });
                    if (waitResult == 0)
                        return null;
                    
                    var client = _listener.EndAcceptTcpClient(asyncResult);

                    try
                    {
                        return CreateChannel(client, _serializer);
                    }
                    catch (Exception ex)
                    {
                        OnErrorCreatingChannel(new ErrorCreatingChannelEventArgs(ex));
                    }
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
            {
                return null;
            }
        }

        public bool Active => _listener.Active;

        public TcpEndpointData ListeningData { get; }

        public void Start()
        {
            if (Active)
                throw new InvalidOperationException("Already started");

            _listener.Start();
        }

        public void Stop()
        {
            ThrowIfInactive();

            _listener.Stop();
            _stopEvent.Set();
        }

        public TcpChannelAwaiter(TcpEndpointData endpointData, IMessageSerializer serializer, ChannelSettings channelSettings = null, TcpConnectionSettings connectionSettings = null)
        {
            if (endpointData == null) throw new ArgumentNullException(nameof(endpointData));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));

            ListeningData = endpointData;
            _serializer = serializer;
            ChannelSettings = channelSettings;
            ConnectionSettings = connectionSettings;
            _listener = new TcpListenerEx(endpointData.Address, endpointData.Port);
            _listener.ExclusiveAddressUse = true;
        }
    }
}