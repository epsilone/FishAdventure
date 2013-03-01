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

namespace PaddleForm
{
    public partial class MainForm : Form
    {
        #region Constants

        private static readonly string PLAYER_ONE = "PlayerOne";
        private static readonly string PLAYER_TWO = "PlayerTwo";

        private static Matrix MODEL_VIEW = (Matrix.CreateScale(1.0, -1.0, 1.0) * Matrix.CreateTranslation(320, 240, 0.0));

        #endregion

        private Pong Game { get; set; }
        private TcpClient Client { get; set; }
        private MessageQueue Queue { get; set; }
        private int PlayerId { get; set; }
        private bool connected;
        private bool gameStarted;

        public MainForm()
        {
            InitializeComponent();
            Game = new Pong();
            Client = new TcpClient(AddressFamily.InterNetwork);
            Client.NoDelay = true;
            Queue = new MessageQueue();

            //PlayerId = 1;
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

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (connected)
            {
                if (PlayerId == 1)
                {
                    if (e.KeyChar == 'q')
                    {
                        var position = new Vector2(Game.LeftPaddle.Position.X, Game.LeftPaddle.Position.Y + 10);
                        Game.LeftPaddle.Position = position;
                    }
                    else if (e.KeyChar == 'a')
                    {
                        var position = new Vector2(Game.LeftPaddle.Position.X, Game.LeftPaddle.Position.Y - 10);
                        Game.LeftPaddle.Position = position;
                    }

                    Console.WriteLine("On Key Press {0}", Game.LeftPaddle.Position);
                }
                else
                {
                    if (e.KeyChar == 'p')
                    {
                        var position = new Vector2(Game.RightPaddle.Position.X, Game.RightPaddle.Position.Y + 10);
                        Game.RightPaddle.Position = position;
                    }
                    else if (e.KeyChar == 'l')
                    {
                        var position = new Vector2(Game.RightPaddle.Position.X, Game.RightPaddle.Position.Y - 10);
                        Game.RightPaddle.Position = position;
                    }
                }
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
            Client.Connect(new IPEndPoint(IPAddress.Parse("10.9.42.223"), 8123));
            connected = true;
            infoPanel.Visible = false;
        }

        private void doNetwork()
        {
            if (Client.Available > 0)
            {
                Trace.WriteLine("Receive network data");
                var networkStream = Client.GetStream();
                var buffer = new byte[Client.ReceiveBufferSize];
                int read = networkStream.Read(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    // Connection closed
                    Trace.TraceError("Connection remotely closed");
                }
                else
                {
                    //Console.WriteLine("Reading {0}, {1}", read, BitConverter.ToString(buffer).Replace("-", string.Empty));
                    var packet = Utils.Deserialize<PaddlePacket>(buffer);
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
                                Game.PlayerOne = new Player();
                                Game.PlayerTwo = new Player();
                                PlayerId = gameStart.PlayerId;
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
            }

            if (gameStarted)
            {
                //Trace.WriteLine("Send network data");
                var paddlePosition = new EntityPosition();
                if (PlayerId == 1)
                {
                    paddlePosition.Name = PLAYER_ONE;
                    paddlePosition.PositionX = Game.LeftPaddle.Position.X;
                    paddlePosition.PositionY = Game.LeftPaddle.Position.Y;
                }
                else
                {
                    paddlePosition.Name = PLAYER_TWO;
                    paddlePosition.PositionX = Game.RightPaddle.Position.X;
                    paddlePosition.PositionY = Game.RightPaddle.Position.Y;
                }

                Queue.Add(MessageId.ENTITY_POSITION, paddlePosition);
                byte[] raw = Queue.BuildPacket();
                Client.GetStream().Write(raw, 0, raw.Length);
            }

        }
    }
}
