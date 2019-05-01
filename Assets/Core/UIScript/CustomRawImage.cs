using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UGUI
{
    public class CustomRawImage : RawImage
    {
        static int[] tri_rect = new int[] { 0, 1, 3, 3, 1, 2 };
        static void Reset(CustomRawImage image)
        {
            var vert = image.uIVertices;
            vert.Clear();
            var size = image.rectTransform.sizeDelta;
            float x1 = size.x * 0.5f;
            float x0 = -x1;
            float y1 = size.y * 0.5f;
            float y0 = -y1;
            UIVertex vertex = new UIVertex();
            vertex.position = new Vector3(x0, y0, 0);
            vertex.uv0 = Vector2.zero;
            vert.Add(vertex);
            vertex = new UIVertex();
            vertex.position = new Vector3(x0, y1, 0);
            vertex.uv0 = new Vector2(0, 1);
            vert.Add(vertex);
            vertex = new UIVertex();
            vertex.position = new Vector3(x1, y1, 0);
            vertex.uv0 = new Vector2(1, 1);
            vert.Add(vertex);
            vertex = new UIVertex();
            vertex.position = new Vector3(x1, y0, 0);
            vertex.uv0 = new Vector2(1, 0);
            vert.Add(vertex);

            image.triangle.Clear();
            image.triangle.AddRange(tri_rect);
        }
        [SerializeField]
        public Sprite spriteA;
        [SerializeField]
        public Sprite spriteB;
        public object DataContext;
        public List<UIVertex> uIVertices { get; set; }
        public List<int> triangle { get; set; }
        public bool Custom;
        bool fresh;
        public void Refresh()
        {
            SetVerticesDirty();
        }
        public CustomRawImage()
        {
            uIVertices = new List<UIVertex>();
            triangle = new List<int>();
            triangle.AddRange(tri_rect);
        }
        Sprite m_sprite;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (uIVertices.Count == 0)
            {
                base.OnPopulateMesh(vh);
            }
            else
            {
                vh.Clear();
                vh.AddUIVertexStream(uIVertices, triangle);
            }
        }
        public void SetSprite(Sprite sprite, bool nativeSize = true)
        {
            if (sprite == null)
                return;
            m_sprite = sprite;
            texture = sprite.texture;
            var r = m_sprite.rect;
            float w = m_sprite.texture.width;
            float h = m_sprite.texture.height;
            uvRect = new Rect(r.x / w, r.y / h, r.width / w, r.height / h);
            if (nativeSize)
            {
                SetSpriteNativeSize(this);
                SetVerticesDirty();
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
                Reset();
            }
        }
        static void SetSpriteNativeSize(CustomRawImage image)
        {
            var sprite = image.m_sprite;
            if (sprite == null)
                return;
            var w = sprite.rect.width;
            var h = sprite.rect.height;
            image.rectTransform.sizeDelta = new Vector2(w, h);
            Vector2 piovt = sprite.pivot;
            float x = sprite.rect.x;
            float y = sprite.rect.y;
            float x0 = 0 - piovt.x;
            float x1 = x0 + w;
            float y0 = 0 - piovt.y;
            float y1 = y0 + h;

            float u0 = x;
            float v0 = y;
            float u1 = w + u0;
            float v1 = h + v0;
            w = image.texture.width;
            h = image.texture.height;
            u0 /= w;
            u1 /= w;
            v0 /= h;
            v1 /= h;
            var vert = image.uIVertices;
            vert.Clear();
            UIVertex vertex = new UIVertex();
            vertex.position = new Vector3(x0, y0, 0);
            vertex.uv0 = new Vector2(u0, v0);
            vert.Add(vertex);
            vertex = new UIVertex();
            vertex.position = new Vector3(x0, y1, 0);
            vertex.uv0 = new Vector2(u0, v1);
            vert.Add(vertex);
            vertex = new UIVertex();
            vertex.position = new Vector3(x1, y1, 0);
            vertex.uv0 = new Vector2(u1, v1);
            vert.Add(vertex);
            vertex = new UIVertex();
            vertex.position = new Vector3(x1, y0, 0);
            vertex.uv0 = new Vector2(u1, v0);
            vert.Add(vertex);
        }
        /// <summary>
        /// 重置顶点
        /// </summary>
        public void Reset(bool sprite = false)
        {
            Reset(this);
            SetVerticesDirty();
        }
    }
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
