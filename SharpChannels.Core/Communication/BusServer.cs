using System;
using System.IO;
using System.Threading.Tasks;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Communication
{
    internal class BusServer<TMessage> : IBusServer<TMessage> where TMessage : IMessage
    {
        public static string PublicationConfirmString => "OK";

        private readonly Publisher<TMessage> _publisher;
        private readonly Lazy<StringMessageSerializer> _stringMessageSerializer = new Lazy<StringMessageSerializer>(() => new StringMessageSerializer());

        public BusServer(ICommunication<TMessage> subscribersCommunication,
                         ICommunication<TMessage> publishersCommunication)
        {
            var subscriptionChannelAwaiter = subscribersCommunication.CreateChannelAwaiter();
            var subscriptionRequestAcceptor = new NewChannelRequestAcceptor(subscriptionChannelAwaiter);

            _publisher = new Publisher<TMessage>(subscriptionRequestAcceptor, true);
            subscriptionRequestAcceptor.StartAcceptLoop();

            var publisherChannelAwaiter = publishersCommunication.CreateChannelAwaiter();
            var publicationRequestAcceptor = new NewChannelRequestAcceptor(publisherChannelAwaiter);
            publicationRequestAcceptor.ClientAccepted += PublisherChannelAccepted;

            publicationRequestAcceptor.StartAcceptLoop();
        }

        public int SubscriprionNumber => _publisher.SubscriprionNumber;

        public event EventHandler<ExceptionEventArgs> CommunicationFailed;
        public event EventHandler<ClientAcceptedEventArgs> ClientConnected;

        private void PublisherChannelAccepted(object sender, ClientAcceptedEventArgs args)
        {
            ClientConnected?.Invoke(this, args);

            var channel = args.Channel;
            Task.Factory.StartNew(() =>
            {
                while (args.Channel.IsOpened)
                {
                    try
                    {
                        var topicMessage = channel.Receive(_stringMessageSerializer.Value);
                        if (topicMessage == null)
                            return;

                        var contentMessage = channel.Receive();
                        if (contentMessage == null)
                            return;

                        channel.Send(new StringMessage(PublicationConfirmString), _stringMessageSerializer.Value);

                        _publisher.Broadcast(topicMessage.ToString(), contentMessage);
                    }
                    catch (Exception ex) when(ex is DataTransferException || ex is IOException)
                    {
                        CommunicationFailed?.Invoke(this, new ExceptionEventArgs(ex));

                        if (channel.IsOpened)
                            channel.Close();

                        return;
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}