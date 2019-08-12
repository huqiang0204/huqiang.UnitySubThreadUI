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
        public object DataContext;
        public List<UIVertex> uIVertices;
        public List<int> triangle;
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
}
