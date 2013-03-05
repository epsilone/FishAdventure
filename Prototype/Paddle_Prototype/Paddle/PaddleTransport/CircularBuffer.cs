using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaddleTransport
{
    public class CircularBuffer<T>
    {
        private T[] buffer;
        private int head;
        private int tail;
        private int size;   // TODO: remove the size

        public CircularBuffer() :
            this(1024)
        {

        }

        public CircularBuffer(int size)
        {
            buffer = new T[size];
            head = 0;
            tail = 0;
            size = 0;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public bool IsFull()
        {
            return size == buffer.Length;
        }

        public bool Put(T element)
        {
            bool hasSpace = !IsFull();
            if (hasSpace)
            {
                ++size;
                buffer[tail] = element;
                tail = (tail + 1) % buffer.Length;
            }

            return hasSpace;
        }

        public bool Put(T[] elements, int offset, int length)
        {
            bool hasSpace = ((buffer.Length - size) >= (length - offset));
            if (hasSpace)
            {
                int available = buffer.Length - tail;
                int wrote = available > length ? length : available;
                Array.Copy(elements, offset, buffer, tail, wrote);
                tail = (tail + wrote) % buffer.Length;
                int remains = length - wrote;
                if (remains > 0)
                {
                    Array.Copy(elements, offset + wrote, buffer, tail, remains);
                    tail = (tail + remains) % buffer.Length;
                }
                size += length;
            }

            return hasSpace;
        }

        public T Get()
        {
            T value = buffer[head];
            buffer[head] = default(T);
            head = (head + 1) % buffer.Length;
            --size;
            return value;
        }

    }
}
