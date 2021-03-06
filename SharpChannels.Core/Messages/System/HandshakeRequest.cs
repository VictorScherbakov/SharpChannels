﻿using SharpChannels.Core.Contracts;

namespace SharpChannels.Core.Messages.System
{
    internal class HandshakeRequest : IMessage
    {
        public ushort RequestMarker { get; }

        public bool IsResponseCorrect(HandshakeResponse response)
        {
            return RequestMarker == response.ResponseMarker;
        }

        public HandshakeRequest(ushort requestMarker)
        {
            Enforce.NotNull(requestMarker, nameof(requestMarker));

            RequestMarker = requestMarker;
        }
    }
}
