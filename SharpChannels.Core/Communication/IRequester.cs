using SharpChannels.Core.Channels;
using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IRequester<TMessage>
        where TMessage : IMessage
    {
        IChannel<TMessage> Channel { get; }
        TMessage Request(TMessage request);
    }
}