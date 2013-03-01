using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace PaddleServer
{
    public class ConnectionListener
    {
        public delegate void OnClientConnected(TcpClient client);
        public event OnClientConnected ClientConnected;

        private TcpListener server;

        public ConnectionListener(string ipAddress, int port)
        {
            server = new TcpListener(new IPEndPoint(IPAddress.Parse(ipAddress), port));
        }

        public void Start()
        {
            server.Start();
            server.BeginAcceptTcpClient(TcpClientAccepted, null);
        }
        
        public void Stop()
        {
            server.Stop();
        }

        private void TcpClientAccepted(IAsyncResult result)
        {
            TcpClient client = server.EndAcceptTcpClient(result);
            client.NoDelay = true;
            if (ClientConnected != null)
            {
                Trace.Write("Client accepted");
                ClientConnected(client);
            }
            else
            {
                Trace.Write("Client accepted but flushed since no callbacks");
                client.Close();
            }
            

            server.BeginAcceptTcpClient(TcpClientAccepted, null);
        }
    }
}
