using System;
using System.Net;
using SharpChannels.Core;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace Examples.RequestResponse
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializer = new StringMessageSerializer();

            var communication = new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000), serializer);

            var server = Scenarios.RequestResponse.SetupServer(communication)
                .UsingNewClientHandler((sender, a) => { Console.WriteLine("channel opened"); })
                .UsingRequestHandler((sender, a) => { a.Response = new StringMessage(a.Request.Message.Replace("request", "response")); })
                .UsingChannelClosedHandler((sender, a) => { Console.WriteLine("channel closed"); })
                .Go();

            var clientCommunication = new TcpCommunication<StringMessage>(
                                        new TcpEndpointData(IPAddress.Loopback, 2000), 
                                        serializer);

            var r = Scenarios.RequestResponse.Requester(clientCommunication);

            using (r.Channel)
            {
                r.Channel.Open();

                for (int i = 0; i < 100; i++)
                {
                    var requestMessage = new StringMessage($"request #{i}");
                    Console.WriteLine(requestMessage);

                    var responseMessage = r.Request(requestMessage);
                    Console.WriteLine(responseMessage);
                }

                r.Channel.Close();
            }

            server.Stop();

            Console.ReadKey();
        }
    }
}
