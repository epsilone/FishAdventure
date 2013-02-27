using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Paddle;

namespace PaddleServer.ServerState
{
    public class InitializeState : IPaddleState
    {
        private PaddleContext Context { get; set; }

        public InitializeState(PaddleContext context)
        {
            Context = context;
        }

        void IPaddleState.Update(long dt)
        {
            Context.Transition(new ReadyState(Context));
        }

        public void OnEnter()
        {
            Context.Players.Clear();
        }

        public void OnExit()
        {
            
        }
    }
}
