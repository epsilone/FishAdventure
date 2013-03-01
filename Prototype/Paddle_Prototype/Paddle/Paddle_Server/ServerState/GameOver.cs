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
            Context.Players.ForEach(Disconnect);
        }

        public void OnExit()
        {
            Context.Players.Clear();
        }

        public void Update(long dt)
        {
            Context.Transition(new ReadyState(Context));
        }

        private void Disconnect(Player player)
        {
            if (player.Client.Connected)
            {
                player.Client.Close();
            }
        }
    }
}
