using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UGUI
{
    public class EmojiString
    {
        string m_str="";
        string f_str = "";
        public string FilterString { get { return m_str; } }
        public List<EmojiInfo> emojis;
        public string FullString { get { return f_str; }
            set {
                f_str = value;
                emojis.Clear();
                m_str= EmojiMap.CheckEmoji(f_str, emojis);
            } }
        public int Length { get { return m_str.Length; } }
        public EmojiString()
        {
            emojis = new List<EmojiInfo>();
        }
        public EmojiString(string str)
        {
            emojis = new List<EmojiInfo>();
            m_str = EmojiMap.CheckEmoji(str,emojis);
        }
        void RemoveEmoji(int start,int end)
        {
            int c = emojis.Count-1;
            for(;c>=0;c--)
            {
                var e = emojis[c];
                if(e.pos==start)
                {
                    emojis.RemoveAt(c);
                    continue;
                }
                if(e.pos>start&e.pos<end)
                {
                    emojis.RemoveAt(c);
                    continue;
                }
            }
        }
        public void RemoveAt(int index,int count=1)
        {
            if (index + count > m_str.Length)
                count = m_str.Length - index;
            if (count < 1)
                return;
            int start = index;
            int end = index + count;
            RemoveEmoji(start, end);
            m_str = m_str.Remove(index,count);
            f_str = EmojiMap.EmojiToFullString(m_str,emojis);
        }
        public void Insert(int index, string str)
        {
            if (index > m_str.Length)
                index = m_str.Length;
            int offset = 0;
            for(int i=0;i<emojis.Count;i++)
            {
                var e = emojis[i];
                if (index > e.pos)
                    offset += e.chr.Length;
            }
            emojis.Clear();
            int os = index + offset;
            if (os > f_str.Length)
                os = f_str.Length;
            f_str = f_str.Insert(os,str);
            m_str = EmojiMap.CheckEmoji(f_str,emojis);
        }
    }
}
