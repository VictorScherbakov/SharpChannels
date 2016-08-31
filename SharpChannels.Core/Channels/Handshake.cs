using System;
using System.Reflection;
using SharpChannels.Core.Messages.System;
using SharpChannels.Core.Serialization.System;

namespace SharpChannels.Core.Channels
{
    internal class Handshake
    {
        static Handshake()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            int hash = 0;

            hash |= (version.Major & 0x0000000F) << 28;
            hash |= (version.Minor & 0x000000FF) << 20;
            hash |= (version.Build & 0x000000FF) << 12;

            byte byte2 = (byte)((hash & 0x0000FF00) >> 8);

            byte byte3 = (byte)((hash & 0x00FF0000) >> 16);
            byte byte4 = (byte)((hash & 0xFF000000) >> 24);

            ushort n1 = (ushort)(byte1 | (byte2 << 8));
            ushort n2 = (ushort)(byte3 | (byte4 << 8));

            _marker = (ushort) (n1 ^ n2);
        }

        private static readonly ushort _marker;

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

