using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UGUI
{
    public class ShareText:EmojiText
    {
        public ShareTextElement context;
        protected override void OnPopulateMesh(VertexHelper vertex)
        {
            if (font == null)
                return;
            if(context!=null)
            {
                context.OnPopulateMesh(vertex);
            }
            else
            {
                var vert = new List<UIVertex>();
                for (int i = 0; i < transform.childCount; i++)
                {
                    var c = transform.GetChild(i);
                    var help = c.GetComponent<ShareTextChild>();
                    if (help != null)
                        help.GetUVInfo(this, vert, Vector3.zero, Quaternion.identity, Vector3.one);
                }
                var tri = CreateTri(vert.Count);
                vertex.Clear();
                vertex.AddUIVertexStream(vert, new List<int>(tri));
            }
        }
        public void Refresh()
        {
            SetVerticesDirty();
        }
    }
}
