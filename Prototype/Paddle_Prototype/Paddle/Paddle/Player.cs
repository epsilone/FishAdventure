using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using PaddleTransport;
using PaddleThrift;
using Thrift.Protocol;

namespace Paddle
{
    public class Player : IPaddlePlayer
    {
        #region IPaddlePlayer

        public string Name { get; set; }
        public int Points { get; set; }

        #endregion

        #region Network

        public TcpClient Client { get; set; }
        public List<KeyValuePair<MessageId, TBase>> MessageQueue { get; private set; }
        public FrameBuffer Buffer { get; private set; }

        #endregion

        public GameEntity Controlled { get; set; }

        public Player()
        {
            MessageQueue = new List<KeyValuePair<MessageId, TBase>>();
            Buffer = new FrameBuffer();
        }

        public void AddMessage(MessageId messageId, TBase message)
        {
            MessageQueue.Add(new KeyValuePair<MessageId, TBase>(messageId, message));
        }

        public override string ToString()
        {
            return "Player: " + Name + " with points: " + Points;
        }
    }
}
