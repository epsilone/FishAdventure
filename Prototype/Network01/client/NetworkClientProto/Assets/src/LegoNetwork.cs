using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Text;
using com.funcom.lego.generated;
using Thrift.Protocol;
using System.IO;
using Thrift.Transport;

public class LegoNetwork : MonoBehaviour
{
    # region Constants

    private const string HOST = "10.9.42.223";
    private const int PORT = 8123;

    private enum NetworkState
    {
        Disconnected,
        Connected,
        Reading,
        Read,
        Writing,
        Wrote,
    }

    #endregion

    private TcpClient client;
    private NetworkDataHandler handler;

    private NetworkState State { get; set; }
    private List<KeyValuePair<MessageId, TBase>> SendQueue { get; set; }
    private List<KeyValuePair<MessageId, TBase>> ReceivedQueue { get; set; }

    private GameState gameState;

    // Use this for initialization
    void Start() 
    {
        gameState = GameObject.Find("Player").GetComponent<PlayerScript>().GameState;
        if (gameState == null)
        {
            Debug.LogError("GameState is null!");
        }
        handler = new NetworkDataHandler(gameState);
        SendQueue = new List<KeyValuePair<MessageId, TBase>>();
        ReceivedQueue = new List<KeyValuePair<MessageId, TBase>>();
        State = NetworkState.Disconnected;
        client = new TcpClient(AddressFamily.InterNetwork);
        Connect();
        StartCoroutine(SendReceive());
    }

    public void Add(MessageId id, TBase message)
    {
        SendQueue.Add(new KeyValuePair<MessageId, TBase>(id, message));
    }

    private IEnumerator SendReceive()
    {
        while (true)
        {
            if (State == NetworkState.Connected)
            {
                var playerConnect = new PlayerConnect();
                playerConnect.Id = gameState.Id;
                playerConnect.Name = gameState.Name;
                Add(MessageId.PLAYER_CONNECT, playerConnect);
                Debug.Log("Added connect");
                yield return null;

                while (true)
                {
                    Debug.Log("Internal loop");
                    if (SendQueue.Count != 0)
                    {
                        Debug.Log("Flushing queue");
                        SendPlayerActions();
                        while (State != NetworkState.Wrote)
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        Debug.Log("Nothing to send");
                        yield return null;
                    }

                    Read();
                    yield return null;

                    while (State != NetworkState.Read)
                    {
                        Debug.Log("Waiting for read over");
                        yield return null;
                    }
                }
            }
        }
    }

    private void SendPlayerActions()
    {
        var playerActions = new ClientActions();
        playerActions.PlayerId = gameState.Id;
        playerActions.Actions = ThriftUtils.CustomSerialize(SendQueue);
        SendQueue.Clear();
        Write(ThriftUtils.Serialize(playerActions));
    }

    # region Connection
    public void Connect() 
    {
        Debug.Log("Connection Begin");
        client.BeginConnect(HOST, PORT, ConnectCallback, null);
    }

    private void ConnectCallback(IAsyncResult result)
    {
        Debug.Log("Connection Done");
        client.EndConnect(result);
        State = NetworkState.Connected;
    }

    public void Write(byte[] raw)
    {
        State = NetworkState.Writing;
        //Debug.Log("Write Begin " + BitConverter.ToString(raw).Replace("-", string.Empty));
        NetworkStream stream = client.GetStream();
        stream.BeginWrite(raw, 0, raw.Length, WriteCallback, null);
    }

    private void WriteCallback(IAsyncResult result)
    {
        Debug.Log("Write Done");
        NetworkStream stream = client.GetStream();
        stream.EndWrite(result);
        State = NetworkState.Wrote;
    }

    public void Read()
    {
        Debug.Log("Read Begin");
        Debug.Log("Availabe value is " + client.Available);
        if (client.Available > 0)
        {
            State = NetworkState.Reading;
            NetworkStream stream = client.GetStream();
            var raw = new byte[client.ReceiveBufferSize];
            stream.BeginRead(raw, 0, raw.Length, ReadCallback, raw);
        }
        else
        {
            State = NetworkState.Read;
        }
    }

    private void ReadCallback(IAsyncResult result)
    {
        Debug.Log("Read Done");
        NetworkStream stream = client.GetStream();
        int read = stream.EndRead(result);
        State = NetworkState.Read;

        if (read == 0)
        {
            Debug.Log("Nothing read");
            return;
        }

        var raw = result.AsyncState as byte[];
        handler.Handle(raw);
    }
    #endregion
}
