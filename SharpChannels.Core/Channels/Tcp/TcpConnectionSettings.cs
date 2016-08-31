using System;
using System.Net.Sockets;

namespace SharpChannels.Core.Channels.Tcp
{
    public class TcpConnectionSettings
    {
        public int ReceiveBufferSize { get; set; }
        public int SendBufferSize { get; set; }
        public TimeSpan SendTimeout { get; set; }
        public TimeSpan ReceiveTimeout { get; set; }

        public static TcpConnectionSettings GetDefault()
        {
            return new TcpConnectionSettings
            {
                SendTimeout = TimeSpan.FromMilliseconds(1000),
                ReceiveTimeout = TimeSpan.FromMilliseconds(1000),
                SendBufferSize = 8192,
                ReceiveBufferSize = 8192
            };
        }

        public void SetupClient(TcpClient client)
        {
            client.ReceiveBufferSize = ReceiveBufferSize;
            client.SendBufferSize = SendBufferSize;
            client.SendTimeout = (int)SendTimeout.TotalMilliseconds;
            client.ReceiveTimeout = (int)ReceiveTimeout.TotalMilliseconds;
        }
    }
}