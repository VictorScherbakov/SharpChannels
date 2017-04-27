using System;
using System.Threading.Tasks;
using SharpChannels.Core.Contracts;

namespace SharpChannels.Core.Channels
{
    public class NewChannelRequestAcceptor : INewChannelRequestAcceptor
    {
        private readonly IChannelAwaiter<IChannel> _awaiter;

        public void StartAcceptLoop()
        {
            Enforce.State.FitsTo(!Active, "Already started");

            Active = true;

            if(!_awaiter.Active)
                _awaiter.Start();

            var tf = new TaskFactory();
            tf.StartNew(() =>
            {
                try
                {
                    while (_awaiter.Active && Active)
                    {
                        var channel = _awaiter.AwaitNewChannel();
                        if (channel != null)
                            OnClientAccepted(new ClientAcceptedEventArgs(channel));
                    }
                }
                finally
                {
                    Active = false;
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            Enforce.State.FitsTo(Active, "Already stopped");

            Active = false;
            if(_awaiter.Active)
                _awaiter.Stop();
        }

        public bool Active { get; private set; }
        public event EventHandler<ClientAcceptedEventArgs> ClientAccepted;


        public NewChannelRequestAcceptor(IChannelAwaiter<IChannel> awaiter)
        {
            _awaiter = awaiter;
        }

        protected virtual void OnClientAccepted(ClientAcceptedEventArgs e)
        {
            ClientAccepted?.Invoke(this, e);
        }
    }
}