namespace SharpChannels.Core.Messages.System
{
    internal class HandshakeResponse : IMessage
    {
        public ushort ResponseMarker { get; }

        public HandshakeResponse(ushort responseMarker)
        {
            ResponseMarker = responseMarker;
        }
    }
}