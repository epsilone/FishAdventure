using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Paddle;
using XnaGeometry;
using System.Net.Sockets;
using PaddleTransport;
using PaddleThrift;
using System.IO;
using Thrift.Transport;
using Thrift.Protocol;
using System.Net;

public class PongGame : MonoBehaviour
{
    #region

    private static Matrix MODEL_VIEW = Matrix.CreateScale(0.080, 0.080, 1.0);
    private static Matrix MODEL_VIEW_SCALE = Matrix.CreateScale(0.160, 0.160, 1.0);

    #endregion

    private Transform LeftPaddle { get; set; }
    private Transform RightPaddle { get; set; }
    private Transform Ball { get; set; }

    private Pong Game { get; set; }
    private int PlayerId { get; set; }

    #region Network

    private enum NetGameState
    {
        Offline,
        Connected,
        Started,
    }

    private static readonly string PLAYER_ONE = "PlayerOne";
    private static readonly string PLAYER_TWO = "PlayerTwo";

    private NetGameState State { get; set; }
    private TcpClient Client { get; set; }
    private MessageQueue Queue { get; set; }
    private bool Move { get; set; } // HACK

    #endregion

    void Start()
    {
        DebugConsole.Log("Hello World!");
        Game = new Pong();
        Game.PlayerOne = new LocalPlayer();
        Game.PlayerTwo = new LocalPlayer();
        PlayerId = 1;

        ApplyModelViewPosition(Game.BottomBorder.Position, GameObject.Find("BottomBorder").transform);
        ApplyModelViewScale(Game.BottomBorder.Scale, GameObject.Find("BottomBorder").transform);

        ApplyModelViewPosition(Game.TopBorder.Position, GameObject.Find("TopBorder").transform);
        ApplyModelViewScale(Game.TopBorder.Scale, GameObject.Find("TopBorder").transform);

        LeftPaddle = GameObject.Find("LeftPaddle").transform;
        ApplyModelViewPosition(Game.LeftPaddle.Position, LeftPaddle.transform);
        ApplyModelViewScale(Game.LeftPaddle.Scale, LeftPaddle.transform);

        RightPaddle = GameObject.Find("RightPaddle").transform;
        ApplyModelViewPosition(Game.RightPaddle.Position, RightPaddle.transform);
        ApplyModelViewScale(Game.RightPaddle.Scale, RightPaddle.transform);

        Ball = GameObject.Find("Ball").transform;
        ApplyModelViewPosition(Game.Ball.Position, Ball.transform);
        ApplyModelViewScale(Game.Ball.Scale, Ball.transform);


        // Network
        State = NetGameState.Offline;
        Client = new TcpClient(AddressFamily.InterNetwork);
        Client.NoDelay = true;
        Queue = new MessageQueue();
        Connect();
    }

    void Connect()
    {
        Client.Connect(new IPEndPoint(IPAddress.Parse("10.9.42.223"), 8123));
        State = NetGameState.Connected;
    }

    private void SendPaddlePosition(GameEntity entity, string name)
    {
        var paddlePosition = new EntityPosition();
        paddlePosition.Name = name;
        paddlePosition.PositionX = entity.Position.X;
        paddlePosition.PositionY = entity.Position.Y;

        Queue.Add(MessageId.ENTITY_POSITION, paddlePosition);
    }
                
    void Update()
    {
        var dt = Time.deltaTime;
        if (State == NetGameState.Started)
        {
            //Game.Update(dt);  // Control single player or network
            ApplyModelViewPosition(Game.Ball.Position, Ball.transform);

            float vertical = Input.GetAxis("Vertical");
            if (PlayerId == 1)
            {
                var move = Game.LeftPaddle.Position.Y + (vertical * dt * 100);
                Game.LeftPaddle.Position = new XnaGeometry.Vector2(Game.LeftPaddle.Position.X, move);
                ApplyModelViewPosition(Game.LeftPaddle.Position, LeftPaddle.transform);
                Move = true;
            }
            else
            {
                var move = Game.RightPaddle.Position.Y + (vertical * dt * 100);
                Game.RightPaddle.Position = new XnaGeometry.Vector2(Game.RightPaddle.Position.X, move);
                ApplyModelViewPosition(Game.RightPaddle.Position, RightPaddle.transform);
                Move = true;
            }
        }

        if (State >= NetGameState.Connected)
        {
            try
            {
                doNetwork();
            }
            catch (IOException e)
            {
                Debug.LogWarning("" + e);
                State = NetGameState.Offline;
                Client.Close();
            }
            catch (SocketException e)
            {
                Debug.LogWarning("" + e);
                State = NetGameState.Offline;
                Client.Close();
            }
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
        GUILayout.BeginVertical();
        var p1Style = new GUIStyle(GUI.skin.label);
        p1Style.fontSize = 20;
        GUILayout.Label("Player 1", p1Style);
        p1Style = new GUIStyle(GUI.skin.label);
        p1Style.fontSize = 40;
        GUILayout.Label("" + Game.PlayerOne.Points, p1Style);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        var p2Style = new GUIStyle(GUI.skin.label);
        p2Style.fontSize = 20;
        p2Style.alignment = TextAnchor.MiddleRight;
        GUILayout.Label("Player 2", p2Style);
        p2Style = new GUIStyle(GUI.skin.label);
        p2Style.fontSize = 40;
        p2Style.alignment = TextAnchor.MiddleRight;
        GUILayout.Label("" + Game.PlayerTwo.Points, p2Style);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    private static void ApplyModelViewPosition(XnaGeometry.Vector2 model, Transform view)
    {
        XnaGeometry.Vector2 transformed;
        XnaGeometry.Vector2.Transform(ref model, ref MODEL_VIEW, out transformed);
        view.localPosition = new UnityEngine.Vector3((float)transformed.X, (float)transformed.Y, view.localPosition.z);
    }

    private static void ApplyModelViewScale(XnaGeometry.Vector2 model, Transform view)
    {
        XnaGeometry.Vector2 transformed;
        XnaGeometry.Vector2.Transform(ref model, ref MODEL_VIEW_SCALE, out transformed);
        view.localScale = new UnityEngine.Vector3((float)transformed.X, (float)transformed.Y, view.localScale.z);
    }

    private void doNetwork()
    {
        if (Client.Available > 0)
        {
            Debug.Log("Receive network data");
            var networkStream = Client.GetStream();
            var buffer = new byte[Client.Available];
            int read = networkStream.Read(buffer, 0, buffer.Length);
            if (read == 0)
            {
                // Connection closed
                Debug.Log("Connection remotely closed");
            }
            else
            {
                Debug.Log("Reading " + BitConverter.ToString(buffer).Replace("-", string.Empty));
                var packet = Utils.Deserialize<PaddlePacket>(buffer);
                if (packet.Count > 0)   // TODO: properly read from the socket
                {
                    using (var stream = new MemoryStream(packet.RawData))
                    using (var transport = new TStreamTransport(stream, null))
                    {
                        var protocol = new TBinaryProtocol(transport);
                        for (int i = 0; i < packet.Count; ++i)
                        {
                            var messageId = (MessageId)protocol.ReadI32();
                            if (messageId == MessageId.ENTITY_POSITION)
                            {
                                var entityPosition = new EntityPosition();
                                entityPosition.Read(protocol);
                                if (entityPosition.Name.Equals(PLAYER_ONE))
                                {
                                    Game.LeftPaddle.Position = new XnaGeometry.Vector2(entityPosition.PositionX, entityPosition.PositionY);
                                    ApplyModelViewPosition(Game.LeftPaddle.Position, LeftPaddle.transform);
                                }
                                else if (entityPosition.Name.Equals(PLAYER_TWO))
                                {
                                    Game.RightPaddle.Position = new XnaGeometry.Vector2(entityPosition.PositionX, entityPosition.PositionY);
                                    ApplyModelViewPosition(Game.RightPaddle.Position, RightPaddle.transform);
                                }
                                else if (entityPosition.Name.Equals("Ball"))
                                {
                                    Game.Ball.Position = new XnaGeometry.Vector2(entityPosition.PositionX, entityPosition.PositionY);
                                }
                            }
                            else if (messageId == MessageId.POINTS_UPDATE)
                            {
                                Debug.Log("Received Points Update message");
                                var pointsUpdate = new PointsUpdate();
                                pointsUpdate.Read(protocol);
                                Game.PlayerOne.Points = pointsUpdate.PlayerOneScore;
                                Game.PlayerTwo.Points = pointsUpdate.PlayerTwoScore;
                            }
                            else if (messageId == MessageId.GAME_START)
                            {
                                Debug.Log("Received Game Start message");
                                var gameStart = new GameStart();
                                gameStart.Read(protocol);
                                Game.PlayerOne = new LocalPlayer();
                                Game.PlayerTwo = new LocalPlayer();
                                PlayerId = gameStart.PlayerId;
                                State = NetGameState.Started;
                            }
                            else if (messageId == MessageId.GAME_OVER)
                            {
                                Debug.Log("Received Game Over message");
                                State = NetGameState.Offline;
                            }
                            else
                            {
                                Debug.LogError("Unsupported message id: " + messageId);
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Lost packet");
                }
            }
        }

        if (State == NetGameState.Started)
        {
            if (Move)
            {
                Move = false;
                if (PlayerId == 1)
                {
                    SendPaddlePosition(Game.LeftPaddle, PLAYER_ONE);
                }
                else
                {
                    SendPaddlePosition(Game.RightPaddle, PLAYER_TWO);
                }

                byte[] raw = Queue.BuildPacket();
                Debug.Log("Writing " + BitConverter.ToString(raw).Replace("-", string.Empty));
                Client.GetStream().Write(raw, 0, raw.Length);
            }
            
        }

    }
}
