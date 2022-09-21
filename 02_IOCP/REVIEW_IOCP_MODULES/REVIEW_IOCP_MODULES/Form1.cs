using System.Net.Sockets;

namespace REVIEW_IOCP_MODULES
{
    public partial class ServerForm : Form
    {
        public ServerForm()
        {
            InitializeComponent();
            InitializeIOCPServer();
        }


        private void InitializeIOCPServer()
        {
            IOCPServer server = new();
            server.OnPrintEvent += IOCPClient_OnPrint;
            server.OnConnectionChangeEvent += IOCPServer_OnConnectionChanged;
            
        }

        private void IOCPClient_OnPrint(string clientName, string message)
        {
            if (!string.IsNullOrEmpty(clientName))
            {
                txtMessage.AppendText($"{clientName} - {message}");
            }
            else
            {
                txtMessage.AppendText($"Anonymous - {message}");
            }
        }
        private void IOCPServer_OnConnectionChanged(ConnectionChangedEventArgs e, SocketError? socketError)
        {
            txtLog.AppendText($"{e.RemoteEndPoint.Address}:{e.RemoteEndPoint.Port} Connected");
            if (!string.IsNullOrEmpty(e.ClientName))
            {
                txtLog.AppendText($"\t >> Called by '{e.ClientName}'");
            }
        }
    }
}