using System;
using System.Threading.Tasks;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public class Receiver : IReceiver
    {
        public IChannel Channel { get; }

        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<EventArgs> ChannelClosed;

        public event EventHandler<ExceptionEventArgs> ReceiveMessageFailed;

        public bool Active { get; private set; }

        public void StartReceiving()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Active = true;
                    try
                    {
                        while (Channel.IsOpened && Active)
                        {
                            var message = Channel.Receive();
                            if (message != null)
                                OnMessageReceived(new MessageEventArgs(message, Channel));
                        }
                    }
                    finally
                    {
                        Active = false;
                    }
                }
                catch (Exception ex)
                {
                    OnReceiveMessageFailed(new ExceptionEventArgs(ex));
                }

                OnChannelClosed();
            }, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            Active = false;

            if(Channel.IsOpened)
                Channel.Close();
        }

        public Receiver(IChannel channel)
        {
            Enforce.NotNull(channel, nameof(channel));

            Channel = channel;
        }

        protected virtual void OnMessageReceived(MessageEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        protected virtual void OnChannelClosed()
        {
            ChannelClosed?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnReceiveMessageFailed(ExceptionEventArgs e)
        {
            ReceiveMessageFailed?.Invoke(this, e);
        }
    }
}