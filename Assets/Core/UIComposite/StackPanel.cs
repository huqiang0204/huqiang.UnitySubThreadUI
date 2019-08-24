using huqiang.Data;
using huqiang.UI;
using UnityEngine;

namespace huqiang.UIComposite
{
    public unsafe struct StackPanelData
    {
        public Direction direction;
        public Vector2 minSize;
        public bool autoSize;
        public static int Size = sizeof(StackPanelData);
        public static int ElementSize = Size / 4;
    }
    public class StackPanel: ModelInital
    {
        public ModelElement model;
        public Direction direction { get => data.direction; set => data.direction = value; }
        public StackPanelData data;
        public unsafe override void Initial(ModelElement mod)
        {
            model = mod;
            mod.SizeChanged = SizeChanged;
            FakeStruct fake = mod.GetExtand() as FakeStruct;
            if(fake!=null)
            {
                data = *(StackPanelData*)fake.ip;
                direction = data.direction;
            }
        }
        void SizeChanged(ModelElement mod)
        {
            data.minSize = mod.data.sizeDelta;
            Order();
        }
        public void Order()
        {
            var child = model.child;
            if (data.autoSize)
            {
                float x = 0,y=0;
                for (int i = 0; i < child.Count; i++)
                {
                    x += child[i].data.sizeDelta.x;
                    y += child[i].data.sizeDelta.y;
                }
                model.data.sizeDelta.x = x;
                model.data.sizeDelta.y = y;
            }
            var size = model.data.sizeDelta;
            if(direction==Direction.Horizontal)
            {
                float h = size.y;
                float x = size.x * 0.5f;
                float sx = -x;
                for (int i = 0; i < child.Count; i++)
                {
                    var c = child[i];
                    if (c.data.sizeDelta.y != h)
                    {
                        c.data.sizeDelta.y = h;
                        ModelElement.ScaleSize(c);
                    }
                    float ix = c.data.sizeDelta.x;
                    c.data.localPosition.x = sx + ix * 0.5f;
                    c.data.localPosition.y = 0;
                    if (sx > x)
                        c.activeSelf = false;
                    else c.activeSelf = true;
                    sx += ix;
                    c.IsChanged = true;
                }
            }
            else
            {
                float w = size.x;
                float sy = size.y * 0.5f;
                float y = -sy;
                for (int i = 0; i < child.Count; i++)
                {
                    var c = child[i];
                    if (c.data.sizeDelta.x != w)
                    {
                        c.data.sizeDelta.x = w;
                        ModelElement.ScaleSize(c);
                    }
                    float iy = c.data.sizeDelta.y;
                    c.data.localPosition.y = sy - iy * 0.5f;
                    c.data.localPosition.x = 0;
                    if (sy < y)
                        c.activeSelf = false;
                    else c.activeSelf = true;
                    sy -= iy;
                    c.IsChanged = true;
                }
            }
        }
    }
}
