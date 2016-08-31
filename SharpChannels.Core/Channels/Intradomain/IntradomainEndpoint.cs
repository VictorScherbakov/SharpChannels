namespace SharpChannels.Core.Channels.Intradomain
{
    public class IntradomainEndpoint : IEndpointData
    {
        public string Hub { get; private set; }

        public IntradomainEndpoint(string hub)
        {
            Hub = hub;
        }
    }
}
