namespace SharpChannels.Core.Channels
{
    public enum ProtocolErrorCode
    {
        InvalidHandshakeRequest = 1,
        InvalidHandshakeResponse = 2,

        UnexpectedMessageType = 3,
        HandshakeRequired = 4,

        UnknownMessageType = 5,
        MessageTooLong = 6,

        InvalidTopicNumber = 7,
        TooManyTopics = 8,
    }
}