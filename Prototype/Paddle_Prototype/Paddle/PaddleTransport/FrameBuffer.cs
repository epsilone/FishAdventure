using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace PaddleTransport
{
    public class FrameBuffer
    {
        #region Constants

        private const int HEADER_SIZE = 4;
        private static readonly byte[] DUMMY_SIZE_HEADER = new byte[HEADER_SIZE];

        #endregion

        private int FrameSize { get; set; }
        
        private MemoryStream Buffer { get; set; }
        private int Position { get; set; }

        public FrameBuffer()
        {
            Buffer = new MemoryStream();
            FrameSize = -1;
        }

        public int ReadFrame(byte[] raw)
        {
            int read = 0;
            if (FrameSize == -1)
            {
                UpdateFrameSize();
            }

            if ((FrameSize > 0) && (Buffer.Length >= FrameSize))
            {
                Debug.Assert(raw.Length > FrameSize, "Buffer is too small please allocate more, frame size: " + FrameSize);
                read = FrameSize;
                Buffer.Read(raw, 0, FrameSize);
                UpdateFrameSize();
            }

            return read;
        }

        public static byte[] CreateFrame<T>(Action<T, Stream> serializer, T context)
        {
            byte[] framed;
            using (var stream = new MemoryStream())
            {
                stream.Write(DUMMY_SIZE_HEADER, 0, HEADER_SIZE);
                serializer(context, stream);
                framed = stream.ToArray();
                framed[0] = (byte)(0xff & (framed.Length >> 24));
                framed[1] = (byte)(0xff & (framed.Length >> 16));
                framed[2] = (byte)(0xff & (framed.Length >> 8));
                framed[3] = (byte)(0xff & (framed.Length));
            }

            return framed;
        }

        public void Append(byte[] raw)
        {
            Buffer.Write(raw, 0, raw.Length);
        }

        private void UpdateFrameSize()
        {
            FrameSize = -1;
            if (Buffer.Length >= HEADER_SIZE)
            {
                var b1 = Buffer.ReadByte();
                var b2 = Buffer.ReadByte();
                var b3 = Buffer.ReadByte();
                var b4 = Buffer.ReadByte();
                FrameSize = ((b1 & 0xff) << 24) | ((b2 & 0xff) << 16) | ((b3 & 0xff) << 8) | ((b4 & 0xff));
            }
        }
    }
}
