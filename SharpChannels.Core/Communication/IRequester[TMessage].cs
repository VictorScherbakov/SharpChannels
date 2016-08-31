using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IRequester
    {
        IChannel Channel { get; }
        IMessage Request(IMessage request);
    }
}