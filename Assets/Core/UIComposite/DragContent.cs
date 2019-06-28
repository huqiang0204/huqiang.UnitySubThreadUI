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
        protected Vector2 ScrollNone(EventCallBack eventCall, ref Vector2 v, ref float x, ref float y)
        {
            Vector2 v2 = Vector2.zero;
            float vx = x - v.x;
            if (vx < 0)
            {
                x = 0;
                eventCall.VelocityX = 0;
                v.x = 0;
            }
            else if (vx + Size.x > ContentSize.x)
            {
                x = ContentSize.x - Size.x;
                eventCall.VelocityX = 0;
                v.x = 0;
            }
            else
            {
                x -= v.x;
                v2.x = v.x;
            }
            float vy = y + v.y;
            if (vy < 0)
            {
                y = 0;
                eventCall.VelocityY = 0;
                v.y = 0;
            }
            else if (vy + Size.y > ContentSize.y)
            {
                y = ContentSize.y - Size.y;
                eventCall.VelocityY = 0;
                v.y = 0;
            }
            else
            {
                y += v.y;
                v2.y = v.y;
            }
            return v2;
        }
        protected Vector2 BounceBack(EventCallBack eventCall, ref Vector2 v, ref float x, ref float y)
        {
            x -= v.x;
            y += v.y;
            if (!eventCall.Pressed)
            {
                if (x < 0)
                {
                    if (v.x > 0)
                        if (eventCall.DecayRateX >= 0.99f)
                        {
                            eventCall.DecayRateX = 0.9f;
                            eventCall.VelocityX = eventCall.VelocityX;
                        }
                }
                else if (x + Size.x > ContentSize.x)
                {
                    if (v.x < 0)
                        if (eventCall.DecayRateX >= 0.99f)
                        {
                            eventCall.DecayRateX = 0.9f;
                            eventCall.VelocityX = eventCall.VelocityX;
                        }
                }
                if (y < 0)
                {
                    if (v.y < 0)
                        if (eventCall.DecayRateY >= 0.99f)
                        {
                            eventCall.DecayRateY = 0.9f;
                            eventCall.VelocityY = eventCall.VelocityY;
                        }
                }
                else if (y + Size.y > ContentSize.y)
                {
                    if (v.y > 0)
                        if (eventCall.DecayRateY >= 0.99f)
                        {
                            eventCall.DecayRateY = 0.9f;
                            eventCall.VelocityY = eventCall.VelocityY;
                        }
                }
            }
            return v;
        }
        public Vector2 Size;
        public Vector2 Position;
        public Vector2 ContentSize;
        public ScrollType scrollType = ScrollType.BounceBack;
        public ModelElement view;
        public ModelElement Content;
        public EventCallBack eventCall;
        public Action<DragContent, Vector2> Scroll;
        public override void Initial(ModelElement element)
        {
            view = element;
            Size = element.data.sizeDelta;
            element.RegEvent<EventCallBack>();
            eventCall = element.baseEvent;
            eventCall.Drag = (o, e, s) => {
                Scrolling(o, s);
            };
            eventCall.DragEnd = (o, e, s) => {
                Scrolling(o, s);
                o.DecayRateX = 0.998f;
                o.DecayRateY = 0.998f;
            };
            eventCall.ScrollEndX = OnScrollEndX;
            eventCall.ScrollEndY = OnScrollEndY;
            eventCall.Scrolling = Scrolling;
            eventCall.ForceEvent = true;
            view.data.anchorMin = view.data.anchorMax = view.data.pivot = new Vector2(0.5f, 0.5f);
            eventCall.CutRect = true;
            Content = element.Find("Content");
            if(Content!=null)
                ContentSize = Content.data.sizeDelta;
        }
    
        void Scrolling(EventCallBack back, Vector2 v)
        {
            if (view == null)
                return;
            if (Content == null)
                return;
            v.x /= view.data.localScale.x;
            v.y /= view.data.localScale.y;
            switch (scrollType)
            {
                case ScrollType.None:
                    v = ScrollNone(back, ref v, ref Position.x, ref Position.y);
                    break;
                case ScrollType.BounceBack:
                    v = BounceBack(back, ref v, ref Position.x, ref Position.y);
                    break;
            }
            var offset = ContentSize - Size;
            offset *= 0.5f;
            var p = Position;
            p.x = - p.x;
            p.x += offset.x;
            p.y -= offset.y;
            Content.data.localPosition = p;
            Content.IsChanged = true;
            if (Scroll != null)
                Scroll(this, v);
        }
        void OnScrollEndX(EventCallBack back)
        {
            if (scrollType == ScrollType.BounceBack)
            {
                if (Position.x < -ScrollContent.Tolerance)
                {
                    back.DecayRateX = 0.988f;
                    float d = -Position.x;
                    back.ScrollDistanceX = -d * eventCall.Context.data.localScale.x;
                }
                else
                {
                    float max = ContentSize.x + ScrollContent.Tolerance;
                    if (max < Size.x)
                        max = Size.x + ScrollContent.Tolerance;
                    if (Position.x + Size.x > max)
                    {
                        back.DecayRateX = 0.988f;
                        float d = ContentSize.x - Position.x - Size.x;
                        back.ScrollDistanceX = -d * eventCall.Context.data.localScale.x;
                    }
                    else
                    {
                        //if (ScrollEnd != null)
                        //    ScrollEnd(this);
                    }
                }
            }
            //else if (ScrollEnd != null)
            //    ScrollEnd(this);
        }
        void OnScrollEndY(EventCallBack back)
        {
            if (scrollType == ScrollType.BounceBack)
            {
                if (Position.y < -ScrollContent.Tolerance)
                {
                    back.DecayRateY = 0.988f;
                    float d = -Position.y;
                    back.ScrollDistanceY = d * eventCall.Context.data.localScale.y;
                }
                else
                {
                    float max = ContentSize.y + ScrollContent.Tolerance;
                    if (max < Size.y)
                        max = Size.y + ScrollContent.Tolerance;
                    if (Position.y + Size.y > max)
                    {
                        back.DecayRateY = 0.988f;
                        float d = ContentSize.y - Position.y - Size.y;
                        back.ScrollDistanceY = d * eventCall.Context.data.localScale.y;
                    }
                    else
                    {
                        //if (ScrollEnd != null)
                        //    ScrollEnd(this);
                    }
                }
            }
            //else if (ScrollEnd != null)
            //    ScrollEnd(this);
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
