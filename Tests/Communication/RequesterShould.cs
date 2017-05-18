using Moq;
using NUnit.Framework;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;

namespace Tests.Communication
{
    [TestFixture]
    class RequesterShould
    {
        [Test]
        public void ReturnNullOnClosedChannel()
        {
            var channel = new Mock<IChannel>();
            channel.Setup(c => c.IsOpened).Returns(false);

            var requester = new Requester(channel.Object);

            var result = requester.Request(new StringMessage(string.Empty));

            Assert.IsNull(result);
        }

        [Test]
        public void CallRequestOnOpenChannel()
        {
            var channel = new Mock<IChannel>();
            channel.Setup(c => c.IsOpened).Returns(true);

            bool sendWasCalled = false;
            channel.Setup(c => c.Send(It.IsAny<IMessage>()))
                .Callback(() => { sendWasCalled = true; });

            var requester = new Requester(channel.Object);

            requester.Request(new StringMessage(string.Empty));

            Assert.IsTrue(sendWasCalled);
        }

        [Test]
        public void ReturnReceivedMessageOnOpenChannel()
        {
            var channel = new Mock<IChannel>();
            channel.Setup(c => c.IsOpened).Returns(true);

            var message = new StringMessage(string.Empty);
            channel.Setup(c => c.Receive()).Returns(message);

            var requester = new Requester(channel.Object);

            var result = requester.Request(new StringMessage(string.Empty));

            Assert.AreEqual(message, result);
        }
    }
}
