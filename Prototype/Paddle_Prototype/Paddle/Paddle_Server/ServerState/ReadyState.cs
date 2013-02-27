using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Paddle;
using System.Net.Sockets;

namespace PaddleServer.ServerState
{
    public class ReadyState : IPaddleState
    {
        private PaddleContext Context { get; set; }

        public ReadyState(PaddleContext context)
        {
            Context = context;
        }

        public void OnEnter()
        {
            Context.Server.ClientConnected += OnClientConnected;
            Context.Server.Start();
        }

        public void OnExit()
        {
            Context.Server.ClientConnected -= OnClientConnected;
        }

        public void Update(long dt)
        {
            if (Context.Players.Count == 2)
            {
                Context.Transition(new PlayState(Context));
            }
        }

        private void OnClientConnected(TcpClient client)
        {
            Trace.WriteLine("Client connected");
            var player = new Player();
            player.Client = client;
            player.Client.NoDelay = true;
            Context.Players.Add(player);
        }
    }
}
