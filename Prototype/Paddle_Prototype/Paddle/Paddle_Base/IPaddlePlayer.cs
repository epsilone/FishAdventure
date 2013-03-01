using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paddle
{
    public interface IPaddlePlayer
    {
        string Name { get; set; }
        int Points { get; set; }
    }
}
