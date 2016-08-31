using System.Threading.Tasks;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Intradomain;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;
using NUnit.Framework;

namespace Tests.Channels.Intradomain
{
    class IntradomainChannelShould
    {
        [SetUp]
        public void Setup()
        {
            IntradomainConnectionManager.Reset();
        }

        [Test]
        public void SuccessfulyOpen()
        {
            var listenterEndpoint = new IntradomainEndpoint("1"); 
            var awaiter = new IntradomainChannelAwaiter(listenterEndpoint, new StringMessageSerializer(), ChannelSettings.GetDefaultSettings());
            awaiter.Start();

            var task = Task.Run(() => { awaiter.AwaitNewChannel(); });

            var channel = new IntradomainChannel(new IntradomainEndpoint("1"), new StringMessageSerializer(), null, null);
            channel.Open();

            task.Wait();

            Assert.That(channel.IsOpened);
        }

        [Test]
        public void SuccessfulyTransmitMessage()
        {
            var listenterEndpoint = new IntradomainEndpoint("1"); 
            var awaiter = new IntradomainChannelAwaiter(listenterEndpoint, new StringMessageSerializer(), ChannelSettings.GetDefaultSettings());
            awaiter.Start();

            IChannel server = null;
            var task = Task.Run(() => { server = awaiter.AwaitNewChannel(); });

            var channel = new IntradomainChannel(new IntradomainEndpoint("1"), new StringMessageSerializer(), null);
            channel.Open();

            task.Wait();

            var message = new StringMessage("message");
            channel.Send(message);
            var receivedMessage = (StringMessage)server.Receive();

            Assert.That(receivedMessage.Message, Is.EqualTo(message.Message));
        }
    }
}