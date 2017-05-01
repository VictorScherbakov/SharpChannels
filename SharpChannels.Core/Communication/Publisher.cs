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
        private bool _isDisposed;

        private readonly object _locker = new object();

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

        public int SubscriberCount
        {
            get
            {
                lock (_locker)
                {
                    return _channels.Count(channel => channel.IsOpened);
                }
            }
        }

        public bool Active { get; private set; }

        private IChannel[] GetOpenChannels()
        {
            lock (_locker)
            {
                RemoveClosedChannels();
                return _channels.Where(channel => channel.IsOpened).ToArray();
            }
        }

        public void Broadcast(IMessage message)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.NotDisposed(this, _isDisposed);

            foreach (var channel in GetOpenChannels())
                SafeSend(channel, message);
        }

        public void ParallelBroadcast(IMessage message, int parallelismDegree)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.Positive(parallelismDegree, nameof(parallelismDegree));
            Enforce.NotDisposed(this, _isDisposed);

            var openChannels = GetOpenChannels();

            var options = new ParallelOptions {MaxDegreeOfParallelism = parallelismDegree};
            Parallel.ForEach(openChannels, options, channel => SafeSend(channel, message));
        }

        public async Task ParallelBroadcastAsync(IMessage message, int parallelismDegree)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.Positive(parallelismDegree, nameof(parallelismDegree));
            Enforce.NotDisposed(this, _isDisposed);

            var openChannels = GetOpenChannels();

            await Task.Run(() =>
            {
                var options = new ParallelOptions {MaxDegreeOfParallelism = parallelismDegree};
                Parallel.ForEach(openChannels, options, channel => SafeSend(channel, message));
            });
        }

        public async Task BroadcastAsync(IMessage message)
        {
            Enforce.NotNull(message, nameof(message));
            Enforce.NotDisposed(this, _isDisposed);

            var openChannels = GetOpenChannels();

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
            lock (_locker)
            {
                _channels.Add(args.Channel);
            }
        }

        public Publisher(INewChannelRequestAcceptor newChannelRequestAcceptor, bool stopAcceptorOnClose)
        {
            Enforce.NotNull(newChannelRequestAcceptor, nameof(newChannelRequestAcceptor));

            _newChannelRequestAcceptor = newChannelRequestAcceptor;
            _stopAcceptorOnClose = stopAcceptorOnClose;

            _newChannelRequestAcceptor.ClientAccepted += ClientAccepted;

            Active = true;
        }
    }
}