using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Paddle;
using PaddleThrift;
using PaddleTransport;
using Thrift.Protocol;
using Thrift.Transport;
using XnaGeometry;
using System.Collections.Generic;

namespace PaddleForm
{
    public partial class MainForm : Form
    {
        #region Constants

        private static readonly string PLAYER_ONE = "PlayerOne";
        private static readonly string PLAYER_TWO = "PlayerTwo";

        private static Matrix MODEL_VIEW = (Matrix.CreateScale(1.0, -1.0, 1.0) * Matrix.CreateTranslation(320, 240, 0.0));

        private static byte[] FRAME = new byte[2048];

        #endregion

        private Pong Game { get; set; }
        private Player Player { get; set; }
        private byte[] SocketBuffer { get; set; }

        private bool connected;
        private bool gameStarted;

        public MainForm()
        {
            InitializeComponent();
            Game = new Pong();
            Player = new Player();
            Player.Client = new TcpClient(AddressFamily.InterNetwork);
            Player.Client.NoDelay = true;
            SocketBuffer = new byte[2048];

            //Player.Controlled = Game.LeftPaddle;
            //connected = true;
            //infoPanel.Visible = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (connected)
            {
                doNetwork();
                this.Invalidate();
                //Game.Update(0.03);
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            using (Brush brush = new SolidBrush(Color.White))
            {
                Vector2 transformed;    // reused

                var graphics = e.Graphics;
                graphics.Clear(Color.Black);

                var ballPosition = Game.Ball.Position;
                Vector2.Transform(ref ballPosition, ref MODEL_VIEW, out transformed);
                graphics.FillEllipse(brush, (float)(transformed.X - Game.Ball.Scale.X), (float)(transformed.Y - Game.Ball.Scale.Y), (float)Game.Ball.Scale.X * 2.0f, (float)Game.Ball.Scale.Y * 2.0f);

                var leftPaddlePos = Game.LeftPaddle.Position;
                Vector2.Transform(ref leftPaddlePos, ref MODEL_VIEW, out transformed);
                graphics.FillRectangle(brush, (float)(transformed.X - Game.LeftPaddle.Scale.X), (float)(transformed.Y - Game.LeftPaddle.Scale.Y), (float)Game.LeftPaddle.Scale.X * 2.0f, (float)Game.LeftPaddle.Scale.Y * 2.0f);

                var rightPaddlePos = Game.RightPaddle.Position;
                Vector2.Transform(ref rightPaddlePos, ref MODEL_VIEW, out transformed);
                graphics.FillRectangle(brush, (float)(transformed.X - Game.RightPaddle.Scale.X), (float)(transformed.Y - Game.RightPaddle.Scale.Y), (float)Game.RightPaddle.Scale.X * 2.0f, (float)Game.RightPaddle.Scale.Y * 2.0f);

                var topBorderPos = Game.TopBorder.Position;
                Vector2.Transform(ref topBorderPos, ref MODEL_VIEW, out transformed);
                graphics.FillRectangle(brush, (float)(transformed.X - Game.TopBorder.Scale.X), (float)(transformed.Y - Game.TopBorder.Scale.Y), (float)Game.TopBorder.Scale.X * 2.0f, (float)Game.TopBorder.Scale.Y * 2.0f);

                var bottomBorderPos = Game.BottomBorder.Position;
                Vector2.Transform(ref bottomBorderPos, ref MODEL_VIEW, out transformed);
                graphics.FillRectangle(brush, (float)(transformed.X - Game.BottomBorder.Scale.X), (float)(transformed.Y - Game.BottomBorder.Scale.Y), (float)Game.BottomBorder.Scale.X * 2.0f, (float)Game.BottomBorder.Scale.Y * 2.0f);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Player.Client.Connect(new IPEndPoint(IPAddress.Parse("10.9.42.223"), 8123));
            connected = true;
            infoPanel.Visible = false;
        }

        private void doNetwork()
        {
            DoReceive(Player);

            if (gameStarted)
            {
                //Trace.WriteLine("Send network data");
                var paddlePosition = new EntityPosition();
                paddlePosition.Name = Player.Name;
                paddlePosition.PositionX = Player.Controlled.Position.X;
                paddlePosition.PositionY = Player.Controlled.Position.Y;

                Player.AddMessage(MessageId.ENTITY_POSITION, paddlePosition);
                DoSend(Player);
            }

        }

        private void DoSend(Player player)
        {
            try
            {
                if (player.MessageQueue.Count > 0)
                {
                    var packet = Utils.CreatePacket(player.MessageQueue);
                    player.MessageQueue.Clear();
                    byte[] raw = FrameBuffer.CreateFrame(Utils.Serialize, packet);
                    //Console.WriteLine("Writing {0}, {1}", raw.Length, BitConverter.ToString(raw).Replace("-", string.Empty));
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

        private void DoReceive(Player player)
        {
            int available = player.Client.Available;
            if (available > 0)
            {
                var networkStream = player.Client.GetStream();
                if (available > SocketBuffer.Length)
                {
                    SocketBuffer = new byte[available];
                }
                int read = networkStream.Read(SocketBuffer, 0, SocketBuffer.Length);
                if (read == 0)
                {
                    Trace.TraceError("Connection remotely closed for player {0}", player);
                }
                else
                {
                    Player.Buffer.Append(SocketBuffer, 0, available);
                    ReadFrames(player.Buffer);
                }

                Array.Clear(SocketBuffer, 0, SocketBuffer.Length);
            }
        }

        private void ReadFrames(FrameBuffer buffer)
        {
#if DEBUG
            int frames = 0;
#endif
            int frameSize = -1;
            do
            {
                Array.Clear(FRAME, 0, FRAME.Length);
                frameSize = buffer.ReadFrame(FRAME);
                if (frameSize > 0)
                {
#if DEBUG
                    frames++;
#endif
                    //Console.WriteLine("Reading, {0}", BitConverter.ToString(FRAME).Replace("-", string.Empty));
                    var packet = Utils.Deserialize<PaddlePacket>(FRAME);
                    ProcessPacket(packet);
                }
            }
            while (frameSize > 0);
#if DEBUG
            Console.WriteLine("Processed frames {0} {1}", frames, buffer.QueueSize());
#endif
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
                            Game.LeftPaddle.Position = new Vector2(entityPosition.PositionX, entityPosition.PositionY);
                            Console.WriteLine("Player one position updated {0}", Game.LeftPaddle.Position);
                        }
                        else if (entityPosition.Name.Equals(PLAYER_TWO))
                        {
                            Game.RightPaddle.Position = new Vector2(entityPosition.PositionX, entityPosition.PositionY);
                            Console.WriteLine("Player two position updated {0}", Game.RightPaddle.Position);
                        }
                        else if (entityPosition.Name.Equals("Ball"))
                        {
                            Game.Ball.Position = new Vector2(entityPosition.PositionX, entityPosition.PositionY);
                            //Console.WriteLine("Ball position updated {0}", Game.Ball.Position);
                        }
                    }
                    else if (messageId == MessageId.POINTS_UPDATE)
                    {
                        var pointsUpdate = new PointsUpdate();
                        pointsUpdate.Read(protocol);
                        Game.PlayerOne.Points = pointsUpdate.PlayerOneScore;
                        Game.PlayerTwo.Points = pointsUpdate.PlayerTwoScore;
                    }
                    else if (messageId == MessageId.GAME_START)
                    {
                        Trace.TraceInformation("Received Game Start message");
                        var gameStart = new GameStart();
                        gameStart.Read(protocol);
                        
                        if (gameStart.PlayerId == 1)
                        {
                            Player.Name = PLAYER_ONE;
                            Game.PlayerOne = Player;
                            Game.PlayerTwo = new Player();
                            Player.Controlled = Game.LeftPaddle;
                        }
                        else
                        {
                            Player.Name = PLAYER_TWO;
                            Game.PlayerOne = new Player();
                            Game.PlayerTwo = Player;
                            Player.Controlled = Game.RightPaddle;
                        }
                        gameStarted = true;
                    }
                    else if (messageId == MessageId.GAME_OVER)
                    {
                        Trace.TraceInformation("Received Game Over message");
                        connected = false;
                    }
                    else
                    {
                        Trace.TraceError("Unsupported message id: {0}", messageId);
                    }
                }
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (connected)
            {
                if (e.KeyCode == Keys.Up)
                {
                    var position = new Vector2(Player.Controlled.Position.X, Player.Controlled.Position.Y + 10);
                    Player.Controlled.Position = position;
                }
                else if (e.KeyCode == Keys.Down)
                {
                    var position = new Vector2(Player.Controlled.Position.X, Player.Controlled.Position.Y - 10);
                    Player.Controlled.Position = position;
                }

                Console.WriteLine("On Key Up {0}", Player.Controlled.Position);
            }
        }
    }
}
