using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Data;
using UGUI;
using UnityEngine;

namespace huqiang.UI
{
    public unsafe struct ShareTextChildData
    {
        public HorizontalWrapMode horizontalOverflow;
        public VerticalWrapMode verticalOverflow ;
        public bool generateOutOfBounds;
        public TextAnchor textAnchor;
        public FontStyle fontStyle;
        public bool richText;
        public float lineSpacing ;
        public Int32 fontSize;
        public Color color;
        public bool alignByGeometry;
        public Int32 text;
        public static int Size = sizeof(ShareTextChildData);
        public static int ElementSize = Size / 4;
    }
    public class ShareTextChildElement:DataConversion, Coloring
    {
        public ShareTextChildData data;
        private string m_str;
        public string text { get=>m_str; set { m_str = value;emojiString.FullString = value; IsChanged = true; } }
        public EmojiString emojiString = new EmojiString();
        public UIVertex[] buffer;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(ShareTextChildData*)fake.ip;
            text = fake.buffer.GetData(data.text) as string;
            model.Entity = false;
        }
        public Color color { get => data.color; set => data.color = value;}
        public override ModelElement model { get => base.model; set { base.model = value; value.ColorController = this; } }
        public override void Reset()
        {
            model.Entity = false;
        }
    }
}
