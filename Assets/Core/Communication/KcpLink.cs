using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace huqiang
{
    public class KcpLink
    {
        protected KcpServer kcp;
        public int Index;
        public KcpLink(KcpServer listener)
        {
            kcp = listener;
        }
        public Int32 id;
        public Int32 ip;
        public Int32 port;
        public string uniId;
        public IPEndPoint endpPoint;
        public KcpEnvelope envelope = new KcpEnvelope();
        public long time;
        public QueueBuffer<byte[]> metaData = new QueueBuffer<byte[]>();
        /// <summary>
        /// 5秒
        /// </summary>
        public static long TimeOut = 50000000;
        protected long lastTime;
        internal bool _connect;
        public bool Connected { get { return _connect; } }
        public void Recive(long time)
        {
            int c = metaData.Count;
            byte[][] tmp = new byte[c][];
            for (int i = 0; i < c; i++)
                tmp[i] = metaData.Dequeue();
            if (c == 0)
            {
                if (time - lastTime > TimeOut)
                {
                    envelope.Clear();
                    Disconnect();
                    _connect = false;
                }
            }
            else {
                lastTime = time;
                if (!_connect)
                    ConnectionOK();
                _connect = true;
            }
            for (int i = 0; i < c; i++)
            {
                var list = envelope.Unpack(tmp[i], tmp[i].Length);
                try
                {
                    if(list!=null)
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
            var ss= envelope.GetFailedData(time);
            if(ss!=null)
                for(int i=0;i<ss.Length;i++)
                    kcp.soc.Send(ss[i],ss[i].Length,endpPoint);
            ss = envelope.ValidateData.ToArray();
            envelope.ValidateData.Clear();
            for (int i = 0; i < ss.Length; i++)
                kcp.soc.Send(ss[i], ss[i].Length, endpPoint);
        }
        public void Send(byte[] data, byte type)
        {
            var ss = envelope.Pack(data, type);
            for (int i = 0; i < ss.Length; i++)
                kcp.soc.Send(ss[i], ss[i].Length,endpPoint);
        }
        public virtual void Disconnect()
        {
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
    }
}
