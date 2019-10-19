using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using huqiang.UI;
using huqiang.Data;

namespace UGUI
{
    public class ShareTextChild:MonoBehaviour,DataStorage
    {
        [HideInInspector]
        public int resizeTextMaxSize = 40;
        [HideInInspector]
        public int resizeTextMinSize = 14;
        [HideInInspector]
        public bool resizeTextForBestFit = false;
        [HideInInspector]
        public float scaleFactor = 1;

        public HorizontalWrapMode horizontalOverflow = HorizontalWrapMode.Wrap;
        public VerticalWrapMode verticalOverflow = VerticalWrapMode.Truncate;
        public bool generateOutOfBounds = false;
        public TextAnchor textAnchor;
        public FontStyle fontStyle = FontStyle.Normal;
        public bool richText;
        public float lineSpacing = 1;
        private int fontSize = 14;
        public Color color = Color.white;
        public bool alignByGeometry;

        protected EmojiString emojiString = new EmojiString();
        string m_Text;
        public string text
        {
            get { return m_Text; }
            set
            {
                m_Text =
                emojiString.FullString = value;
            }
        }
        public int FontSize { get => fontSize;set {
                if (value < 0)
                    value = 0;
                else if (value > 300)
                    value = 300;
                fontSize = Mathf.Clamp(value, 0, 300);
                resizeTextMinSize = fontSize;//Mathf.Clamp(0,resizeTextMinSize, fontSize);
                resizeTextMaxSize = fontSize; //Mathf.Clamp(resizeTextMaxSize, fontSize, 300);
            } }
        public void GetUVInfo(ShareText ori, List<UIVertex> vertices, Vector3 position, Quaternion quate, Vector3 scale)
        {
            var rect = transform as RectTransform;
            float w = rect.localScale.x * rect.sizeDelta.x;
            float h = rect.localScale.y * rect.sizeDelta.y;
            var pos = rect.localPosition;
            pos = quate * pos + position;
            Vector3 ls = rect.localScale;
            ls.x *= scale.x;
            ls.y *= scale.y;

            ///注意顺序quate要放前面
            var q = quate * rect.localRotation;
            
            if (gameObject.activeSelf & m_Text != null&m_Text!="")
            {
                TextGenerationSettings settings = new TextGenerationSettings();
                settings.font = ori.font;
                settings.pivot = (transform as RectTransform).pivot;
                settings.generationExtents = (transform as RectTransform).rect.size;
                settings.horizontalOverflow = horizontalOverflow;
                settings.verticalOverflow = verticalOverflow;
                settings.resizeTextMaxSize = resizeTextMaxSize;
                settings.resizeTextMinSize = resizeTextMinSize;
                settings.generateOutOfBounds = generateOutOfBounds;
                settings.resizeTextForBestFit = resizeTextForBestFit;
                settings.textAnchor = textAnchor;
                settings.fontStyle = fontStyle;
                settings.scaleFactor = scaleFactor;
                settings.richText = richText;
                settings.lineSpacing = lineSpacing;
                settings.fontSize = fontSize;
                settings.color = color;
                settings.alignByGeometry = alignByGeometry;
                var vert = ShareText.CreateEmojiMesh(ori, emojiString,ref settings);
                if(vert!=null)
                {
                    for(int i=0;i<vert.Length;i++)
                    {
                        vert[i].position= q * vert[i].position+pos;
                    }
                    vertices.AddRange(vert);
                }
            }

            for (int i = 0; i < rect.childCount; i++)
            {
                var help = rect.GetChild(i).GetComponent<ShareTextChild>();
                if (help != null)
                {
                    help.GetUVInfo(ori, vertices,  pos, q, ls);
                }
            }
        }

        public unsafe FakeStruct ToBufferData(DataBuffer data)
        {
            var fake = new FakeStruct(data,ShareTextChildData.ElementSize);
            ShareTextChildData* dat = (ShareTextChildData*)fake.ip;
            dat->alignByGeometry = alignByGeometry;
            dat->color = color;
            dat->fontSize = fontSize;
            dat->fontStyle = fontStyle;
            dat->generateOutOfBounds = generateOutOfBounds;
            dat->horizontalOverflow = horizontalOverflow;
            dat->lineSpacing = lineSpacing;
            dat->richText = richText;
            dat->textAnchor = textAnchor;
            dat->verticalOverflow = verticalOverflow;
            dat->text = data.AddData(m_Text);
            return fake;
        }
    }
}
