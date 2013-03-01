using System;
using System.Diagnostics;
using Paddle;
using PaddleThrift;
using Thrift.Protocol;
using PaddleTransport;
using System.IO;
using Thrift.Transport;

namespace PaddleServer.ServerState
{
    public class PlayState : IPaddleState
    {
        #region Constants

        private const int POINTS_TO_WIN = 10;

        private static readonly string PLAYER_ONE = "PlayerOne";
        private static readonly string PLAYER_TWO = "PlayerTwo";

        private const int NETWORK_UPDATE = 30;

        #endregion
        
        public PaddleContext Context { get; private set; }
        public Pong Pong { get; private set; }
        private MessageQueue PlayerOneQueue { get; set; }
        private MessageQueue PlayerTwoQueue { get; set; }
        private long Accumulator { get; set; }
        private bool Running { get; set; }

        public PlayState(PaddleContext context)
        {
            Context = context;
            PlayerOneQueue = new MessageQueue();
            PlayerTwoQueue = new MessageQueue();
            Pong = new Pong();
            Pong.PlayerOne = Context.Players[0];
            Pong.PlayerTwo = Context.Players[1];
        }

        public void OnEnter()
        {
            Context.Players[0].Name = PLAYER_ONE;
            Context.Players[0].OnPointsUpdated += OnPointsUpdated;

            Context.Players[1].Name = PLAYER_TWO;
            Context.Players[1].OnPointsUpdated += OnPointsUpdated;

            var ballPosition = createBallPosition();
            var playerOnePosition = createPlayerOnePosition();
            var playerTwoPosition = createPlayerTwoPosition();

            PlayerOneQueue.Add(MessageId.ENTITY_POSITION, ballPosition);
            PlayerOneQueue.Add(MessageId.ENTITY_POSITION, playerOnePosition);
            PlayerOneQueue.Add(MessageId.ENTITY_POSITION, playerTwoPosition);

            {
                var startGame = new GameStart();
                startGame.BallDisplacementX = Pong.Ball.Displacement.X;
                startGame.BallDisplacementY = Pong.Ball.Displacement.Y;
                startGame.PlayerId = 1;
                PlayerOneQueue.Add(MessageId.GAME_START, startGame);
            }

            PlayerTwoQueue.Add(MessageId.ENTITY_POSITION, ballPosition);
            PlayerTwoQueue.Add(MessageId.ENTITY_POSITION, playerOnePosition);
            PlayerTwoQueue.Add(MessageId.ENTITY_POSITION, playerTwoPosition);

            {
                var startGame = new GameStart();
                startGame.BallDisplacementX = Pong.Ball.Displacement.X;
                startGame.BallDisplacementY = Pong.Ball.Displacement.Y;
                startGame.PlayerId = 2;
                PlayerTwoQueue.Add(MessageId.GAME_START, startGame);
            }

            Running = true;
        }

        public void OnExit()
        {
            OnPointsUpdated();

            var gameOver = new PaddleThrift.GameOver();
            PlayerOneQueue.Add(MessageId.GAME_OVER, gameOver);
            PlayerTwoQueue.Add(MessageId.GAME_OVER, gameOver);

            foreach (var player in Context.Players)
            {
                player.OnPointsUpdated -= OnPointsUpdated;
            }
        }

        public void Update(long dt)
        {
            Accumulator += dt;
            if (Accumulator > NETWORK_UPDATE)
            {
                if (!Running)
                {
                    Console.WriteLine("Sending disconnect to remaining clients");
                    var gameOver = new PaddleThrift.GameOver();
                    PlayerOneQueue.Add(MessageId.GAME_OVER, gameOver);
                    PlayerTwoQueue.Add(MessageId.GAME_OVER, gameOver);
                }

                doNetwork();

                if (!Running)
                {
                    Context.Transition(new GameOver(Context));
                }

                Accumulator -= NETWORK_UPDATE;
            }
            
            
            if (Pong.PlayerOne.Points >= POINTS_TO_WIN || Pong.PlayerTwo.Points >= POINTS_TO_WIN)
            {
                Running = false;
            }
            
            if (Running)
            {
                Pong.Update(dt / 100.0);
            }
        }

        private void doNetwork()
        {
            int removed = Context.Players.RemoveAll(p => !p.Client.Connected);
            Running = removed == 0;

            Trace.WriteLine("Receive network data");
            Context.Players.ForEach(doReceive);

            PlayerOneQueue.Add(MessageId.ENTITY_POSITION, createBallPosition());
            PlayerOneQueue.Add(MessageId.ENTITY_POSITION, createPlayerTwoPosition());

            PlayerTwoQueue.Add(MessageId.ENTITY_POSITION, createBallPosition());
            PlayerTwoQueue.Add(MessageId.ENTITY_POSITION, createPlayerOnePosition());

            Trace.WriteLine("Send network data");
            Context.Players.ForEach(doSend);
        }

        private void doReceive(Player player)
        {
            if (player.Client.Available > 0)
            {
                var networkStream = player.Client.GetStream();
                var buffer = new byte[player.Client.Available];
                int read = networkStream.Read(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    // Connection closed
                    Trace.TraceError("Connection remotely closed for player {0}", player);
                }
                else
                {
                    Console.WriteLine("Reading {0}, {1}", read, BitConverter.ToString(buffer).Replace("-", string.Empty));
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
                                        Pong.LeftPaddle.Position = new XnaGeometry.Vector2(entityPosition.PositionX, entityPosition.PositionY);
                                    }
                                    else if (entityPosition.Name.Equals(PLAYER_TWO))
                                    {
                                        Pong.RightPaddle.Position = new XnaGeometry.Vector2(entityPosition.PositionX, entityPosition.PositionY);
                                    }
                                }
                                else
                                {
                                    Trace.TraceError("Unsupported message id: {0}", messageId);
                                }
                            }
                        }
                    }
                    else
                    {
                        Trace.TraceInformation("Lost packet");
                    }
                }
            }
        }

        private void doSend(Player player)
        {
            try
            {
                var queue = player.Name.Equals(PLAYER_ONE) ? PlayerOneQueue : PlayerTwoQueue;
                if (!queue.IsEmpty())
                {
                    byte[] raw = queue.BuildPacket();
                    Console.WriteLine("Writing {0}, {1}", raw.Length, BitConverter.ToString(raw).Replace("-", string.Empty));

                    var stream = player.Client.GetStream();
                    stream.Write(raw, 0, raw.Length);
                }
            }
            catch (IOException e)
            {
                Trace.TraceError("Client is not responding {0}", player);
                player.Client.Close();
            }
        }

        private EntityPosition createBallPosition()
        {
            var entityPosition = new EntityPosition();
            entityPosition.Name = "Ball";
            entityPosition.PositionX = Pong.Ball.Position.X;
            entityPosition.PositionY = Pong.Ball.Position.Y;
            return entityPosition;
        }

        private EntityPosition createPlayerOnePosition()
        {
            var entityPosition = new EntityPosition();
            entityPosition.Name = PLAYER_ONE;
            entityPosition.PositionX = Pong.LeftPaddle.Position.X;
            entityPosition.PositionY = Pong.LeftPaddle.Position.Y;
            return entityPosition;
        }

        private EntityPosition createPlayerTwoPosition()
        {
            var entityPosition = new EntityPosition();
            entityPosition.Name = PLAYER_TWO;
            entityPosition.PositionX = Pong.RightPaddle.Position.X;
            entityPosition.PositionY = Pong.RightPaddle.Position.Y;
            return entityPosition;
        }

        private PointsUpdate createPointsUpdate()
        {
            var pointsUpdate = new PointsUpdate();
            pointsUpdate.PlayerOneScore = Pong.PlayerOne.Points;
            pointsUpdate.PlayerTwoScore = Pong.PlayerTwo.Points;
            return pointsUpdate;
        }

        private void OnPointsUpdated()
        {
            var createPoints = createPointsUpdate();
            PlayerOneQueue.Add(MessageId.POINTS_UPDATE, createPoints);
            PlayerTwoQueue.Add(MessageId.POINTS_UPDATE, createPoints);
        }
    }
}
