using System;
using System.Net;
using SharpChannels.Core;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace Examples.PubSub
{
    class Program
    {
        private static void Subscribe(ICommunicationObjectsFactory<StringMessage> factory, string clientName)
        {
            Scenarios.PubSub.SetupSubscription(factory)
                    .UsingMessageReceivedHandler((sender, a) => { Console.WriteLine($"{clientName} received message: {a.Message}"); })
                    .Go();
        }

        static void Main(string[] args)
        {
            var serializer = new StringMessageSerializer();

            var serverFactory = new TcpCommunicationObjectsFactory<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000), serializer);
            using (var publisher = Scenarios.PubSub.Publisher(serverFactory))
            {
                var clientFactory = new TcpCommunicationObjectsFactory<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000), serializer);
                Subscribe(clientFactory, "client1");
                Subscribe(clientFactory, "client2");
                Subscribe(clientFactory, "client3");

                publisher.Broadcast(new StringMessage("broadcast message 1"));
                publisher.Broadcast(new StringMessage("broadcast message 2"));
                publisher.Broadcast(new StringMessage("broadcast message 3"));
            }

            Console.ReadKey();
        }
    }
}
