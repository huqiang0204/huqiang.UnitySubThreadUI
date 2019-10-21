using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.Data;
using UnityEngine;

namespace huqiang.UI
{
    public unsafe struct ShareImageChildData
    {
        public Color color;
        public Vector2 txtSize;
        public Rect rect;
        public Vector2 pivot;
        public float fillAmountX;
        public float fillAmountY;
        public int spriteName;
        public static int Size = sizeof(ShareImageChildData);
        public static int ElementSize = Size / 4;
    }
    public class ShareImageChildElement:DataConversion,Coloring
    {
        public ShareImageChildData data;
        public string spriteName;
        public Color color { get => data.color; set => data.color = value; }
        public override ModelElement model { get => base.model; set { base.model = value; value.ColorController = this; } }
        public UIVertex[] buff = new UIVertex[4];
        public Vector2[] uvs = new Vector2[4];
        public override void Reset()
        {
            model.Entity = false;
            data.fillAmountX = 1;
            data.fillAmountY = 1;
            data.color = Color.white;
            model.ColorController = this;
        }
        public override unsafe void Load(FakeStruct fake)
        {
            data = *(ShareImageChildData*)fake.ip;
            model.color = data.color;
            spriteName= fake.buffer.GetData(data.spriteName) as string;
            model.Entity = false;
            float tx = data.txtSize.x;
            float ty = data.txtSize.y;
            float x = data.rect.x / tx;
            float y = data.rect.y / ty;
            float w = data.rect.width;
            float h = data.rect.height;
            float r = x + w / tx;
            float t = y + h / ty;
            uvs[0].x = x;
            uvs[0].y = y;
            uvs[1].x = x;
            uvs[1].y = t;
            uvs[2].x = r;
            uvs[2].y = t;
            uvs[3].x = r;
            uvs[3].y = y;
        }
        public void SetNactiveSize()
        {
            float w = data.rect.width;
            float h = data.rect.height;
            model.data.sizeDelta = new Vector2(w, h);
        }
        public void SetSpritePivot()
        {
           model.data.pivot = new Vector2(data.pivot.x / data.rect.width, data.pivot.y / data.rect.height);
        }
    }
}
