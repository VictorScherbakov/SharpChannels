using System;
using System.Net;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Serialization;
using SharpChannels.Core.Serialization.Protobuf;
using System.Linq;

namespace Examples.Serialization
{
    class Program
    {
        private static NewChannelRequestAcceptor _requestAcceptor;

        public static void StartServer(IMessageSerializer serializer)
        {
            var awaiter = new TcpChannelAwaiter<Message>(new TcpEndpointData(IPAddress.Any, 2000), serializer);

            _requestAcceptor = new NewChannelRequestAcceptor(awaiter);
            _requestAcceptor.ClientAccepted += (sender, a) =>
            {
                Console.WriteLine("channel opened");

                var responder = new Responder<Message>((IChannel<Message>)a.Channel);
                responder.RequestReceived += (o, args) =>
                {
                    args.Response = new Message
                    {
                        StringField = "Response",
                        IntField = args.Request.IntField,
                        DateTimeField = DateTime.Now
                    };
                };

                responder.ChannelClosed += (o, args) =>
                {
                    Console.WriteLine("channel closed");
                    ((Responder<Message>)o).Channel.Dispose();
                };

                responder.StartResponding();
            };

            _requestAcceptor.StartAcceptLoop();
        }
        private static void StopServer()
        {
            _requestAcceptor.Stop();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press '1' to use native (BinaryFormatter) serializer");
            Console.WriteLine("Press '2' to use ProtoBuf serializer");

            ConsoleKeyInfo ch;
            var chars = new[] { '1', '2' };
            do
            {
                ch = Console.ReadKey();

            } while (!chars.Contains(ch.KeyChar));

            var serializer = ch.KeyChar == '1' 
                ? new NativeSerializer<Message>() 
                : (IMessageSerializer)new ProtobufSerializer<Message>();

            Console.WriteLine();

            StartServer(serializer);

            using (var channel = new TcpChannel<Message>(new TcpEndpointData(IPAddress.Loopback, 2000), serializer))
            {
                channel.Open();

                var requester = new Requester<Message>(channel);

                for (int i = 0; i < 100; i++)
                {
                    var requestMessage = new Message
                    { StringField = "Request",
                        IntField = i,
                        DateTimeField = DateTime.Now
                    }; 
                    Console.WriteLine(requestMessage);

                    var responseMessage = requester.Request(requestMessage); 
                    Console.WriteLine(responseMessage);
                }

                channel.Close();
            }

            StopServer();

            Console.ReadKey();
        }
    }
}
