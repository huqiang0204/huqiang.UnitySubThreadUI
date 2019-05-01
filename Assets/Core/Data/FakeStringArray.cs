using System;
using System.IO;
using System.Linq;
using System.Text;


namespace huqiang.Data
{
    public class FakeStringArray : ToBytes
    {
        int len;
        public int Length { get { return len; } }
        static byte[] zreo = new byte[4];
        string[] buf;
        public string[] Data { get { return buf; } }
        public FakeStringArray(int size)
        {
            buf = new string[size];
            len = size;
        }
        public FakeStringArray(string[] dat)
        {
            buf = dat;
            len = dat.Length;
        }
        public string this[int index]
        {
            set
            {
                buf[index] = value;
            }
            get
            {
                return buf[index];
            }
        }
        public unsafe FakeStringArray(DataBuffer data, Int32* point, int len)
        {
            buf = new string[len];
            for (int i = 0; i < len; i++)
            {
                int c = *point;
                point++;
                if (c > 0)
                {
                    byte* bp = (byte*)point;
                    if (data.AddressDetection(bp, c))
                        buf[i] = Encoding.UTF8.GetString(Tool.GetByteArray(bp, c));
                    bp += c;
                    point = (Int32*)bp;
                }
            }
        }
        public byte[] ToBytes()
        {
            if (buf == null)
                return zreo;
            int len = buf.Length;
            MemoryStream ms = new MemoryStream();
            var tmp = len.ToBytes();
            ms.Write(tmp, 0, 4);
            for (int i = 0; i < len; i++)
            {
                var str = buf[i];
                if (str == null)
                    ms.Write(zreo, 0, 4);
                else
                {
                    tmp = Encoding.UTF8.GetBytes(str);
                    int c = tmp.Length;
                    ms.Write(c.ToBytes(), 0, 4);
                    ms.Write(tmp, 0, c);
                }
            }
            tmp = ms.ToArray();
            ms.Dispose();
            return tmp;
        }
    }
}
