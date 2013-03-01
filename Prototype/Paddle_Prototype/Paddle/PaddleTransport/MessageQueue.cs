using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaddleThrift;
using Thrift.Protocol;

namespace PaddleTransport
{
    public class MessageQueue
    {
        private List<KeyValuePair<MessageId, TBase>> Queue { get; set; }

        public MessageQueue()
        {
            Queue = new List<KeyValuePair<MessageId, TBase>>();
        }

        public void Add(MessageId messageId, TBase message)
        {
            Queue.Add(new KeyValuePair<MessageId, TBase>(messageId, message));
        }

        public byte[] BuildPacket()
        {
            var packet = Utils.CreatePacket(Queue);
            Queue.Clear();
            return Utils.Serialize(packet);
        }

        public bool IsEmpty()
        {
            return Queue.Count == 0;
        }
    }
}
