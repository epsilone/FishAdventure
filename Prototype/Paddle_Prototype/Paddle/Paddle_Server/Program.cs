using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaGeometry;
using PaddleTransport;

namespace PaddleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //var app = new GameApp();
            //app.Run();
            var buffer = new CircularBuffer<byte>(10);
            buffer.Put(1);
            buffer.Put(2);
            buffer.Put(3);

            byte[] raw = { 12, 13, 14, 15, 16, 17 };
            buffer.Put(raw, 0, raw.Length);

            buffer.Put(4);
            for (int i = 0; i < raw.Length; ++i)
            {
                buffer.Get();
            }

            buffer.Put(5);
            buffer.Put(6);
            buffer.Get();
            buffer.Get();
            buffer.Put(raw, 0, raw.Length);
            for (int i = 0; i < raw.Length; ++i)
            {
                buffer.Get();
            }
            buffer.Put(raw, 0, raw.Length);

        }
    }
}
