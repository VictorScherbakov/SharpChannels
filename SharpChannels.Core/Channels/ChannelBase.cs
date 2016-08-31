using System;
using System.IO;
using SharpChannels.Core.Messages;
using SharpChannels.Core.Messages.System;
using SharpChannels.Core.Serialization;
using SharpChannels.Core.Serialization.System;

namespace SharpChannels.Core.Channels
{
    public abstract class ChannelBase : IChannel
    {
        private readonly object _readLock = new object();
        private readonly object _writeLock = new object();

        private bool _isHandShaken;

        protected abstract void CloseInternal();
        protected abstract Stream Stream { get; }

        protected int MaxMessageLength { get; set; }

        protected IMessageSerializer Serializer;

        protected void RequestHandshake()
        {
            ThrowIfHandshaken();

            var handshake = new Handshake();

            if (!handshake.Request(this))
            {
                CloseInternal();
                throw new ProtocolException("Invalid handshake response", ProtocolErrorCode.InvalidHandshakeResponse);
            }

            _isHandShaken = true;
        }

        protected void ResponseHandshake()
        {
            ThrowIfHandshaken();

            var handshake = new Handshake();

            if (!handshake.Response(this))
            {
                CloseInternal();
                throw new ProtocolException("Invalid handshake request", ProtocolErrorCode.InvalidHandshakeRequest);
            }

            _isHandShaken = true;
        }

        private void ThrowIfHandshaken()
        {
            if (_isHandShaken)
                throw new InvalidOperationException("Already handshaken");
        }

        public abstract IEndpointData EndpointData { get; }

        protected abstract void OpenTransport();

        public virtual void Open()
        {
            if (IsOpened)
                throw new InvalidOperationException("Already opened");

            OpenTransport();

            RequestHandshake();
        }

        public void Close()
        {
            Send(EndSessionMessage.Instance, new EndSessionSerializer());
            CloseInternal();
        }

        public abstract bool IsOpened { get; }

        public string Name { get; set; }

        public void Send(IMessage message)
        {
            Send(message, Serializer);
        }

        private void WriteBinaryMessage(IBinaryMessageData binaryMessage)
        {
            try
            {
                lock (_writeLock)
                {
                    BinaryMessageWriter.Write(Stream, binaryMessage);
                    Stream.Flush();
                }
            }
            catch (IOException ex)
            {
                ThrowChannelClosedIfNeeded(ex);

                CloseInternal();
                throw;
            }
        }

        public void Send(IMessage message, IMessageSerializer serializer)
        {
            if (!IsOpened)
                throw new InvalidOperationException();

            var binaryMessage = serializer.Serialize(message);
            WriteBinaryMessage(binaryMessage);
        }

        public IMessage Receive()
        {
            return Receive(Serializer);
        }

        private void ThrowChannelClosedIfNeeded(Exception ex)
        {
            if (!IsOpened)
                throw new DataTransferException("Message channel is unexpectedly closed by opposite side",
                    DataTransferErrorCode.ChannelClosed, ex);
        }

        private IBinaryMessageData ReadBinaryMessage()
        {
            try
            {
                lock (_readLock)
                {
                    return BinaryMessageReader.Read(Stream, MaxMessageLength);
                }
            }
            catch (IOException ex)
            {
                ThrowChannelClosedIfNeeded(ex);

                CloseInternal();
                throw;
            }
        }

        public IMessage Receive(IMessageSerializer serializer)
        {
            if (!IsOpened)
                throw new InvalidOperationException();

            var binaryMessage = ReadBinaryMessage();

            try
            {
                switch (binaryMessage.Type)
                {
                    case MessageType.EndSession:
                        new EndSessionSerializer().Deserialize(binaryMessage);
                        CloseInternal();
                        return null;

                    case MessageType.User:
                        if (!_isHandShaken)
                            throw new ProtocolException("Handshake required", ProtocolErrorCode.HandshakeRequired);
                        return serializer.Deserialize(binaryMessage);

                    case MessageType.HandshakeRequest:
                    case MessageType.HandshakeResponse:
                        if (_isHandShaken)
                            throw new ProtocolException("Unexpected message type",
                                ProtocolErrorCode.UnexpectedMessageType);

                        return serializer.Deserialize(binaryMessage);
                    default:
                        throw new ProtocolException("Unknown message type", ProtocolErrorCode.UnknownMessageType);
                }
            }
            catch
            {
                if (IsOpened)
                    CloseInternal();

                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

