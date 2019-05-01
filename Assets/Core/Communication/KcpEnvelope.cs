using System;
using System.Collections.Generic;
using System.Text;

namespace huqiang
{
    public class KcpEnvelope : TcpEnvelope
    {
        class DataItem
        {
            public Int16 id;
            public long time;
            public byte[] dat;
        }
        List<DataItem> sendBuffer = new List<DataItem>();
        public List<byte[]> ValidateData = new List<byte[]>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffLen">256kb</param>
        public KcpEnvelope(int buffLen = 262144)
        {
            Fragment = 1472;
            sss = 1401;
        }
        public override byte[][] Pack(byte[] dat, byte type)
        {
            var tmp = Envelope.PackAll(dat, type, id, Fragment);
            long now = DateTime.Now.Ticks;
            for (int i = 0; i < tmp.Length; i++)
            {
                DataItem item = new DataItem();
                item.id = id;
                id++;
                if (id >= MaxID)
                    id = MinID;
                item.dat = tmp[i];
                item.time = now;
                sendBuffer.Add(item);
            }
            return tmp;
        }
        public override List<EnvelopeData> Unpack(byte[] dat, int len)
        {
            try
            {
                ClearTimeout();
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
                        Success(item.head.PartID);
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
        public override void Clear()
        {
            remain = 0;
            for (int i = 0; i < 128; i++)
            {
                pool[i].head.MsgID = 0;
            }
            sendBuffer.Clear();
        }
        void Success(Int16 _id)
        {
            lock (sendBuffer)
                for (int i = 0; i < sendBuffer.Count; i++)
                    if (sendBuffer[i].id == _id)
                    {
                        sendBuffer.RemoveAt(i);
                        break;
                    }
        }
        /// <summary>
        /// 获取超时数据
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public byte[][] GetFailedData(long timeout = 10000)
        {
            long now = DateTime.Now.Ticks;
            List<byte[]> tmp = new List<byte[]>();
            lock (sendBuffer)
                for (int i = 0; i < sendBuffer.Count; i++)
                {
                    var dat = sendBuffer[i];
                    if (dat != null)
                        if (now - dat.time > timeout)
                        {
                            dat.time -= timeout;
                            tmp.Add(dat.dat);
                        }
                }
            return tmp.ToArray();
        }
        void ReciveOk(Int16 _id)
        {
            var tmp = Envelope.PackAll(new byte[2], 128, _id, Fragment)[0];
            ValidateData.Add(tmp);
        }
    }
}
