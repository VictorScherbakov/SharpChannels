using SharpChannels.Core.Serialization;

namespace SharpChannels.Core.Channels
{
    public interface IChannelEssentials
    {
        IEndpointData EndpointData { get; }
        IMessageSerializer Serializer { get; }
        void Open();
        void Close();
        bool IsOpened { get; }
        string Name { get; set; }
    }
}