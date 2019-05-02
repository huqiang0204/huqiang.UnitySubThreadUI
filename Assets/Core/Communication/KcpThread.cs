using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace huqiang
{
    public class LinkBuffer<T> where T : KcpLink, new()
    {
        protected T[] buffer;
        protected int max;
        protected int top;
        public bool running;
        public LinkBuffer(int size = 2048)
        {
            buffer = new T[size];
            max = size;
        }
        public T Find(int ip)
        {
            for (int i = 0; i < top; i++)
            {
                var link = buffer[i];
                if (link != null)
                    if (link.ip == ip)
                        return link;
            }
            return null;
        }
        public T Find(Int64 id)
        {
            for (int i = 0; i < top; i++)
            {
                var link = buffer[i];
                if (link != null)
                    if (link.id == id)
                        return link;
            }
            return null;
        }
        public void Add(T link)
        {
            link.Index = top;
            buffer[top] = link;
            top++;
        }
        public void Delete(KcpLink link)
        {
            int index = link.Index;
            buffer[index] = null;
            top--;
            buffer[index] = buffer[top];
            buffer[top] = null;
            if (top > 0)
                buffer[index].Index = index;
        }
        public int Count { get { return top; } }
        public void SendAll(KcpListener soc, byte[] data)
        {
            for (int i = 0; i < top; i++)
            {
                var l = buffer[i];
                if (l != null)
                    soc.Send(data, l.endpPoint);
            }
        }
        public void SendAll(KcpListener soc, byte[][] data)
        {
            for (int i = 0; i < top; i++)
            {
                var l = buffer[i];
                if (l != null)
                    for (int j = 0; j < data.Length; j++)
                        soc.Send(data[j], l.endpPoint);
            }
        }
        public void Recive()
        {
            var now = DateTime.Now.Ticks;
            for (int i = 0; i < top; i++)
            {
                var c = buffer[i];
                if (c != null)
                {
                    c.Recive(now);
                }
            }
        }
    }
    public class KcpThread<T> : LinkBuffer<T> where T : KcpLink, new()
    {
        Thread thread;
        public KcpThread(int size =2048):base (size)
        {
            running = true;
            thread = new Thread(Run);
            thread.Start();
        }
        void Run()
        {
            while (running)
            {
                var now = DateTime.Now.Ticks;
                Recive();
                long t = DateTime.Now.Ticks;
                t -= now;
                t /= 10000;
                if (t < 10)
                    Thread.Sleep(10 - (int)t);
            }
        }
    }
}
