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
        class CharInfo
        {
            public int len;
            public char[] dat;
            public CharUV[] uVs;
            public bool Find(char[] vs,int index, ref  CharUV uv)
            {
                if (dat == null)
                    return false;
                int c = dat.Length / len;
                for (int i = 0; i < c; i++)
                {
                    int s = i;
                    int t = index;
                    for (int j = 0; j < len; j++)
                    {
                        if (vs[t] != dat[s])
                            goto label;
                        t++;
                        s++;
                    }
                    uv.uv0 = uVs[i].uv0;
                    uv.uv1 = uVs[i].uv1;
                    uv.uv2 = uVs[i].uv2;
                    uv.uv3 = uVs[i].uv3;
                    return true;
                label:;
                }
                return false;
            }
        }
        
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
        class CharInfoA
        {
            public int len;
            public List<char> dat=new List<char>();
            public List< CharUV> uvs=new List<CharUV>();
        }
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
        private unsafe static int AddSpriteInfo(Sprite spr)
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
            }
            return len;
        }

        public static void CreateMapInfo(Sprite[] sprites, string savepath)
        {
            CharInfoA[] tmp = new CharInfoA[7];
            for(int i=0;i<7;i++)
            {
                tmp[i] = new CharInfoA();
                tmp[i].len = i + 1;
            }
            CharUV uv = new CharUV();
            unsafe
            {
                fixed(byte* bp=&buff[0])
                {
                    for (int i = 0; i < sprites.Length; i++)
                    {
                        var sp = sprites[i];
                        int len = AddSpriteInfo(sp);
                        var dat = tmp[len].dat;
                        char* cp = (char*)bp;
                        for (int j = 0; j < len; j++)
                        {
                            dat.Add(*cp);
                            cp++;
                        }
                        CalculUV(sp.rect,sp.texture.width,sp.texture.height,ref uv);
                        tmp[len].uvs.Add(uv);
                    }
                }
            }
            DataBuffer db = new DataBuffer();
            FakeStruct fake = new FakeStruct(db,7);
            for(int i=0;i<7;i++)
            {
                FakeStruct fs = new FakeStruct(db,3);
                fs[0] = tmp[i].len;
                if(tmp[i].dat.Count>0)
                {
                    fs[1] = db.AddArray<char>(tmp[i].dat.ToArray());
                    fs[2] = db.AddArray<CharUV>(tmp[i].uvs.ToArray());
                }
                fake.SetData(i,fs);
            }
           byte[] data = db.ToBytes();
            File.WriteAllBytes(savepath,data);
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
        unsafe void LoadInfo(byte[] dat)
        {
     
        }
        #endregion
#endif
        static int m_len = 0;
        static int[] partLen;
 
        public static unsafe int FindEmoji(char[] buff, int index,  Vector2[] uv)
        {
            if (index >= buff.Length)
                return 0;
            int len = 7;
            return 0;
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
            return 0;
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
    }
}