using System;

namespace SharpChannels.Core.Messages.System
{
    internal class EndSessionMessage : IMessage
    {
        private static readonly Lazy<EndSessionMessage> _lazy = new Lazy<EndSessionMessage>(() => new EndSessionMessage());
        public static EndSessionMessage Instance => _lazy.Value;
    }
}