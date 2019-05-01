using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace huqiang
{
    public class UdpSocket
    {
        UdpClient soc;
        Thread thread;
        TcpEnvelope envelope;
        IPEndPoint endPoint;
        public bool Packaging = false;
        bool running;
        bool auto;
        Queue<SocData> queue;
        public UdpSocket(int port, IPEndPoint remote, bool subThread = true, PackType type = PackType.Total, int es = 262144)
        {
           
            endPoint = remote;
            //Links = new Linker[thread * 1024];
            soc = new UdpClient(port);
            soc.Client.ReceiveTimeout = 1000;

            if (type != PackType.None)
            {
                Packaging = true;
                envelope = new TcpEnvelope(es);
                envelope.type = type;
            }
            running = true;
            auto = subThread;
            if (thread == null)
            {
                thread = new Thread(Run);
                thread.Start();
            }
            queue = new Queue<SocData>();
        }
    
        void Run()
        {
            while (running)
            {
                try
                {
                    byte[] data = soc.Receive(ref endPoint);//接收数据报
                    if (Packaging)
                    {
                        var dat= envelope.Unpack(data, data.Length);
                        if (dat != null)
                        {
                            for (int i = 0; i < dat.Count; i++)
                            {
                                var item = dat[i];
                                EnvelopeCallback(item.data, item.type);
                            }
                        }
                    }
                    else
                    {
                        EnvelopeCallback(data, 0);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        void EnvelopeCallback(byte[] data,byte tag)
        {
            if (auto)
            {
                if (MainDispatch != null)
                    MainDispatch(data, tag, endPoint);
            }
            else
            {
                SocData soc = new SocData();
                soc.data = data;
                soc.tag = tag;
                soc.obj = endPoint;
                lock (queue)
                    queue.Enqueue(soc);
            }
        }
        public Action<byte[], byte, IPEndPoint> MainDispatch;
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
                        MainDispatch(soc.data, soc.tag, soc.obj as IPEndPoint);
                }
            }
        }
        public void Close()
        {
            soc.Close();
            running = false;
        }
        public bool Send(byte[] dat, IPEndPoint point, byte tag)
        {
            try
            {
                if (Packaging)
                {
                    var buf = envelope.Pack(dat, tag);
                    if (buf != null)
                        for (int i = 0; i < buf.Length; i++)
                            soc.Send(buf[i], buf[i].Length, point);
                }
                else soc.Send(dat, dat.Length, point);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void Broadcast(byte[] dat, int port,byte tag)
        {
            var ip = new IPEndPoint(IPAddress.Broadcast, port);
            if (Packaging)
            {
                var buf = envelope.Pack(dat, tag);
                if (buf != null)
                    for (int i = 0; i < buf.Length; i++)
                        soc.Send(buf[i], buf[i].Length, ip);
            }
            else soc.Send(dat, dat.Length,ip);
            endPoint.Address = IPAddress.Any;
        }
        public void Redirect(IPAddress address)
        {
            endPoint.Address = address;
        }
    }
}