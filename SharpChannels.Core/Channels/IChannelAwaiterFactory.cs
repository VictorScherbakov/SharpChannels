using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels
{
    public interface IChannelAwaiterFactory<out T> 
        where T : IChannel
    {
        IChannelAwaiter<T> CreateInstance(IEndpointData endpointData, IMessageSerializer serializer);
    }
}