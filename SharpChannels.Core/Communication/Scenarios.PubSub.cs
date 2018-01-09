using System;
using System.Collections.Generic;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Messages.PubSub;
using SharpChannels.Core.Serialization.PubSub;

namespace SharpChannels.Core.Communication
{
    public static partial class Scenarios
    {
        public static class PubSub
        {
            public static IPublisher<TMessage> Publisher<TMessage>(ICommunicationObjectsFactory<TMessage> factory)
                where TMessage : IMessage
            {
                var awaiter = factory.CreateChannelAwaiter();
                var requestAcceptor = new NewChannelRequestAcceptor(awaiter);
                var publisher = new Publisher<TMessage>(requestAcceptor, true);
                requestAcceptor.StartAcceptLoop();

                return publisher;
            }

            public static SubscriptionSetup<TMessage> SetupSubscription<TMessage>(ICommunicationObjectsFactory<TMessage> factory, IEnumerable<string> topics)
                where TMessage : IMessage
            {
                Enforce.NotNull(factory, nameof(factory));

                return new SubscriptionSetup<TMessage>(factory, topics);
            }

            public class SubscriptionSetup<TMessage>
                where TMessage : IMessage
            {
                private readonly ICommunicationObjectsFactory<TMessage> _factory;
                private readonly IEnumerable<string> _topics;
                private EventHandler<MessageEventArgs> _messageReceivedHandler;
                private EventHandler<EventArgs> _channelClosedHandler;
                private EventHandler<ExceptionEventArgs> _receiveMessageFailedHandler;
                private bool _setupFinished;
                private readonly string _setupFinishedDescription = "'Using...' methods should be called before 'Go'";

                internal SubscriptionSetup(ICommunicationObjectsFactory<TMessage> factory, IEnumerable<string> topics)
                {
                    _factory = factory;
                    _topics = topics;
                }

                public SubscriptionSetup<TMessage> UsingMessageReceivedHandler(EventHandler<MessageEventArgs> messageReceivedHandler)
                {
                    Enforce.State.FitsTo(!_setupFinished, _setupFinishedDescription);

                    _messageReceivedHandler = messageReceivedHandler;
                    return this;
                }

                public SubscriptionSetup<TMessage> UsingChannelClosedHandler(EventHandler<EventArgs> channelClosedHandler)
                {
                    Enforce.State.FitsTo(!_setupFinished, _setupFinishedDescription);

                    _channelClosedHandler = channelClosedHandler;
                    return this;
                }

                public SubscriptionSetup<TMessage> UsingReceiveMessageFailedHandler(EventHandler<ExceptionEventArgs> receiveMessageFailedHandler)
                {
                    Enforce.State.FitsTo(!_setupFinished, _setupFinishedDescription);

                    _receiveMessageFailedHandler = receiveMessageFailedHandler;
                    return this;
                }

                public IChannel<TMessage> Go()
                {
                    _setupFinished = true;

                    var channel = _factory.CreateChannel();
                    var receiver = new Receiver(channel);

                    receiver.MessageReceived += (sender, eventArgs) =>
                    {
                        _messageReceivedHandler?.Invoke(sender, eventArgs);
                    };
                    receiver.ChannelClosed += (sender, args) =>
                    {
                        _channelClosedHandler?.Invoke(sender, args);
                        receiver.Channel.Dispose();
                    };

                    receiver.ReceiveMessageFailed += (sender, args) =>
                    {
                        _receiveMessageFailedHandler?.Invoke(sender, args);
                    };

                    channel.Open();
                    receiver.StartReceiving();

                    channel.Send(new SubscribeMessage(_topics), new SubscribeMessageSerializer());

                    return channel;
                }
            }
        }
    }
}
