#define DEBUG

using System;
using System.Diagnostics;
using System.Net.Sockets;

public class ClientConnection
{
    # region Constants

    private static readonly byte[] EMPTY_BUFFER = new byte[0];

    #endregion

    #region Delegates

    public delegate void OnDataReceived(byte[] raw);
    public event OnDataReceived DataReceived;

    public delegate void OnDataSent();
    public event OnDataSent DataSent;

    public delegate void OnConnected();
    public event OnConnected Connected;

    #endregion
    
    private TcpClient client;

    public ClientConnection() 
    {
        client = new TcpClient(AddressFamily.InterNetwork);
    }

    public ClientConnection(TcpClient client)
    {
        this.client = client;
    }

    # region Connection

    public bool IsConnected()
    {
        return client.Connected;
    }

    public void Connect(string host, int port)
    {
        Trace.Write("Connection Begin");
        client.BeginConnect(host, port, ConnectCallback, null);
    }

    private void ConnectCallback(IAsyncResult result)
    {
        Trace.Write("Connection Done");
        client.EndConnect(result);
        if (Connected != null)
        {
            Connected();
        }
    }

    public void Write(byte[] raw)
    {
        Trace.Write("Write Begin " + BitConverter.ToString(raw).Replace("-", string.Empty));
        NetworkStream stream = client.GetStream();
        stream.BeginWrite(raw, 0, raw.Length, WriteCallback, null);
    }

    private void WriteCallback(IAsyncResult result)
    {
        Trace.Write("Write Done");
        NetworkStream stream = client.GetStream();
        stream.EndWrite(result);
        if (DataSent != null)
        {
            DataSent();
        }
    }

    public bool Read()
    {
        Trace.Write("Read Begin");
        Trace.Write("Availabe value is " + client.Available);
        bool canRead = client.Available > 0;
        if (canRead)
        {
            NetworkStream stream = client.GetStream();
            var raw = new byte[client.ReceiveBufferSize];
            stream.BeginRead(raw, 0, raw.Length, ReadCallback, raw);
        }

        return canRead;
    }

    private void ReadCallback(IAsyncResult result)
    {
        Trace.Write("Read Done");
        NetworkStream stream = client.GetStream();
        int read = stream.EndRead(result);

        byte[] raw = EMPTY_BUFFER;
        if (read != 0)
        {
            raw = result.AsyncState as byte[];
        }

        if (DataReceived != null)
        {
            DataReceived(raw);
        }
    }

    public void Disconnect()
    {
        Trace.Write("Disconnecting");
        client.Close();
    }

    #endregion
}
