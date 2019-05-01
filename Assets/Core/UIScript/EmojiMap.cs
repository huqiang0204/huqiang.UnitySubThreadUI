using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using huqiang;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using huqiang.Data;
using System.Threading;

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
        struct DoubleChar
        {
            public UInt32 chr;
            public CharUV uv;
        }
        struct FourChar
        {
            public UInt64 chr;
            public CharUV uv;
        }
        struct EightChar
        {
            public UInt64 chr;
            public UInt64 ex;
            public CharUV uv;
        }

#if UNITY_EDITOR
        #region
        static void CalculUV(Rect sr, float w, float h, ref CharUV uv)
        {
            float x = sr.x;
            float rx = sr.width + x;
            float y = sr.y;
            float ty = sr.height + y;
            x /= w;
            rx /= w;
            y /= h;
            ty /= h;
            uv.uv0.x = x;
            uv.uv0.y = ty;
            uv.uv1.x = rx;
            uv.uv1.y = ty;
            uv.uv2.x = rx;
            uv.uv2.y = y;
            uv.uv3.x = x;
            uv.uv3.y = y;
        }
        static int UnicodeToUtf16(string code)
        {
            int uni = int.Parse(code, System.Globalization.NumberStyles.HexNumber);
            if (uni > 0x10000)
            {
                uni = uni - 0x10000;
                int vh = (uni & 0xFFC00) >> 10;
                int vl = uni & 0x3ff;
                int h = 0xD800 | vh;
                int l = 0xDC00 | vl;
                int value = h << 16 | l;
                return value;
            }
            return uni;
        }
        static byte[] buff = new byte[16];
        static List<DoubleChar> singles;
        static List<DoubleChar> doubles;
        static List<FourChar> three;
        static List<FourChar> fours;
        static List<EightChar> five;
        static List<EightChar> six;
        static List<EightChar> seven;
        private unsafe static void AddSpriteInfo(Sprite spr)
        {
            for (int i = 0; i < 16; i++)
            {
                buff[i] = 0;
            }
            string str = spr.name;
            int len = 0;
            var t = spr.uv;
            fixed (byte* bp = &buff[0])
            {
                UInt16* ip = (UInt16*)bp;
                string[] ss = str.Split('-');
                for (int j = 0; j < ss.Length; j++)
                {
                    UInt32 uni = UInt32.Parse(ss[j], System.Globalization.NumberStyles.HexNumber);
                    if (uni > 0x10000)
                    {
                        uni = uni - 0x10000;
                        UInt32 vh = (uni & 0xFFC00) >> 10;
                        UInt32 vl = uni & 0x3ff;
                        UInt32 h = 0xD800 | vh;
                        UInt32 l = 0xDC00 | vl;
                        //int value = h << 16 | l;
                        *ip = (UInt16)h;
                        ip++;
                        *ip = (UInt16)l;
                        ip++;
                        len += 2;
                    }
                    else
                    {
                        *ip = (UInt16)uni;
                        ip++;
                        len++;
                    }
                }
                if (len < 3)
                {
                    DoubleChar dc = new DoubleChar();
                    dc.chr = *(UInt32*)bp;
                    CalculUV(spr.rect, spr.texture.width, spr.texture.height, ref dc.uv);
                    if (len == 1)
                        singles.Add(dc);
                    else doubles.Add(dc);
                }
                else if (len < 5)
                {
                    FourChar dc = new FourChar();
                    dc.chr = *(UInt64*)bp;
                    CalculUV(spr.rect, spr.texture.width, spr.texture.height, ref dc.uv);
                    if (len == 3)
                        three.Add(dc);
                    else fours.Add(dc);
                }
                else
                {
                    EightChar dc = new EightChar();
                    UInt64* p = (UInt64*)bp;
                    dc.chr = *p;
                    p++;
                    dc.ex = *p;
                    CalculUV(spr.rect, spr.texture.width, spr.texture.height, ref dc.uv);
                    if (len == 5)
                        five.Add(dc);
                    else if (len == 6)
                        six.Add(dc);
                    else
                        seven.Add(dc);
                }
            }
        }

        public static void CreateMapInfo(Sprite[] sprites, string savepath)
        {
            singles = new List<DoubleChar>();
            doubles = new List<DoubleChar>();
            three = new List<FourChar>();
            fours = new List<FourChar>();
            five = new List<EightChar>();
            six = new List<EightChar>();
            seven = new List<EightChar>();
            for (int i = 0; i < sprites.Length; i++)
            {
                AddSpriteInfo(sprites[i]);
            }
            var s = singles.ToArray();
            int sl = s.Length * 36;
            var d = doubles.ToArray();
            int dl = d.Length * 36;
            var f = fours.ToArray();
            int fl = f.Length * 40;
            var e = seven.ToArray();
            int el = e.Length * 48;
            if (File.Exists(savepath))
                File.Delete(savepath);
            var fs = File.Create(savepath);
            fs.Write(new byte[4], 0, 4);
            int seg = 0;
            if (singles.Count > 0)
            { WriteTable(fs, singles.ToArray(), 36, 1); seg++; }
            if (doubles.Count > 0)
            { WriteTable(fs, doubles.ToArray(), 36, 2); seg++; }
            if (three.Count > 0)
            { WriteTable(fs, three.ToArray(), 40, 3); seg++; }
            if (fours.Count > 0)
            { WriteTable(fs, fours.ToArray(), 40, 4); seg++; }
            if (five.Count > 0)
            { WriteTable(fs, five.ToArray(), 48, 5); seg++; }
            if (six.Count > 0)
            { WriteTable(fs, six.ToArray(), 48, 6); seg++; }
            if (seven.Count > 0)
            { WriteTable(fs, seven.ToArray(), 48, 7); seg++; }
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(seg.ToBytes(), 0, 4);
            fs.Dispose();
        }
        static void WriteTable(Stream stream, Array array,Int32 structLen,Int32 charLen)
        {
            int len = array.Length * structLen;
            stream.Write(charLen.ToBytes(), 0, 4);
            stream.Write(len.ToBytes(), 0, 4);
            var tmp = new byte[len];
            Marshal.Copy(Marshal.UnsafeAddrOfPinnedArrayElement(array, 0), tmp, 0, len);
            stream.Write(tmp, 0, len);
        }
       public  static void WriteToCode(Sprite[] sprites)
        {
            singles = new List<DoubleChar>();
            doubles = new List<DoubleChar>();
            three = new List<FourChar>();
            fours = new List<FourChar>();
            five = new List<EightChar>();
            six = new List<EightChar>();
            seven = new List<EightChar>();
            for (int i = 0; i < sprites.Length; i++)
            {
                AddSpriteInfo(sprites[i]);
            }

            StringBuilder builder = new StringBuilder();
            if (singles.Count>0 )
                WriteToCode("STable",singles.ToArray(),builder);
            if(doubles.Count>0)
                WriteToCode("DTable", doubles.ToArray(), builder);
            if (three.Count>0)
                WriteToCode("TTable",three.ToArray(),builder);
            if (fours.Count>0)
                WriteToCode("FTable",fours.ToArray(),builder);
            if(five.Count>0)
                WriteToCode("FiTable", five.ToArray(), builder);
            if (six.Count>0)
                WriteToCode("SiTable",six.ToArray(),builder);
            if (seven.Count>0)
                WriteToCode("SeTable",seven.ToArray(),builder);
            var path = Environment.CurrentDirectory + "/Assets/Resources/emoji.cs";
            if (File.Exists(path))
                File.Delete(path);
           var fs=  File.Create(path);
           var tmp= Encoding.UTF8.GetBytes(  builder.ToString());
            fs.Write(tmp,0,tmp.Length);
            fs.Dispose();
        }
        static void WriteToCode(string name,DoubleChar[] table,StringBuilder builder)
        {
            builder.Append("static DoubleChar[] " );
            builder.Append(name);
            builder.Append(" = new DoubleChar[] {");
            for (int i=0;i<table.Length;i++)
            {
                builder.Append("new DoubleChar(){chr=");
                builder.Append(table[i].chr.ToString());

                builder.Append(",uv=new CharUV(){uv0=new Vector2(){x=");
                builder.Append(table[i].uv.uv0.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv0.y.ToString());
                builder.Append("f}");

                builder.Append(",uv1=new Vector2(){x=");
                builder.Append(table[i].uv.uv1.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv1.y.ToString());
                builder.Append("f}");

                builder.Append(",uv2=new Vector2(){x=");
                builder.Append(table[i].uv.uv2.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv2.y.ToString());
                builder.Append("f}");

                builder.Append(",uv3=new Vector2(){x=");
                builder.Append(table[i].uv.uv3.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv3.y.ToString());
                builder.Append("f}}},");
                builder.Append(Environment.NewLine);
            }
            builder.Append("};");
            builder.Append(Environment.NewLine);
        }
        static void WriteToCode(string name, FourChar[] table,StringBuilder builder)
        {
            builder.Append("static FourChar[] ");
            builder.Append(name);
            builder.Append(" =new FourChar[] {");
            for (int i = 0; i < table.Length; i++)
            {
                builder.Append("new FourChar(){chr=");
                builder.Append(table[i].chr.ToString());

                builder.Append(",uv=new CharUV(){uv0=new Vector2(){x=");
                builder.Append(table[i].uv.uv0.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv0.y.ToString());
                builder.Append("f}");

                builder.Append(",uv1=new Vector2(){x=");
                builder.Append(table[i].uv.uv1.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv1.y.ToString());
                builder.Append("f}");

                builder.Append(",uv2=new Vector2(){x=");
                builder.Append(table[i].uv.uv2.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv2.y.ToString());
                builder.Append("f}");

                builder.Append(",uv3=new Vector2(){x=");
                builder.Append(table[i].uv.uv3.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv3.y.ToString());
                builder.Append("f}}},");
                builder.Append(Environment.NewLine);
            }
            builder.Append("};");
            builder.Append(Environment.NewLine);
        }
        static void WriteToCode(string name, EightChar[] table,StringBuilder builder)
        {
            builder.Append("static FourChar[] ");
            builder.Append(name);
            builder.Append("=new FourChar[] {");
            for (int i = 0; i < table.Length; i++)
            {
                builder.Append("new FourChar(){chr=");
                builder.Append(table[i].chr.ToString());
                builder.Append(",ex=");
                builder.Append(table[i].ex.ToString());

                builder.Append(",uv=new CharUV(){uv0=new Vector2(){x=");
                builder.Append(table[i].uv.uv0.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv0.y.ToString());
                builder.Append("f}");

                builder.Append(",uv1=new Vector2(){x=");
                builder.Append(table[i].uv.uv1.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv1.y.ToString());
                builder.Append("f}");

                builder.Append(",uv2=new Vector2(){x=");
                builder.Append(table[i].uv.uv2.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv2.y.ToString());
                builder.Append("f}");

                builder.Append(",uv3=new Vector2(){x=");
                builder.Append(table[i].uv.uv3.x.ToString());
                builder.Append("f,y=");
                builder.Append(table[i].uv.uv3.y.ToString());
                builder.Append("f}}},");
                builder.Append(Environment.NewLine);
            }
            builder.Append("};");
            builder.Append(Environment.NewLine);
        }
        static unsafe void LoadTable(Int32* t, Int32* s, int len)
        {
            for (int i = 0; i < len; i++)
            {
                *t = *s;
                t++;
                s++;
            }
        }
        unsafe void LoadInfo(byte[] dat)
        {
            for (int i = 0; i < 7; i++)
                partLen[i] = 0;
            fixed (byte* bp = &dat[0])
            {
                byte* tp = bp;
                int ts = *(Int32*)tp;
                tp += 4;
                for (int i = 0; i < ts; i++)
                {
                    Int32 tag = *(Int32*)tp;
                    tp += 4;
                    int len = *(Int32*)tp;
                    tp += 4;
                    if (tag > 4)
                    {
                        int tl = len / 48;
                        m_len += tl;
                        var tmp = new EightChar[tl];
                        fixed (EightChar* t = &tmp[0])
                            LoadTable((Int32*)t, (Int32*)tp, len);
                        if (tag == 7)
                        { SeTable = tmp; partLen[6] = tl; }
                        else if (tag == 6)
                        { SiTable = tmp; partLen[5] = tl; }
                        else { FiTable = tmp; partLen[4] = tl; }
                    }
                    else if (tag > 2)
                    {
                        int tl = len / 40;
                        m_len += tl;
                        var tmp = new FourChar[tl];
                        fixed (FourChar* t = &tmp[0])
                            LoadTable((Int32*)t, (Int32*)tp, len);
                        if (tag == 4)
                        { FTable = tmp; partLen[3] = tl; }
                        else { TTable = tmp; partLen[2] = tl; }
                    }
                    else
                    {
                        int tl = len / 36;
                        m_len += tl;
                        var tmp = new DoubleChar[tl];
                        fixed (DoubleChar* t = &tmp[0])
                            LoadTable((Int32*)t, (Int32*)tp, len);
                        if (tag == 2)
                        { DTable = tmp; partLen[1] = tl; }
                        else { STable = tmp; partLen[0] = tl; }
                    }
                    tp += len;
                }
            }
        }
        #endregion
#endif
        static int m_len = 0;
        static int[] partLen;
        //static DoubleChar[] STable;
        //static DoubleChar[] DTable;
        static FourChar[] TTable;
        //static FourChar[] FTable;
        static EightChar[] FiTable;
        static EightChar[] SiTable;
        static EightChar[] SeTable;
 
        //1 char
        public static bool FindEmoji(UInt16 chr, Vector2[] uv)
        {
            if (STable == null)
                return false;
            for (int i = 0; i < STable.Length; i++)
            {
                if (STable[i].chr == chr)
                {
                    if (uv != null)
                    {
                        uv[0] = STable[i].uv.uv0;
                        uv[1] = STable[i].uv.uv1;
                        uv[2] = STable[i].uv.uv2;
                        uv[3] = STable[i].uv.uv3;
                    }
                    return true;
                }
            }
            return false;
        }
        //2 char
        static bool FindEmoji(UInt32 chr, Vector2[] uv, DoubleChar[] table)
        {
            if (table == null)
                return false;
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i].chr == chr)
                {
                    if (uv != null)
                    {
                        uv[0] = table[i].uv.uv0;
                        uv[1] = table[i].uv.uv1;
                        uv[2] = table[i].uv.uv2;
                        uv[3] = table[i].uv.uv3;
                    }
                    return true;
                }
            }
            return false;
        }
        //4 char
        static bool FindEmoji(UInt64 chr, Vector2[] uv, FourChar[] table)
        {
            if (table == null)
                return false;
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i].chr == chr)
                {
                    if (uv != null)
                    {
                        uv[0] = table[i].uv.uv0;
                        uv[1] = table[i].uv.uv1;
                        uv[2] = table[i].uv.uv2;
                        uv[3] = table[i].uv.uv3;
                    }
                    return true;
                }
            }
            return false;
        }
        //8 char
        static bool FindEmoji(UInt64 chr, UInt64 chr2, Vector2[] uv, EightChar[] table)
        {
            if (table == null)
                return false;
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i].chr == chr)
                    if (table[i].ex == chr2)
                    {
                        if (uv != null)
                        {
                            uv[0] = table[i].uv.uv0;
                            uv[1] = table[i].uv.uv1;
                            uv[2] = table[i].uv.uv2;
                            uv[3] = table[i].uv.uv3;
                        }
                        return true;
                    }
            }
            return false;
        }

        public static unsafe int FindEmoji(char[] buff, int index,  Vector2[] uv)
        {
            if (index >= buff.Length)
                return 0;
            int len = 7;
            if (index + len > buff.Length)
                len = buff.Length - index;
            fixed (char* cp = &buff[index])
            {
                if (len > 4)
                {
                    UInt64* p = (UInt64*)cp;
                    UInt64 chr = *p;
                    p++;
                    UInt64 chr2 = *p;
                    if (len > 6)
                    {

                        var c2 = chr2 & 0xffffffffffff;
                        if (FindEmoji(chr, c2, uv,SeTable))
                            return 7;
                    }
                    if (len > 5)
                    {
                        var c2 = chr2 & 0xffffffff;
                        if (FindEmoji(chr, c2, uv,SiTable))
                            return 6;
                    }
                    if (len > 4)
                    {
                        var c2 = chr2 & 0xffff;
                        if (FindEmoji(chr, c2, uv,FiTable))
                            return 5;
                    }

                }
                if (len > 2)
                {
                    UInt64* p = (UInt64*)cp;
                    UInt64 chr = *p;
                    if (len > 3)
                    {
                        if (FindEmoji(chr, uv,FTable))
                            return 4;
                    }
                    chr &= 0xffffffffffff;
                    if (FindEmoji(chr, uv,TTable))
                        return 4;
                }
                UInt32* ip = (UInt32*)cp;
                UInt32 c = *ip;
                if (FindEmoji(c, uv, DTable))
                    return 2;
                return 0;
            }
        }
        /// <summary>
        /// 先前查询Emoji
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="index"></param>
        /// <param name="uv"></param>
        /// <returns></returns>
        public static unsafe int FindEmojiForward(char[] buff, int index, Vector2[] uv)
        {
            if (buff == null)
                return 0;
            int len = 7;
            if (index < len)
                len = index;
            for(;len>0;len--)
            {
               switch(len)
                {
                    case 7:
                        if (FindSevenTable(buff, index - len, uv))
                            return 7;
                        break;
                    case 6:
                        if (FindSixTable(buff, index - len, uv))
                            return 6;
                        break;
                    case 5:
                        if (FindFiveTable(buff, index - len, uv))
                            return 5;
                        break;
                    case 4:
                        if (FindFourTable(buff, index - len, uv))
                            return 4;
                        break;
                    case 3:
                        if (FindThreeTable(buff, index - len, uv))
                            return 3;
                        break;
                    case 2:
                        if (FindTwoTable(buff, index - len, uv))
                            return 2;
                        break;
                    case 1:
                        if (FindEmoji(buff[index-1], uv))
                            return 1;
                        break;
                }
            }
            return 0;
        }
        unsafe static bool FindSevenTable(char[] buff, int index, Vector2[] uv)
        {
            fixed (char* cp = &buff[index ])
            {
                UInt64* p = (UInt64*)cp;
                UInt64 chr = *p;
                p++;
                UInt64 chr2 = *p;
                var c2 = chr2 & 0xffffffffffff;
                if (FindEmoji(chr, c2, uv, SeTable))
                    return true;
            }
            return false;
        }
        unsafe static bool FindSixTable(char[] buff,int index,Vector2[] uv)
        {
            fixed (char* cp = &buff[index])
            {
                UInt64* p = (UInt64*)cp;
                UInt64 chr = *p;
                p++;
                UInt64 chr2 = *p;
                var c2 = chr2 & 0xffffffff;
                if (FindEmoji(chr, c2, uv, SiTable))
                    return true;
            }
            return false;
        }
        unsafe static bool FindFiveTable(char[] buff, int index, Vector2[] uv)
        {
            fixed (char* cp = &buff[index])
            {
                UInt64* p = (UInt64*)cp;
                UInt64 chr = *p;
                p++;
                UInt64 chr2 = *p;
                var c2 = chr2 & 0xffff;
                if (FindEmoji(chr, c2, uv, FiTable))
                    return true;
            }
            return false;
        }
        unsafe static bool FindFourTable(char[] buff, int index, Vector2[] uv)
        {
            fixed (char* cp = &buff[index])
            {
                UInt64* p = (UInt64*)cp;
                UInt64 chr = *p;
                if (FindEmoji(chr, uv, FTable))
                    return true;
            }
            return false;
        }
        unsafe static bool FindThreeTable(char[] buff, int index, Vector2[] uv)
        {
            fixed (char* cp = &buff[index])
            {
                UInt64* p = (UInt64*)cp;
                UInt64 chr = *p;
                chr &= 0xffffffffffff;
                if (FindEmoji(chr, uv, TTable))
                    return true;
            }
            return false;
        }
        unsafe static bool FindTwoTable(char[] buff, int index, Vector2[] uv)
        {
            fixed (char* cp = &buff[index])
            {
                UInt32* ip = (UInt32*)cp;
                UInt32 c = *ip;
                if (FindEmoji(c, uv, DTable))
                    return true;
            }
            return false;
        }

        public const char emSpace = '\u2001';
        public static string CheckEmoji(string str, List<EmojiInfo> list)
        {
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
                        FindEmoji(v, pst.uv);
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
                    else i++;
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
        public static void CreateEmojiMesh(Text txt, List<EmojiInfo> list,List<UIVertex> EmojiVertices, List<int> EmojiTri)
        {
            float s = txt.fontSize;
            if (list != null)
            {
                int c = list.Count;
                EmojiVertices.Clear();
                EmojiTri.Clear();
                TextGenerator textGen = txt.cachedTextGenerator;
                var vs = textGen.verts;
                UIVertex vertex = new UIVertex();
                int start = 0;
                for (int j = 0; j < c; j++)
                {
                    int i = list[j].pos;
                    i *= 4;
                    if (i >= vs.Count)
                        break;
                    float x = vs[i].position.x;
                    float u = vs[i].position.y - 2f;
                    float r = vs[i + 1].position.x;
                    r = (r - x) * s * 0.5f + x;
                    float y = vs[i + 3].position.y - 2f;
                    u = (u - y) * s * 0.5f + y;
                    vertex.position.x = x;
                    vertex.position.y = u;
                    vertex.uv0 = list[j].uv[0];
                    EmojiVertices.Add(vertex);
                    i++;
                    vertex.position.x = r;
                    vertex.position.y = u;
                    vertex.uv0 = list[j].uv[1];
                    EmojiVertices.Add(vertex);
                    i++;
                    vertex.position.x = r;
                    vertex.position.y = y;
                    vertex.uv0 = list[j].uv[2];
                    EmojiVertices.Add(vertex);
                    i++;
                    vertex.position.x = x;
                    vertex.position.y = y;
                    vertex.uv0 = list[j].uv[3];
                    EmojiVertices.Add(vertex);
                    EmojiTri.Add(start);
                    EmojiTri.Add(start + 1);
                    EmojiTri.Add(start + 2);
                    EmojiTri.Add(start + 2);
                    EmojiTri.Add(start + 3);
                    EmojiTri.Add(start + 0);
                    start += 4;
                }
            }
        }
        public static char[] GetChar(int index,ref CharUV uV)
        {
            if(partLen==null)
            {
                partLen = new int[7];
                partLen[0] = STable.Length;
                partLen[1] = DTable.Length;
                partLen[3] = FTable.Length;
            }
            int i = 0;
            for(;i<7;i++)
            {
                if(index<partLen[i])
                    break;
                index -= partLen[i];
            }
            switch (i)
            {
                case 0:
                    uV= STable[index].uv;
                    return GetChar(STable[index].chr,1);
                case 1:
                    uV = DTable[index].uv;
                    return GetChar(DTable[index].chr, 2);
                //case 2:
                //    uV = TTable[index].uv;
                //    return GetChar(TTable[index].chr, 3);
                case 3:
                    uV = FTable[index].uv;
                    return GetChar(FTable[index].chr, 4);
                //case 4:
                //    uV = FiTable[index].uv;
                //    return GetChar(FiTable[index].chr, FiTable[index].ex, 5);
                //case 5:
                //    uV = SiTable[index].uv;
                //    return GetChar(SiTable[index].chr, SiTable[index].ex, 6);
                //case 6:
                //    uV = SeTable[index].uv;
                //    return GetChar(SeTable[index].chr, SeTable[index].ex, 7);
            }
            return null;
        }
        static char[] GetChar(UInt32 v,int len)
        {
            char[] tmp = new char[len];
            for(int i=0;i<len;i++)
            {
                tmp[i] =(char)( v & 0xffff);
                v >>= 16;
            }
            return tmp;
        }
        static char[] GetChar(UInt64 v,int len)
        {
            char[] tmp = new char[len];
            for (int i = 0; i < len; i++)
            {
                tmp[i] = (char)(v & 0xffff);
                v >>= 16;
            }
            return tmp;
        }
        static char[] GetChar(UInt64 v0,UInt64 v1,int len)
        {
            char[] tmp = new char[len];
            for (int i = 0; i < 4; i++)
            {
                tmp[i] = (char)(v0 & 0xffff);
                v0 >>= 16;
            }
            int e = len - 4;
            int s = 4;
            for (int i = 0; i < e; i++)
            {
                tmp[s] = (char)(v1 & 0xffff);
                s++;
                v1 >>= 16;
            }
            return tmp;
        }
        public static Int32 Length {
            get {
                return STable.Length + DTable.Length + FTable.Length;
            }
        }
    }
}