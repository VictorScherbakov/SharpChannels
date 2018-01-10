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

            var communication = new IntradomainCommunication<StringMessage>(new IntradomainEndpoint("pubsubpayload"),
                                                                            new StringMessageSerializer(),
                                                                            ChannelSettings.GetDefault(),
                                                                            connectionSettings,
                                                                            null);

            return Scenarios.PubSub.Publisher(communication);
        }

        private IPublisher<StringMessage> StartTcpPublisher()
        {
            var connectionSettings = new TcpConnectionSettingsBuilder()
                .UsingSendTimeout(TimeSpan.FromHours(1))
                .Build();

            var communication = new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000),
                                                                    new StringMessageSerializer(),
                                                                    ChannelSettings.GetDefault(), 
                                                                    connectionSettings,
                                                                    null);

            return Scenarios.PubSub.Publisher(communication);
        }

        private ICommunication<StringMessage> GetTcpSubscriberCommunication()
        {
            var connectionSettings = new TcpConnectionSettingsBuilder()
                .UsingReceiveTimeout(TimeSpan.FromHours(1))
                .Build();

            return new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000),
                                                                     new StringMessageSerializer(),
                                                                     ChannelSettings.GetDefault(),
                                                                     connectionSettings,
                                                                     null);
        }

        private ICommunication<StringMessage> GetIntradomainSubscriberCommunication()
        {
            var connectionSettings = new IntradomainConnectionSettingsBuilder()
                .UsingReceiveTimeout(TimeSpan.FromHours(1))
                .Build();

            return new IntradomainCommunication<StringMessage>(new IntradomainEndpoint("pubsubpayload"),
                                                                             new StringMessageSerializer(),
                                                                             ChannelSettings.GetDefault(),
                                                                             connectionSettings,
                                                                             null);
        }

        private void Subscribe(Transport transport)
        {
            var communication = transport == Transport.Tcp
                ? GetTcpSubscriberCommunication()
                : GetIntradomainSubscriberCommunication();

            Scenarios.PubSub.SetupSubscription(communication, new []{"topic"})
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
            publisher.Broadcast("topic", message);
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

                while (true)
                {
                    log(GetLogString(subscriberNumber, messagesBroadcasted));
                    if(task.Wait(40)) break;
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