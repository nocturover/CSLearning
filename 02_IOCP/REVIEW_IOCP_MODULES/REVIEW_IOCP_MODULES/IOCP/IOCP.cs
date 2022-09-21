using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

    enum SocketConnection
    {
        ERROR = -1,
        CLOSE = 0,
        CONNECT = 1,
        CONNECT_SETNAME = 2,
    }
    interface IOCP
    {
        public void Send(string msg);
    }
    class ConnectionInfoArgs
    {
        public bool IsConnected { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public ConnectionInfoArgs(bool IsConnected, IPEndPoint RemoteEndPoint)
        {
            this.IsConnected = IsConnected;
            this.RemoteEndPoint = RemoteEndPoint;
        }
    }
    class ConnectionChangedEventArgs
    {
        public SocketConnection ConnState { get; private set; }
        public string ClientName { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }

        public ConnectionChangedEventArgs(SocketConnection connState, string clientName, IPEndPoint LocalEndPoint, IPEndPoint RemoteEndPoint)
        {
            this.ConnState = connState;
            this.ClientName = clientName;
            this.LocalEndPoint = LocalEndPoint;
            this.RemoteEndPoint = RemoteEndPoint;
        }
    }
