using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paddle;
using System.Net.Sockets;

namespace PaddleServer
{
    public class Player : IPaddlePlayer
    {
        public delegate void PointsUpdated();
        public event PointsUpdated OnPointsUpdated;

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
        public TcpClient Client { get; set; }

        public override string ToString()
        {
            return "Player: " + Name + " with points: " + Points;
        }
    }
}
