using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paddle;

namespace PaddleServer.ServerState
{
    public class GameOver : IPaddleState
    {
        private PaddleContext Context { get; set; }

        public GameOver(PaddleContext context)
        {
            Context = context;
        }

        public void OnEnter()
        {
            
        }

        public void OnExit()
        {
            
        }

        public void Update(long dt)
        {
            
        }
    }
}
