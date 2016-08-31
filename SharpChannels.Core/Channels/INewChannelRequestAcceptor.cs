using System;

namespace SharpChannels.Core.Channels
{
    public interface INewChannelRequestAcceptor
    {
        void StartAcceptLoop();
        void Stop();

        bool Active { get; }

        event EventHandler<ClientAcceptedEventArgs> ClientAccepted;
    }
}