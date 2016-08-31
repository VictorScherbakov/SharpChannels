namespace SharpChannels.Core.Channels
{
    public class ChannelSettings
    {
        public int MaxMessageLength { get; set; }

        public static ChannelSettings GetDefaultSettings()
        {
            return new ChannelSettings { MaxMessageLength = 65536 };
        }
    }
}