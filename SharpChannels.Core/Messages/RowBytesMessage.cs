using System;

namespace SharpChannels.Core.Messages
{
    [Serializable]
    public class RowBytesMessage : IMessage
    {
        public byte[] Message { get; }

        public RowBytesMessage(byte[] message)
        {
            Message = message;
        }
    }
}