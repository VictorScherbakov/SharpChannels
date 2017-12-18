using System;
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

            ICommunicationObjectsFactory<StringMessage> serverFactory;
            if (transport == Transport.Tcp)
            {
                serverFactory = new TcpCommunicationObjectsFactory<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000), serializer);
            }
            else
            {
                serverFactory = new IntradomainCommunicationObjectsFactory<StringMessage>(new IntradomainEndpoint("test"), serializer);
            }


            var server = Scenarios.RequestResponse.SetupServer(serverFactory)
                .UsingNewClientHandler((sender, a) => { Console.WriteLine("channel opened"); })
                .UsingRequestHandler((sender, a) => { a.Response = new StringMessage(a.Request.Message.Replace("request", "response")); })
                .UsingChannelClosedHandler((sender, a) => { Console.WriteLine("channel closed"); })
                .Go();

            ICommunicationObjectsFactory<StringMessage> clientFactory;
            if (transport == Transport.Tcp)
            {
                clientFactory = new TcpCommunicationObjectsFactory<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000), serializer);
            }
            else
            {
                clientFactory = new IntradomainCommunicationObjectsFactory<StringMessage>(new IntradomainEndpoint("test"), serializer);
            }

            var r = Scenarios.RequestResponse.Requester(clientFactory);

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
