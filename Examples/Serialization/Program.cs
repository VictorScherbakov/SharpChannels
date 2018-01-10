using System;
using System.Net;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Serialization;
using SharpChannels.Core.Serialization.Protobuf;
using System.Linq;
using SharpChannels.Core;

namespace Examples.Serialization
{
    class Program
    {
        private static IMessageSerializer RequestSerializer()
        {
            ConsoleKeyInfo ch;
            var chars = new[] { '1', '2' };
            do
            {
                ch = Console.ReadKey();

            } while (!chars.Contains(ch.KeyChar));

            return ch.KeyChar == '1'
                ? new NativeSerializer<Message>()
                : (IMessageSerializer)new ProtobufSerializer<Message>();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press '1' to use native (BinaryFormatter) serializer");
            Console.WriteLine("Press '2' to use ProtoBuf serializer");

            var serializer = RequestSerializer();

            Console.WriteLine();

            var serverCommunication = new TcpCommunication<Message>(new TcpEndpointData(IPAddress.Any, 2000), serializer);

            var server = Scenarios.RequestResponse.SetupServer(serverCommunication)
                .UsingRequestHandler((sender, a) =>
                                            {
                                                a.Response = new Message
                                                {
                                                    StringField = "Response",
                                                    IntField = a.Request.IntField,
                                                    DateTimeField = DateTime.Now
                                                };
                                            })
                .Go();


            var clientCommunication = new TcpCommunication<Message>(new TcpEndpointData(IPAddress.Loopback, 2000), serializer);

            var r = Scenarios.RequestResponse.Requester(clientCommunication);

            using (r.Channel)
            {
                r.Channel.Open();

                for (int i = 0; i < 100; i++)
                {
                    var requestMessage = new Message
                    {
                        StringField = "Request",
                        IntField = i,
                        DateTimeField = DateTime.Now
                    }; 
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
