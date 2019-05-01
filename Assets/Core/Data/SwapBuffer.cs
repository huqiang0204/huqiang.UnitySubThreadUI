using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huqiang.Data
{
    public class SwapBuffer<T, U> where T : class
    {
        T[] A;
        T[] B;
        int length;
        int maxA;
        int maxB;
        public SwapBuffer(int len)
        {
            length = len;
            A = new T[len];
            B = new T[len];
        }
        public T Exchange(Func<T, U, bool> condition, U u)
        {
            for (int i = 0; i < maxA; i++)
            {
                var t = A[i];
                if (condition(t, u))
                {
                    B[maxB] = t;
                    maxB++;
                    maxA--;
                    A[i] = A[maxA];
                    return t;
                }
            }
            return null;
        }
        public void Add(T t)
        {
            if (maxB < length)
            {
                B[maxB] = t;
                maxB++;
            }
        }
        public T[] Done()
        {
            var t = A;
            A = B;
            B = t;
            if (maxA > 0)
            {
                T[] tmp = new T[maxA];
                for (int i = 0; i < maxA; i++)
                    tmp[i] = t[i];
                maxA = maxB;
                maxB = 0;
                return tmp;
            }
            maxA = maxB;
            maxB = 0;
            return null;
        }
        public void Clear()
        {
            for (int i = 0; i < length; i++)
            {
                A[i] = null;
                B[i] = null;
            }
            maxA = 0;
            maxB = 0;
        }
    }
}
