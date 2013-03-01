using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paddle;
using System.Net.Sockets;
using PaddleThrift;
using Thrift.Protocol;
using PaddleTransport;

namespace PaddleServer
{
    public class Player : IPaddlePlayer
    {
        public delegate void PointsUpdated();
        public event PointsUpdated OnPointsUpdated;

        #region IPaddlePlayer

        public string Name { get; set; }

        private int points;
        public int Points
        {
            get
            {
                return points;
            }

            set
            {
                this.points = value;
                if (OnPointsUpdated != null)
                {
                    OnPointsUpdated();
                }
            }
        }

        #endregion

        #region Network
        
        public TcpClient Client { get; set; }
        public List<KeyValuePair<MessageId, TBase>> MessageQueue { get; private set; }
        public FrameBuffer Buffer { get; private set; }

        #endregion

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
