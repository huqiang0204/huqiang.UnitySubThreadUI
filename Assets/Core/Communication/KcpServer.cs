using System;
using System.Net;

namespace huqiang
{
    public class KcpServer<T>:KcpListener where T:KcpLink,new()
    {
        public static int SingleCount = 2048;
        public LinkBuffer<T>[] linkBuff;
        int maxLink;
        int tCount=0;
        public Int32 allLink;
        public KcpServer(int port = 0, int remote = 0) :base(port,remote)
        {
            Instance = this;
        }
        public void Run(int threadCount = 8,int threadbuff = 2048)
        {
            tCount = threadCount;
            if(tCount>0)
            {
                linkBuff = new KcpThread<T>[threadCount];
                for (int i = 0; i < threadCount; i++)
                    linkBuff[i] = new KcpThread<T>(threadbuff);
            }
            else
            {
                tCount = 1;
                linkBuff = new LinkBuffer<T>[1];
                linkBuff[0] = new LinkBuffer<T>();
            }
            Start();
        }
        public void Close()
        {
            soc.Close();
        }
        //设置用户的udp对象用于发送消息
        public T FindOrCreateLink(IPEndPoint ep)
        {
            var b = ep.Address.GetAddressBytes();
            int ip = 0;
            unsafe
            {
                fixed (byte* bp = &b[0])
                    ip = *(Int32*)bp;
            }
            var link = FindLink(ip);
            if (link == null)
            {
                link = new T();
                link.envelope = new KcpEnvelope();
                link.kcp = this;
                link.ConnectTime = DateTime.Now.Ticks;
                link._connect = true;
                int s = 0;
                int c = linkBuff[0].Count;
                for (int i = 1; i < tCount; i++)
                {
                    if (c > linkBuff[i].Count)
                    {
                        s = i;
                        c = linkBuff[i].Count;
                    }
                }
                link.buffIndex = s;
                linkBuff[s].Add(link);
            }
            return link;
        }
        public override void Dispatch(byte[] dat, IPEndPoint ep)
        {
            var link = FindOrCreateLink(ep);
            if(link!=null)
            {
                link.metaData.Enqueue(dat);
            }
        }
        public override void RemoveLink(KcpLink link)
        {
            linkBuff[link.buffIndex].Delete(link);
            maxLink--;
        }
        public override void Dispose()
        {
            base.Dispose();
            soc.Close();
            Instance = null;
            for (int i = 0; i < tCount; i++)
                linkBuff[i].running = false;
        }
        public T FindLink(Int64 id)
        {
            for (int i = 0; i < tCount; i++)
            {
                var l = linkBuff[i].Find(id);
                if (l != null)
                    return l;
            }
            return null;
        }
        public T FindLink(int ip)
        {
            for (int i = 0; i < tCount; i++)
            {
                var l = linkBuff[i].Find(ip);
                if (l != null)
                    return l;
            }
            return null;
        }
        ThreadTimer timer;
        static byte[] Heart = new byte[1];
        public void OpenHeart()
        {
            if(timer==null)
            {
                timer = new ThreadTimer(1000);
                timer.Tick = (o,e) => {
                    try
                    {
                        var dat = envelope.Pack(Heart, EnvelopeType.Mate);
                        for(int i=0;i<tCount;i++)
                        {
                            linkBuff[i].SendAll(this, dat);
                        }
                    }
                    catch
                    {
                    }
                };
            }
        }
        public void CloseHeart()
        {
            if(timer!=null)
            {
                timer.Dispose();
                timer = null;
            }
        }
    }
}
