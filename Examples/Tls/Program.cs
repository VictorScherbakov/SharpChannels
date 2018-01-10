using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using SharpChannels.Core;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;
using SharpChannels.Security;

namespace Examples.Tls
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TLS example");


            var serializer = new StringMessageSerializer();

            var serverTlsWrapper = new ServerTlsWrapper(new X509Certificate2("server.pfx"), false, CertificateValidationCallback);

            ICommunication<StringMessage> serverCommunication = 
                new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000),
                                                                  serializer,
                                                                  ChannelSettings.GetDefault(),
                                                                  TcpConnectionSettingsBuilder.GetDefaultSettings(),
                                                                  serverTlsWrapper);

            var server = Scenarios.RequestResponse.SetupServer(serverCommunication)
                .UsingNewClientHandler((sender, a) => { Console.WriteLine("channel opened"); })
                .UsingRequestHandler((sender, a) => { a.Response = new StringMessage(a.Request.Message.Replace("request", "response")); })
                .UsingChannelClosedHandler((sender, a) => { Console.WriteLine("channel closed"); })
                .Go();

            var clientTlsWrapper = new ClientTlsWrapper("SharpChannels Example Server Certificate",
                                                        new X509Certificate[] { new X509Certificate2("client.pfx") },
                                                        false,
                                                        CertificateValidationCallback);

            ICommunication<StringMessage> clientCommunication = 
                new TcpCommunication<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000),
                                                                  serializer,
                                                                  ChannelSettings.GetDefault(),
                                                                  TcpConnectionSettingsBuilder.GetDefaultSettings(),
                                                                  clientTlsWrapper);

            var r = Scenarios.RequestResponse.Requester(clientCommunication);

            using (r.Channel)
            {
                r.Channel.Open();

                var requester = new Requester(r.Channel);

                var requestMessage = new StringMessage($"request using tls");
                Console.WriteLine(requestMessage);

                var responseMessage = requester.Request(requestMessage);
                Console.WriteLine(responseMessage);

                r.Channel.Close();
            }

            server.Stop();

            Console.ReadKey();
        }

        private static bool CertificateValidationCallback(object sender,
                                           X509Certificate certificate,
                                           X509Chain chain,
                                           SslPolicyErrors sslPolicyErrors)
        {
            switch (sslPolicyErrors)
            {
                case SslPolicyErrors.None:
                case SslPolicyErrors.RemoteCertificateChainErrors:
                    return true;
            }

            return false;
        }
    }
}
