using System;
using System.Collections.Generic;
using System.Text;

namespace huqiang
{
    public struct EnvelopeItem
    {
        public EnvelopeHead head;
        public Int32 part;
        public Int32 rcvLen;
        public byte[] buff;
        public long time;
        public Int32[] checks;
    }
    public class TcpEnvelope
    {
        public  static Int16 MinID = 22000;
        public static Int16 MaxID = 32000;

        public PackType type = PackType.All;
        protected EnvelopeItem[] pool = new EnvelopeItem[128];
        protected int remain = 0;
        protected byte[] buffer;
        protected Int16 id = 22000;
        protected Int16 Fragment = 1460;
        /// <summary>
        /// Solution Slices Segment
        /// </summary>
        protected Int16 sss = 1389;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffLen">256kb</param>
        public TcpEnvelope(int buffLen = 262144)
        {
            buffer = new byte[buffLen];
        }
        public virtual byte[][] Pack(byte[] dat, byte tag)
        {
            var all = Envelope.Pack(dat, tag, type, id,Fragment);
            id += (Int16)all.Length;
            if (id >= MaxID)
                id = MinID;
            return all;
        }
        public virtual List<EnvelopeData> Unpack(byte[] dat, int len)
        {
            try
            {
                ClearTimeout();
                switch (type)
                {
                    case PackType.Part:
                        return OrganizeSubVolume(Envelope.UnpackPart(dat, len, buffer, ref remain, Fragment), Fragment - 16);
                    case PackType.Total:
                        return Envelope.UnpackInt(dat, len, buffer, ref remain);
                    case PackType.All:
                        var list = Envelope.UnpackInt(dat, len, buffer, ref remain);
                        return OrganizeSubVolume(Envelope.EnvlopeDataToPart(list), sss);
                }
                return null;
            }
            catch
            {
                remain = 0;
                return null;
            }
         
        }
        protected List<EnvelopeData> OrganizeSubVolume(List<EnvelopePart> list, int fs)
        {
            if (list != null)
            {
                List<EnvelopeData> datas = new List<EnvelopeData>();
                for (int j = 0; j < list.Count; j++)
                {
                    var item = list[j];
                    int ap = item.head.AllPart;
                    if (ap> 1)
                    {
                        int s = -1;
                        for (int i = 0; i < 128; i++)
                        {
                            if (s < 0)
                            {
                                if (pool[i].head.MsgID == 0)
                                    s = i;
                            }
                            if (item.head.MsgID == pool[i].head.MsgID)
                            {
                                if(Envelope.SetChecked(pool[i].checks, item.head.CurPart))
                                {
                                    Envelope.CopyToBuff(pool[i].buff, item.data, 0, item.head, fs);
                                    pool[i].part++;
                                    pool[i].rcvLen += item.head.PartLen;
                                    if (pool[i].rcvLen >= item.head.Lenth)
                                    {
                                        EnvelopeData data = new EnvelopeData();
                                        data.data = pool[i].buff;
                                        data.type = (byte)(pool[i].head.Type);
                                        pool[i].head.MsgID = 0;
                                        datas.Add(data);
                                    }
                                }
                                goto label;
                            }
                        }
                        pool[s].head = item.head;
                        pool[s].part = 1;
                        pool[s].rcvLen = item.head.PartLen;
                        pool[s].buff = new byte[item.head.Lenth];
                        pool[s].time = DateTime.Now.Ticks;
                        Envelope.CopyToBuff(pool[s].buff, item.data, 0, item.head, fs);
                        int c = ap / 32 + 1;
                        pool[s].checks = new Int32[c];
                    }
                    else
                    {
                        EnvelopeData data = new EnvelopeData();
                        data.data = item.data;
                        data.type = (byte)(item.head.Type);
                        datas.Add(data);
                    }
                label:;
                }
                return datas;
            }
            return null;
        }
        protected void ClearTimeout()
        {
            var now = DateTime.Now.Ticks;
            for (int i = 0; i < 128; i++)
            {
                if (pool[i].head.MsgID > 0)
                    if (now - pool[i].time > 20 * 10000000)//清除超时20秒的消息
                        pool[i].head.MsgID = 0;
            }
        }
        public virtual void Clear()
        {
            remain = 0;
            for (int i = 0; i < 128; i++)
            {
                pool[i].head.MsgID = 0;
            }
        }
    }
    public class UdpEnvelope:TcpEnvelope
    {
        public UdpEnvelope()
        {
            Fragment = 1472;
            sss = 1401;
        }
    }
}
