namespace SharpChannels.Core.Channels
{
    public class ClientAcceptedEventArgs
    {
        public IChannel Channel { get; private set; }

        public ClientAcceptedEventArgs(IChannel channel)
        {
            Channel = channel;
        }
    }
}