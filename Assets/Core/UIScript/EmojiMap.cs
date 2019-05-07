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
        static CharInfo[] charInfos;
        public static void Initial(byte[] data)
        {
            var db = new DataBuffer(data);
            var fake = db.fakeStruct;
            charInfos = new CharInfo[7];
            for(int i=0;i<7;i++)
            {
                FakeStruct fs = fake.GetData<FakeStruct>(i);
                if(fs!=null)
                {
                    CharInfo info = new CharInfo();
                    info.len = fs[0];
                    info.dat = db.GetArray<char>(1);
                    info.uVs = db.GetArray<CharUV>(2);
                    charInfos[i] = info;
                }
            }
        }
        public static int FindEmoji(char[] buff, int index,  Vector2[] uv)
        {
            if (index >= buff.Length)
                return 0;
            CharUV cv = new CharUV();
            for(int i=6;i>=0;i--)
            {
                var ci = charInfos[i];
                if(ci!=null)
                {
                    if(ci.Find(buff,index,ref cv))
                    {
                        return i + 1;
                    }
                }
            }
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