using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Paddle;
using PaddleServer.ServerState;

namespace PaddleServer
{
    public class PaddleContext
    {
        #region Constants

        private const int PORT = 8123;
        private static readonly string HOST = "10.9.42.223";

        #endregion

        private IPaddleState CurrentState { get; set; }
        public List<Player> Players { get; private set; }
        public ConnectionListener Server { get; private set; }

        public PaddleContext()
        {
            CurrentState = new InitializeState(this);
            Players = new List<Player>();
            Server = new ConnectionListener(HOST, PORT);
        }

        public void Transition(IPaddleState next)
        {
            Console.WriteLine("Transition to state {0}", next.ToString());
            CurrentState.OnExit();
            CurrentState = next;
            CurrentState.OnEnter();
            Trace.WriteLine("Transition done");
        }

        public void Update(long dt)
        {
            CurrentState.Update(dt);
        }
    }
}
