using System;
using System.Net;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace Examples.RequestResponse
{
    class Program
    {
        private static NewChannelRequestAcceptor _requestAcceptor;

        public static void StartServer()
        {
            var awaiter = new TcpChannelAwaiter<StringMessage>( // await tcp connections 
                    new TcpEndpointData(IPAddress.Any, 2000),   // at port 2000
                    new StringMessageSerializer());             // using StringMessageSerializer for new channels

            // setup the request acceptor
            _requestAcceptor = new NewChannelRequestAcceptor(awaiter);
            _requestAcceptor.ClientAccepted += (sender, a) =>
            {
                Console.WriteLine("channel opened");

                var responder = new Responder<StringMessage>((IChannel<StringMessage>)a.Channel);
                responder.RequestReceived += (o, args) =>
                {
                    // form the response message
                    args.Response = new StringMessage(args.Request.Message.Replace("request", "response"));
                };

                responder.ChannelClosed += (o, args) =>
                {
                    // handle channel closing 
                    Console.WriteLine("channel closed");
                    ((Responder<StringMessage>)o).Channel.Dispose();
                };

                // start response loop for this channel
                responder.StartResponding();
            };

            // here the server actually becomes available
            _requestAcceptor.StartAcceptLoop();
        }
        private static void StopServer()
        {
            _requestAcceptor.Stop();
        }

        static void Main(string[] args)
        {
            StartServer();

            using (var channel = new TcpChannel<StringMessage>(                     // create a new channel
                                     new TcpEndpointData(IPAddress.Loopback, 2000), // with localhost at port 2000
                                     new StringMessageSerializer()))                // using StringMessageSerializer
            {
                channel.Open();

                // setup the requester
                var requester = new Requester<StringMessage>(channel);

                for (int i = 0; i < 100; i++)
                {
                    var requestMessage = new StringMessage($"request #{i}"); // prepare the request message
                    Console.WriteLine(requestMessage);

                    var responseMessage = requester.Request(requestMessage); // and send it
                    Console.WriteLine(responseMessage);
                }

                channel.Close();
            }

            StopServer();

            Console.ReadKey();
        }
    }
}
