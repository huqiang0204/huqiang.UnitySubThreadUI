﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UGUI
{
    public class EmojiText:Text
    {
        public static Texture Emoji;
        public static float NormalDpi = 96;
        public Action<EmojiText, VertexHelper> OnPopulate;
        List<EmojiInfo> list = new List<EmojiInfo>();
        readonly UIVertex[] m_TempVerts = new UIVertex[4];
        static UIVertex[] CreateEmojiMesh(IList<UIVertex> verts,  List<EmojiInfo> emoji, float fontSize, float unitsPerPixel,Vector2 roundingOffset)
        {
            int vertCount = verts.Count - 4;
            if (vertCount <= 0)
                return null;
            UIVertex[] buf = new UIVertex[vertCount];
            for (int i=0;i<vertCount;i++)
            {
                buf[i] = verts[i];
                buf[i].position *= unitsPerPixel;
                buf[i].position.x += roundingOffset.x;
                buf[i].position.y += roundingOffset.y;
                buf[i].uv0.x *= 0.5f;
            }
            int c = 0;
            if (emoji != null)
                c = emoji.Count;
            for (int j = 0; j < c; j++)
            {
                int i = emoji[j].pos;
                i *= 4;
                if (i >= vertCount)
                    break;
                float x = buf[i].position.x;
                float u = buf[i].position.y - 2f;
                float r = buf[i + 1].position.x;
                r = (r - x) * fontSize * 0.5f + x;
                float y = buf[i + 3].position.y - 2f;
                u = (u - y) * fontSize * 0.5f + y;
                buf[i].position.x = x;
                buf[i].position.y = u;
                buf[i].uv0 = emoji[j].uv[0];
                buf[i].uv0.x *= 0.5f;
                buf[i].uv0.x += 0.5f;
                i++;
                buf[i].position.x = r;
                buf[i].position.y = u;
                buf[i].uv0 = emoji[j].uv[1];
                buf[i].uv0.x *= 0.5f;
                buf[i].uv0.x += 0.5f;
                i++;
                buf[i].position.x = r;
                buf[i].position.y = y;
                buf[i].uv0 = emoji[j].uv[2];
                buf[i].uv0.x *= 0.5f;
                buf[i].uv0.x += 0.5f;
                i++;
                buf[i].position.x = x;
                buf[i].position.y = y;
                buf[i].uv0 = emoji[j].uv[3];
                buf[i].uv0.x *= 0.5f;
                buf[i].uv0.x += 0.5f;
            }
            return buf;
        }
        public static List<UIVertex> CreateEmojiMesh(Text text,  List<EmojiInfo> emojis)
        {
            float s = Screen.dpi / NormalDpi;
            Vector2 extents = text.rectTransform.rect.size;
            var settings = text.GetGenerationSettings(extents * s);

            float t = settings.fontSize;
            t *= s;
            settings.fontSize = (int)t;
            t = settings.resizeTextMinSize;
            t *= s;
            settings.resizeTextMinSize = (int)t;
            t = settings.resizeTextMaxSize;
            t *= s;
            settings.resizeTextMaxSize = (int)t;
            var txt = text.text;
            if (txt != null & txt != "")
            {
                string str = EmojiMap.CheckEmoji(txt, emojis);

                text.cachedTextGenerator.PopulateWithErrors(str, settings, text.gameObject);

                IList<UIVertex> verts = text.cachedTextGenerator.verts;

                float unitsPerPixel = 1 / text.pixelsPerUnit / s;
                Vector2 roundingOffset = new Vector2(verts[0].position.x / s, verts[0].position.y / s) * unitsPerPixel;
                roundingOffset = text.PixelAdjustPoint(roundingOffset) - roundingOffset;
                var vs = CreateEmojiMesh(text.cachedTextGenerator.verts, emojis, text.fontSize, unitsPerPixel, roundingOffset);

                if (vs != null)
                {
                    return new List<UIVertex>(vs);
                }
            }
            return null;
        }
        public static int[] CreateTri(int len)
        {
            int c = len / 4;
            if (c < 0)
                return null;
            int max = c * 6;
            int[] tri = new int[max];
            for(int i=0;i<c;i++)
            {
                int p = i * 4;
                int s = i * 6;
                tri[s] = p;
                s++;
                tri[s] = p + 1;
                s++;
                tri[s] = p + 2;
                s++;
                tri[s] = p + 2;
                s++;
                tri[s] = p + 3;
                s++;
                tri[s] = p;
            }
            return tri;
        }
        protected override void OnPopulateMesh(VertexHelper vertex)
        {
            if (font == null)
                return;
            m_DisableFontTextureRebuiltCallback = true;
            list.Clear();
            vertex.Clear();
            var vert = CreateEmojiMesh(this,list);
            if(vert!=null)
            {
                var tri = CreateTri(vert.Count);
                vertex.AddUIVertexStream(vert, new List<int>(tri));
                if (OnPopulate != null)
                    OnPopulate(this, vertex);
            }
            m_DisableFontTextureRebuiltCallback = false;
        }
    }
}
