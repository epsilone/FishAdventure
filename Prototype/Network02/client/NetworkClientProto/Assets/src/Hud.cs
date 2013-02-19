using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Funcom.Lego.Thrift.Generated;
using Thrift.Protocol;
using System.IO;
using Thrift.Transport;
using System.Linq;

public class Hud : MonoBehaviour
{
    #region Constants

    private const string HOST = "10.9.42.223";
    private const int PORT = 8123;

    private enum ButtonAction
    {
        None = -1,
        Refresh = 0,
        Join = 1,
        Create = 2
    }

    private static readonly string[] entries = new string[] { "", "", "", "", "", "", "", "", "", "" };
    private static readonly string[] buttons = new string[] { "Refresh", "Join", "Create" };

    #endregion
    
    private int gameSelected;
    private ButtonAction buttonClicked;
    private TcpConnection connection;
    private bool connected;
    private bool processing;
    private List<NetGameInfo> AvailableGames { get; set; }

    void Start() 
    {
        PlayerPrefs.SetString(Constants.PLAYER_ID, Guid.NewGuid().ToString());
        PlayerPrefs.SetString(Constants.PLAYER_NAME, "Philippe");
        AvailableGames = new List<NetGameInfo>();
        connected = false;
        gameSelected = 0;
        buttonClicked = ButtonAction.None;
        connection = new TcpConnection();
        connection.Connected += OnConnected;
        connection.DataReceived += OnDataReceived;
        connection.DataSent += OnDataSent;
        connection.Connect(HOST, PORT);
    }

    void OnDisable()
    {
        if (connection != null)
        {
            connection.Disconnect();
            connection.DataReceived -= OnDataReceived;
            connection.DataSent -= OnDataSent;
        }
    }
    
    void Update() 
    {
        switch (buttonClicked)
        {
            case ButtonAction.Refresh:
                StartCoroutine(Refresh());
                break;
            case ButtonAction.Join:
                GameApp.Instance.MessageId = MessageId.JOIN_GAME;
                var joinGame = new JoinGame();
                joinGame.GameId = AvailableGames[gameSelected].GameId;
                joinGame.PlayerName = PlayerPrefs.GetString(Constants.PLAYER_NAME);
                GameApp.Instance.Message = joinGame;
                GameApp.Instance.doTransition(GameApp.Scene.GameJoin);
                break;
            case ButtonAction.Create:
                GameApp.Instance.doTransition(GameApp.Scene.GameCreate);
                break;
            default:
                break;
        }

        buttonClicked = ButtonAction.None;
    }

    void OnGUI()
    {
        if (!connected)
        {
            GUILayout.Label("Connecting...");
        }
        else if (AvailableGames.Count == 0)
        {
            GUILayout.Label("No game found!");
            buttonClicked = (ButtonAction)GUILayout.Toolbar((int)buttonClicked, buttons);
        }
        else
        {
            GUILayout.BeginHorizontal("");
            GUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(Screen.width / 2), GUILayout.Height(Screen.height / 2));
            gameSelected = GUILayout.SelectionGrid(gameSelected, AvailableGames.Select(x => x.Name).ToArray(), 1);
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(Screen.width / 2), GUILayout.Height(Screen.height / 2));
            GUILayout.Label(AvailableGames[gameSelected].ToString());
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            buttonClicked = (ButtonAction)GUILayout.Toolbar((int)buttonClicked, buttons);
        }
    }

    private void OnConnected()
    {
        connection.Connected -= OnConnected;
        connected = true;
    }

    private IEnumerator Refresh()
    {
        var queue = new List<KeyValuePair<MessageId, TBase>>();
        var message = new RetrieveGames();
        message.PlayerId = PlayerPrefs.GetString(Constants.PLAYER_ID);
        message.Full = true;
        queue.Add(new KeyValuePair<MessageId, TBase>(MessageId.RETRIEVE_GAMES, message));
        var actions = new ClientActions();
        actions.PlayerId = PlayerPrefs.GetString(Constants.PLAYER_ID);
        actions.Actions = ThriftUtils.CustomSerialize(queue);
        byte[] raw = ThriftUtils.Serialize(actions);

        processing = true;
        connection.Write(raw);
        while(processing)
        {
            yield return null;
        }

        processing = true;
        connection.Read();
        while (processing)
        {
            yield return null;
        }

        yield break;
    }

    private void OnDataReceived(byte[] raw)
    {
        processing = false;
        using (var stream = new MemoryStream(raw))
        {
            var transport = new TStreamTransport(stream, null);
            var protocol = new TBinaryProtocol(transport);
            int count = protocol.ReadI32();
            if (count != 1)
            {
                Debug.LogError("Unexpected amount of messages!");
            }

            MessageId messageId = (MessageId)protocol.ReadByte();
            if (messageId != MessageId.AVAILABLE_GAMES)
            {
                Debug.LogError("Unexpected message id!");
            }

            var games = new NetGames();
            games.Read(protocol);
            AvailableGames = games.GameInfos;
        }
    }
    
    private void OnDataSent()
    {
        processing = false;
    }
}
