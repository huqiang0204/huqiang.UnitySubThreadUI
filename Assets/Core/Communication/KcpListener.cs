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
        public Socket soc;
        public ThreadEx thread;
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
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, _port);
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //new UdpClient(_port);//new IPEndPoint(IPAddress.Parse(ip),
            soc.Bind(ip);
            soc.ReceiveTimeout = 1000;
            running = true;
            if (thread == null)
            {
                //创建消息接收线程
                thread = new ThreadEx(Run);
                thread.Start();
            }
        }
        void Run()
        {
            byte[] buffer = new byte[65536];
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
            while (running)
            {
                try
                {
                    EndPoint end = ip;
                    int len = 0;
                    try
                    {
                        len = soc.ReceiveFrom(buffer, ref end);//接收数据报
                    }
                    catch {
                        //System.Diagnostics.Debug.WriteLine("time out");
                    } 
                    if(len>0)
                    {
                        byte[] dat = new byte[len];
                        for (int i = 0; i < len; i++)
                            dat[i] = buffer[i];
                        Dispatch(dat, end as IPEndPoint);
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.Log(ex.StackTrace);
                }
            }
        }
        public virtual void Dispose()
        {
            running = false;
            thread = null;
#if UNITY_WSA
            soc.Dispose();
#else
            soc.Close();
#endif
        }
        public virtual void Dispatch(byte[] dat, IPEndPoint endPoint)
        {
        }
        public void Send(byte[] data, byte type, IPEndPoint ip)
        {
            var ss = envelope.Pack(data, type);
            for (int i = 0; i < ss.Length; i++)
                soc.SendTo(ss[i],  ip);
        }
        public void Send(byte[] data, IPEndPoint ip)
        {
            soc.SendTo(data,  ip);
        }
        public virtual void RemoveLink(KcpLink link)
        {
        }
    }
}
