using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class DragContent: ModelInital
    {
        public override void Initial(ModelElement element)
        {
            view = element;
            element.RegEvent<EventCallBack>();
            eventCall = element.baseEvent;
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.DragEnd = (o, e, s) => { Scrolling(o, s); };
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            view.data.anchorMin = view.data.anchorMax = view.data.pivot = new Vector2(0.5f, 0.5f);
            eventCall.CutRect = true;
        }
        public static Vector3 Correction(Vector2 parentSize, Vector3 sonPos, Vector2 sonSize)
        {
            if (sonSize.x <= parentSize.x)
            {
                sonPos.x = 0;
                if (sonSize.y <= parentSize.y)
                {
                    sonPos.y = 0;
                    return sonPos;
                }
            }
            else
            {
                if (sonSize.y <= parentSize.y)
                {
                    sonPos.y = 0;
                }
            }
            Vector2 dotA = Vector2.zero;
            if (sonPos.x != 0)
            {
                float right = parentSize.x * 0.5f;
                float left = -right;
                float w = sonSize.x * 0.5f;
                float a = sonPos.x - w;
                if (a > left)
                {
                    sonPos.x = left + w;
                }
                else
                {
                    a = sonPos.x + w;
                    if (a < right)
                        sonPos.x = right - w;
                }
            }
            if (sonPos.y != 0)
            {
                float top = parentSize.y * 0.5f;
                float down = -top;
                float h = sonSize.y * 0.5f;
                float a = sonPos.y - h;
                if (a > down)
                {
                    sonPos.y = down + h;
                }
                else
                {
                    a = sonPos.y + h;
                    if (a < top)
                        sonPos.y = top - h;
                }
            }
            return sonPos;
        }
        public ModelElement view;
        public ModelElement Content;
        public EventCallBack eventCall;
        public DragContent()
        {
        }

        public Action<DragContent, Vector2> Scroll;
        void Scrolling(EventCallBack back, Vector2 v)
        {
            if (view == null)
                return;
            if (Content == null)
                return;
            v.x /= view.data.localScale.x;
            v.y /= view.data.localScale.y;

            var p = Content.data.localPosition;
            var s = Content.data.sizeDelta;
            p.x += v.x;
            p.y += v.y;
            v = Correction(view.data.sizeDelta, p, s);
            if (v.x == 0)
                back.VelocityX = 0;
            if (v.y == 0)
                back.VelocityY = 0;
            Content.data.localPosition = v;
            if (Scroll != null)
                Scroll(this, v);
        }
        public float Pos
        {
            get
            {
                float y = Content.data.sizeDelta.y - view.data.sizeDelta.y;
                float p = Content.data.localPosition.y;
                p += 0.5f * y;
                p /= y;
                if (p < 0)
                    p = 0;
                else if (p > 1)
                    p = 1;
                return p;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                float y = Content.data.sizeDelta.y - view.data.sizeDelta.y;
                if (y < 0)
                    y = 0;
                y *= (value - 0.5f);
                Content.data.localPosition = new Vector3(0, y, 0);
            }
        }
    }
}
