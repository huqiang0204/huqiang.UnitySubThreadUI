using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class StackPanel: ModelInital
    {
        public ModelElement model;

        public Direction direction = Direction.Vertical;
        public override void Initial(ModelElement mod)
        {
            model = mod;
            mod.SizeChanged = SizeChanged;

        }
        void SizeChanged(ModelElement mod)
        {
            Order();
        }
        public void Order()
        {
            var size = model.data.sizeDelta;
            var child = model.child;
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
                    if (sy > y)
                        c.activeSelf = false;
                    else c.activeSelf = true;
                    sy -= iy;
                    c.IsChanged = true;
                }
            }
        }
    }
}
