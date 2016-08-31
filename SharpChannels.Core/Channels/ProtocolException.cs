using System;

namespace SharpChannels.Core.Channels
{
    public class ProtocolException : Exception
    {
        public ProtocolException(string message, ProtocolErrorCode code) : base(message)
        {
            Code = code;
        }

        public ProtocolException(string message, ProtocolErrorCode code, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }

        public ProtocolErrorCode Code { get; }
    }
}