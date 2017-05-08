using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SharpChannels.Core;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Intradomain;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace Examples.Payload
{
    public class PubSubPayload
    {
        private int _closedChannels;
        private int _messagesReceived;

        private readonly AutoResetEvent _channelClosed = new AutoResetEvent(false);

        private IPublisher<StringMessage> StartIntradomainPublisher()
        {
            var connectionSettings = new IntradomainConnectionSettingsBuilder()
                .UsingSendTimeout(TimeSpan.FromHours(1))
                .Build();

            var factory = new IntradomainCommunicationObjectsFactory<StringMessage>(new IntradomainEndpoint("pubsubpayload"),
                                                                                    new StringMessageSerializer(),
                                                                                    ChannelSettings.GetDefault(),
                                                                                    connectionSettings);

            return Scenarios.PubSub.Publisher(factory);
        }

        private IPublisher<StringMessage> StartTcpPublisher()
        {
            var connectionSettings = new TcpConnectionSettingsBuilder()
                .UsingSendTimeout(TimeSpan.FromHours(1))
                .Build();

            var factory = new TcpCommunicationObjectsFactory<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000),
                                                                            new StringMessageSerializer(),
                                                                            ChannelSettings.GetDefault(), 
                                                                            connectionSettings);

            return Scenarios.PubSub.Publisher(factory);
        }

        private ICommunicationObjectsFactory<StringMessage> GetTcpSubscriberFactory()
        {
            var connectionSettings = new TcpConnectionSettingsBuilder()
                .UsingReceiveTimeout(TimeSpan.FromHours(1))
                .Build();

            return new TcpCommunicationObjectsFactory<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000),
                                                                     new StringMessageSerializer(),
                                                                     ChannelSettings.GetDefault(),
                                                                     connectionSettings);
        }

        private ICommunicationObjectsFactory<StringMessage> GetIntradomainSubscriberFactory()
        {
            var connectionSettings = new IntradomainConnectionSettingsBuilder()
                .UsingReceiveTimeout(TimeSpan.FromHours(1))
                .Build();

            return new IntradomainCommunicationObjectsFactory<StringMessage>(new IntradomainEndpoint("pubsubpayload"),
                                                                             new StringMessageSerializer(),
                                                                             ChannelSettings.GetDefault(),
                                                                             connectionSettings);
        }

        private void Subscribe(Transport transport)
        {
            var factory = transport == Transport.Tcp
                ? GetTcpSubscriberFactory()
                : GetIntradomainSubscriberFactory();

            Scenarios.PubSub.SetupSubscription(factory)
                .UsingMessageReceivedHandler((sender, a) =>
                {
                    Interlocked.Increment(ref _messagesReceived);
                })
                .UsingChannelClosedHandler((sender, a) =>
                {
                    Interlocked.Increment(ref _closedChannels);
                    _channelClosed.Set();

                })
                .Go();
        }

        private void Broadcast(IPublisher<StringMessage> publisher, StringMessage message)
        {
            publisher.Broadcast(message);
        }

        private string GetLogString(int subscriberNumber, int messagesBroadcasted)
        {
            return $"Subscribers: {subscriberNumber - _closedChannels} messages broadcasted: {messagesBroadcasted} messages received: {_messagesReceived}";
        }

        public void Run(Transport transport, int subscriberNumber, int messageNumber, Action<string> log)
        {
            _closedChannels = 0;
            _messagesReceived = 0;

            var publisher = transport == Transport.Tcp
                ? StartTcpPublisher()
                : StartIntradomainPublisher();

            int messagesBroadcasted = 0;

            using (publisher)
            {
                for (int i = 0; i < subscriberNumber; i++)
                {
                    Subscribe(transport);
                }

                var task = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < messageNumber; j++, messagesBroadcasted++)
                    {
                        Broadcast(publisher, new StringMessage(j.ToString()));
                    }
                }, TaskCreationOptions.LongRunning);

                while (!task.IsCompleted)
                {
                    log(GetLogString(subscriberNumber, messagesBroadcasted));
                    Thread.Sleep(40);
                }
            }

            // wait until all subscribers finish their receiving loops
            while (_closedChannels != subscriberNumber)
            {
                _channelClosed.WaitOne();
            }

            log(GetLogString(subscriberNumber, messagesBroadcasted));
        }
    }
}