using System;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace ConsoleTests
{
    public class SimpleSendReceiveExample : ExampleBase
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

                var messageReceiver = new Receiver(a.Channel);
                messageReceiver.MessageReceived += (receiver, ar) => { Console.WriteLine(ar.Message); };
                messageReceiver.ChannelClosed += (receiver, ar) =>
                {
                    var channel = ((Receiver)receiver).Channel;
                    Console.WriteLine($"Channel '{channel.Name}' closed!");
                    channel.Dispose();
                };
                messageReceiver.ReceiveMessageFailed += (s, ar) => { Console.WriteLine($"Error receiving message:  '{ar.Exception}'"); };

                messageReceiver.StartReceiving();
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
            var clientEndpoint = CreateEndpoint(Transport);

            StartServer(Transport, serverEndpoint, serializer);

            using (var channel1 = CreateChannel(Transport, clientEndpoint, serializer))
            using (var channel2 = CreateChannel(Transport, clientEndpoint, serializer))
            {
                channel1.Open();
                channel2.Open();

                channel1.Send(new StringMessage("message from client 1"));
                channel1.Send(new StringMessage("message from client 1"));
                channel1.Close();
                
                channel2.Send(new StringMessage("message from client 2"));
                channel2.Send(new StringMessage("message from client 2"));
                channel2.Close();
            }

            StopServer();

            Console.ReadKey();
        }

        public SimpleSendReceiveExample(Transport transport)
        {
            Transport = transport;
        }
    }
}