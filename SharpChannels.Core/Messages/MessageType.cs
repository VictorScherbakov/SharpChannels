namespace SharpChannels.Core.Messages
{
    public enum MessageType : ushort
    {
        HandshakeRequest = 0,
        HandshakeResponse = 1,
        EndSession = 2,
        User = 15,
    }
}