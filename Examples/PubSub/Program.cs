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

        private static IChannel<StringMessage> Subscribe(ICommunicationObjectsFactory<StringMessage> factory, string clientName)
        {
            return Scenarios.PubSub.SetupSubscription(factory, new[] { "topic" })
                    .UsingMessageReceivedHandler((sender, a) => { Console.WriteLine($"{clientName} received message: {a.Message}"); })
                    .UsingChannelClosedHandler((sender, args) => { Interlocked.Increment(ref _closedChannels); })
                    .Go();
        }

        static void Main(string[] args)
        {
            var serializer = new StringMessageSerializer();

            var serverFactory = new TcpCommunicationObjectsFactory<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000), serializer);
            using (var publisher = Scenarios.PubSub.Publisher(serverFactory))
            {
                var clientFactory = new TcpCommunicationObjectsFactory<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000), serializer);

                using (Subscribe(clientFactory, "client1"))
                using (Subscribe(clientFactory, "client2"))
                using (Subscribe(clientFactory, "client3"))
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