using System;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public static partial class Scenarios
    {
        public static class RequestResponse
        {
            public static IRequester Requester<TMessage>(ICommunicationObjectsFactory<TMessage> factory)
                where TMessage : IMessage
            {
                var channel = factory.CreateChannel();
                return new Requester(channel);
            }

            public static ServerSetup<TMessage> SetupServer<TMessage>(ICommunicationObjectsFactory<TMessage> factory)
                where TMessage : IMessage
            {
                Enforce.NotNull(factory, nameof(factory));

                return new ServerSetup<TMessage>(factory);
            }

            public class ServerSetup<TMessage>
                where TMessage : IMessage
            {
                private readonly ICommunicationObjectsFactory<TMessage> _factory;
                private EventHandler<RequestReceivedArgs<TMessage>> _requestHandler;
                private EventHandler<EventArgs> _channelClosedHandler;
                private EventHandler<ClientAcceptedEventArgs> _newClientHandler;
                private bool _setupFinished;
                private readonly string _setupFinishedDescription = "'Using...' methods should be called before 'Go'";

                internal ServerSetup(ICommunicationObjectsFactory<TMessage> factory)
                {
                    _factory = factory;
                }

                public ServerSetup<TMessage> UsingRequestHandler(EventHandler<RequestReceivedArgs<TMessage>> requestHandler)
                {
                    Enforce.State.FitsTo(!_setupFinished, _setupFinishedDescription);

                    _requestHandler = requestHandler;
                    return this;
                }

                public ServerSetup<TMessage> UsingChannelClosedHandler(EventHandler<EventArgs> channelClosedHandler)
                {
                    Enforce.State.FitsTo(!_setupFinished, _setupFinishedDescription);

                    _channelClosedHandler = channelClosedHandler;
                    return this;
                }

                public ServerSetup<TMessage> UsingNewClientHandler(EventHandler<ClientAcceptedEventArgs> newClientHandler)
                {
                    Enforce.State.FitsTo(!_setupFinished, _setupFinishedDescription);

                    _newClientHandler = newClientHandler;
                    return this;
                }

                public NewChannelRequestAcceptor Go()
                {
                    _setupFinished = true;
                    var awaiter = _factory.CreateChannelAwaiter();

                    var requestAcceptor = new NewChannelRequestAcceptor(awaiter);
                    requestAcceptor.ClientAccepted += (sender, a) =>
                    {
                        _newClientHandler?.Invoke(sender, a);

                        var responder = new Responder<TMessage>((IChannel<TMessage>)a.Channel);
                        responder.RequestReceived += (o, args) => { _requestHandler?.Invoke(o, args); };
                        responder.ChannelClosed += (o, args) =>
                        {
                            _channelClosedHandler?.Invoke(o, args);
                            ((IResponder<TMessage>)o).Channel.Dispose();
                        };
                        responder.StartResponding();
                    };

                    requestAcceptor.StartAcceptLoop();
                    return requestAcceptor;
                }
            }
        }
    }
}