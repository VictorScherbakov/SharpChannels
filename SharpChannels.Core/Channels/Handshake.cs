using SharpChannels.Core.Messages.System;
using SharpChannels.Core.Serialization.System;

namespace SharpChannels.Core.Channels
{
    internal class Handshake
    {
        private static readonly ushort _marker = 555;

        private static readonly HandshakeRequestSerializer _handshakeRequestSerializer = new HandshakeRequestSerializer();
        private static readonly HandshakeResponseSerializer _handshakeResponseSerializer = new HandshakeResponseSerializer();

        public bool Request(IChannel channel)
        {
            var request = new HandshakeRequest(_marker);
            channel.Send(request, _handshakeRequestSerializer);
            var response = (HandshakeResponse)channel.Receive(_handshakeResponseSerializer);

            return request.IsResponseCorrect(response);
        }

        public bool Response(IChannel channel)
        {
            var request = (HandshakeRequest)channel.Receive(_handshakeRequestSerializer);

            if (request.RequestMarker != _marker)
                return false;

            var response = new HandshakeResponse(request.RequestMarker);

            channel.Send(response, _handshakeResponseSerializer);
            return true;
        }
    }
}

