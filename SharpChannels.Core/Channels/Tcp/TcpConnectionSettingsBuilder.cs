using System;

namespace SharpChannels.Core.Channels.Tcp
{
    public class TcpConnectionSettingsBuilder
    {
        private readonly TcpConnectionSettings _settings = GetDefaultSettings();

        public static TcpConnectionSettings GetDefaultSettings()
        {
            return new TcpConnectionSettings
            {
                SendTimeout = TimeSpan.FromMilliseconds(1000),
                ReceiveTimeout = TimeSpan.FromMilliseconds(1000),
                SendBufferSize = 8192,
                ReceiveBufferSize = 8192
            };

        }

        public TcpConnectionSettingsBuilder UsingSendTimeout(TimeSpan sendTimeout)
        {
            _settings.SendTimeout = sendTimeout;
            return this;
        }

        public TcpConnectionSettingsBuilder UsingReceiveTimeout(TimeSpan receiveTimeout)
        {
            _settings.ReceiveTimeout = receiveTimeout;
            return this;
        }

        public TcpConnectionSettingsBuilder UsingSendBuffer(int sendBufferSize)
        {
            _settings.SendBufferSize = sendBufferSize;
            return this;
        }

        public TcpConnectionSettingsBuilder UsingReceiveBufferSize(int receiveBufferSize)
        {
            _settings.ReceiveBufferSize = receiveBufferSize;
            return this;
        }

        public TcpConnectionSettings Build()
        {
            return _settings;
        }
    }
}