using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using huqiang.Data;

namespace UGUI
{
    public struct CharUV
    {
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
    }
    public class EmojiInfo
    {
        public int pos;
        public string chr;
        public Vector2[] uv;
    }
    public partial class EmojiMap
    {
        class CharInfo
        {
            public int len;
            public char[] dat;
            public CharUV[] uVs;
            public bool Find(char[] vs,int index, Vector2[] uv)
            {
                if (dat == null)
                    return false;
                if (index + len > vs.Length)
                    return false;
                int c = dat.Length / len;
                for (int i = 0; i < c; i++)
                {
                    int s = i*len;
                    int t = index;
                    for (int j = 0; j < len; j++)
                    {
                        if (vs[t] != dat[s])
                            goto label;
                        t++;
                        s++;
                    }
                    uv[0] = uVs[i].uv0;
                    uv[1] = uVs[i].uv1;
                    uv[2] = uVs[i].uv2;
                    uv[3] = uVs[i].uv3;
                    return true;
                label:;
                }
                return false;
            }
        }
        static CharInfo[] charInfos;
        static EmojiMap()
        {
            Initial(huqiang.Resources.Assets.EmojiInfo);
        }
        public static void Initial(byte[] data)
        {
            var db = new DataBuffer(data);
            var fake = db.fakeStruct;
            charInfos = new CharInfo[16];
            for(int i=0;i<16;i++)
            {
                FakeStruct fs = fake.GetData<FakeStruct>(i);
                if(fs!=null)
                {
                    CharInfo info = new CharInfo();
                    info.len = fs[0];
                    info.dat = db.GetArray<char>(fs[1]);
                    info.uVs = db.GetArray<CharUV>(fs[2]);
                    charInfos[i] = info;
                }
            }
        }
        public static int FindEmoji(char[] buff, int index,  Vector2[] uv)
        {
            if (index >= buff.Length)
                return 0;
            for(int i=15;i>=0;i--)
            {
                var ci = charInfos[i];
                if(ci!=null)
                {
                    if(ci.Find(buff,index,uv))
                    {
                        return i + 1;
                    }
                }
            }
            return 0;
        }

        public const char emSpace ='@' ;//'\u2001';
        public static string CheckEmoji(string str, List<EmojiInfo> list)
        {
            if (str == ""|str==null)
                return str;
            StringBuilder sb = new StringBuilder();
            char[] cc = str.ToCharArray();
            int i = 0;
            int len = cc.Length;
            int pos = 0;
            while (i < len)
            {
                char c = cc[i];
                UInt16 v = (UInt16)c;
                if (v < 0x8cff)
                {
                    i++;
                    if (v >= 0x2600 & v <= 0x27bf)
                    {
                        var pst = new EmojiInfo();
                        pst.chr = new string(c, 1);
                        pst.uv = new Vector2[4];
                        FindEmoji(cc,i, pst.uv);
                        pst.pos = pos;
                        list.Add(pst);
                        sb.Append(emSpace);
                        pos++;
                    }
                    else
                    {
                        sb.Append(c);
                        pos++;
                    }
                }
                else
                {
                    var pst = new EmojiInfo();
                    pst.pos = pos;
                    pst.uv = new Vector2[4];
                    int a = FindEmoji(cc, i, pst.uv);
                    if (a > 0)
                    {
                        pst.chr = new string(cc, i, a);
                        i += a;
                        list.Add(pst);
                        sb.Append(emSpace);
                        pos++;
                    }
                    else {
                        i++;
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }
        public static string EmojiToFullString(string str, List<EmojiInfo> list)
        {
            StringBuilder sb = new StringBuilder();
            char[] cc = str.ToCharArray();
            int s = 0;
            for(int i=0;i<cc.Length;i++)
            {
                if(cc[i]==emSpace)
                {
                    sb.Append(list[s].chr);
                    s++;
                }
                else
                {
                    sb.Append(cc[i]);
                }
            }
            return sb.ToString();
        }
    }
}