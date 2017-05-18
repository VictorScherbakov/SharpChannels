using Moq;
using NUnit.Framework;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Communication;

namespace Tests.Communication
{
    [TestFixture]
    public class SubscriptionManagerShould
    {
        [Test]
        public void SuccessfulyAddNewSubsription()
        {
            var subsriptionManager = new SubscriptionManager();
            subsriptionManager.AddSubscription(Mock.Of<IChannel>(), new []{ "topic" });

            Assert.IsTrue(true);
        }

        [Test]
        public void ReturnChannelsByTopic()
        {
            var subsriptionManager = new SubscriptionManager();

            var ch1 = new Mock<IChannel>();
            ch1.Setup(c => c.IsOpened).Returns(true);

            subsriptionManager.AddSubscription(ch1.Object, new[] { "topic1", "topic2" });
            subsriptionManager.AddSubscription(Mock.Of<IChannel>(), new[] { "topic1" });
            subsriptionManager.AddSubscription(Mock.Of<IChannel>(), new[] { "topic3" });

            var channels = subsriptionManager.GetChannelsByTopic("topic1");


            Assert.AreEqual(1, channels.Length);
            Assert.AreEqual(ch1.Object, channels[0]);
        }
    }
}