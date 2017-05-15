using SharpChannels.Core.Messages;
using SharpChannels.Core.Messages.PubSub;
using SharpChannels.Core.Serialization;
using SharpChannels.Core.Serialization.PubSub;

namespace SharpChannels.Core.Channels
{
    public class PubSubChannel : IChannel
    {
        private readonly IChannel _wrappingChannel;
        private readonly CompoundSerializer _serializer;

        public IEndpointData EndpointData => _wrappingChannel.EndpointData;
        public IMessageSerializer Serializer => _wrappingChannel.Serializer;

        public void Open()
        {
            _wrappingChannel.Open();
        }

        public void Close()
        {
            _wrappingChannel.Close();
        }

        public bool IsOpened => _wrappingChannel.IsOpened;

        public string Name
        {
            get { return _wrappingChannel.Name; }
            set { _wrappingChannel.Name = value; }
        }

        public void Dispose()
        {
            _wrappingChannel.Dispose();
        }

        public void Send(IMessage message)
        {
            _wrappingChannel.Send(message, _serializer);
        }

        public IMessage Receive()
        {
            return _wrappingChannel.Receive(_serializer);
        }

        public void Send(IMessage message, IMessageSerializer serializer)
        {
            _wrappingChannel.Send(message, serializer);
        }

        public IMessage Receive(IMessageSerializer serializer)
        {
            return _wrappingChannel.Receive(serializer);
        }

        public PubSubChannel(IChannel wrappingChannel)
        {
            _wrappingChannel = wrappingChannel;

            _serializer = new CompoundSerializer(new []
            {
                new CompoundSerializer.SerializerInfo
                {
                    Code = 1,
                    Serializer = new SubscribeMessageSerializer(),
                    Type = typeof(SubscribeMessage)
                },
                new CompoundSerializer.SerializerInfo
                {
                    Code = 2,
                    Serializer = _wrappingChannel.Serializer,
                    Type = typeof(IMessage)
                },
            });
        }
    }
}