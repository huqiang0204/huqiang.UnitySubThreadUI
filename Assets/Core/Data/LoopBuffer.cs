using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Data
{
    public class LoopBuffer<T> where T : class
    {
        T[] buffer;
        int mlen;
        int point;
        public int Lenth { get { return mlen; } }
        public LoopBuffer(int len)
        {
            buffer = new T[len];
            mlen = len;
            point = 0;
        }
        public void Push(T t)
        {
            buffer[point]=t;
            if (point >= mlen - 1)
                point = 0;
            else point++;
        }
        public T this[int index]
        {
            get
            {
                if (index < 0)
                    return null;
                if (index >= mlen)
                    return null;
                index += point;
                if (index >= mlen)
                    index -= mlen;
                return buffer[index];
            }
            set
            {
                if (index < 0)
                    return;
                if (index >= mlen)
                    return;
                index += point;
                if (index >= mlen)
                    index -= mlen;
                buffer[index] = value;
            }
        }
        public void Clear()
        {
            for (int i = 0; i < mlen; i++)
                buffer[i] = null;
        }
    }
}
