namespace SharpChannels.Core.Channels
{
    public interface IChannelFactory
    {
        IChannel CreateInstance();
    }
}