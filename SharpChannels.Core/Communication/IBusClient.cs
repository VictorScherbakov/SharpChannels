using SharpChannels.Core.Messages;

namespace SharpChannels.Core.Communication
{
    public interface IBusClient<in TMessage> where TMessage : IMessage
    {
        void Publish(string topic, TMessage message);
        void Close();
    }
}