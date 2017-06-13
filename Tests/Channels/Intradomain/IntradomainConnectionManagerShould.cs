using System.Linq;
using SharpChannels.Core.Channels.Intradomain;
using NUnit.Framework;

namespace Tests.Channels.Intradomain
{
    [TestFixture]
    class IntradomainConnectionManagerShould
    {
        [SetUp]
        public void Setup()
        {
            IntradomainConnectionManager.Reset();
        }

        [Test]
        public void ShowRegisteredListener()
        {
            IntradomainConnectionManager.Instance.Listen("1", endpoint => null);

            Assert.That(IntradomainConnectionManager.Instance.IsListening("1"));
            Assert.That(IntradomainConnectionManager.Instance.Listeners.Contains("1"));
        }

        [Test]
        public void SuccessfulyUnregisterListener()
        {
            IntradomainConnectionManager.Instance.Listen("1", endpoint => null);
            IntradomainConnectionManager.Instance.StopListening("1");

            Assert.IsFalse(IntradomainConnectionManager.Instance.IsListening("1"));
            Assert.IsFalse(IntradomainConnectionManager.Instance.Listeners.Contains("1"));
        }

        [Test]
        public void RejectRegisteringOfAlreadyPresentListener()
        {
            IntradomainConnectionManager.Instance.Listen("1", endpoint => null);
            var result = IntradomainConnectionManager.Instance.Listen("1", endpoint => null);

            Assert.IsFalse(result);
        }

        [Test]
        public void CallProvidedMethodOnNewClientConnection()
        {
            bool wasCalled = false;
            IntradomainConnectionManager.Instance.Listen("1", endpoint =>
            {
                wasCalled = true;
                return null;
            });

            IntradomainConnectionManager.Instance.Connect(IntradomainSocket.ClientSocket("1"));

            Assert.That(wasCalled);
        }

        [Test]
        public void ShowEstablishedConnection()
        {
            var clientSocket = IntradomainSocket.ClientSocket("1");
            IntradomainConnectionManager.Instance.Listen("1", endpoint => IntradomainSocket.ServerSocket("1", clientSocket.ConnectionId));

            clientSocket.ConnectionId = "1";

            var connected = IntradomainConnectionManager.Instance.Connect(clientSocket);

            Assert.That(connected);
            Assert.That(IntradomainConnectionManager.Instance.Connected(clientSocket));
        }

        [Test]
        public void NotShowClosedConnection()
        {
            var clientSocket = IntradomainSocket.ClientSocket("1");
            IntradomainConnectionManager.Instance.Listen("1", endpoint => IntradomainSocket.ServerSocket("1", clientSocket.ConnectionId));

            clientSocket.ConnectionId = "1";

            IntradomainConnectionManager.Instance.Connect(clientSocket);
            IntradomainConnectionManager.Instance.Disconnect(clientSocket);

            Assert.IsFalse(IntradomainConnectionManager.Instance.Connected(clientSocket));
        }

        [Test]
        public void RejectConnectionIfListenerDoesntExists()
        {
            var clientSocket = IntradomainSocket.ClientSocket("1");
            var connected = IntradomainConnectionManager.Instance.Connect(clientSocket);

            Assert.IsFalse(connected);
            Assert.IsFalse(IntradomainConnectionManager.Instance.Connected(clientSocket));
        }
    }
}
