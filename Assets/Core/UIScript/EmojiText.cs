using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UGUI
{
    public class EmojiText:Text
    {
        public static float NormalDpi = 96;
        public Action<EmojiText, VertexHelper> OnPopulate;
        protected EmojiString emojiString = new EmojiString();

        public override string text {
            get { return emojiString.FullString; }
            set {
                m_Text=
                emojiString.FullString = value;
                SetVerticesDirty();
            }
        }
        readonly UIVertex[] m_TempVerts = new UIVertex[4];
        static UIVertex[] CreateEmojiMesh(IList<UIVertex> verts,  List<EmojiInfo> emoji, float fontSize, float unitsPerPixel,Vector2 roundingOffset)
        {
            int vertCount = verts.Count ;
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
                buf[i].uv0 = emoji[j].uv[0];
                buf[i].uv0.x *= 0.5f;
                buf[i].uv0.x += 0.5f;
                i++;
                buf[i].uv0 = emoji[j].uv[1];
                buf[i].uv0.x *= 0.5f;
                buf[i].uv0.x += 0.5f;
                i++;
                buf[i].uv0 = emoji[j].uv[2];
                buf[i].uv0.x *= 0.5f;
                buf[i].uv0.x += 0.5f;
                i++;
                buf[i].uv0 = emoji[j].uv[3];
                buf[i].uv0.x *= 0.5f;
                buf[i].uv0.x += 0.5f;
            }
            return buf;
        }
        public static UIVertex[] CreateEmojiMesh(Text text, EmojiString emoji)
        {
            //float s = Screen.dpi / NormalDpi;
            Vector2 extents = text.rectTransform.rect.size;
            var settings = text.GetGenerationSettings(extents );//* s

            float t = settings.fontSize;
            //t *= s;
            settings.fontSize = (int)t;
            t = settings.resizeTextMinSize;
           // t *= s;
            settings.resizeTextMinSize = (int)t;
            t = settings.resizeTextMaxSize;
            //t *= s;
            settings.resizeTextMaxSize = (int)t;
            string str = emoji.FilterString;
            if (str != null & str != "")
            {
                text.cachedTextGenerator.PopulateWithErrors(str, settings, text.gameObject);

                IList<UIVertex> verts = text.cachedTextGenerator.verts;
                if (verts.Count == 0)
                    return null;
                float unitsPerPixel = 1 / text.pixelsPerUnit ;/// s
                Vector2 roundingOffset = new Vector2(verts[0].position.x , verts[0].position.y) * unitsPerPixel;/// s
                roundingOffset = text.PixelAdjustPoint(roundingOffset) - roundingOffset;
                var vs = CreateEmojiMesh(text.cachedTextGenerator.verts, emoji.emojis, text.fontSize, unitsPerPixel, roundingOffset);//*s

                return vs;
            }
            return null;
        }
        public static UIVertex[] CreateEmojiMesh(Text text, EmojiString emoji,ref TextGenerationSettings settings)
        {
            string str = emoji.FilterString;
            if (str != null & str != "")
            {
                text.cachedTextGenerator.Populate(str, settings);
                IList<UIVertex> verts = text.cachedTextGenerator.verts;
                if (verts.Count == 0)
                    return null;
                float unitsPerPixel = 1 / text.pixelsPerUnit;/// s
                Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;/// s
                roundingOffset = text.PixelAdjustPoint(roundingOffset) - roundingOffset;
                var vs = CreateEmojiMesh(text.cachedTextGenerator.verts, emoji.emojis, text.fontSize, unitsPerPixel, roundingOffset);//*s

                return vs;
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
            vertex.Clear();
            if (m_Text != emojiString.FullString)
                emojiString.FullString = m_Text;
            var vert = CreateEmojiMesh(this,emojiString);
            if(vert!=null)
            {
                var v = new List<UIVertex>(vert);
                var tri = CreateTri(vert.Length);
                vertex.AddUIVertexStream(v, new List<int>(tri));
                if (OnPopulate != null)
                    OnPopulate(this, vertex);
            }
            m_DisableFontTextureRebuiltCallback = false;
            var tg = cachedTextGenerator;
        }
    }
}
