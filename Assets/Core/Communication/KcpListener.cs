using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace huqiang
{
    public class KcpListener
    {
        public static KcpListener Instance;
        public UdpClient soc;
        Thread thread;
        protected bool running;
        int remotePort;
        int _port;
        public int Port { get { return _port; } }
        public KcpEnvelope envelope = new KcpEnvelope();
        public KcpListener(int port = 0,int remote=0)
        {
            remotePort = remote;
            _port = port;
        }
        public void Start()
        {
            if (_port == 0)
                _port = FreePort.FindNextAvailableUDPPort(10000);
            soc = new UdpClient(_port);//new IPEndPoint(IPAddress.Parse(ip),

            running = true;
            if (thread == null)
            {
                //创建消息接收线程
                thread = new Thread(Run);
                thread.Start();
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
                    Dispatch(dat,ip);
                }
                catch 
                {
                }
            }
        }
        public virtual void Dispose()
        {
            running = false;
            thread = null;
            soc.Close();
        }
        public virtual void Dispatch(byte[] dat, IPEndPoint endPoint)
        {
        }
        public void Send(byte[] data, byte type, IPEndPoint ip)
        {
            var ss = envelope.Pack(data, type);
            for (int i = 0; i < ss.Length; i++)
                soc.Send(ss[i], ss[i].Length, ip);
        }
        public void Send(byte[] data, IPEndPoint ip)
        {
            soc.Send(data, data.Length, ip);
        }
        public virtual void RemoveLink(KcpLink link)
        {
        }
    }
}
