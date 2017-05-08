using System;
using System.Net.Sockets;
using System.Threading;
using SharpChannels.Core.Contracts;
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
            Enforce.State.FitsTo(Active, "TcpChannelAwaiter is inactive");
        }

        protected readonly ChannelSettings ChannelSettings;
        protected readonly TcpConnectionSettings ConnectionSettings;

        protected virtual void OnErrorCreatingChannel(ExceptionEventArgs e)
        {
            ErrorCreatingChannel?.Invoke(this, e);
        }

        protected virtual TcpChannel CreateChannel(TcpClient client, IMessageSerializer serializer)
        {
            return new TcpChannel(client, serializer, 
                ChannelSettings ?? ChannelSettings.GetDefault(), 
                ConnectionSettings ?? TcpConnectionSettingsBuilder.GetDefaultSettings());
        }

        public event EventHandler<ExceptionEventArgs> ErrorCreatingChannel;

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
                        OnErrorCreatingChannel(new ExceptionEventArgs(ex));
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
            Enforce.State.FitsTo(!Active, "Already started");

            _listener.Start();
        }

        public void Stop()
        {
            ThrowIfInactive();

            _stopEvent.Set();
            _listener.Stop();
        }

        public TcpChannelAwaiter(TcpEndpointData endpointData, IMessageSerializer serializer, ChannelSettings channelSettings = null, TcpConnectionSettings connectionSettings = null)
        {
            Enforce.NotNull(endpointData, nameof(endpointData));
            Enforce.NotNull(serializer, nameof(serializer));

            ListeningData = endpointData;
            _serializer = serializer;
            ChannelSettings = channelSettings;
            ConnectionSettings = connectionSettings;
            _listener = new TcpListenerEx(endpointData.Address, endpointData.Port);
            _listener.ExclusiveAddressUse = true;
        }
    }
}