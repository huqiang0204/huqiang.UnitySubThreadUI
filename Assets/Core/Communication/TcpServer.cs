using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace huqiang
{
    public class TcpServer
    {
        const int SingleCount=2048;
        Socket soc;
        /// <summary>
        /// 所有玩家的连接
        /// </summary>
        TcpLinker[] Links;
        /// <summary>
        /// 创建一个新的默认连接 参数socket
        /// </summary>
        public Func<Socket,PackType , TcpLinker> CreateModle = (s,p) => { return new TcpLinker(s,p); };
        /// <summary>
        /// 默认的派发消息
        /// </summary>
        public Action<TcpLinker, byte[]> DispatchMessage = (o, e) => { };
        /// <summary>
        /// 单例服务器实例
        /// </summary>
        public static TcpServer Instance;

        Thread server;
        Thread[] threads;
        PackType packType;
        IPEndPoint endPoint;
        int ThreadCount;
        public TcpServer(string ip, int port,PackType type = PackType.All, int thread = 8)
        {
            ThreadCount = thread;
            packType = type;
            Links = new TcpLinker[thread * SingleCount];
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            soc.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //端点
            endPoint= new IPEndPoint(IPAddress.Parse(ip), port);
            //绑定
            try
            {
                soc.Bind(endPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            soc.Listen(0);
            Instance = this;
            threads = new Thread[thread];
            for (int i = 0; i < thread; i++)
            {
                threads[i] = new Thread(Run);
                threads[i].Start(i);
            }
        }
        public void Start()
        {
            if(server==null)
            {
                server = new Thread(AcceptClient);
                server.Start();
            }
            if(threadTimer==null)
            {
                threadTimer = new ThreadTimer();
                threadTimer.Interal = 1000;
                threadTimer.Tick = (o, e) => {
                    try
                    {
                        Heartbeat();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                };
            }
        }
        ThreadTimer threadTimer;
        Int32 id = 100000;
        byte[] nil = { 0 };
        public void Dispose()
        {
            soc.Disconnect(true);
            server.Abort();
            for (int i = 0; i < threads.Length; i++)
                threads[i].Abort();
            if (threadTimer != null)
                threadTimer.Dispose();
        }
        void AcceptClient()
        {
            while (true)
            {
                try
                {
                    var client = soc.Accept();
                    for (int i = 0; i < Links.Length; i++)
                    {
                        if (Links[i] == null)
                        {
                            Links[i] = CreateModle(client, packType);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }
        void Run(object index)
        {
            int os = (int)index;
            while (true)
            {
                var now = DateTime.Now;
                long a = now.Ticks;
                int s = os;
                for (int i = 0; i < SingleCount; i++)
                {
                    var c = Links[s];
                    if (c != null)
                    {
                        c.Recive();
                    }
                    s +=ThreadCount;
                }
                long t = DateTime.Now.Ticks;
                t -= a;
                t /= 10000;
                if (t < 10)
                    Thread.Sleep(10 - (int)t);
            }
        }

        /// <summary>
        /// 统计tcp连接
        /// </summary>
        void StatisticsTcp()
        {
            int c = 0;
            for (int i = 0; i < Links.Length; i++)
            {
                if (Links[i] != null)
                {
                    c++;
                }
            }
        }
        /// <summary>
        /// 给用户发送心跳
        /// </summary>
        void Heartbeat()
        {
            int max = threads.Length * SingleCount;
            for (int i = 0; i < max; i++)
            {
                var link = Links[i];
                if (link != null)
                {
                    if (link.Send(nil) < 0)
                    {
                        Links[i] = null;
                        link.Dispose();
                    }
                }
            }
        }
        /// <summary>
        /// 广播所有在线用户
        /// </summary>
        /// <param name="action"></param>
        public void Broadcasting(Action<TcpLinker> action)
        {
            int max = threads.Length * SingleCount;
            for (int i = 0; i < max; i++)
            {
                var link = Links[i];
                if (link != null)
                    action(link);
            }
        }
    }
}