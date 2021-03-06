using System;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IResponder
    {
        IChannel Channel { get; }
        void StartResponding();

        event EventHandler<RequestReceivedArgs> RequestReceived;
    }
}