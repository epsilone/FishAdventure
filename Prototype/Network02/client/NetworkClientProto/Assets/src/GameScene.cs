using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Funcom.Lego.Thrift.Generated;
using System.IO;
using Thrift.Transport;
using Thrift.Protocol;
using System;
using System.Text;

public class GameScene : MonoBehaviour 
{
    #region Constants

    private const string HOST = "10.9.42.223";
    private const int PORT = 8123;

    #endregion

    public GameObject prefab;

    public GameState GameState { get; private set; }
    private Dictionary<Vector3, GameObject> bricks;
    private NetworkClient client;
    private string Notification { get; set; }

    // Player information
    private string PlayerId { get; set; }
    private BrickColor PlayerColor { get; set; }
    
    void Start()
    {
        Notification = GameApp.Instance.MessageId.ToString();
        GameState = new GameState();
        bricks = new Dictionary<Vector3, GameObject>();
        client = GameObject.Find("NetworkClient").GetComponent<NetworkClient>();
        client.Received += OnNetworkResponse;
        client.Connect(HOST, PORT);
        client.Add(GameApp.Instance.MessageId, GameApp.Instance.Message);
        PlayerId = PlayerPrefs.GetString(Constants.PLAYER_ID);
    }

    void OnDisable()
    {
        if (client != null)
        {
            client.Received -= OnNetworkResponse;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var leftMouseButtonUp = Input.GetMouseButtonUp(0);
        if (leftMouseButtonUp)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("clicked at (x=" + mousePosition.x + ",y=" + mousePosition.y + ",z=" + mousePosition.z + ")");
            //Debug.Log("" + GameState.GetBrick((int)mousePosition.x, (int)mousePosition.y));
            var position = new Vector3((int)mousePosition.x, (int)mousePosition.y, 0);
            GameState.SetBrick((int)position.x, (int)position.y, PlayerColor);
            GameObject go;
            if (bricks.TryGetValue(position, out go))
            {
                go.renderer.material.color = Convert(PlayerColor);

                var message = new BrickUpdate();
                message.Row = (int)position.x;
                message.Column = (int)position.y;
                message.Color = PlayerColor;

                client.Add(MessageId.BRICK_UPDATE, message);
            }
        }

        if (GameState.Dirty)
        {
            if (bricks.Count == 0)
            {
                for (int i = 0; i < GameState.Rows; ++i)
                {
                    for (int j = 0; j < GameState.Columns; ++j)
                    {
                        var position = new Vector3(i, j, 0);
                        var go = Instantiate(prefab, position, Quaternion.identity) as GameObject;
                        go.renderer.material.color = Convert(GameState.GetBrick(i, j));
                        bricks[position] = go;
                    }
                }
            }
            else
            {
                for (int i = 0; i < GameState.Rows; ++i)
                {
                    for (int j = 0; j < GameState.Columns; ++j)
                    {
                        var position = new Vector3(i, j, 0);
                        GameObject go;
                        if (bricks.TryGetValue(position, out go))
                        {
                            go.renderer.material.color = Convert(GameState.GetBrick(i, j));
                        }
                    }
                }
            }

            GameState.Dirty = false;
        }
    }

    void OnGUI()
    {
        if (Notification.Length > 0)
        {
            GUILayout.Label(Notification);
        }
    }

    private static Color Convert(BrickColor color)
    {
        switch (color)
        {
            case BrickColor.RED: return Color.red;
            case BrickColor.BLUE: return Color.blue;
            case BrickColor.GREEN: return Color.green;
            case BrickColor.YELLOW: return Color.yellow;
            default: return Color.white;
        }
    }

    public void OnWorldInit()
    {
        Debug.Log("OnWorldInit");
        for (int i = 0; i < GameState.Rows; ++i)
        {
            for (int j = 0; j < GameState.Columns; ++j)
            {
                var position = new Vector3(i, j, 0);
                var go = Instantiate(prefab, position, Quaternion.identity) as GameObject;
                bricks[position] = go;
            }
        }
    }

    public void OnBrickUpdated(int x, int y, BrickColor color)
    {
        var position = new Vector3(x, y, 0);
        GameObject go;
        if (bricks.TryGetValue(position, out go))
        {
            go.renderer.material.color = Convert(PlayerColor);
        }
    }

    private void OnNetworkResponse(byte[] raw)
    {
        Debug.Log("Data received" + BitConverter.ToString(raw).Replace("-", string.Empty));
        using (var stream = new MemoryStream(raw))
        {
            var transport = new TStreamTransport(stream, null);
            var protocol = new TBinaryProtocol(transport);
            int count = protocol.ReadI32();
            MessageId[] messageIds = new MessageId[count];
            for (int i = 0; i < count; ++i)
            {
                messageIds[i] = (MessageId)protocol.ReadByte();
            }

            for (int i = 0; i < count; ++i)
            {
                var messageId = messageIds[i];
                Debug.Log ("Processing message: " + messageId);
                switch (messageId)
                {
                    case MessageId.PLAYER_CONNECTED:
                        var playerConnected = new PlayerConnected();
                        playerConnected.Read(protocol);
                        Notification = "Player connected: " + playerConnected.Name;
                        break;

                    case MessageId.GAME_JOINED:
                        var gameJoined = new GameJoined();
                        gameJoined.Read(protocol);
                        var sb = new StringBuilder();
                        gameJoined.Players.ForEach(x => sb.Append(x.PlayerId).Append(" ").Append(x.Color));
                        Debug.Log(sb.ToString());
                        PlayerColor = gameJoined.Players.Find(x => x.PlayerId.Equals(PlayerId)).Color;
                        GameState.WorldInfo = gameJoined.World;
                        GameState.Dirty = true;
                        break;

                    case MessageId.BRICK_UPDATE:
                        var brickUpdate = new BrickUpdate();
                        brickUpdate.Read(protocol);
                        GameState.UpdateBrick(brickUpdate.Row, brickUpdate.Column, brickUpdate.Color);
                        break;

                    case MessageId.PLAYER_DISCONNECTED:
                        var playerDisconnect = new PlayerDisconnect();
                        playerDisconnect.Read(protocol);
                        Notification = "Player " + playerDisconnect.PlayerId + " left the game.";
                        break;

                    default:
                        Debug.LogError("Unexpected type from server: " + messageId);
                        break;
                }
            }
        }
    }
}
