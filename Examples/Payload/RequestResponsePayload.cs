using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SharpChannels.Core;
using SharpChannels.Core.Channels;
using SharpChannels.Core.Channels.Intradomain;
using SharpChannels.Core.Channels.Tcp;
using SharpChannels.Core.Communication;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Serialization;

namespace Payload
{
    public class RequestResponsePayload
    {

        private NewChannelRequestAcceptor StartServer(ICommunicationObjectsFactory<StringMessage> factory)
        {
            return Scenarios.RequestResponse.SetupServer(factory)
                .UsingRequestHandler((sender, a) => { a.Response = new StringMessage(a.Request.Message + " response"); })
                .Go();
        }

        private NewChannelRequestAcceptor StartTcpServer()
        {
            var connectionSettings = new TcpConnectionSettingsBuilder()
                                        .UsingSendTimeout(TimeSpan.FromHours(1))
                                        .UsingReceiveTimeout(TimeSpan.FromHours(1))
                                        .Build();

            var factory = new TcpCommunicationObjectsFactory<StringMessage>(new TcpEndpointData(IPAddress.Any, 2000),
                                                                            new StringMessageSerializer(),
                                                                            ChannelSettings.GetDefault(),
                                                                            connectionSettings);

            return StartServer(factory);
        }

        private NewChannelRequestAcceptor StartIntradomainServer()
        {
            var connectionSettings = new IntradomainConnectionSettingsBuilder()
                                        .UsingSendTimeout(TimeSpan.FromHours(1))
                                        .UsingReceiveTimeout(TimeSpan.FromHours(1))
                                        .Build();

            var factory = new IntradomainCommunicationObjectsFactory<StringMessage>(new IntradomainEndpoint("payload"),
                                                                                    new StringMessageSerializer(),
                                                                                    ChannelSettings.GetDefault(), 
                                                                                    connectionSettings);

            return StartServer(factory);
        }

        private IRequester GetTcpRequester()
        {
            var connectionSettings = new TcpConnectionSettingsBuilder()
                                        .UsingSendTimeout(TimeSpan.FromHours(1))
                                        .UsingReceiveTimeout(TimeSpan.FromHours(1))
                                        .Build();

            var clientFactory = new TcpCommunicationObjectsFactory<StringMessage>(new TcpEndpointData(IPAddress.Loopback, 2000),
                                                                                  new StringMessageSerializer(),
                                                                                  ChannelSettings.GetDefault(), 
                                                                                  connectionSettings);

            return Scenarios.RequestResponse.Requester(clientFactory);
        }

        private IRequester GetIntradomainRequester()
        {
            var connectionSettings = new IntradomainConnectionSettingsBuilder()
                                        .UsingSendTimeout(TimeSpan.FromHours(1))
                                        .UsingReceiveTimeout(TimeSpan.FromHours(1))
                                        .Build();

            var clientFactory = new IntradomainCommunicationObjectsFactory<StringMessage>(new IntradomainEndpoint("payload"),
                                                                                          new StringMessageSerializer(),
                                                                                          ChannelSettings.GetDefault(),
                                                                                          connectionSettings);

            return Scenarios.RequestResponse.Requester(clientFactory);
        }

        public void Run(Transport transport, int clientCount, int messagePerClient, Action<string> log)
        {
            var server = transport == Transport.Tcp 
                ? StartTcpServer() 
                : StartIntradomainServer();

            int requestCount = 0;
            int tasksStarted = 0;

            var tasks = new List<Task>();
            for (int i = 0; i < clientCount; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var requester = transport == Transport.Tcp 
                        ? GetTcpRequester() 
                        : GetIntradomainRequester();

                    using (requester.Channel)
                    {
                        Interlocked.Increment(ref tasksStarted);
                        requester.Channel.Open();
                        for (int j = 0; j < messagePerClient; j++)
                        {
                            var response = requester.Request(new StringMessage(Guid.NewGuid().ToString()));
                            Interlocked.Increment(ref requestCount);
                        }

                        requester.Channel.Close();
                    }
                }, TaskCreationOptions.LongRunning));
            }

            var task = Task.WhenAll(tasks.ToArray());
            var sw = new Stopwatch();
            sw.Start();
            while (!task.IsCompleted)
            {
                var requestsPerSeconds = requestCount / sw.Elapsed.TotalSeconds;
                log?.Invoke($"Tasks running:{tasksStarted} Requests per second:{requestsPerSeconds}");
                Thread.Sleep(40);
            }

            server.Stop();
        }
    }
}