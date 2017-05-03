# SharpChannels
Simple and lightweight communication library for .NET. It allows to implement message based communications in your apps.

#### Transports
- TCP
- In-process (inside AppDomain)

#### Scenarios
- publisher-subscriber
- request-response
- freeway messaging

#### Serialization
- built-in for text messages
- built-in for binary messages
- native (using BinaryFormatter class)
- proto-buf
- custom user serialization

#### Other features
- version based handshake
- explicit channel closing (as in SCTP)

### Usage
Нere is a simple example of request-response messaging using SharpChannels

Server side:
```c#
var serializer = new StringMessageSerializer();

var serverFactory = new TcpCommunicationObjectsFactory<StringMessage>(
                            new TcpEndpointData(IPAddress.Any, 2000), 
                            serializer);

var server = Scenarios.RequestResponse.SetupServer(serverFactory)
    .UsingNewClientHandler((sender, a) => { Console.WriteLine("channel opened"); })
    .UsingRequestHandler((sender, a) => 
            { 
                        a.Response = new StringMessage(a.Request.Message.Replace("request", "response")); 
            })
    .UsingChannelClosedHandler((sender, a) => { Console.WriteLine("channel closed"); })
    .Go();

```

Client side:
```c#
var clientFactory = new TcpCommunicationObjectsFactory<StringMessage>(
                            new TcpEndpointData(IPAddress.Loopback, 2000), 
                            serializer);

var r = Scenarios.RequestResponse.Requester(clientFactory);

using (r.Channel)
{
    r.Channel.Open();

    for (int i = 0; i < 100; i++)
    {
        var requestMessage = new StringMessage($"request #{i}");
        Console.WriteLine(requestMessage);

        var responseMessage = r.Request(requestMessage);
        Console.WriteLine(responseMessage);
    }

    r.Channel.Close();
}
```
