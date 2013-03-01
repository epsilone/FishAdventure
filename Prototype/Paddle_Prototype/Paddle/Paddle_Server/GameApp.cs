using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Paddle;
using PaddleServer.ServerState;

namespace PaddleServer
{
    class GameApp
    {
        #region Constants

        private const int DESIRED_FPS = 25;

        #endregion

        private PaddleContext Context { get; set; }

        public GameApp()
        {
            Context = new PaddleContext();
        }

        public void Run()
        {
            var gameTimer = new GameTimer();
            long tick = 0L;
            while (true)
            {
                gameTimer.Reset();
                Context.Update(tick);
                tick = 0L;
                while (tick < DESIRED_FPS)
                {
                    tick = gameTimer.GetTicks();
                }
            }
        }
    }
}
