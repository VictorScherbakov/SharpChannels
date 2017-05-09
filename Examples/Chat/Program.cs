using System;
using System.Net;
using System.Net.Sockets;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace Examples.Chat
{
    class Program
    {
        private static TcpConnectionSettings GetChatConnectionSettings()
        {
            return new TcpConnectionSettingsBuilder()
                        .UsingReceiveTimeout(TimeSpan.FromHours(1))
                        .UsingSendTimeout(TimeSpan.FromHours(1))
                        .Build();
        }

        private static IChannel<StringMessage> StartFirstInstance()
        {
            var endpointData = new TcpEndpointData(IPAddress.Any, 2000);
            var channelAwaiter = new TcpChannelAwaiter<StringMessage>(endpointData,
                                                                      new StringMessageSerializer(),
                                                                      ChannelSettings.GetDefault(),
                                                                      GetChatConnectionSettings());

            try
            {
                channelAwaiter.Start();
                Console.WriteLine("Run another instance to begin chat");
                return (IChannel<StringMessage>)channelAwaiter.AwaitNewChannel();
            }
            catch (SocketException ex) when (ex.ErrorCode == 10048)
            {
                // someone has already got running listener on localhost:2000
                return null;
            }
        }

        private static IChannel<StringMessage> StartSecondInstance()
        {
            var channel = new TcpChannel<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000), 
                                                        new StringMessageSerializer(),
                                                        ChannelSettings.GetDefault(),
                                                        GetChatConnectionSettings());
            channel.Open();
            return channel;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("SharpChannels chat example");

            var channel = StartFirstInstance();
            if (channel == null)
            {
                Console.WriteLine("Trying to connect...");
                channel = StartSecondInstance();
            }
            
            var receiver = new Receiver(channel);
            receiver.MessageReceived += (sender, eventArgs) =>
            {
                // don't care about incomplete user input here
                Console.WriteLine(eventArgs.Message);
            };
            receiver.StartReceiving();

            Console.WriteLine("Ready to chat");

            while (channel.IsOpened)
            {
                var msg = Console.ReadLine();
                if(channel.IsOpened)
                    channel.Send(new StringMessage(msg));
            }

            Console.WriteLine("Chat closed");
            Console.ReadLine();
        }
    }
}
