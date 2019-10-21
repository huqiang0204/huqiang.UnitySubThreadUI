using huqiang.Data;
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
    public class ShareText:EmojiText,DataStorage
    {
        public ShareTextElement context;
        public Transform Collector;
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
                var trans = Collector;
                if (trans == null)
                    trans = transform;
                var vert = new List<UIVertex>();
                for (int i = 0; i < trans.childCount; i++)
                {
                    var c = trans.GetChild(i);
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

        public FakeStruct ToBufferData(DataBuffer data)
        {
            var fake = TextElement.LoadFromObject(this,data);
            int len = fake.Length-1;
            if (Collector != null)
                fake[len] = Collector.GetInstanceID();
            return fake;
        }
    }
}
