using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Intradomain;
using SharpChannels.Core.Serialization;
using NUnit.Framework;

namespace Tests.Channels.Intradomain
{
    class IntradomainChannelAwaiterShould
    {
        [SetUp]
        public void Setup()
        {
            IntradomainConnectionManager.Reset();
        }

        [Test]
        public void SuccessfulyStart()
        {
            var serverEndpoint = new IntradomainEndpoint("1");
            var awaiter = new IntradomainChannelAwaiter(serverEndpoint, new StringMessageSerializer(), ChannelSettings.GetDefaultSettings());
            awaiter.Start();

            Assert.That(awaiter.Active);
        }

        [Test]
        public void SuccessfulyStop()
        {
            var serverEndpoint = new IntradomainEndpoint("1");
            var awaiter = new IntradomainChannelAwaiter(serverEndpoint, new StringMessageSerializer(), ChannelSettings.GetDefaultSettings());
            awaiter.Start();
            awaiter.Stop();

            Assert.IsFalse(awaiter.Active);
        }
    }
}