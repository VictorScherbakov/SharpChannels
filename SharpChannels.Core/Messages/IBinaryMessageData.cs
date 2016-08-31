namespace SharpChannels.Core.Messages
{
    public interface IBinaryMessageData
    {
        MessageType Type { get; }

        byte[] Data { get; }
    }
}