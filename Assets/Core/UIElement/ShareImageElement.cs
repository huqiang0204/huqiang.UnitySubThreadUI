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
                for (int i = 0; i < model.child.Count; i++)
                {
                    var c = model.child[i];
                    var help = c.GetComponent<ShareImageChildElement>();
                    if (help != null)
                        help.GetUVInfo(vert, tri, Vector3.zero, Quaternion.identity, Vector3.one);
                }
                vertex = vert;
                tris = tri;
                vertChanged = true;
            }
        }
    }
}
