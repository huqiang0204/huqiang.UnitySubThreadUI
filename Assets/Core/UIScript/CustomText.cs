using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UGUI
{
    public class CustomText
    {
        struct UVRect
        {
            /// <summary>
            /// 左下
            /// </summary>
            public Vector2 uv0;
            /// <summary>
            /// 左上
            /// </summary>
            public Vector2 uv1;
            /// <summary>
            /// 右下
            /// </summary>
            public Vector2 uv2;
            /// <summary>
            /// 右上
            /// </summary>
            public Vector2 uv3;
            /// <summary>
            /// 比例
            /// </summary>
            public Vector2 Scale;
        }
        float width, height;
        public enum Alignment
        {
            Left, Center
        }
        public Alignment alignment;
        public CustomText(Texture t2d)
        {
            texture = t2d;
            width = t2d.width;
            height = t2d.height;
            buffer = new Dictionary<char, UVRect>();
            vert = new List<UIVertex>();
            tri = new List<int>();
        }
        List<UIVertex> vert;
        List<int> tri;
        public Texture texture { get; private set; }
        Dictionary<char, UVRect> buffer;
        public void AddCharMap(char key, Rect rect)
        {
            float x0 = rect.x / width;
            float y0 = rect.y / height;
            float x1 = (rect.x + rect.width) / width;
            float y1 = (rect.y + rect.height) / height;
            UVRect uv = new UVRect();
            uv.uv0.x = x0;
            uv.uv0.y = y0;
            uv.uv1.x = x0;
            uv.uv1.y = y1;
            uv.uv2.x = x1;
            uv.uv2.y = y0;
            uv.uv3.x = x1;
            uv.uv3.y = y1;
            uv.Scale.x = rect.width / NormalSize;
            uv.Scale.y = rect.height / NormalSize;
            buffer.Add(key, uv);
        }
        public void AddCharMap(char key, Sprite sprite)
        {
            var rect = sprite.rect;
            var t = sprite.uv;
            UVRect uv = new UVRect();
            uv.uv0 = t[3];
            uv.uv1 = t[0];
            uv.uv2 = t[2];
            uv.uv3 = t[1];
            uv.Scale.x = rect.width / NormalSize;
            uv.Scale.y = rect.height / NormalSize;
            buffer.Add(key, uv);
        }
        public float NormalSize = 24;
        public float FontSize = 24;
        public Vector2 SizeDelta = new Vector2(160, 40);
        public string text;
        public void Refresh()
        {
            if (FontSize <= 0)
                return;
            if (text == null)
                return;
            float line = FontSize * 1.25f;
            var size = SizeDelta;
            float endx = size.x * 0.5f;
            float startx = -endx;
            float starty = size.y * 0.5f;
            float endy = -starty;
            char[] tmp = text.ToArray();
            vert = new List<UIVertex>();
            tri = new List<int>();
            float dx = GetLineStart(tmp, 0, size.x);
            float dy = starty;
            for (int i = 0; i < tmp.Length; i++)
            {
                var c = tmp[i];
                if (buffer.ContainsKey(c))
                {
                    UVRect uv = buffer[c];
                    float w = uv.Scale.x * FontSize;
                    float h = uv.Scale.y * FontSize;
                label:;
                    float x0 = dx;
                    float y0 = dy - h;
                    float x1 = dx + w;
                    float y1 = dy;
                    if (y0 < endy)
                        break;
                    if (x1 > endx)
                    {
                        dy -= line;
                        dx = GetLineStart(tmp, i, size.x);
                        goto label;
                    }
                    int start = vert.Count;
                    var v = new UIVertex();
                    v.position.x = x0;
                    v.position.y = y0;
                    v.uv0 = uv.uv0;
                    vert.Add(v);
                    v.position.x = x0;
                    v.position.y = y1;
                    v.uv0 = uv.uv1;
                    vert.Add(v);
                    v.position.x = x1;
                    v.position.y = y0;
                    v.uv0 = uv.uv2;
                    vert.Add(v);
                    v.position.x = x1;
                    v.position.y = y1;
                    v.uv0 = uv.uv3;
                    vert.Add(v);
                    tri.Add(start);
                    tri.Add(start + 1);
                    tri.Add(start + 2);
                    tri.Add(start + 2);
                    tri.Add(start + 1);
                    tri.Add(start + 3);
                    dx += w;
                }
            }
        }
        public void Apply(CustomRawImage image)
        {
            if (image == null)
                return;
            image.texture = texture;
            image.uIVertices.Clear();
            image.uIVertices.AddRange(vert);
            image.triangle.Clear();
            image.triangle.AddRange(tri);
            image.Refresh();
        }
        float GetLineStart(char[] tmp, int index, float w)
        {
            switch (alignment)
            {
                case Alignment.Center:
                    float s = 0, last = 0;
                    for (; index < tmp.Length; index++)
                    {
                        var c = tmp[index];
                        if (buffer.ContainsKey(c))
                            s += buffer[c].Scale.x * FontSize;
                        if (s > w)
                            break;
                        else last = s;
                    }
                    last *= -0.5f;
                    return last;
                default:
                    return -0.5f * w;
            }

        }
    }
}
