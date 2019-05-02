using System;
using System.Collections.Generic;
using System.Text;

namespace huqiang
{
    public class KcpEnvelope
    {
        class DataItem
        {
            public Int16 id;
            public long time;
            public byte[] dat;
        }
        int delayStart;
        int delayEnd;
        Int16[] delays = new short[256];
        public static Int16 MinID = 22000;
        public static Int16 MaxID = 32000;
        List<DataItem> sendBuffer = new List<DataItem>();
        public List<byte[]> ValidateData = new List<byte[]>();
        protected Int16 id = 22000;
        protected Int16 Fragment = 1472;
        protected Int16 sss = 1401;
        protected EnvelopeItem[] pool = new EnvelopeItem[128];
        protected int remain = 0;
        protected byte[] buffer;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffLen">256kb</param>
        public KcpEnvelope(int buffLen = 262144)
        {
            buffer = new byte[buffLen];
        }
        public byte[][] Pack(byte[] dat, byte type)
        {
            var tmp = Envelope.PackAll(dat, type,id,Fragment);
            long now = DateTime.Now.Ticks;
            for(int i=0;i<tmp.Length;i++)
            {
                DataItem item = new DataItem();
                item.id = id;
                id++;
                if (id>=MaxID)
                    id = MinID;
                item.dat = tmp[i];
                item.time = now;
                sendBuffer.Add(item);
            }
            return tmp;
        }
        protected void ClearTimeout(long now)
        {
            for (int i = 0; i < 128; i++)
            {
                if (pool[i].head.MsgID > 0)
                    if (now - pool[i].time > 20 * 10000000)//清除超时20秒的消息
                        pool[i].head.MsgID = 0;
            }
        }
        public  List<EnvelopeData> Unpack(byte[] dat, int len,long now)
        {
            try
            {
                ClearTimeout(now);
                var list = Envelope.UnpackInt(dat, len, buffer, ref remain);
                var dats = Envelope.EnvlopeDataToPart(list);
                int c = dats.Count - 1;
                for (; c >= 0; c--)
                {
                    var item = dats[c];
                    Int16 tag = item.head.Type;
                    byte type = (byte)(tag);
                    if (type == EnvelopeType.Success)
                    {
                        Success(item.head.PartID,now);
                        dats.RemoveAt(c);
                    }
                    else
                    {
                        ReciveOk(item.head.PartID);
                    }
                }
                return OrganizeSubVolume(dats, 1401);
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
                    if (ap > 1)
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
                                if (Envelope.SetChecked(pool[i].checks, item.head.CurPart))
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
        public void Clear()
        {
            remain = 0;
            for (int i = 0; i < 128; i++)
            {
                pool[i].head.MsgID = 0;
            }
            sendBuffer.Clear();
        }
        
        void Success(Int16 _id,long now)
        {
            for(int i=0;i<sendBuffer.Count;i++)
                if(sendBuffer[i].id==_id)
                {
                    now -= sendBuffer[i].time;
                    now /= 10000;
                    delays[delayEnd]=(Int16)now;
                    if (delayEnd < 255)
                        delayEnd++;
                    else delayEnd = 0;
                    sendBuffer.RemoveAt(i);
                    break;
                }
        }
        public int Delay { get {
                int a = 0;
                int i = 0;
                for (; i < 256; i++)
                {
                    if (delayStart == delayEnd)
                        break;
                    a += delays[delayStart];
                    delays[delayStart] = 0;
                    if (delayStart < 255)
                        delayStart++;
                    else delayStart = 0;
                }
                if (i == 0)
                    return 0;
                return a / i;
            } }
        /// <summary>
        /// 获取超时数据
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public byte[][] GetFailedData(long now, long timeout = 5000000)
        {
            List<byte[]> tmp = new List<byte[]>();
            lock (sendBuffer)
                for (int i = 0; i < sendBuffer.Count; i++)
                {
                    var dat = sendBuffer[i];
                    if (dat != null)
                        if (now - dat.time > timeout)
                        {
                            dat.time += timeout;
                            tmp.Add(dat.dat);
                        }
                }
            return tmp.ToArray();
        }
        void ReciveOk(Int16 _id)
        {
            var tmp = Envelope.PackAll(new byte[2], 128,_id,Fragment)[0];
            ValidateData.Add(tmp);
        }
    }
}
