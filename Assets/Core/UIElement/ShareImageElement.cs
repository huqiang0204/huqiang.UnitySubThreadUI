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
        public static void GetUVInfo(ModelElement child, List<UIVertex> vertices, List<int> tri, Vector3 position, Quaternion quate, Vector3 scale)
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
                var sic = child.GetComponent<ShareImageChildElement>();
                if(sic!=null)
                {
                    UIVertex[] buff = sic.buff;
                    float w = child.data.sizeDelta.x;
                    float h = child.data.sizeDelta.y;
                    float left = -child.data.pivot.x * w;
                    float right = left + w * sic.data.fillAmountX;
                    float down = -child.data.pivot.y * h;
                    float top = down + h;
                    right *= s.x;
                    left *= s.x;
                    down *= s.y;
                    top *= s.y;
                    buff[0].position = q * new Vector3(left, down) + o;
                    buff[1].position = q * new Vector3(left, top) + o;
                    buff[2].position = q * new Vector3(right, top) + o;
                    buff[3].position = q * new Vector3(right, down) + o;
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
                    int c = vertices.Count;
                    vertices.AddRange(buff);
                    tri.Add(c);
                    tri.Add(c + 1);
                    tri.Add(c + 2);
                    tri.Add(c + 2);
                    tri.Add(c + 3);
                    tri.Add(c);
                }
                for (int i = 0; i < child.child.Count; i++)
                {
                    GetUVInfo(child.child[i], vertices, tri, o, q, s);
                }
            }
        }
    }
}
