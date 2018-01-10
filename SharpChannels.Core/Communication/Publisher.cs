using System;
using System.Threading.Tasks;

using SharpChannels.Core.Channels;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Messages.PubSub;
using SharpChannels.Core.Serialization.PubSub;

namespace SharpChannels.Core.Communication
{
    public class Publisher : IPublisher
    {
        private readonly INewChannelRequestAcceptor _newChannelRequestAcceptor;
        private readonly bool _stopAcceptorOnClose;

        private bool _isDisposed;

        private readonly SubscribeMessageSerializer _subscribeMessageSerializer;
        private readonly SubscriptionManager _subscriptionManager;

        private static void SafeSend(IChannel channel, IMessage message)
        {
            try
            {
                if(channel.IsOpened)
                    channel.Send(message);
            }
            catch (DataTransferException ex) when(ex.Code == DataTransferErrorCode.ChannelClosed)
            {
                // trying to send to the closed channel, do nothing
            }
            catch (Exception)
            {
                if (channel.IsOpened)
                    channel.Close();

                throw;
            }
        }

        public int SubscriprionNumber => _subscriptionManager.SubscriprionNumber;

        public bool Active { get; private set; }

        public void Broadcast(string topic, IMessage message)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.NotDisposed(this, _isDisposed);

            foreach (var channel in _subscriptionManager.GetChannelsByTopic(topic))
                SafeSend(channel, message);
        }

        public void ParallelBroadcast(string topic, IMessage message, int parallelismDegree)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.Positive(parallelismDegree, nameof(parallelismDegree));
            Enforce.NotDisposed(this, _isDisposed);

            var openChannels = _subscriptionManager.GetChannelsByTopic(topic);

            var options = new ParallelOptions {MaxDegreeOfParallelism = parallelismDegree};
            Parallel.ForEach(openChannels, options, channel => SafeSend(channel, message));
        }

        public async Task ParallelBroadcastAsync(string topic, IMessage message, int parallelismDegree)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.Positive(parallelismDegree, nameof(parallelismDegree));
            Enforce.NotDisposed(this, _isDisposed);

            var openChannels = _subscriptionManager.GetChannelsByTopic(topic);

            await Task.Run(() =>
            {
                var options = new ParallelOptions {MaxDegreeOfParallelism = parallelismDegree};
                Parallel.ForEach(openChannels, options, channel => SafeSend(channel, message));
            });
        }

        public async Task BroadcastAsync(string topic, IMessage message)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.NotDisposed(this, _isDisposed);

            var openChannels = _subscriptionManager.GetChannelsByTopic(topic);

            await Task.Run(() =>
            {
                foreach (var channel in openChannels)
                    SafeSend(channel, message);
            });
        }

        public void Close()
        {
            Enforce.NotDisposed(this, _isDisposed);
            try
            {
                _newChannelRequestAcceptor.ClientAccepted -= ClientAccepted;

                if (_stopAcceptorOnClose && _newChannelRequestAcceptor.Active)
                    _newChannelRequestAcceptor.Stop();

                foreach (var channel in _subscriptionManager.GetOpenChannels())
                    channel.Close();
            }
            finally
            {
                Active = false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ClientAccepted(object sender, ClientAcceptedEventArgs args)
        {
            Task.Run(() =>
            {
                var subscription = (SubscribeMessage)args.Channel.Receive(_subscribeMessageSerializer);
                _subscriptionManager.AddSubscription(args.Channel, subscription.Topics);
                ClientSubscribed?.Invoke(this, args);
            });
        }

        public event EventHandler<ClientAcceptedEventArgs> ClientSubscribed;

        public Publisher(INewChannelRequestAcceptor newChannelRequestAcceptor, bool stopAcceptorOnClose)
        {
            Enforce.NotNull(newChannelRequestAcceptor, nameof(newChannelRequestAcceptor));

            _subscribeMessageSerializer = new SubscribeMessageSerializer();

            _newChannelRequestAcceptor = newChannelRequestAcceptor;
            _stopAcceptorOnClose = stopAcceptorOnClose;

            _subscriptionManager = new SubscriptionManager();

            _newChannelRequestAcceptor.ClientAccepted += ClientAccepted;

            Active = true;
        }
    }
}