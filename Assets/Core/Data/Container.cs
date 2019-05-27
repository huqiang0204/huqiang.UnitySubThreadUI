﻿using System;


namespace huqiang.Data
{
    public class Container<T> where T : class
    {
        int top = 0;
        T[] buffer;
        int mlen;
        public int Count { get { return top; } }
        public T this[int index]
        {
            get
            {
                if (index < 0)
                    return null;
                if (index >= top)
                    return null;
                return buffer[index];
            }
            set
            {
                if (index < 0)
                    return;
                if (index >= top)
                    return;
                buffer[index] = value;
            }
        }
        public Container(int len = 2048)
        {
            buffer = new T[len];
            mlen = len;
        }
        public void Add(T data)
        {
            buffer[top] = data;
            top++;
        }
        public void RemoveAt(int index)
        {
            if (index < 0)
                return;
            if (index >= top)
                return;
            top--;
            buffer[index] = buffer[top];
            buffer[top] = null;
        }
        public void Remove(T data)
        {
            for (int i = 0; i < top; i++)
            {
                if (buffer[i] == data)
                {
                    top--;
                    buffer[i] = buffer[top];
                    buffer[top] = null;
                    return;
                }
            }
        }
        public void Remove(Func<T, bool> action)
        {
            if (action == null)
                return;
            for (int i = 0; i < top; i++)
            {
                if (action(buffer[i]))
                {
                    top--;
                    buffer[i] = buffer[top];
                    buffer[top] = null;
                    return;
                }
            }
        }
        public int FindIndex(Func<T, bool> action)
        {
            if (action == null)
                return -1;
            for (int i = 0; i < top; i++)
                if (action(buffer[i]))
                    return i;
            return -1;
        }
        public T Find(Func<T, bool> action)
        {
            if (action == null)
                return null;
            for (int i = 0; i < top; i++)
                if (action(buffer[i]))
                    return buffer[i];
            return null;
        }
        public T FindAndSwap(Func<T, bool> action, int index)
        {
            if (action == null)
                return null;
            for (int i = 0; i < top; i++)
                if (action(buffer[i]))
                {
                    var t = buffer[index];
                    var r = buffer[i];
                    buffer[i] = t;
                    buffer[index] = r;
                    return r;
                }
            return null;
        }
        public void Swap(int source, int target)
        {
            var s = buffer[source];
            var t = buffer[target];
            buffer[source] = t;
            buffer[target] = s;
        }
        public void Clear()
        {
            for (int i = 0; i < top; i++)
            {
                buffer[i] = null;
            }
            top = 0;
        }
        public void Sort(Func<T, T, bool> com)
        {
            for (int i = 0; i < top; i++)
            {
                var t = buffer[i];
                int s = i;
                for (int j = i + 1; j < top; j++)
                {
                    if (com(t, buffer[j]))
                    {
                        t = buffer[j];
                        s = j;
                    }
                }
                var u = buffer[i];
                buffer[i] = t;
                buffer[s] = u;
            }
        }
    }
}
