using System;

namespace SharpChannels.Core.Channels.Intradomain
{
    public class IntradomainConnectionSettings
    {
        public TimeSpan SendTimeout { get; set; }
        public TimeSpan ReceiveTimeout { get; set; }
        public TimeSpan ConnectTimeout { get; set; }
    }
}