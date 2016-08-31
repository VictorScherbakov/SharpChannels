using System;
using System.Threading.Tasks;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public class Receiver
    {
        public IChannel Channel { get; }

        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<EventArgs> ChannelClosed;

        public bool Active { get; private set; }

        public void StartReceiving()
        {
            Task.Run(() =>
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

                OnChannelClosed();
            });
        }

        public void Stop()
        {
            Active = false;

            if(Channel.IsOpened)
                Channel.Close();
        }

        public Receiver(IChannel channel)
        {
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
    }
}