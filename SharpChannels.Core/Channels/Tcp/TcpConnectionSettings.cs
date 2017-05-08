using System;
using System.Net.Sockets;
using SharpChannels.Core.Contracts;

namespace SharpChannels.Core.Channels.Tcp
{
    public class TcpConnectionSettings
    {
        public int ReceiveBufferSize { get; set; }
        public int SendBufferSize { get; set; }
        public TimeSpan SendTimeout { get; set; }
        public TimeSpan ReceiveTimeout { get; set; }

        public void SetupClient(TcpClient client)
        {
            Enforce.NotNull(client, nameof(client));

            client.ReceiveBufferSize = ReceiveBufferSize;
            client.SendBufferSize = SendBufferSize;
            client.SendTimeout = (int)SendTimeout.TotalMilliseconds;
            client.ReceiveTimeout = (int)ReceiveTimeout.TotalMilliseconds;
        }
    }
}