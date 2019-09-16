using huqiang.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UGUI;
using UnityEngine;

namespace huqiang.UI
{
    public enum LayoutType
    {
        StackPanelV, StackPanelH, GridV,GridH
    }
    public unsafe struct LayoutData
    {
        public LayoutType type;
        public Vector2 minBox;
        public Vector2 minSize;
        public Vector2 interval;
        public bool fillSize;
        public bool extandSize;
        public static int Size = sizeof(LayoutData);
        public static int ElementSize = Size / 4;
    }
    public class LayoutElement : DataConversion, UpdateInterface
    {
        public override ModelElement model { get => base.model; set { base.model = value; value.updating = this; } }
        LayoutData data;
        public unsafe override void Load(FakeStruct fake)
        {
            data = *(LayoutData*)fake.ip;
        }
        public void Update()
        {
            if(IsChanged)
            {
                IsChanged = false;
                switch(data.type)
                {
                    case LayoutType.StackPanelV:
                        StackPanelY(this);
                        break;
                    case LayoutType.StackPanelH:
                        StackPanelX(this);
                        break;
                    case LayoutType.GridV:
                        GridY(this);
                        break;
                    case LayoutType.GridH:
                        GridX(this);
                        break;
                }
                ModelElement.ScaleSize(model);
            }
        }
        static void StackPanelY(LayoutElement element)
        {
            var child = element.model.child;
            int c = child.Count;
            if (element.data.extandSize)
            {
                float h = c * element.data.interval.y;
                for(int i=0;i<c;i++)
                {
                    h += child[i].data.sizeDelta.y;
                }
                element.model.data.sizeDelta.y = h;
            }
            float x = element.model.data.sizeDelta.x;
            float sy = element.model.data.sizeDelta.y*0.5f;
            for (int i = 0; i < c; i++)
            {
                var o = child[i];
                o.data.sizeDelta.x = x;
                float y = o.data.sizeDelta.y;
                o.data.localPosition.x = 0;
                o.data.localPosition.y = sy - 0.5f * y;
                sy += y;
                o.IsChanged = true;
            }
        }
        static void StackPanelX(LayoutElement element)
        {
            var child = element.model.child;
            int c = child.Count;
            if (element.data.extandSize)
            {
                float w = c * element.data.interval.x;
                for (int i = 0; i < c; i++)
                {
                    w += child[i].data.sizeDelta.x;
                }
                element.model.data.sizeDelta.x = w;
            }
            float y = element.model.data.sizeDelta.y;
            float sx = element.model.data.sizeDelta.x * -0.5f;
            for (int i = 0; i < c; i++)
            {
                var o = child[i];
                o.data.sizeDelta.y = y;
                float x = o.data.sizeDelta.x;
                o.data.localPosition.x = sx + 0.5f * x;
                o.data.localPosition.y = 0;
                sx += x;
                o.IsChanged = true;
            }
        }
        static void GridY(LayoutElement element)
        {
            var mod = element.model;
            float x = mod.data.sizeDelta.x;
            float y = mod.data.sizeDelta.y;
            Vector2 size = element.data.minSize;
            float ax = size.x + element.data.interval.x;
            int num = (int)(x / ax);
            if (num < 1)
                num = 1;
            if (element.data.fillSize)
            {
                float i = x / num;
                float r = i / size.x;
                size.x = i;
                size.y *= r;
                ax = size.x + element.data.interval.x;
            }
            float sx = -0.5f * x;
            float sy = 0.5f * y;
            float ay = size.y + element.data.interval.y;
            if (element.data.extandSize)
            {
                int c = mod.child.Count;
                int r = c / num;
                if (c % num > 0)
                    r++;
                mod.data.sizeDelta.y = r * ay;
            }
            var child = mod.child;
            for (int i = 0; i < child.Count; i++)
            {
                int c = i % num;
                int r = i / num;
                float ox = c * ax + 0.5f * ax + sx;
                float oy = sy - r * ay - 0.5f * ay;
                var o = child[i];
                o.data.sizeDelta = size;
                o.data.localPosition.x = ox;
                o.data.localPosition.y = oy;
                o.IsChanged = true;
            }
        }
        static void GridX(LayoutElement element)
        {
            var mod = element.model;
            float x = mod.data.sizeDelta.x;
            float y = mod.data.sizeDelta.y;
            Vector2 size = element.data.minSize;
            float ay = size.y + element.data.interval.y;
           int  num = (int)(y / ay);
            if (num < 1)
                num = 1;
            if (element.data.fillSize)
            {
                float i = y / num;
                float r = i / size.y;
                size.x = i;
                size.y *= r;
                ay = size.y + element.data.interval.y;
            }
            float sx = -0.5f * x;
            float sy = 0.5f * y;
            float ax = size.x + element.data.interval.x;
            if (element.data.extandSize)
            {
                int c = mod.child.Count;
                int r = c / num;
                if (c % num > 0)
                    r++;
                mod.data.sizeDelta.x = r * ax;
            }
            var child = mod.child;
            for (int i = 0; i < child.Count; i++)
            {
                int c = i / num;
                int r = i % num;
                float ox = c * ax + 0.5f * ax + sx;
                float oy = sy - r * ay - 0.5f * ay;
                var o = child[i];
                o.data.sizeDelta = size;
                o.data.localPosition.x = ox;
                o.data.localPosition.y = oy;
                o.IsChanged = true;
            }
        }
        public static unsafe FakeStruct LoadFromObject(Component com, DataBuffer buffer)
        {
            var img = com as UILayout;
            if (img == null)
                return null;
            FakeStruct fake = new FakeStruct(buffer, LayoutData.ElementSize);
            LayoutData* data = (LayoutData*)fake.ip;
            data->type = img.type;
            data->minBox = img.minBox;
            data->minSize = img.minSize;
            data->interval = img.interval;
            data->fillSize = img.fillSize;
            data->extandSize = img.extandSize;
            return fake;
        }
    }
}
