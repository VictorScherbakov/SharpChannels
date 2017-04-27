using System;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IReceiver
    {
        IChannel Channel { get; }
        void StartReceiving();
        void Stop();

        event EventHandler<MessageEventArgs> MessageReceived;
        event EventHandler<EventArgs> ChannelClosed;
        event EventHandler<ExceptionEventArgs> ReceiveMessageFailed;
    }
}