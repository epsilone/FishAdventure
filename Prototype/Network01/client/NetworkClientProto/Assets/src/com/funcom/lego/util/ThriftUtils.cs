using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Thrift.Protocol;
using Thrift.Transport;
using System.IO;
using com.funcom.lego.generated;

public static class ThriftUtils 
{
    public static byte[] Serialize(TBase instance)
    {
        byte[] raw = null;
        using (var stream = new MemoryStream())
        {
            var transport = new TStreamTransport(null, stream);
            var protocol = new TBinaryProtocol(transport);
            instance.Write(protocol);
            raw = stream.GetBuffer();
        }

        return raw;
    }

    public static T Deserialize<T>(byte[] raw) where T : TBase, new()
    {
        T instance = new T();
        using (var stream = new MemoryStream(raw))
        {
            var transport = new TStreamTransport(stream, null);
            var protocol = new TBinaryProtocol(transport);
            instance.Read(protocol);
        }

        return instance;
    }

    public static byte[] CustomSerialize(List<KeyValuePair<MessageId, TBase>> queue)
    {
        byte[] raw = null;
        using (var stream = new MemoryStream())
        {
            var transport = new TStreamTransport(null, stream);
            var protocol = new TBinaryProtocol(transport);

            protocol.WriteI32(queue.Count);

            foreach (KeyValuePair<MessageId, TBase> kv in queue)
            {
                protocol.WriteByte((byte)kv.Key);
            }

            foreach (KeyValuePair<MessageId, TBase> kv in queue)
            {
                kv.Value.Write(protocol);
            }

            raw = stream.GetBuffer();
        }

        return raw;
    }
}
