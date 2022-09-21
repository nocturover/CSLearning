using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


class IOCPServer : IOCP, IDisposable
{
    bool disposing = false;

    private Socket client;
    private string clientName;
    private byte[] buffer = new byte[1024];
    private StringBuilder sb = new StringBuilder();

    //private static char CR = (char)0x0D;
    //private static char LF = (char)0x0A;

    public delegate void PrintEventHandler(string clientName, string message);
    public delegate void ConnectionEventHandler(ConnectionChangedEventArgs e, SocketError? socketError);

    public event PrintEventHandler OnPrintEvent;
    public event ConnectionEventHandler OnConnectionChangeEvent;

    public string ClientName
    {
        get => clientName;
    }
    public IPEndPoint remoteEndPoint
    {
        get => (IPEndPoint)client.RemoteEndPoint;
    }

    public IOCPServer()
    {
    }
    public void BeginReceive(Socket client)
    {
        this.client = client;
        this.client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Callback_Receive, this);
        clientName = "";

        OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.CONNECT, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), null);
        //Send("Input user name : ");
    }
    private void Callback_Receive(IAsyncResult result)
    {
        if (this.client.Connected)
        {
            int size;
            try
            {
                size = this.client.EndReceive(result);

                sb.Append(Encoding.ASCII.GetString(buffer, 0, size));
                if (sb.Length >= 2 && sb[sb.Length - 1] == ';')
                {
                    sb.Length = sb.Length - 1;
                    string msg = sb.ToString();
                    OnPrintEvent(clientName, msg);

                    if (msg.StartsWith("/UN", StringComparison.OrdinalIgnoreCase))
                    {
                        clientName = msg.Split(' ')[^1].ToString();
                        OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.CONNECT_SETNAME, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), null);
                    }
                    if ("/EXIT".Equals(msg, StringComparison.OrdinalIgnoreCase))
                    {
                        OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.CLOSE, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), null);
                        this.client.Close();
                        this.Dispose();
                        return;
                    }
                    sb.Clear();
                }
                this.client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Callback_Receive, this);
            }
            catch (SocketException e)
            {
                if ((SocketError)e.ErrorCode == SocketError.ConnectionReset)
                {
                    OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.ERROR, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), (SocketError)e.SocketErrorCode);
                    client.Close();
                }
                else
                {
                    OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.ERROR, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), (SocketError)e.SocketErrorCode);
                    client.Close();
                    System.Windows.Forms.MessageBox.Show(((SocketError)e.ErrorCode).ToString() + Environment.NewLine
                        + e.Message, "Server 기타에러");
                }
                return;
            }
        }
    }
    public void Send(string msg)
    {
        if (client.Connected)
        {
            byte[] data = Encoding.ASCII.GetBytes(msg + ";");
            client.Send(data, data.Length, SocketFlags.None);
            if (msg.Equals("/EXIT", StringComparison.OrdinalIgnoreCase))
            {
                OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.CLOSE, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), null);
                this.client.Close();
                this.Dispose();
                return;
            }
        }
    }

    /// <summary>
    /// 호스트가 클라이언트를 끊음.
    /// </summary>
    public void CloseEvent()
    {
        OnConnectionChangeEvent(new ConnectionChangedEventArgs(SocketConnection.CLOSE, clientName, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint), null);
    }
    public void Close()
    {
        client.Close();
        client.Dispose();
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
