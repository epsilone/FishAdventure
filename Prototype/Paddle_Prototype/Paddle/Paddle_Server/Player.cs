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
        public string Name { get; set; }
        public int Points { get; set; }
        public TcpClient Client { get; set; }
    }
}
