namespace SharpChannels.Core.Channels
{
    public interface IChannelAwaiter<out T>
        where T : IChannel
    {
        T AwaitNewChannel();

        bool Active { get; }

        void Stop();
        void Start();
    }
}