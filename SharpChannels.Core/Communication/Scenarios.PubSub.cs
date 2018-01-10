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
            public static IPublisher<TMessage> Publisher<TMessage>(ICommunication<TMessage> communication)
                where TMessage : IMessage
            {
                var awaiter = communication.CreateChannelAwaiter();
                var requestAcceptor = new NewChannelRequestAcceptor(awaiter);
                var publisher = new Publisher<TMessage>(requestAcceptor, true);
                requestAcceptor.StartAcceptLoop();

                return publisher;
            }

            public static SubscriptionSetup<TMessage> SetupSubscription<TMessage>(ICommunication<TMessage> communication, IEnumerable<string> topics)
                where TMessage : IMessage
            {
                Enforce.NotNull(communication, nameof(communication));

                return new SubscriptionSetup<TMessage>(communication, topics);
            }

            public class SubscriptionSetup<TMessage> where TMessage : IMessage
            {
                private readonly ICommunication<TMessage> _communication;
                private readonly IEnumerable<string> _topics;
                private EventHandler<MessageEventArgs> _messageReceivedHandler;
                private EventHandler<EventArgs> _channelClosedHandler;
                private EventHandler<ExceptionEventArgs> _receiveMessageFailedHandler;
                private bool _setupFinished;
                private readonly string _setupFinishedDescription = "'Using...' methods should be called before 'Go'";

                internal SubscriptionSetup(ICommunication<TMessage> communication, IEnumerable<string> topics)
                {
                    _communication = communication;
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

                    var channel = _communication.CreateChannel();
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
