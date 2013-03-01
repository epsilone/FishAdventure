using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Thrift.Protocol;
using Thrift.Transport;
using PaddleThrift;

namespace PaddleTransport
{
    public static class Utils
    {
        private static TProtocolFactory PROTOCOL_FACTORY = new TBinaryProtocol.Factory();

        public static byte[] Serialize(TBase instance)
        {
            byte[] raw = null;
            using (var stream = new MemoryStream())
            {
                var transport = new TStreamTransport(null, stream);
                var protocol = PROTOCOL_FACTORY.GetProtocol(transport);
                instance.Write(protocol);
                raw = stream.ToArray();
            }

            return raw;
        }

        public static void Serialize(TBase instance, Stream stream)
        {
            var transport = new TStreamTransport(null, stream);
            var protocol = PROTOCOL_FACTORY.GetProtocol(transport);
            instance.Write(protocol);
        }

        public static T Deserialize<T>(byte[] raw) where T : TBase, new()
        {
            T instance = new T();
            using (var stream = new MemoryStream(raw))
            {
                var transport = new TStreamTransport(stream, null);
                var protocol = PROTOCOL_FACTORY.GetProtocol(transport);
                instance.Read(protocol);
            }

            return instance;
        }

        public static PaddlePacket CreatePacket(List<KeyValuePair<MessageId, TBase>> queue)
        {
            var packet = new PaddlePacket();
            packet.Count = queue.Count;

            using (var stream = new MemoryStream())
            {
                var transport = new TStreamTransport(null, stream);
                var protocol = PROTOCOL_FACTORY.GetProtocol(transport);
                foreach (var kv in queue)
                {
                    protocol.WriteI32((int)kv.Key);
                    kv.Value.Write(protocol);
                }

                packet.RawData = stream.ToArray();
            }

            return packet;
        }
    }
}
