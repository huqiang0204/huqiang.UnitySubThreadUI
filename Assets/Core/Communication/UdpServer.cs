using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace huqiang
{
    public class UdpLink
    {
        public Int32 id;
        public Int32 ip;
        public Int32 port;
        public string uniId;
        public IPEndPoint endpPoint;
        public UdpEnvelope envelope;
        public long time;
    }
    public class UdpServer
    {
        UdpClient soc;
        Thread thread;
        int remotePort;
        Queue<SocData> queue;
        public bool Packaging = true;
        bool running;
        bool auto;
        PackType packType = PackType.All;
        Int16 id = 10000;
        static Int16 MinID = 11000;
        static Int16 MaxID = 21000;
        /// <summary>
        /// UdpServer构造
        /// </summary>
        /// <param name="port"></param>
        /// <param name="remote"></param>
        /// <param name="subThread"></param>
        public UdpServer(int port, int remote, bool subThread = true, PackType type = PackType.Total)
        {
            queue = new Queue<SocData>();
            packType = type;
            remotePort = remote;
            //udp服务器端口绑定
            soc = new UdpClient(port);
            running = true;
            auto = subThread;
            links = new List<UdpLink>();
            if (thread == null)
            {
                //创建消息接收线程
                thread = new Thread(Run);
                thread.Start();
            }
        }
        public void Send(byte[] dat, IPEndPoint ip, byte tag)
        {
            switch (packType)
            {
                case PackType.Part:
                    var all = Envelope.SubVolume(dat, tag, id, 1472);
                    for (int i = 0; i < all.Length; i++)
                        soc.Send(all[i], all[i].Length, ip);
                    id += (Int16)all.Length;
                    if (id > MaxID)
                        id = MinID;
                    break;
                case PackType.Total:
                    dat = Envelope.PackingInt(dat, tag);
                    soc.Send(dat, dat.Length, ip);
                    break;
                case PackType.All:
                    all = Envelope.PackAll(dat, tag, id, 1472);//1472-25
                    for (int i = 0; i < all.Length; i++)
                        soc.Send(all[i], all[i].Length, ip);
                    id += (Int16)all.Length;
                    if (id > MaxID)
                        id = MinID;
                    break;
                default:
                    soc.Send(dat, dat.Length, ip);
                    break;
            }
        }
        public void SendAll(byte[] dat, byte tag)
        {
            switch (packType)
            {
                case PackType.Part:
                    var all = Envelope.SubVolume(dat, tag, id, 1472);
                    SendAll(all);
                    id++;
                    if (id > MaxID)
                        id = MinID;
                    break;
                case PackType.Total:
                    SendAll(Envelope.PackingInt(dat, tag));
                    break;
                case PackType.All:
                    all = Envelope.PackAll(dat, tag, id, 1472);//1472-25
                    SendAll(all);
                    id++;
                    if (id > MaxID)
                        id = MinID;
                    break;
                default:
                    SendAll(dat);
                    break;
            }
        }
        void SendAll(byte[][] dat)
        {
            lock (links)
            {
                for (int i = 0; i < links.Count; i++)
                {
                    var link = links[i];
                    for (int j = 0; j < dat.Length; j++)
                        soc.Send(dat[j], dat[j].Length, link.endpPoint);
                }
            }
        }
        void SendAll(byte[] dat)
        {
            lock (links)
            {
                for (int i = 0; i < links.Count; i++)
                {
                    soc.Send(dat, dat.Length, links[i].endpPoint);
                }
            }
        }
        void Run()
        {
            while (running)
            {
                try
                {
                    IPEndPoint ip = new IPEndPoint(IPAddress.Any, remotePort);
                    byte[] dat = soc.Receive(ref ip);//接收数据报
                    var env = FindEnvelope(ip);
                    if (Packaging)
                    {
                        var data = env.envelope.Unpack(dat, dat.Length);
                        for (int i = 0; i < data.Count; i++)
                        {
                            var item = data[i];
                            EnvelopeCallback(item.data, item.type, env);
                        }
                    }
                    else
                    {
                        EnvelopeCallback(dat, 0, env);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        void EnvelopeCallback(byte[] data, byte tag, UdpLink iP)
        {
            if (auto)
            {
                if (MainDispatch != null)
                    MainDispatch(data, tag, iP);
            }
            else
            {
                SocData soc = new SocData();
                soc.data = data;
                soc.tag = tag;
                soc.obj = iP;
                lock (queue)
                    queue.Enqueue(soc);
            }

        }
        public Action<byte[], byte, UdpLink> MainDispatch;
        public void Dispatch()
        {
            if (queue != null)
            {
                int c = queue.Count;
                SocData soc;
                for (int i = 0; i < c; i++)
                {
                    lock (queue)
                        soc = queue.Dequeue();
                    if (MainDispatch != null)
                        MainDispatch(soc.data, soc.tag, soc.obj as UdpLink);
                }
            }
            ClearUnusedLink();
        }
        public void Close()
        {
            soc.Close();
            running = false;
        }
        public List<UdpLink> links;
        //设置用户的udp对象用于发送消息
        UdpLink FindEnvelope(IPEndPoint ep)
        {
            var ip = ep.Address.GetAddressBytes();
            int id = 0;
            unsafe
            {
                fixed (byte* bp = &ip[0])
                    id = *(Int32*)bp;
            }
            for (int i = 0; i < links.Count; i++)
            {
                if (id == links[i].ip)
                {
                    if (ep.Port == links[i].port)
                    {
                        links[i].time = DateTime.Now.Ticks;
                        return links[i];
                    }
                }
            }
            UdpLink link = new UdpLink();
            link.ip = id;
            link.port = ep.Port;
            link.endpPoint = ep;
            link.envelope = new UdpEnvelope();
            link.envelope.type = packType;
            link.time = DateTime.Now.Ticks;
            links.Add(link);
            return link;
        }
        /// <summary>
        /// 移除超过10秒为响应的用户
        /// </summary>
        void ClearUnusedLink()
        {
            lock (links)
            {
                var time = DateTime.Now.Ticks;
                int i = links.Count - 1;
                for (; i >= 0; i--)
                {
                    long a = time - links[i].time;
                    if (a < 0)
                        a = -a;
                    if (a > 100000000)//10*1000*10000
                        links.RemoveAt(i);
                }
            }
        }
    }
}
