using System;
using System.Threading.Tasks;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public abstract class ResponderBase
    {
        protected IChannel _channel;

        protected virtual void Response()
        {
            if (!_channel.IsOpened)
                return;

            var request = _channel.Receive();
            if (request == null)
                return;

            var response = OnRequestReceived(request);
            _channel.Send(response);
        }

        public event EventHandler<EventArgs> ChannelClosed;

        public void StartResponding()
        {
            Task.Factory.StartNew(() =>
            {
                while (_channel.IsOpened)
                {
                    Response();
                }
                OnChannelClosed();
            }, TaskCreationOptions.LongRunning);
        }

        protected abstract IMessage OnRequestReceived(IMessage request);

        protected virtual void OnChannelClosed()
        {
            ChannelClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}