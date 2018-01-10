using System;
using System.Net;
using System.Threading;
using SharpChannels.Core;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace Examples.PubSub
{
    class Program
    {
        private static int _closedChannels;

        private static IChannel<StringMessage> Subscribe(ICommunication<StringMessage> communication, string clientName)
        {
            return Scenarios.PubSub.SetupSubscription(communication, new[] { "topic" })
                    .UsingMessageReceivedHandler((sender, a) => { Console.WriteLine($"{clientName} received message: {a.Message}"); })
                    .UsingChannelClosedHandler((sender, args) => { Interlocked.Increment(ref _closedChannels); })
                    .Go();
        }

        static void Main(string[] args)
        {
            var serializer = new StringMessageSerializer();

            var serverCommunication = new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000), serializer);
            using (var publisher = Scenarios.PubSub.Publisher(serverCommunication))
            {
                var clientCommunication = new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000), serializer);

                using (Subscribe(clientCommunication, "client1"))
                using (Subscribe(clientCommunication, "client2"))
                using (Subscribe(clientCommunication, "client3"))
                {
                    publisher.Broadcast("topic", new StringMessage("broadcast message 1"));
                    publisher.Broadcast("topic", new StringMessage("broadcast message 2"));
                    publisher.Broadcast("topic", new StringMessage("broadcast message 3"));

                    while (_closedChannels < 3)
                    {
                        Thread.Sleep(0);
                    }
                }
            }

            Console.ReadKey();
        }
    }
}