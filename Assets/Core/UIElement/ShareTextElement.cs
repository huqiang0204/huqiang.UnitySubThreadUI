using System;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

namespace huqiang.UI
{
    public class ShareTextElement:EmojiElement
    {
        public ModelElement Collector;
        ShareText shareText;
        public override void RestoringRelationships(List<AssociatedInstance> table)
        {
            var id = data.ex;
            if(id!=0)
            {
                var ins = table.Find((o) => { return o.id == id; });
                if (ins != null)
                    Collector = ins.target;
            }
        }
        public override void LoadToObject(Component game)
        {
            base.LoadToObject(game);
            shareText = Context as ShareText;
            shareText.context = this;
        }
        public List<UIVertex> OnPopulateMesh(ShareText shareText)
        {
            var vert = new List<UIVertex>();
            var child =  model.child;
            if (Collector != null)
                child = Collector.child;
            for (int i = 0; i < child.Count; i++)
            {
                GetChildUVInfo(child[i], shareText, vert, Vector3.zero, Quaternion.identity, Vector3.one);
            }
            return vert;
        }
        public override void Apply()
        {
            base.Apply();
            //if (CheckChanged(model))
                shareText.Refresh();
        }
        public static bool CheckChanged(ShareTextElement ste)
        {
            if (ste.Collector == null)
                return CheckChanged(ste.model);
            else return CheckChanged(ste.Collector);
        }
        static bool CheckChanged(ModelElement mod)
        {
            var stc = mod.GetComponent<ShareTextChildElement>();
            if (stc != null)
                if (stc.IsChanged)
                    return true;
            for (int i = 0; i < mod.child.Count; i++)
            {
                var b = CheckChanged(mod.child[i]);
                if (b)
                    return b;
            }
            return false;
        }
        static void GetChildUVInfo(ModelElement child, ShareText ori, List<UIVertex> vertices, Vector3 position, Quaternion quate, Vector3 scale)
        {
            var pos = child.data.localPosition;
            Vector3 p = quate * pos;
            Vector3 o = Vector3.zero;
            o.x = p.x * scale.x;
            o.y = p.y * scale.y;
            o.z = p.z * scale.z;
            o += position;

            Vector3 s = child.data.localScale;
            Quaternion q = quate * child.data.localRotation;
            s.x *= scale.x;
            s.y *= scale.y;
         
            if (child.activeSelf)
            {
                var stc = child.GetComponent<ShareTextChildElement>();
                if(stc!=null)
                {
                    if (stc.text != null & stc.text != "")
                    {
                        if (stc.IsChanged)
                        {
                            TextGenerationSettings settings = new TextGenerationSettings();
                            settings.font = ori.font;
                            settings.pivot = child.data.pivot;
                            settings.generationExtents = child.data.sizeDelta;
                            settings.horizontalOverflow = stc.data.horizontalOverflow;
                            settings.verticalOverflow = stc.data.verticalOverflow;
                            settings.resizeTextMaxSize = stc.data.fontSize;
                            settings.resizeTextMinSize = stc.data.fontSize;
                            settings.generateOutOfBounds = stc.data.generateOutOfBounds;
                            settings.resizeTextForBestFit = false;
                            settings.textAnchor = stc.data.textAnchor;
                            settings.fontStyle = stc.data.fontStyle;
                            settings.scaleFactor = 1;
                            settings.richText = stc.data.richText;
                            settings.lineSpacing = stc.data.lineSpacing;
                            settings.fontSize = stc.data.fontSize;
                            settings.color = stc.data.color;
                            settings.alignByGeometry = stc.data.alignByGeometry;
                            stc.buffer = ShareText.CreateEmojiMesh(ori, stc.emojiString, ref settings);
                            stc.IsChanged = false;
                        }
                    }
                    var buf = stc.buffer;
                    if (buf != null)
                    {
                        UIVertex[] vert = new UIVertex[buf.Length];
                        Array.Copy(buf, vert, buf.Length);
                        for (int i = 0; i < vert.Length; i++)
                        {
                            vert[i].position.x *= s.x;
                            vert[i].position.y *= s.y;
                            vert[i].position = q * vert[i].position + o;
                        }
                        vertices.AddRange(vert);
                    }
                }
                for (int i = 0; i < child.child.Count; i++)
                {
                    GetChildUVInfo(child.child[i], ori, vertices, o, q, s);
                }
            }
        }
    }
}
