using System;

namespace SharpChannels.Core.Messages.System
{
    internal class EndSessionMessage : IMessage
    {
        private static readonly object _syncRoot = new object();
        private static volatile EndSessionMessage _instance;
        
        public static EndSessionMessage Instance 
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new EndSessionMessage();
                    }
                }
                return _instance;
            }
        }
    }
}