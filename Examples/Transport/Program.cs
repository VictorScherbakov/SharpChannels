﻿using System;
using System.Linq;
using System.Net;
using SharpChannels.Core;
using SharpChannels.Core.Channels.Intradomain;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace Examples.Transport
{
    class Program
    {
        private static Transport RequestTransport()
        {
            ConsoleKeyInfo ch;
            var chars = new[] { '1', '2' };
            do
            {
                ch = Console.ReadKey();

            } while (!chars.Contains(ch.KeyChar));

            return ch.KeyChar == '1' ? Transport.Tcp : Transport.Intradomain;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press '1' for TCP transport");
            Console.WriteLine("Press '2' for Intradomain transport");

            var transport = RequestTransport();
            Console.WriteLine();

            var serializer = new StringMessageSerializer();

            ICommunication<StringMessage> serverCommunication;
            if (transport == Transport.Tcp)
            {
                serverCommunication = new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000), serializer);
            }
            else
            {
                serverCommunication = new IntradomainCommunication<StringMessage>(new IntradomainEndpoint("test"), serializer);
            }


            var server = Scenarios.RequestResponse.SetupServer(serverCommunication)
                .UsingNewClientHandler((sender, a) => { Console.WriteLine("channel opened"); })
                .UsingRequestHandler((sender, a) => { a.Response = new StringMessage(a.Request.Message.Replace("request", "response")); })
                .UsingChannelClosedHandler((sender, a) => { Console.WriteLine("channel closed"); })
                .Go();

            ICommunication<StringMessage> clientCommunication;
            if (transport == Transport.Tcp)
            {
                clientCommunication = new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000), serializer);
            }
            else
            {
                clientCommunication = new IntradomainCommunication<StringMessage>(new IntradomainEndpoint("test"), serializer);
            }

            var r = Scenarios.RequestResponse.Requester(clientCommunication);

            using (r.Channel)
            {
                r.Channel.Open();

                var requester = new Requester(r.Channel);

                var requestMessage = new StringMessage($"request using {transport} transport");
                Console.WriteLine(requestMessage);

                var responseMessage = requester.Request(requestMessage); 
                Console.WriteLine(responseMessage);

                r.Channel.Close();
            }

            server.Stop();

            Console.ReadKey();
        }
    }
}
