# ![SharpChannels](logo.png "SharpChannels") 

[![Build status](https://ci.appveyor.com/api/projects/status/923senof89ihceqy?svg=true)](https://ci.appveyor.com/project/VictorScherbakov/sharpchannels) [![license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/VictorScherbakov/SharpChannels/blob/master/LICENSE)

SharpChannels is a simple and lightweight communication library for .NET.

### What?

Two things:
1. Gives you the higher level entities to use for messaging instead of the byte streams and sockets.  
Namely: channels and messages
2. Implements well-known messaging scenarios
- request-response
- publisher-subscriber
- service bus


### Why?

It is very expensive to develop a reliable server and client (using TCP for example), 
implement an application layer protocol for messaging, and then use all this stuff in a communication scenarios, like response to the requestor, broadcasts message to the subscribers etc.

SharpChannels has already done it all.

Use entities which can send and receive messages rather than write the byte sequence to or read it from stream.
It also gives you a seamless switching between transport protocols.

#### Transports
- TCP
- In-process (inside AppDomain)

#### Security
- TLS 1.2

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

var serverCommunication = new TcpCommunication<StringMessage>(
                            new TcpEndpointData(IPAddress.Any, 2000), 
                            serializer);

var server = Scenarios.RequestResponse.SetupServer(serverCommunication)
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
var clientCommunication = new TcpCommunication<StringMessage>(
                            new TcpEndpointData(IPAddress.Loopback, 2000), 
                            serializer);

var r = Scenarios.RequestResponse.Requester(clientCommunication);

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
### License

SharpChannels is licensed under [The MIT License](https://opensource.org/licenses/MIT)  
See [LICENSE](LICENSE) for details.
