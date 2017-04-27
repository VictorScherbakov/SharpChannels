using SharpChannels.Core.Contracts;

namespace SharpChannels.Core.Messages.System
{
    internal class HandshakeResponse : IMessage
    {
        public ushort ResponseMarker { get; }

        public HandshakeResponse(ushort responseMarker)
        {
            Enforce.NotNull(responseMarker, nameof(responseMarker));

            ResponseMarker = responseMarker;
        }
    }
}