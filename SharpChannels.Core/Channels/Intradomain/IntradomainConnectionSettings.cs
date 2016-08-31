using System;

namespace SharpChannels.Core.Channels.Intradomain
{
    public class IntradomainConnectionSettings
    {
        public TimeSpan SendTimeout { get; set; }
        public TimeSpan ReceiveTimeout { get; set; }
        public TimeSpan ConnectTimeout { get; set; }

        public static IntradomainConnectionSettings GetDefault()
        {
            return new IntradomainConnectionSettings
            {
                SendTimeout = TimeSpan.FromMilliseconds(1000),
                ReceiveTimeout = TimeSpan.FromMilliseconds(1000),
                ConnectTimeout = TimeSpan.FromMilliseconds(1000)
            };
        }
    }
}