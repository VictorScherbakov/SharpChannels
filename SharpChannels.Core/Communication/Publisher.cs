using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<string, List<IChannel>> _subscriptions = new Dictionary<string, List<IChannel>>(); 

        private bool _isDisposed;

        private readonly object _locker = new object();
        private readonly SubscribeMessageSerializer _subscribeMessageSerializer;

        private static void SafeSend(IChannel channel, IMessage message)
        {
            try
            {
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

        private void RemoveClosedChannels()
        {
            foreach (var key in _subscriptions.Keys)
            {
                _subscriptions[key].RemoveAll(channel => !channel.IsOpened);
            }
        }

        public int SubscriprionNumber
        {
            get
            {
                lock (_locker)
                {
                    return _subscriptions.Keys.Sum(key => _subscriptions[key].Count(channel => channel.IsOpened));
                }
            }
        }

        public bool Active { get; private set; }

        private IChannel[] GetOpenChannels()
        {
            lock (_locker)
            {
                RemoveClosedChannels();
                return _subscriptions.Keys.SelectMany(key => _subscriptions[key]).ToArray();
            }
        }

        private IChannel[] GetChannelsByTopic(string topic)
        {
            lock (_locker)
            {
                RemoveClosedChannels();

                return !_subscriptions.ContainsKey(topic) 
                    ? new IChannel[] {} 
                    : _subscriptions[topic].ToArray();
            }
        }

        public void Broadcast(string topic, IMessage message)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.NotDisposed(this, _isDisposed);

            foreach (var channel in GetChannelsByTopic(topic))
                SafeSend(channel, message);
        }

        public void ParallelBroadcast(string topic, IMessage message, int parallelismDegree)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.Positive(parallelismDegree, nameof(parallelismDegree));
            Enforce.NotDisposed(this, _isDisposed);

            var openChannels = GetChannelsByTopic(topic);

            var options = new ParallelOptions {MaxDegreeOfParallelism = parallelismDegree};
            Parallel.ForEach(openChannels, options, channel => SafeSend(channel, message));
        }

        public async Task ParallelBroadcastAsync(string topic, IMessage message, int parallelismDegree)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.Positive(parallelismDegree, nameof(parallelismDegree));
            Enforce.NotDisposed(this, _isDisposed);

            var openChannels = GetChannelsByTopic(topic);

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

            var openChannels = GetChannelsByTopic(topic);

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

                foreach (var channel in GetOpenChannels())
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
                lock (_locker)
                {
                    foreach (var topic in subscription.Topics)
                    {
                        EnsureListExists(topic);
                        _subscriptions[topic].Add(args.Channel);
                    }
                }
            });
        }

        private void EnsureListExists(string topic)
        {
            if(!_subscriptions.ContainsKey(topic))
                _subscriptions.Add(topic, new List<IChannel>());
        }

        public Publisher(INewChannelRequestAcceptor newChannelRequestAcceptor, bool stopAcceptorOnClose)
        {
            Enforce.NotNull(newChannelRequestAcceptor, nameof(newChannelRequestAcceptor));

            _subscribeMessageSerializer = new SubscribeMessageSerializer();

            _newChannelRequestAcceptor = newChannelRequestAcceptor;
            _stopAcceptorOnClose = stopAcceptorOnClose;

            _newChannelRequestAcceptor.ClientAccepted += ClientAccepted;

            Active = true;
        }
    }
}