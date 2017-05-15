using System.Collections.Generic;
using System.Linq;
using SharpChannels.Core.Contracts;

namespace SharpChannels.Core.Messages.PubSub
{
    internal class SubscribeMessage : IMessage
    {
        public string[] Topics { get; }

        public SubscribeMessage(IEnumerable<string> topics)
        {
            Enforce.NotNull(topics, nameof(topics));

            Topics = topics as string[] ?? topics.ToArray();
        }

        public override string ToString()
        {
            return string.Join(",", Topics);
        }
    }
}