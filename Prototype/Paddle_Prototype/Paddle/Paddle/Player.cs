using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paddle
{
    public class Player : IPaddlePlayer
    {
        public string Name { get; set; }
        public int Points { get; set; }
    }
}
