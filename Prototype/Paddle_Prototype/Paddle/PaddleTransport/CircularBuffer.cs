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

        public int Length
        {
            get { return size; }
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
#if DEBUG
            buffer[head] = default(T);
#endif
            head = (head + 1) % buffer.Length;
            --size;
            return value;
        }

        public bool Get(T[] elements, int offset, int length)
        {
            bool hasEnough = length <= size;
            if (hasEnough)
            {
                int available = buffer.Length - head;
                int read = available > length ? length : available;
                Array.Copy(buffer, head, elements, offset, read);
#if DEBUG
                Array.Clear(buffer, head, read);
#endif
                head = (head + read) % buffer.Length;
                int remains = length - read;
                if (remains > 0)
                {
                    Array.Copy(buffer, head, elements, offset + read, remains);
#if DEBUG
                    Array.Clear(buffer, head, remains);
#endif
                    head = (head + remains) % buffer.Length;
                }
                size -= length;
            }

            return hasEnough;
        }

    }
}
