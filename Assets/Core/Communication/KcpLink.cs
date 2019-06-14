using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace huqiang
{
    public class KcpLink:NetworkLink
    {
        internal KcpListener kcp;
        public Int64 id;
        public string uniId;

        public KcpEnvelope envelope = new KcpEnvelope();
     
        public QueueBuffer<byte[]> metaData = new QueueBuffer<byte[]>();
        /// <summary>
        /// 5秒
        /// </summary>
        public static long TimeOut = 50000000;//5*1000*10000
        /// <summary>
        /// 第一次连接时间
        /// </summary>
        public long ConnectTime;
        /// <summary>
        /// 最后一次接收到数据的时间
        /// </summary>
        protected long lastTime;
        internal bool _connect;
        public bool Connected { get { return _connect; } }
        public override void Recive(long now)
        {
            int c = metaData.Count;
            byte[][] tmp = new byte[c][];
            lock (metaData)
                for (int i = 0; i < c; i++)
                    tmp[i] = metaData.Dequeue();
            if (c == 0)
            {
                if (now - lastTime > TimeOut)
                {
                    envelope.Clear();
                    Disconnect();
                }
            }
            else {
                lastTime = now;
                if (!_connect)
                    ConnectionOK();
                _connect = true;
            }
            for (int i = 0; i < c; i++)
            {
                var list = envelope.Unpack(tmp[i], tmp[i].Length,now);
                try
                {
                    if (list != null)
                        for (int j = 0; j < list.Count; j++)
                    {
                        var dat = list[j];
                        Dispatch(dat.data, dat.type);
                    }
                }
                catch
                {
                }
            }
            var ss= envelope.GetFailedData(now);//获取超时未反馈的数据
            if(ss!=null)
                for(int i=0;i<ss.Length;i++)
                    kcp.soc.SendTo(ss[i],endpPoint);//重新发送超时的数据
            ss = envelope.ValidateData.ToArray();
            envelope.ValidateData.Clear();//获取接收成功的数据
            for (int i = 0; i < ss.Length; i++)
                kcp.soc.SendTo(ss[i],  endpPoint);//通知对方接收数据成功
        }
        public void Send(byte[] data, byte type)
        {
            try
            {
                var ss = envelope.Pack(data, type);
                for (int i = 0; i < ss.Length; i++)
                    kcp.soc.SendTo(ss[i], endpPoint);
            }catch(Exception ex)
            {
            }
        }
        public virtual void Awake()
        {
        }
        public virtual void Connect()
        {
        }
        public virtual void Disconnect()
        {
            kcp.RemoveLink(this);
        }
        public virtual void ConnectionOK()
        {
        }
        public virtual void Dispatch(byte[] dat, byte tag)
        {
        }
        public virtual void Dispose()
        {
        }
        public void Redirect(int ciP,int cport)
        {
            endpPoint.Address = new IPAddress(ciP.ToBytes());
            endpPoint.Port = cport;
            ip = ciP;
            port = cport;
            envelope.Clear();
            metaData.Clear();
        }
    }
}
