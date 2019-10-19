using huqiang.Data;
using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UGUI
{
    [RequireComponent(typeof(RectTransform))]
    public class UILayout:MonoBehaviour,DataStorage
    {
        public LayoutType type;
        public Vector2 minBox = new Vector2(400,400);
        public Vector2 minSize=new Vector2(100,100);
        public Vector2 interval;
        public bool fillSize;
        public bool extandSize;
        public void Refresh()
        {
            switch (type)
            {
                case LayoutType.StackPanelH:
                    StackPanelX(this);
                    break;
                case LayoutType.StackPanelV:
                    StackPanelY(this);
                    break;
                case LayoutType.GridV:
                    GridY(this);
                    break;
                case LayoutType.GridH:
                    GridX(this);
                    break;
            }
            SizeScaleEx.RefreshChild(transform);
        }
        static void StackPanelY(UILayout element)
        {
            var trans = element.transform as RectTransform;
            int c = trans.childCount;
            float iy = element.interval.y;
            if (element.extandSize)
            {
                float h = c * iy;
                for (int i = 0; i < c; i++)
                {
                    var rect = trans.GetChild(i) as RectTransform;
                    h += rect.sizeDelta.y;
                }
                var s = trans.sizeDelta;
                s.y = h;
                trans.sizeDelta = s;
            }
            float x = trans.sizeDelta.x;
            float sy = trans.sizeDelta.y * 0.5f;
            for (int i = 0; i < c; i++)
            {
                var o = trans.GetChild(i) as RectTransform;
                var s = Vector2.zero;
                s.x = x;
                s.y = o.sizeDelta.y;
                o.sizeDelta = s;
                o.localPosition = new Vector3(0, sy - 0.5f * s.y, 0);
                sy -= s.y + iy;
            }
        }
        static void StackPanelX(UILayout element)
        {
            var trans = element.transform as RectTransform;
            int c = trans.childCount;
            float ix = element.interval.x;
            if (element.extandSize)
            {
                float w = c * ix;
                for (int i = 0; i < c; i++)
                {
                    var rect = trans.GetChild(i) as RectTransform;
                    w += rect.sizeDelta.x;
                }
                var s = trans.sizeDelta;
                s.x = w;
                trans.sizeDelta = s;
            }
            float y = trans.sizeDelta.y;
            float sx = trans.sizeDelta.x * -0.5f;
            for (int i = 0; i < c; i++)
            {
                var o = trans.GetChild(i) as RectTransform;
                var s = Vector2.zero;
                s.x = o.sizeDelta.x;
                s.y = y;
                o.sizeDelta = s;
                o.localPosition = new Vector3( sx + 0.5f * s.x,0, 0);
                sx += s.x+ix;
            }
        }
        static void GridY(UILayout element)
        {
            var mod = element.transform as RectTransform;
            float x = mod.sizeDelta.x;
            float y = mod.sizeDelta.y;
            Vector2 size = element.minSize;
            float ax = size.x + element.interval.x;
           int num = (int)(x / ax);
            if (num < 1)
                num = 1;
            if (element.fillSize)
            {
                float i = x / num;
                float r = i / size.x;
                size.x = i;
                size.y *= r;
                ax = size.x + element.interval.x;
            }
            float sx = -0.5f * x;
            float sy = 0.5f * y;
            float ay = size.y + element.interval.y;
            if (element.extandSize)
            {
                int c = mod.childCount;
                int r = c / num;
                if (c % num > 0)
                    r++;
                var s = mod.sizeDelta;
                s.y = r * ay;
                mod.sizeDelta = s;
            }
            for (int i = 0; i < mod.childCount; i++)
            {
                int c = i % num;
                int r = i / num;
                float ox = c * ax + 0.5f * ax + sx;
                float oy = sy - r * ay - 0.5f * ay;
                var o = mod.GetChild(i)as RectTransform;
                o.sizeDelta = size;
                o.localPosition = new Vector3(ox, oy, 0);
            }
        }
        static void GridX(UILayout element)
        {
            var mod = element.transform as RectTransform;
            float x = mod.sizeDelta.x;
            float y = mod.sizeDelta.y;
            Vector2 size = element.minSize;
            float ay = size.y + element.interval.y;
           int num = (int)(y / ay);
            if (num < 1)
                num = 1;
            if (element.fillSize)
            {
                float i = y / num;
                float r = i / size.y;
                size.x = i;
                size.y *= r;
                ay = size.y + element.interval.y;
            }
            float sx = -0.5f * x;
            float sy = 0.5f * y;
            float ax = size.x + element.interval.x;
            if (element.extandSize)
            {
                int c = mod.childCount;
                int r = c / num;
                if (c % num > 0)
                    r++;
                var s = mod.sizeDelta;
                s.x = r * ax;
                mod.sizeDelta = s;
            }
            for (int i = 0; i < mod.childCount; i++)
            {
                int c = i / num;
                int r = i % num;
                float ox = c * ax + 0.5f * ax + sx;
                float oy = sy - r * ay - 0.5f * ay;
                var o = mod.GetChild(i)as RectTransform;
                o.sizeDelta = size;
                o.localPosition = new Vector3(ox,oy,0);
            }
        }

        public unsafe FakeStruct ToBufferData(DataBuffer data)
        {
            FakeStruct fake = new FakeStruct(data, LayoutData.ElementSize);
            LayoutData* dat = (LayoutData*)fake.ip;
            dat->type = type;
            dat->minBox = minBox;
            dat->minSize = minSize;
            dat->interval = interval;
            dat->fillSize = fillSize;
            dat->extandSize = extandSize;
            return fake;
        }
    }
}
