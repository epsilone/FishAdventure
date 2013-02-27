using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Paddle
{
    public interface IPaddleState
    {
        void OnEnter();
        void OnExit();
        void Update(long dt);
    }
}
