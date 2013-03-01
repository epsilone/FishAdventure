using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace PaddleServer
{
    public class GameTimer
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern long GetTickCount();

        private long startTimeMs = 0L;

        public GameTimer()
        {
            Reset();
        }

        public void Reset()
        {
            startTimeMs = GetTickCount();
        }

        public long GetTicks()
        {
            long currentTimeMs = GetTickCount();
            return currentTimeMs - startTimeMs;
        }
    }
}
