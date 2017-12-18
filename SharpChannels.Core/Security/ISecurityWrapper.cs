using System.IO;

namespace SharpChannels.Core.Security
{
    public interface ISecurityWrapper
    {
        Stream Wrap(Stream stream);
    }
}