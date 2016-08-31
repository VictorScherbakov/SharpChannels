namespace SharpChannels.Core.Channels.Intradomain
{
    internal class IntradomainSocket 
    {
        public SocketType Type { get; private set; }
        public string Hub { get; private set; }
        public string ConnectionId { get; internal set; }

        public static IntradomainSocket ClientSocket(string hub)
        {
            return new IntradomainSocket
            {
                Type = SocketType.Client,
                Hub = hub
            };
        }

        public static IntradomainSocket ServerSocket(string hub, string id)
        {
            return new IntradomainSocket
            {
                Type = SocketType.Server,
                Hub = hub,
                ConnectionId = id
            };
        }

        public static IntradomainSocket ListenterSocket(string hub)
        {
            return new IntradomainSocket
            {
                Type = SocketType.Listener,
                Hub = hub
            };
        }

        internal IntradomainSocket(SocketType type, string hub)
        {
            Type = type;
            Hub = hub;
        }

        internal IntradomainSocket()
        {
        }
    }
}