using System.Collections.Generic;
using System.Linq;
using SharpChannels.Core.Channels;

namespace SharpChannels.Core.Communication
{
    internal class SubscriptionManager
    {
        private readonly object _locker = new object();
        private readonly Dictionary<string, List<IChannel>> _subscriptions = new Dictionary<string, List<IChannel>>();

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

        public IChannel[] GetChannelsByTopic(string topic)
        {
            lock (_locker)
            {
                RemoveClosedChannels();

                return !_subscriptions.ContainsKey(topic)
                    ? new IChannel[] { }
                    : _subscriptions[topic].ToArray();
            }
        }

        public IChannel[] GetOpenChannels()
        {
            lock (_locker)
            {
                RemoveClosedChannels();
                return _subscriptions.Keys.SelectMany(key => _subscriptions[key]).ToArray();
            }
        }

        public void AddSubscription(IChannel channel, IEnumerable<string> topics)
        {
            lock (_locker)
            {
                foreach (var topic in topics)
                {
                    EnsureListExists(topic);
                    _subscriptions[topic].Add(channel);
                }
            }
        }

        private void EnsureListExists(string topic)
        {
            if (!_subscriptions.ContainsKey(topic))
                _subscriptions.Add(topic, new List<IChannel>());
        }
    }
}