using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UGUI
{
    public class ShareImage : CustomRawImage
    {
#if UNITY_EDITOR
        void VertexCalculation()
        {
            var vert = new List<UIVertex>();
            var tri = new List<int>();
            var trans = transform;
            for (int i = 0; i < trans.childCount; i++)
            {
                    GetUVInfo(trans.GetChild(i)as RectTransform, vert, tri, Vector3.zero, Quaternion.identity, Vector3.one);
            }
            uIVertices = vert;
            triangle = tri;
        }
        public void GetUVInfo(RectTransform rect, List<UIVertex> vertices, List<int> tri, Vector3 position, Quaternion quate, Vector3 scale)
        {
            if(rect.gameObject.activeSelf)
            {
                float w = rect.localScale.x * rect.sizeDelta.x;
                float h = rect.localScale.y * rect.sizeDelta.y;
                var pos = rect.localPosition;
                pos = quate * pos + position;
                Vector3 ls = rect.localScale;
                ls.x *= scale.x;
                ls.y *= scale.y;
                var q = quate * rect.localRotation;
                var sic = rect.GetComponent<ShareImageChild>();
                if (sic != null)
                {
                    float left = -rect.pivot.x * w;
                    float right = left + w * sic.fillAmountX;
                    float down = -rect.pivot.y * h;
                    float top = down + h * sic.fillAmountY;
                    right *= ls.x;
                    left *= ls.x;
                    down *= ls.y;
                    top *= ls.y;
                    var buff = sic.buff;
                    buff[0].color = color;
                    buff[1].color = color;
                    buff[2].color = color;
                    buff[3].color = color;
                    buff[0].position = q * new Vector3(left, down) + pos;
                    buff[1].position = q * new Vector3(left, top) + pos;
                    buff[2].position = q * new Vector3(right, top) + pos;
                    buff[3].position = q * new Vector3(right, down) + pos;
                    var uvs = sic.uvs;
                    float uw = uvs[2].x - uvs[1].x;
                    float ux = uvs[1].x + uw * sic.fillAmountX;
                    float uh = uvs[2].y - uvs[1].y;
                    float uy = uvs[1].y + uh * sic.fillAmountY;
                    buff[0].uv0 = uvs[0];
                    buff[1].uv0 = uvs[1];
                    buff[1].uv0.y = uy;
                    buff[2].uv0.y = uy;
                    buff[2].uv0.x = ux;
                    buff[3].uv0 = uvs[3];
                    buff[3].uv0.x = ux;
                    int s = vertices.Count;
                    vertices.AddRange(buff);
                    tri.Add(s);
                    tri.Add(s + 1);
                    tri.Add(s + 2);
                    tri.Add(s + 2);
                    tri.Add(s + 3);
                    tri.Add(s);
                }

                for (int i = 0; i < rect.childCount; i++)
                {
                    GetUVInfo(rect.GetChild(i) as RectTransform, vertices, tri, pos, q, ls);
                }
            }
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            VertexCalculation();
            base.OnPopulateMesh(vh);
        }
#endif
    }
}

