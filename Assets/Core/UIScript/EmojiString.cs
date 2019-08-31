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
        /// <summary>
        /// 过滤表情符后的字符串
        /// </summary>
        public string FilterString { get { return m_str; } }
        /// <summary>
        /// 表情符信息
        /// </summary>
        public List<EmojiInfo> emojis=new List<EmojiInfo>();
        StringBuilder builder=new StringBuilder();
        /// <summary>
        /// 完整字符串
        /// </summary>
        public string FullString { get { return f_str; }
            set {
                if (value == null)
                {
                    f_str = "";
                    m_str = "";
                    emojis.Clear();
                    builder.Clear();
                }
                else
                {
                    f_str = value;
                    emojis.Clear();
                    m_str = EmojiMap.CheckEmoji(f_str, emojis);
                    builder.Clear();
                    builder.Append(m_str);
                }
            } }
        public int Length { get { return m_str.Length; } }
        public EmojiString()
        {
        }
        public EmojiString(string str)
        {
            m_str = EmojiMap.CheckEmoji(str,emojis);
            builder.Append(m_str);
            f_str = str;
        }
        public void Remove(int index,int count = 1)
        {
            if (index + count > builder.Length)
                count = builder.Length - index;
            if (count < 1)
                return;
            int start = index;
            int end = index + count;
            emojis.RemoveAll((o) => { return o.pos >= start & o.pos <= end; });
            for (int i = 0; i < emojis.Count; i++)
            {
                if (emojis[i].pos > index)
                    emojis[i].pos -= count;
            }
            builder.Remove(index,count);
            m_str = builder.ToString();
            f_str = EmojiMap.EmojiToFullString(m_str,emojis);
        }
        public String SubString(int index,int count=1)
        {
            if (count < 1)
                return "";
            StringBuilder tmp = new StringBuilder();
            for(int i=0;i<count;i++)
            {
                string chr = null;
                for(int j=0;j<emojis.Count;j++)
                {
                    if (emojis[j].pos == index)
                        chr = emojis[j].chr;
                    else if (emojis[j].pos > i)
                        break;
                }
                if (chr == null)
                    tmp.Append(m_str[index]);
                else tmp.Append(chr);
                index++;
            }
            return tmp.ToString();
        }
        public void Insert(int index, EmojiString emoji)
        {
            if (index > m_str.Length)
                index = m_str.Length;
            var list = emoji.emojis;
            for (int i = 0; i < list.Count; i++)
                list[i].pos += index;
            string str = emoji.FilterString;
            builder.Insert(index, str);
            int l = str.Length;
            for (int i = 0; i < emojis.Count; i++)
                if (emojis[i].pos >= index)
                    emojis[i].pos += l;
            emojis.AddRange(list);
            emojis.Sort((a, b) => { return a.pos > b.pos ? 1 : -1; });
            m_str = builder.ToString();
            f_str = EmojiMap.EmojiToFullString(m_str, emojis);
        }
    }
}
