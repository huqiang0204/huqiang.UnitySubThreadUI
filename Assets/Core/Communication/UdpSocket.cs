using huqiang.Data;
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
        Socket soc;
#if UNITY_WSA
        System.Threading.Tasks.Task thread;
#else
         Thread thread;
#endif
        TcpEnvelope envelope;
        IPEndPoint endPoint;
        public bool Packaging = false;
        bool running;
        bool auto;
        QueueBuffer<SocData> queue;
        public UdpSocket(int port, IPEndPoint remote, bool subThread = true, PackType type = PackType.Total, int es = 262144)
        {
            endPoint = remote;
            //Links = new Linker[thread * 1024];
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //new UdpClient(_port);//new IPEndPoint(IPAddress.Parse(ip),
            soc.Bind(ip);
            soc.ReceiveTimeout = 1000;

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
#if UNITY_WSA
                thread = System.Threading.Tasks.Task.Run(Run);
#else
                thread = new Thread(Run);
               thread.Start();
#endif
            }
            queue = new QueueBuffer<SocData>();
        }
    
        void Run()
        {
            byte[] buffer = new byte[65536];
            while (running)
            {
                try
                {
                    EndPoint end = endPoint;
                    int len = soc.ReceiveFrom(buffer, ref end);//接收数据报
                    if (len > 0)
                    {
                        byte[] dat = new byte[len];
                        for (int i = 0; i < len; i++)
                            dat[i] = buffer[i];
                        if (Packaging)
                        {
                            var data = envelope.Unpack(dat, len);
                            for (int i = 0; i < data.Count; i++)
                            {
                                var item = data[i];
                                EnvelopeCallback(item.data, item.type);
                            }
                        }
                        else
                        {
                            EnvelopeCallback(dat, 0);
                        }
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
                    soc = queue.Dequeue();
                    if (soc != null)
                        if (MainDispatch != null)
                            MainDispatch(soc.data, soc.tag, soc.obj as IPEndPoint);
                }
            }
        }
        public void Close()
        {
#if UNITY_WSA
           soc.Dispose();
#else
            soc.Close();
#endif
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
                            soc.SendTo(buf[i],  point);
                }
                else soc.SendTo(dat, point);
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
                        soc.SendTo(buf[i], ip);
            }
            else soc.SendTo(dat, ip);
            endPoint.Address = IPAddress.Any;
        }
        public void Redirect(IPAddress address)
        {
            endPoint.Address = address;
        }
    }
}