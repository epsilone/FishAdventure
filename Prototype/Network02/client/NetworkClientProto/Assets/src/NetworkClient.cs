using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Thrift.Protocol;
using System.IO;
using Funcom.Lego.Thrift.Generated;
using System.Text;

public class NetworkClient : MonoBehaviour
{
    # region Constants

    private enum NetworkState
    {
        Idle,
        Processing,
        Sending,
        Receiving,
    }

    #endregion

    #region Delegates

    public delegate void OnReceived(byte[] raw);
    public event OnReceived Received;

    #endregion

    private TcpConnection Connection { get; set; }
    private NetworkState State { get; set; }
    private List<KeyValuePair<MessageId, TBase>> SendQueue { get; set; }
    private Stream Stream { get; set; }

    void Start()
    {
        State = NetworkState.Idle;
        SendQueue = new List<KeyValuePair<MessageId, TBase>>();
        
        Stream = new MemoryStream();

        Connection = new TcpConnection();
        Connection.Connected += OnConnected;
        Connection.DataReceived += OnDataReceived;
        Connection.DataSent += OnDataSent;
        StartCoroutine(SendReceive());
    }

    void OnDisable()
    {
        if (Connection != null)
        {
            Connection.Connected -= OnConnected;
            Connection.DataReceived -= OnDataReceived;
            Connection.DataSent -= OnDataSent;
            Connection.Disconnect();
        }

        if (Stream != null)
        {
            Stream.Close();
        }
    }

    public void Connect(string host, int port)
    {
        Connection.Connect(host, port);
    }

    public void Stop()
    {
        StopCoroutine("SendReceive");
    }

    private void OnConnected()
    {
        Debug.Log("Connected!" + Connection.IsConnected());
    }

    public void Add(MessageId id, TBase message)
    {
        Debug.Log("Adding MessageId: " + id);
        SendQueue.Add(new KeyValuePair<MessageId, TBase>(id, message));
    }

    private byte[] BuildActions()
    {
        var sb = new StringBuilder();
        sb.Append("SendQueue: ");
        SendQueue.ForEach(x => sb.Append(x.Key).Append(" ").Append(x.Value));
        Debug.Log(sb.ToString());
        var actions = new ClientActions();
        actions.PlayerId = PlayerPrefs.GetString(Constants.PLAYER_ID);
        actions.Actions = ThriftUtils.CustomSerialize(SendQueue);
        return ThriftUtils.Serialize(actions);
    }

    // The State might be useful when doing manual SendReceive
    private IEnumerator SendReceive()
    {
        while (!Connection.IsConnected())
        {
            yield return null;
        }

        while (true)
        {
            Debug.Log("SendReceive Begin");
            State = NetworkState.Processing;
            if (SendQueue.Count > 0)
            {
                byte[] raw = BuildActions();
                SendQueue.Clear();
                Connection.Write(raw);
                State = NetworkState.Sending;
                while (State == NetworkState.Sending)
                {
                    yield return null;
                }
            }
            yield return null;
            bool receiving = Connection.Read();
            if (receiving)
            {
                State = NetworkState.Receiving;
                while (State == NetworkState.Receiving)
                {
                    yield return null;
                }
            }

            State = NetworkState.Idle;
            Debug.Log("SendReceive End");
            yield return null;
        }
    }

    private void OnDataSent()
    {
        State = NetworkState.Processing;
    }

    private void OnDataReceived(byte[] raw)
    {
        State = NetworkState.Processing;
        if (Received != null)
        {
            Received(raw);
        }
    }
}
