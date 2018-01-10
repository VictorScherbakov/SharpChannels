using System;
using System.Net;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Communication
{
    internal class BusClient<TMessage> : IBusClient<TMessage> where TMessage : IMessage
    {
        private readonly IChannel<TMessage> _subscribersChannel;
        private readonly IChannel<TMessage> _publishingChannel;
        private readonly Lazy<StringMessageSerializer> _stringMessageSerializer =
            new Lazy<StringMessageSerializer>(() => new StringMessageSerializer());

        public void Close()
        {
            if(_publishingChannel.IsOpened)
                _publishingChannel.Close();

            if(_subscribersChannel.IsOpened)
                _subscribersChannel.Close();
        }

        internal BusClient(IChannel<TMessage> subscribersChannel, IChannel<TMessage> publishingChannel)
        {
            Enforce.NotNull(subscribersChannel, nameof(subscribersChannel));
            Enforce.NotNull(publishingChannel, nameof(publishingChannel));

            _subscribersChannel = subscribersChannel;
            _publishingChannel = publishingChannel;
        }

        public void Publish(string topic, TMessage message)
        {
            Enforce.NotNull(topic, nameof(topic));
            Enforce.NotNull(message, nameof(message));

            _publishingChannel.Send(new StringMessage(topic), _stringMessageSerializer.Value);
            _publishingChannel.Send(message);
            var confirm = _publishingChannel.Receive(_stringMessageSerializer.Value).ToString();
            if (confirm != BusServer<TMessage>.PublicationConfirmString)
            {
                throw new ProtocolViolationException($"Expected '{BusServer<TMessage>.PublicationConfirmString}' response for publication, but received {confirm}");
            }
        }
    }
}