﻿using System;
using System.Net;
using SharpChannels.Core;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace Examples.Bus
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializer = new StringMessageSerializer();

            var serverSubscribersCommunication = new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000), serializer);
            var serverPublishersCommunication = new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Any, 2001), serializer);

            var clientSubscribersCommunication = new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000), serializer);
            var clientPublishersCommunication = new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2001), serializer);

            var server = Scenarios.Bus.RunServer(serverSubscribersCommunication, serverPublishersCommunication);

            var client1 = Scenarios.Bus.SetupClient(clientSubscribersCommunication, clientPublishersCommunication)
                .UsingTopics(new[] { "first topic" })
                .UsingMessageReceivedHandler((s, a) => { Console.WriteLine($"Client 1 received: {a.Message}"); })
                .Go();

            var client2 = Scenarios.Bus.SetupClient(clientSubscribersCommunication, clientPublishersCommunication)
                .UsingTopics(new[] { "second topic" })
                .UsingMessageReceivedHandler((s, a) => { Console.WriteLine($"Client 2 received: {a.Message}"); })
                .Go();

            var client3 = Scenarios.Bus.SetupClient(clientSubscribersCommunication, clientPublishersCommunication)
                .UsingTopics(new[] { "second topic" })
                .UsingMessageReceivedHandler((s, a) => { Console.WriteLine($"Client 3 received: {a.Message}"); })
                .Go();

            client1.Publish("second topic", new StringMessage("message for second topic"));
            client2.Publish("first topic", new StringMessage("message for first topic"));

            Console.ReadKey();

            client1.Close();
            client2.Close();
            client3.Close();
        }
    }
}
