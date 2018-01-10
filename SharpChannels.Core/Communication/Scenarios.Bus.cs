using System;
using System.Collections.Generic;
using System.Linq;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Messages.PubSub;
using SharpChannels.Core.Serialization.PubSub;

namespace SharpChannels.Core.Communication
{
    public static partial class Scenarios
    {
        public static class Bus
        {
            public static ClientSetup<TMessage> SetupClient<TMessage>(ICommunicationObjectsFactory<TMessage> subscribersFactory,
                                                                      ICommunicationObjectsFactory<TMessage> publishersFactory)
                where TMessage : IMessage
            {
                Enforce.NotNull(subscribersFactory, nameof(subscribersFactory));
                Enforce.NotNull(publishersFactory, nameof(publishersFactory));

                return new ClientSetup<TMessage>(subscribersFactory, publishersFactory);
            }

            public class ClientSetup<TMessage> where TMessage : IMessage
            {
                private bool _setupFinished;
                private readonly ICommunicationObjectsFactory<TMessage> _subscribersFactory;
                private readonly ICommunicationObjectsFactory<TMessage> _publishersFactory;
                private IEnumerable<string> _topics;
                private readonly string _setupFinishedDescription = "'Using...' methods should be called before 'Go'";
                private EventHandler<MessageEventArgs> _messageReceivedHandler;
                private EventHandler<EventArgs> _channelClosedHandler;
                private EventHandler<ExceptionEventArgs> _receiveMessageFailedHandler;

                internal ClientSetup(ICommunicationObjectsFactory<TMessage> subscribersFactory,
                                     ICommunicationObjectsFactory<TMessage> publishersFactory)
                {
                    _subscribersFactory = subscribersFactory;
                    _publishersFactory = publishersFactory;
                }

                public ClientSetup<TMessage> UsingTopics(IEnumerable<string> topics)
                {
                    Enforce.State.FitsTo(!_setupFinished, _setupFinishedDescription);

                    _topics = topics;
                    return this;
                }

                public ClientSetup<TMessage> UsingMessageReceivedHandler(EventHandler<MessageEventArgs> messageReceivedHandler)
                {
                    Enforce.State.FitsTo(!_setupFinished, _setupFinishedDescription);

                    _messageReceivedHandler = messageReceivedHandler;
                    return this;
                }

                public ClientSetup<TMessage> UsingChannelClosedHandler(EventHandler<EventArgs> channelClosedHandler)
                {
                    Enforce.State.FitsTo(!_setupFinished, _setupFinishedDescription);

                    _channelClosedHandler = channelClosedHandler;
                    return this;
                }

                public ClientSetup<TMessage> UsingReceiveMessageFailedHandler(EventHandler<ExceptionEventArgs> receiveMessageFailedHandler)
                {
                    Enforce.State.FitsTo(!_setupFinished, _setupFinishedDescription);

                    _receiveMessageFailedHandler = receiveMessageFailedHandler;
                    return this;
                }

                public IBusClient<TMessage> Go()
                {
                    _setupFinished = true;

                    var subscribersChannel = _subscribersFactory.CreateChannel();
                    var receiver = new Receiver(subscribersChannel);

                    receiver.MessageReceived += (sender, eventArgs) =>
                    {
                        _messageReceivedHandler?.Invoke(sender, eventArgs);
                    };
                    receiver.ChannelClosed += (sender, args) =>
                    {
                        _channelClosedHandler?.Invoke(sender, args);
                    };

                    receiver.ReceiveMessageFailed += (sender, args) =>
                    {
                        _receiveMessageFailedHandler?.Invoke(sender, args);
                    };

                    subscribersChannel.Open();
                    receiver.StartReceiving();

                    if(_topics.Any())
                        subscribersChannel.Send(new SubscribeMessage(_topics), new SubscribeMessageSerializer());

                    var publishersChannel = _publishersFactory.CreateChannel();
                    publishersChannel.Open();

                    return new BusClient<TMessage>(subscribersChannel, publishersChannel);
                }
            }

            public static IBusServer<TMessage> Server<TMessage>(ICommunicationObjectsFactory<TMessage> subscribersFactory,
                                                                ICommunicationObjectsFactory<TMessage> publishersFactory)
                where TMessage : IMessage
            {
                Enforce.NotNull(subscribersFactory, nameof(subscribersFactory));
                Enforce.NotNull(publishersFactory, nameof(publishersFactory));

                return new BusServer<TMessage>(subscribersFactory, publishersFactory);
            }
        }
    }
}
