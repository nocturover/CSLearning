using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

    class IOCPClient : IOCP, IDisposable
    {
        bool disposing = false;

        Socket client;
        string clientName;
        //private static char CR = (char)0x0D;
        //private static char LF = (char)0x0A;

        private byte[] buffer = new byte[1024];
        private StringBuilder sb = new StringBuilder();

        public delegate void PrintEventHandler(string message);
        public event PrintEventHandler OnPrintEvent;
        public delegate void ConnectionEventHandler(ConnectionChangedEventArgs e, SocketError? socketError);
        public event ConnectionEventHandler OnConnectionChangeEvent;

        public string ClientName { get => clientName; }
        public ConnectionInfoArgs ConnInfo
        {
            get
            {
                return new ConnectionInfoArgs(this.client.Connected, (IPEndPoint)this.client.RemoteEndPoint);
            }
        }
        public void Connect(string ip, int port)
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), Callback_Connect, this);
        }
        private void Callback_Connect(IAsyncResult result)
        {
            try
            {
                clientName = "";
                client.EndConnect(result);
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Callback_Receive, this);
                OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.CONNECT, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), null);

            }
            catch (SocketException e)
            {
                if (((SocketError)e.ErrorCode) == SocketError.ConnectionRefused)    // server 연결 거부(닫힘 상태 등)
                {
                    OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.ERROR, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), (SocketError)e.ErrorCode);
                    client.Close();
                    client.Dispose();
                    return;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(((SocketError)e.ErrorCode).ToString() + Environment.NewLine
                            + e.Message, "Callback_Connect 기타에러");
                }
            }
        }
        private void Callback_Receive(IAsyncResult result)
        {
            if (client.Connected)
            {
                try
                {
                    int size = client.EndReceive(result);
                    if (size != 0)
                    {
                        sb.Append(Encoding.ASCII.GetString(buffer, 0, size));
                        sb.Length = sb.Length - 1;
                        string msg = sb.ToString();
                        OnPrintEvent(msg);

                        if ("/EXIT".Equals(msg, StringComparison.OrdinalIgnoreCase))
                        {
                            OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.CLOSE, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), null);
                            this.client.Close();
                            this.Dispose();
                            return;
                        }
                        sb.Clear();
                        client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Callback_Receive, this);
                    }
                }
                catch (SocketException e)
                {
                    if (((SocketError)e.ErrorCode) == SocketError.ConnectionReset)   // server에 의한 끊어짐
                    {
                        OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.ERROR, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), (SocketError)e.ErrorCode);
                        client.Close();
                        client.Dispose();
                        return;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show(((SocketError)e.ErrorCode).ToString() + Environment.NewLine
                            +e.Message, "CallBack_Receive 기타에러");
                    }
                }

            }
        }
        public void Send(string msg)
        {
            if (client.Connected)
            {
                byte[] data = Encoding.ASCII.GetBytes(msg + ';');
                client.Send(data, data.Length, SocketFlags.None);

                if (msg.StartsWith("/UN", StringComparison.OrdinalIgnoreCase))
                {
                    this.clientName = msg.Split(' ')[^1].ToString();
                    OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.CONNECT_SETNAME, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), null);
                }
                if (msg.Equals("/EXIT", StringComparison.OrdinalIgnoreCase))
                {
                    OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.CLOSE, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), null);
                    this.client.Close();
                    this.Dispose();
                    return;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposed)
        {
            if (!disposing)
            {
                if (disposed)
                {
                    client.Close();
                    client.Dispose();
                }
                sb.Clear();
                buffer = null;
                disposing = true;
            }
        }
    }
