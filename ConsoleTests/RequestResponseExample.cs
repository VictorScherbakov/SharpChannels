using System;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace ConsoleTests
{
    public class RequestResponseExample : ExampleBase
    {
        private static NewChannelRequestAcceptor _requestAcceptor;

        private void StartServer(Transport transport, IEndpointData endpoint, IMessageSerializer serializer)
        {
            var awaiter = CreateAwaiter(transport, endpoint, serializer);

            _requestAcceptor = new NewChannelRequestAcceptor(awaiter);
            int i = 0;
            _requestAcceptor.ClientAccepted += (sender, a) =>
            {
                i++;
                a.Channel.Name = "ch" + i.ToString();

                var responder = new Responder<StringMessage>((IChannel<StringMessage>)a.Channel);
                responder.RequestReceived += (o, args) =>
                {
                    args.Response = new StringMessage(args.Request.Message.Replace("request from", "response to"));
                };
                responder.ChannelClosed += (o, args) =>
                {
                    Console.WriteLine($"channel '{((Responder<StringMessage>)o).Channel.Name}' closed");
                };

                responder.StartResponding();
            };

            _requestAcceptor.StartAcceptLoop();
        }

        private static void StopServer()
        {
            _requestAcceptor.Stop();
        }

        public override void Do()
        {
            var serializer = new StringMessageSerializer();
            var serverEndpoint = CreateEndpoint(Transport);
            StartServer(Transport, serverEndpoint, serializer);

            var clientEndpoint = CreateEndpoint(Transport);

            using (var channel1 = CreateChannel(Transport, clientEndpoint, serializer))
            using (var channel2 = CreateChannel(Transport, clientEndpoint, serializer))
            {
                channel1.Open();
                channel2.Open();

                var requester1 = new Requester<StringMessage>(channel1);
                var requester2 = new Requester<StringMessage>(channel2);

                for (int i = 0; i < 100; i++)
                {
                    var responseMessage = requester1.Request(new StringMessage("request from client 1"));
                    Console.WriteLine(responseMessage);
                    responseMessage = requester2.Request(new StringMessage("request from client 2"));
                    Console.WriteLine(responseMessage);
                }

                channel1.Close();
                channel2.Close();
            }

            StopServer();

            Console.ReadKey();
        }

        public RequestResponseExample(Transport transport)
        {
            Transport = transport;
        }
    }
}