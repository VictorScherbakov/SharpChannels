using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SharpChannels.Core.Channels.Intradomain
{
    internal class IntradomainConnectionManager
    {
        private static IntradomainConnectionManager _instance;

        private static readonly object _locker = new object();
        private readonly Dictionary<string, IntradomainConnetcion> _connections;
        private readonly Dictionary<string, Func<IntradomainSocket, IntradomainSocket>> _listeners;

        private AutoResetEvent _connectionCreated = new AutoResetEvent(false);

        /// <summary>
        /// Resets Instance. For testing purpose only!
        /// </summary>
        internal static void Reset()
        {
            _instance = null;
        }

        public static IntradomainConnectionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                            _instance = new IntradomainConnectionManager();
                    }
                }

                return _instance;
            }
        }

        public string[] Listeners
        {
            get
            {
                lock (_locker)
                {
                    return _listeners.Keys.ToArray();
                }
            }
        }

        public bool IsListening(string name)
        {
            lock (_locker)
            {
                return _listeners.ContainsKey(name);
            }
        }

        public bool Listen(string name, Func<IntradomainSocket, IntradomainSocket> acceptClient)
        {
            if (acceptClient == null)
                throw new ArgumentNullException(nameof(acceptClient));

            if (string.IsNullOrEmpty(name))
                return false;


            lock (_locker)
            {
                if (_listeners.ContainsKey(name))
                    return false;

                _listeners.Add(name, acceptClient);
                return true;
            }
        }

        public void StopListening(string name)
        {
            lock (_locker)
            {
                if (_listeners.ContainsKey(name))
                    _listeners.Remove(name);
            }
        }

        public bool Connect(IntradomainSocket socket, IntradomainConnectionSettings connectionSettings = null)
        {
            if (string.IsNullOrEmpty(socket.Hub))
                return false;

            if (socket.Type != SocketType.Client)
                throw new ArgumentException(nameof(socket));

            Func<IntradomainSocket, IntradomainSocket> getSocketData;
            lock(_locker)
            {
                if (!_listeners.ContainsKey(socket.Hub))
                    return false;

                getSocketData = _listeners[socket.Hub];
            }
            socket.ConnectionId = Guid.NewGuid().ToString();
            var remoteSocket = getSocketData(socket);
            if (remoteSocket == null)
                return false;

            lock (_locker)
            {
                var connection = new IntradomainConnetcion(remoteSocket, socket, connectionSettings ?? IntradomainConnectionSettings.GetDefault())
                {
                    ClientSocketState = IntradomainSocketState.Connected
                };

                _connections.Add(socket.ConnectionId, connection);
                _connectionCreated.Set();
                return true;
            }
        }

        public void Disconnect(IntradomainSocket socket)
        {
            if (string.IsNullOrEmpty(socket.ConnectionId))
                return;

            if(socket.Type == SocketType.Listener)
                throw new ArgumentException(nameof(socket));

            lock (_locker)
            {
                if (!_connections.ContainsKey(socket.ConnectionId))
                    return;

                var connection = _connections[socket.ConnectionId];

                if (socket.Type == SocketType.Client)
                    connection.DisconnectClient();

                if (socket.Type == SocketType.Server)
                    connection.DisconnectServer();

                if (connection.IsCompletelyDisconnected())
                    _connections.Remove(socket.ConnectionId);
            }
        }

        public bool Connected(IntradomainSocket socket)
        {
            if (string.IsNullOrEmpty(socket.ConnectionId))
                return false;

            if (socket.Type == SocketType.Listener)
                throw new ArgumentException(nameof(socket));

            lock (_locker)
            {
                if (!_connections.ContainsKey(socket.ConnectionId))
                    return false;

                return socket.Type == SocketType.Client
                    ? _connections[socket.ConnectionId].ClientSocketState == IntradomainSocketState.Connected
                    : _connections[socket.ConnectionId].ServerSocketState == IntradomainSocketState.Connected;
            }
        }

        public void WaitForConnection(string id)
        {
            while (!_connections.ContainsKey(id))
            {
                _connectionCreated.WaitOne();
            }
        }

        public void WaitForConnection(string id, int interval)
        {
            var start = DateTime.Now;
            while (!_connections.ContainsKey(id) && interval > 0)
            {
                _connectionCreated.WaitOne(interval);
                interval -= (int)(DateTime.Now - start).TotalMilliseconds;
            }
        }

        public Stream GetStream(IntradomainSocket socket)
        {
            lock (_locker)
            {
                if (_connections.ContainsKey(socket.ConnectionId))
                {
                    return _connections[socket.ConnectionId].GetStream(socket);
                }
                throw new ArgumentException(nameof(socket));
            }
        }

        public IntradomainConnectionManager()
        {
            _listeners = new Dictionary<string, Func<IntradomainSocket, IntradomainSocket>>();
            _connections = new Dictionary<string, IntradomainConnetcion>();
        }
    }
}