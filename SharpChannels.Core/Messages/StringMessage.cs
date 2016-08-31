using System;

namespace SharpChannels.Core.Messages
{
    [Serializable]
    public class StringMessage : IMessage
    {
        public string Message { get; }

        public StringMessage(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}