using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace huqiang.Data
{
    public class StringBuffer
    {
        public List<string> buffer=new List<string>();
        string[] StringAssets;
        public  byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                for (int i = 0; i < buffer.Count; i++)
                {
                    var str = buffer[i];
                    var buff = Encoding.UTF8.GetBytes(str);
                    var lb = buff.Length.ToBytes();
                    ms.Write(lb, 0, 4);
                    ms.Write(buff, 0, buff.Length);
                }
                return ms.ToArray();
            }
        }
        public string this[int index]
        {
            get
            {
                if (StringAssets == null)
                    return null;
                if (index < 0 | index > StringAssets.Length)
                    return null;
                return StringAssets[index];
            }
        }
        public unsafe byte* LoadStringAsset(byte[] dat, byte* bp)
        {
            byte* p = bp;
            int len = *(int*)p;
            StringAssets = new string[len];
            p += 4;
            for (int i = 0; i < len; i++)
            {
                int c = *(int*)p;
                p += 4;
                int offset = (int)(p - bp);
                StringAssets[i] = Encoding.UTF8.GetString(dat, offset, c);
                p += c;
            }
            return p;
        }
        public int AddString(string str)
        {
            if (str == null)
                return -1;
            int index = buffer.IndexOf(str);
            if (index < 0)
            {
               index = buffer.Count;
               buffer.Add(str);
            }
            return index;
        }
    }
}
