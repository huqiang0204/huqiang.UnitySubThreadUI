using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Data
{
    public class QueueBuffer<T> where T:class
    {
        int start = 0;
        int end = 0;
        T[] buffer;
        int mlen;
        public QueueBuffer(int len =2048)
        {
            buffer = new T[len];
            mlen = len;
        }
        public int BufferLenth { get { return mlen; } }
        public int Count {
            get {
                int a = end - start;
                if (a < 0)
                    a += mlen;
                return a;
            }
        }
        public void Enqueue(T t)
        {
            buffer[end] = t;
            if (end >= mlen - 1)
                end = 0;
            else end++;
        }
        public T Dequeue()
        {
            if (start != end)
            {
                int a = start;
                T t = buffer[a];
                if (start >= mlen - 1)
                    start = 0;
                else start++;
                buffer[a] = null;
                return t;
            }
            return null;
        }
        public void Clear()
        {
            start = end;
            for (int i = 0; i < mlen; i++)
                buffer[i] = null;
        }
    }
}
