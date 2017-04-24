using System;
using System.Linq;
using System.Net;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Intradomain;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace Examples.Transport
{
    class Program
    {
        private static IChannelAwaiter<IChannel<StringMessage>> GetChannelAwaiter(Transport transport, IMessageSerializer serializer)
        {
            switch (transport)
            {
                case Transport.Tcp:
                    var epd = new TcpEndpointData(IPAddress.Any, 2000);
                    return new TcpChannelAwaiter<StringMessage>(epd, serializer);
                case Transport.Intradomain:
                    var ep = new IntradomainEndpoint("test");
                    return new IntradomainChannelAwaiter<StringMessage>(ep, serializer);
                default:
                    throw new NotSupportedException();
            }
        }

        private static IChannel<StringMessage> GetChannel(Transport transport, IMessageSerializer serializer)
        {
            switch (transport)
            {
                case Transport.Tcp:
                    var epd = new TcpEndpointData(IPAddress.Loopback, 2000);
                    return new TcpChannel<StringMessage>(epd, serializer);
                case Transport.Intradomain:
                    var ep = new IntradomainEndpoint("test");
                    return new IntradomainChannel<StringMessage>(ep, serializer);
                default:
                    throw new NotSupportedException();
            }
        }

        private static void StartServer(Transport transport, IMessageSerializer serializer)
        {

            var awaiter = GetChannelAwaiter(transport, serializer);

            var requestAcceptor = new NewChannelRequestAcceptor(awaiter);
            requestAcceptor.ClientAccepted += (sender, a) =>
            {
                Console.WriteLine("channel opened");

                var responder = new Responder<StringMessage>((IChannel<StringMessage>)a.Channel);
                responder.RequestReceived += (o, args) =>
                {
                    args.Response = new StringMessage(args.Request.Message.Replace("request", "response"));
                };

                responder.ChannelClosed += (o, args) =>
                {
                    Console.WriteLine("channel closed");
                    ((Responder<StringMessage>)o).Channel.Dispose();
                };

                responder.StartResponding();
            };

            requestAcceptor.StartAcceptLoop();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press '1' for TCP transport");
            Console.WriteLine("Press '2' for Intradomain transport");

            ConsoleKeyInfo ch;
            var chars = new[] { '1', '2' };
            do
            {
                ch = Console.ReadKey();

            } while (!chars.Contains(ch.KeyChar));

            var transport = ch.KeyChar == '1' ? Transport.Tcp : Transport.Intradomain;
            Console.WriteLine();

            var serializer = new StringMessageSerializer();
            StartServer(transport, serializer);

            using (var channel = GetChannel(transport, serializer))
            {
                channel.Open();

                var requester = new Requester<StringMessage>(channel);

                var requestMessage = new StringMessage($"request using {transport} transport");
                Console.WriteLine(requestMessage);

                var responseMessage = requester.Request(requestMessage); 
                Console.WriteLine(responseMessage);

                channel.Close();
            }

            Console.ReadKey();
        }
    }
}
