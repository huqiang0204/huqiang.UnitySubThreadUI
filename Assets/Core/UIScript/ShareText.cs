using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UGUI
{
    public class ShareText : EmojiText, DataStorage
    {
        public ShareTextElement context;
        public Transform Collector;
        List<UIVertex> vertex;
        int[] tri;
        public void Update()
        {
            if (font == null)
                return;
            if (context != null)
            {
                vertex = context.OnPopulateMesh(this);
                tri = CreateTri(vertex.Count);
                SetVerticesDirty();
            }
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (font == null)
                return;
            if(context!=null)
            {
                vh.Clear();
                if (vertex != null)
                    vh.AddUIVertexStream(vertex, new List<int>(tri));
            }
            else
            {
                var trans = Collector;
                if (trans == null)
                    trans = transform;
                var vert = new List<UIVertex>();
                for (int i = 0; i < trans.childCount; i++)
                {
                    GetUVInfo(trans.GetChild(i) as RectTransform,this, vert, Vector3.zero, Quaternion.identity, Vector3.one);
                }
                var tri = CreateTri(vert.Count);
                vh.Clear();
                vh.AddUIVertexStream(vert, new List<int>(tri));
            }
        }
        public void Refresh()
        {
            SetVerticesDirty();
        }

        public FakeStruct ToBufferData(DataBuffer data)
        {
            var fake = TextElement.LoadFromObject(this,data);
            int len = fake.Length-1;
            if (Collector != null)
                fake[len] = Collector.GetInstanceID();
            return fake;
        }
        static void GetUVInfo(RectTransform child, ShareText ori, List<UIVertex> vertices, Vector3 position, Quaternion quate, Vector3 scale)
        {
            float w = child.localScale.x * child.sizeDelta.x;
            float h = child.localScale.y * child.sizeDelta.y;
            var pos = child.localPosition;
            pos = quate * pos + position;
            Vector3 ls = child.localScale;
            ls.x *= scale.x;
            ls.y *= scale.y;

            ///注意顺序quate要放前面
            var q = quate * child.localRotation;

            if (child.gameObject.activeSelf)
            {
                var stc = child.GetComponent<ShareTextChild>();
                if (stc != null)
                {
                    if (stc.text != null & stc.text != "")
                    {
                        TextGenerationSettings settings = new TextGenerationSettings();
                        settings.font = ori.font;
                        settings.pivot = child.pivot;
                        settings.generationExtents = child.sizeDelta;
                        settings.horizontalOverflow = stc.horizontalOverflow;
                        settings.verticalOverflow = stc.verticalOverflow;
                        settings.resizeTextMaxSize = stc.fontSize;
                        settings.resizeTextMinSize = stc.fontSize;
                        settings.generateOutOfBounds = stc.generateOutOfBounds;
                        settings.resizeTextForBestFit = false;
                        settings.textAnchor = stc.textAnchor;
                        settings.fontStyle = stc.fontStyle;
                        settings.scaleFactor = 1;
                        settings.richText = stc.richText;
                        settings.lineSpacing = stc.lineSpacing;
                        settings.fontSize = stc.fontSize;
                        settings.color = stc.color;
                        settings.alignByGeometry = stc.alignByGeometry;
                        var buf = CreateEmojiMesh(ori, new EmojiString(stc.text), ref settings);
                        if (buf != null)
                        {
                            for (int i = 0; i < buf.Length; i++)
                            {
                                buf[i].position = q * buf[i].position + pos;
                            }
                            vertices.AddRange(buf);
                        }
                    }
                }
                for (int i = 0; i < child.childCount; i++)
                {
                    GetUVInfo(child.GetChild(i) as RectTransform, ori, vertices, pos, q, ls);
                }
            }
        }
    }
}
