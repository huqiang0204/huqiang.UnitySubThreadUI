using huqiang.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UI
{
    public class ShareImageElement : CustomImageElement
    {
        public bool needCalcul = true;
        public override void Update()
        {
            if (needCalcul)
            {
                var vert = new List<UIVertex>();
                var tri = new List<int>();
                var child = model.child;
                for (int i = 0; i < child.Count; i++)
                {
                    GetUVInfo(child[i], vert, tri, Vector3.zero, Quaternion.identity, Vector3.one);
                }
                vertex = vert;
                tris = tri;
                vertChanged = true;
            }
        }
        public static void GetUVInfo(ModelElement mod, List<UIVertex> vertices, List<int> tri, Vector3 position, Quaternion quate, Vector3 scale)
        {
            var pos = mod.data.localPosition;
            pos = quate * pos + position;
            Vector3 ls = mod.data.localScale;
            ls.x *= scale.x;
            ls.y *= scale.y;
            var q = mod.data.localRotation * quate;
            if(mod.activeSelf)
            {
                var sic = mod.GetComponent<ShareImageChildElement>();
                if(sic!=null)
                {
                    UIVertex[] buff = sic.buff;
                    float w = mod.data.localScale.x * mod.data.sizeDelta.x;
                    float h = mod.data.localScale.y * mod.data.sizeDelta.y;
                    float left = -mod.data.pivot.x * w;
                    float right = left + w * sic.data.fillAmountX;
                    float down = -mod.data.pivot.y * h;
                    float top = down + h;
                    right *= ls.x;
                    left *= ls.x;
                    down *= ls.y;
                    top *= ls.y;
                    buff[0].position = q * new Vector3(left, down) + pos;
                    buff[1].position = q * new Vector3(left, top) + pos;
                    buff[2].position = q * new Vector3(right, top) + pos;
                    buff[3].position = q * new Vector3(right, down) + pos;
                    float tx = sic.data.txtSize.x;
                    float ty = sic.data.txtSize.y;
                    float l = sic.data.rect.x / tx;
                    float d = sic.data.rect.y / ty;
                    float r = l + sic.data.rect.width / tx * sic.data.fillAmountX;
                    float t = d + sic.data.rect.height / ty;
                    buff[0].uv0.x = l;
                    buff[0].uv0.y = d;
                    buff[1].uv0.x = l;
                    buff[1].uv0.y = t;
                    buff[2].uv0.x = r;
                    buff[2].uv0.y = t;
                    buff[3].uv0.x = r;
                    buff[3].uv0.y = d;
                    buff[0].color = sic.data.color;
                    buff[1].color = sic.data.color;
                    buff[2].color = sic.data.color;
                    buff[3].color = sic.data.color;
                    int s = vertices.Count;
                    vertices.AddRange(buff);
                    tri.Add(s);
                    tri.Add(s + 1);
                    tri.Add(s + 2);
                    tri.Add(s + 2);
                    tri.Add(s + 3);
                    tri.Add(s);
                }
                for (int i = 0; i < mod.child.Count; i++)
                {
                    GetUVInfo(mod.child[i], vertices, tri, pos, q, ls);
                }
            }
        }
    }
}
