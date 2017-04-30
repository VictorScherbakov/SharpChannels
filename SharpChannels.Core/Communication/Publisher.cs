using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Contracts;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public class Publisher : IPublisher
    {
        private readonly INewChannelRequestAcceptor _newChannelRequestAcceptor;
        private readonly bool _stopAcceptorOnClose;
        private readonly List<IChannel> _channels = new List<IChannel>();

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
            _channels.RemoveAll(channel => !channel.IsOpened);
        }

        public int SubscriberCount => _channels.Count(channel => channel.IsOpened);

        public bool Active { get; private set; }

        public void Broadcast(IMessage message)
        {
            Enforce.NotNull(message, nameof(message));

            RemoveClosedChannels();

            foreach (var channel in _channels.Where(channel => channel.IsOpened))
                SafeSend(channel, message);
        }

        public void ParallelBroadcast(IMessage message, int parallelismDegree)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.Positive(parallelismDegree, nameof(parallelismDegree));

            RemoveClosedChannels();

            var options = new ParallelOptions {MaxDegreeOfParallelism = parallelismDegree};
            Parallel.ForEach(_channels.Where(channel => channel.IsOpened), options, channel => SafeSend(channel, message));
        }

        public async Task ParallelBroadcastAsync(IMessage message, int parallelismDegree)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.Positive(parallelismDegree, nameof(parallelismDegree));

            RemoveClosedChannels();

            await Task.Run(() =>
            {
                var options = new ParallelOptions {MaxDegreeOfParallelism = parallelismDegree};
                Parallel.ForEach(_channels.Where(channel => channel.IsOpened), options, channel => SafeSend(channel, message));
            });
        }

        public async Task BroadcastAsync(IMessage message)
        {
            Enforce.NotNull(message, nameof(message));

            RemoveClosedChannels();

            await Task.Run(() =>
            {
                foreach (var channel in _channels.Where(channel => channel.IsOpened))
                    SafeSend(channel, message);
            });
        }

        public void Close()
        {
            try
            {
                foreach (var channel in _channels.Where(c => c.IsOpened))
                    channel.Close();

                if (_stopAcceptorOnClose && _newChannelRequestAcceptor.Active)
                    _newChannelRequestAcceptor.Stop();
            }
            finally
            {
                Active = false;
            }
        }

        public Publisher(INewChannelRequestAcceptor newChannelRequestAcceptor, bool stopAcceptorOnClose)
        {
            Enforce.NotNull(newChannelRequestAcceptor, nameof(newChannelRequestAcceptor));

            _newChannelRequestAcceptor = newChannelRequestAcceptor;
            _stopAcceptorOnClose = stopAcceptorOnClose;

            _newChannelRequestAcceptor.ClientAccepted += (sender, a) => { _channels.Add(a.Channel); };

            Active = true;
        }
    }
}