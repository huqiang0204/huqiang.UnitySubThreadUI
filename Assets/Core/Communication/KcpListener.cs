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
        public UdpClient soc;
        Thread thread;
        protected bool running;
        int remotePort;
        int _port;
        public int Port { get { return _port; } }
        public KcpListener(int port = 0,int remote=0)
        {
            remotePort = remote;

            _port = port;
            if(_port==0)
                _port = FreePort.FindNextAvailableUDPPort(10000);
            soc = new UdpClient( _port);//new IPEndPoint(IPAddress.Parse(ip),

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
    }
}
