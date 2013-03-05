using System;
using System.Diagnostics;
using Paddle;
using PaddleThrift;
using Thrift.Protocol;
using PaddleTransport;
using System.IO;
using Thrift.Transport;
using System.Collections.Generic;

namespace PaddleServer.ServerState
{
    public class PlayState : IPaddleState
    {
        #region Constants

        private const int POINTS_TO_WIN = 10;

        private static readonly string PLAYER_ONE = "PlayerOne";
        private static readonly string PLAYER_TWO = "PlayerTwo";

        private const int NETWORK_UPDATE = 30;

        private static byte[] FRAME = new byte[2048];

        #endregion
        
        public PaddleContext Context { get; private set; }
        public Pong Pong { get; private set; }
        private long Accumulator { get; set; }
        private bool Running { get; set; }

        private Player PlayerOne { get; set; }
        private Player PlayerTwo { get; set; }

        public PlayState(PaddleContext context)
        {
            Context = context;
            Pong = new Pong();
            Pong.PlayerOne = PlayerOne = Context.Players[0];
            Pong.PlayerTwo = PlayerTwo = Context.Players[1];
        }

        public void OnEnter()
        {
            PlayerOne.Name = PLAYER_ONE;
            PlayerOne.OnPointsUpdated += OnPointsUpdated;

            PlayerTwo.Name = PLAYER_TWO;
            PlayerTwo.OnPointsUpdated += OnPointsUpdated;

            var ballPosition = createBallPosition();
            var playerOnePosition = createPlayerOnePosition();
            var playerTwoPosition = createPlayerTwoPosition();

            PlayerOne.AddMessage(MessageId.ENTITY_POSITION, ballPosition);
            PlayerOne.AddMessage(MessageId.ENTITY_POSITION, playerOnePosition);
            PlayerOne.AddMessage(MessageId.ENTITY_POSITION, playerTwoPosition);

            {
                var startGame = new GameStart();
                startGame.BallDisplacementX = Pong.Ball.Displacement.X;
                startGame.BallDisplacementY = Pong.Ball.Displacement.Y;
                startGame.PlayerId = 1;
                PlayerOne.AddMessage(MessageId.GAME_START, startGame);
            }

            PlayerTwo.AddMessage(MessageId.ENTITY_POSITION, ballPosition);
            PlayerTwo.AddMessage(MessageId.ENTITY_POSITION, playerOnePosition);
            PlayerTwo.AddMessage(MessageId.ENTITY_POSITION, playerTwoPosition);

            {
                var startGame = new GameStart();
                startGame.BallDisplacementX = Pong.Ball.Displacement.X;
                startGame.BallDisplacementY = Pong.Ball.Displacement.Y;
                startGame.PlayerId = 2;
                PlayerTwo.AddMessage(MessageId.GAME_START, startGame);
            }

            Running = true;
        }

        public void OnExit()
        {
            OnPointsUpdated();

            var gameOver = new PaddleThrift.GameOver();
            PlayerOne.AddMessage(MessageId.GAME_OVER, gameOver);
            PlayerTwo.AddMessage(MessageId.GAME_OVER, gameOver);

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
                    PlayerOne.AddMessage(MessageId.GAME_OVER, gameOver);
                    PlayerTwo.AddMessage(MessageId.GAME_OVER, gameOver);
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

            PlayerOne.AddMessage(MessageId.ENTITY_POSITION, createBallPosition());
            PlayerOne.AddMessage(MessageId.ENTITY_POSITION, createPlayerTwoPosition());

            PlayerTwo.AddMessage(MessageId.ENTITY_POSITION, createBallPosition());
            PlayerTwo.AddMessage(MessageId.ENTITY_POSITION, createPlayerOnePosition());

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
                    Trace.TraceError("Connection remotely closed for player {0}", player);
                }
                else
                {
                    player.Buffer.Append(buffer);
                    ReadFrames(player.Buffer);
                }
            }
        }

        private void ReadFrames(FrameBuffer buffer)
        {
            int frameSize = -1;
            do
            {
                Array.Clear(FRAME, 0, FRAME.Length);
                frameSize = buffer.ReadFrame(FRAME);
                if (frameSize > 0)
                {
                    Console.WriteLine("Reading, {0}", BitConverter.ToString(FRAME).Replace("-", string.Empty));
                    var packet = Utils.Deserialize<PaddlePacket>(FRAME);
                    ProcessPacket(packet);
                }
            }
            while (frameSize > 0);
        }

        private void ProcessPacket(PaddlePacket packet)
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

        private void doSend(Player player)
        {
            try
            {
                if (player.MessageQueue.Count > 0)
                {
                    var packet = Utils.CreatePacket(player.MessageQueue);
                    player.MessageQueue.Clear();
                    byte[] raw = FrameBuffer.CreateFrame(Utils.Serialize, packet);
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

        private void SerializePlayerOnePacket(Stream stream)
        {
            SerializePacket(stream, PlayerOne.MessageQueue);
        }

        private void SerializePlayerTwoPacket(Stream stream)
        {
            SerializePacket(stream, PlayerTwo.MessageQueue);
        }

        private void SerializePacket(Stream stream, List<KeyValuePair<MessageId, TBase>> queue)
        {
            var packet = Utils.CreatePacket(queue);
            Utils.Serialize(packet, stream);
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
            PlayerOne.AddMessage(MessageId.POINTS_UPDATE, createPoints);
            PlayerTwo.AddMessage(MessageId.POINTS_UPDATE, createPoints);
        }
    }
}
