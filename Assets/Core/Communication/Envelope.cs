using System;
using System.Collections.Generic;

namespace huqiang
{
    public struct EnvelopeHead
    {
        /// <summary>
        /// 数据压缩类型
        /// </summary>
        public Int16 Type;
        /// <summary>
        /// 此消息的id
        /// </summary>
        public Int16 MsgID;
        /// <summary>
        /// 此消息的分卷id
        /// </summary>
        public Int16 PartID;
        /// <summary>
        /// 此消息的某个分卷
        /// </summary>
        public Int16 CurPart;
        /// <summary>
        /// 此消息总计分卷
        /// </summary>
        public Int16 AllPart;
        /// <summary>
        /// 此消息分卷长度
        /// </summary>
        public Int16 PartLen;
        /// <summary>
        /// 此此消息总计长度
        /// </summary>
        public Int32 Lenth;
    }
    public enum PackType
    {
        None,
        Part,//分卷，速度快，容错低,只有包头
        Total,//整体发送,分包头包尾，效率低，不适合大数据
        All//分卷发送,分包头包尾，效率低
    }
    public class EnvelopeData
    {
        public byte[] data;
        public byte type;
    }
    public class EnvelopePart
    {
        public EnvelopeHead head;
        public byte[] data;
    }
    public class Envelope
    {
        public static unsafe EnvelopeHead ReadHead(byte[] buff, int index)
        {
            fixed (byte* b = &buff[index])
            {
                return *(EnvelopeHead*)b;
            }
        }
        public static unsafe void WriteHead(byte[] buff, int index, EnvelopeHead head)
        {
            fixed (byte* b = &buff[index])
            {
                *(EnvelopeHead*)b = *&head;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="buff">源数据</param>
        /// <param name="len">数据长度</param>
        /// <param name="buffer">缓存数据</param>
        /// <param name="remain"></param>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static List<EnvelopePart> UnpackPart(byte[] buff, int len, byte[] buffer, ref int remain, int fs)
        {
            fs -= 16;
            List<EnvelopePart> list = new List<EnvelopePart>();
            int s = remain;
            for (int i = 0; i < len; i++)
            {
                buffer[s] = buff[i];
                s++;
            }
            len += remain;
            int index = 0;
            for (int i = 0; i < 1024; i++)
            {
                EnvelopeHead head = ReadHead(buffer, index);
                if (head.PartLen > fs)
                {
                    remain = 0;
                    break;
                }
                if (index + 16 + head.PartLen > len)
                {
                    remain = len - index;
                    for (int j = 0; j < len; j++)
                    {
                        buffer[j] = buffer[index];
                        index++;
                    }
                    return list;
                }
                index += 16;
                if (head.Lenth > 2)
                {
                    EnvelopePart part = new EnvelopePart();
                    part.head = head;
                    int l = (int)head.PartLen;
                    var buf = new byte[l];
                    int a = index;
                    for (int j = 0; j < l; j++)
                    {
                        buf[j] = buffer[a]; a++;
                    }
                    part.data = buf;
                    list.Add(part);
                }
                index += head.PartLen;
                if (index >= len)
                {
                    remain = 0;
                    break;
                }
            }
            return list;
        }

        /// <summary>
        /// 对数据进行分卷, 标头4字节，总长度4字节, 当前分卷2字节，总分卷2字节，当前分卷长度4字节，总计16字节
        /// </summary>
        /// <param name="buff">需要打包的数据</param>
        /// <param name="type">数据类型</param>
        /// <param name="id">数据包标志</param>
        /// <param name="fs">每个分卷大小</param>
        /// <returns></returns>
        public static byte[][] SubVolume(byte[] buff, byte type, Int16 id , Int16 fs)
        {
            fs -= 16;
            if (buff.Length < 2)
                buff = new byte[2];
            int len = buff.Length;
            int part = len / fs;
            int r = len % fs;
            Int16 allPart = (Int16)part;
            if (r > 0)
                allPart++;
            byte[][] buf = new byte[allPart][];
            Int16 msgId = id;
            EnvelopeHead head = new EnvelopeHead();
            for (int i = 0; i < part; i++)
            {
                head.MsgID = msgId;
                head.Type = type;
                head.PartID = id;
                head.CurPart = (Int16)i;
                head.AllPart = allPart;
                head.PartLen = fs;
                head.Lenth = len;
                byte[] tmp = new byte[fs + 16];
                WriteHead(tmp, 0, head);
                buf[i] = EnvelopePart(buff, tmp, i, fs, fs);
                id++;
            }
            if (r > 0)
            {
                head.MsgID = msgId;
                head.Type = type;
                head.PartID = id;
                head.CurPart = (Int16)part;
                head.AllPart = allPart;
                head.PartLen = (Int16)r;
                head.Lenth = len;
                byte[] tmp = new byte[r + 16];
                WriteHead(tmp, 0, head);
                buf[part] = EnvelopePart(buff, tmp, part, r, fs);
            }
            return buf;
        }
        static byte[] EnvelopePart(byte[] buff, byte[] tmp, int part, int partLen, int fs)
        {
            int index = part * fs;
            int start = 16;
            for (int j = 0; j < partLen; j++)
            {
                tmp[start] = buff[index];
                start++;
                index++;
            }
            return tmp;
        }

        #region 以Int为单元进行封包解包,带宽损耗3%
        /// <summary>
        /// 包头=255,255,255,255,包尾=255,255,255,254
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="type">数据类型</param>
        /// <returns></returns>
        public static byte[] PackingInt(byte[] dat, byte type)
        {
            int len = dat.Length;
            int part = len / 124;
            int r = len % 124;
            int tl = part * 128;
            if (r > 0)
            {
                part++;
                tl += r + 4;
            }
            byte[] buf = new byte[tl + 9];//包头4字节,类型1字节，包尾4字节
            buf[0] = 255;
            buf[1] = 255;
            buf[2] = 255;
            buf[3] = 255;
            buf[4] = type;
            int s0 = 0;
            int s1 = 5;
            for (int i = 0; i < part; i++)
            {
                PackingInt(dat, s0, buf, s1);
                s0 += 124;
                s1 += 128;
            }
            buf[tl + 5] = 255;
            buf[tl + 6] = 255;
            buf[tl + 7] = 255;
            buf[tl + 8] = 254;
            return buf;
        }
        static void PackingInt(byte[] src, int si, byte[] tar, int ti)
        {
            int st = si + 1;
            int tt = ti + 5;
            for (int i = 0; i < 124; i++)
            {
                tar[tt] = src[st];
                st++;
                if (st >= src.Length)
                    break;
                tt++;
            }
            int a = 0;
            int s = ti + 4;
            for (int i = 0; i < 31; i++)
            {
                byte b = src[si];
                if (b > 127)
                {
                    a |= 1 << i;
                    b -= 128;
                }
                tar[s] = b;
                si += 4;
                if (si >= src.Length)
                    break;
                s += 4;
            }
            tar[ti] = (byte)a;
            a >>= 8;
            tar[ti + 1] = (byte)a;
            a >>= 8;
            tar[ti + 2] = (byte)a;
            a >>= 8;
            tar[ti + 3] = (byte)a;

        }
        static void UnpackInt(byte[] src, int si, byte[] tar, int ti)
        {
            int a = src.ReadInt32(si);
            int st = si + 4;
            int tt = ti;
            for (int i = 0; i < 124; i++)
            {
                if (tt >= tar.Length)
                    break;
                tar[tt] = src[st];
                st++;
                tt++;
            }
            si += 4;
            for (int i = 0; i < 31; i++)
            {
                if (ti >= tar.Length)
                    break;
                byte b = src[si];
                if ((a & 1) > 0)
                    b += 128;
                a >>= 1;
                tar[ti] = b;
                ti += 4;
                si += 4;
            }
        }
        static byte[] UnpackInt(byte[] dat, int start, int end)
        {
            int len = end - start;
            int part = len / 128;
            int r = len % 128;
            int tl = part * 124;
            if (r > 0)
            {
                part++;
                tl += r;
                tl -= 4;
            }
            byte[] buf = new byte[tl];
            int s0 = start;
            int s1 = 0;
            for (int i = 0; i < part; i++)
            {
                UnpackInt(dat, s0, buf, s1);
                s0 += 128;
                s1 += 124;
            }
            return buf;
        }
        public static List<EnvelopeData> UnpackInt(byte[] dat, int len, byte[] buffer, ref int remain)
        {
            List<EnvelopeData> list = new List<EnvelopeData>();
            int s = remain;
            for (int i = 0; i < len; i++)
            {
                buffer[s] = dat[i];
                s++;
            }
            len += remain;
            s = 0;
            int e = 0;
            for (int i = 0; i < len; i++)
            {
                var b = buffer[i];
                if (b == 255)
                {
                    if (i + 3 < buffer.Length)
                    {
                        if (buffer[i + 1] == 255)
                            if (buffer[i + 2] == 255)
                            {
                                b = buffer[i + 3];
                                if (b == 255)
                                {
                                    s = i + 5;
                                }
                                else if (b == 254)
                                {
                                    e = i;
                                    EnvelopeData data = new EnvelopeData();
                                    data.data = UnpackInt(buffer, s, e);
                                    data.type = buffer[s - 1];
                                    list.Add(data);
                                }
                            }
                    }
                }
            }
            if (e != 0)
            {
                remain = len - e - 4;
                for (int i = 0; i < remain; i++)
                {
                    buffer[i] = buffer[e];
                    e++;
                }
            }
            return list;
        }
        #endregion
        static byte[] ReadPart(byte[] data, out EnvelopeHead head)
        {
            head = ReadHead(data, 0);
            int len = data.Length - 16;
            if (len >= head.PartLen)
            {
                byte[] buf = new byte[head.PartLen];
                int start = 16;
                for (int i = 0; i < head.PartLen; i++)
                {
                    buf[i] = data[start];
                    start++;
                }
                return buf;
            }
            return null;
        }
        /// <summary>
        /// 读取数据的包头,将数据解析成分段数据
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static List<EnvelopePart> EnvlopeDataToPart(List<EnvelopeData> datas)
        {
            if (datas == null)
                return null;
            int c = datas.Count;
            List<EnvelopePart> parts = new List<EnvelopePart>();
            for (int i = 0; i < c; i++)
            {
                EnvelopePart part = new EnvelopePart();
                var buf = ReadPart(datas[i].data, out part.head);
                if (buf != null)
                {
                    part.data = buf;
                    parts.Add(part);
                }
            }
            return parts;
        }
        /// <summary>
        /// 当数据量较大,使用分卷
        /// Tcp每个数据包大小1460字节,Udp每个数据包大小1472字节
        /// Tcp Part->1460-16=1444,All->1460-9-16=1435.Udp All->1472-9-16=1447
        /// </summary>
        /// <param name="dat"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[][] PackAll(byte[] dat, byte type, Int16 msgId ,Int16 fs)
        {
            int c = fs;
            c -= 9;
            c *= 124;
            c /= 128;
            fs =(Int16) c;
            var buf = SubVolume(dat, type, msgId, fs);
            if (buf != null)
                for (int i = 0; i < buf.Length; i++)
                    buf[i] = PackingInt(buf[i], type);
            return buf;
        }
        public static byte[][] Pack(byte[] dat, byte tag, PackType type, Int16 msgID,Int16 fs)
        {
            switch (type)
            {
                case PackType.Part:
                    return SubVolume(dat, tag, msgID,fs);
                case PackType.Total:
                    return new byte[][] { PackingInt(dat, tag)};
                case PackType.All:
                    return PackAll(dat, tag, msgID,fs);
            }
            return null;
        }
    }
}
