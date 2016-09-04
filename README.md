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
        var awaiter = new TcpChannelAwaiter<StringMessage>( // await tcp connections 
                new TcpEndpointData(IPAddress.Any, 2000),   // at port 2000
                new StringMessageSerializer());             // using StringMessageSerializer for new channels

        // setup the request acceptor
        var requestAcceptor = new NewChannelRequestAcceptor(awaiter);
        requestAcceptor.ClientAccepted += (sender, a) =>
        {
            Console.WriteLine("channel opened");

            var responder = new Responder<StringMessage>((IChannel<StringMessage>)a.Channel);
            responder.RequestReceived += (o, args) =>
            {
                // form the response message
                args.Response = new StringMessage(args.Request.Message.Replace("request from", "response to"));
            };

            responder.ChannelClosed += (o, args) =>
            {
                // handle channel closing 
                Console.WriteLine("channel closed");
            };

            // start response loop for this channel
            responder.StartResponding();
        };

        // here the server actually becomes available
        requestAcceptor.StartAcceptLoop();

```

Client side:
```c#
          using (var channel = 
              new TcpChannel<StringMessage>(                     // create a new channel
                  new TcpEndpointData(IPAddress.Loopback, 2000), // with localhost at port 2000
                  new StringMessageSerializer()))                // using StringMessageSerializer
          {
              channel.Open();

              // setup the requester
              var requester = new Requester<StringMessage>(channel);

              for (int i = 0; i < 100; i++)
              {
                  var requestMessage = new StringMessage("request from client"); // prepare the request message
                  var responseMessage = requester.Request(requestMessage);       // and send it
                  Console.WriteLine(responseMessage);
              }

              channel.Close();
          }
```
