using huqiang.UI;
using huqiang.UIEvent;
using System;
using UnityEngine;

namespace huqiang.UIComposite
{
    public unsafe struct SliderInfo
    {
        public Vector2 StartOffset;
        public Vector2 EndOffset;
        public float MinScale;
        public float MaxScale;
        public UISlider.Direction direction;
        public static int Size = sizeof(SliderInfo);
        public static int ElementSize = Size / 4;
    }
    public class UISlider : ModelInital
    {
        public enum Direction
        {
            Horizontal, Vertical
        }
        public ModelElement Background;
        ImageElement image;
        public ModelElement Nob;
        SliderInfo info;
        float ratio;
        EventCallBack callBack;
        Vector2 pos;
        public void SetFillSize(float value)
        {
            if (value < 0)
                value = 0;
            else if (value > 1)
                value = 1;
            if(Nob!=null)
            {
                if(Background!=null)
                {
                    if(info.direction==Direction.Horizontal)
                    {
                        float w = Background.data.sizeDelta.x;
                        Nob.data.sizeDelta.x = value * w;
                        ApplyValue();
                    }
                    else
                    {
                        float w = Background.data.sizeDelta.y;
                        Nob.data.sizeDelta.y = value * w;
                    }
                }
            }
        }
        public Action<UISlider> OnValueChanged;
        public float Percentage { get { return ratio; } set {
                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;
                ratio = value;
            } }
        public override void Initial(ModelElement mod)
        {
            var child = mod.child;
            Background = mod.FindChild("Background");
            if (Background != null)
            {
                image = Background.GetComponent<ImageElement>();
                callBack =  EventCallBack.RegEvent<EventCallBack>(Background);
                callBack.Drag = callBack.DragEnd = Draging;
                callBack.Click = Click;
            }
            Nob = mod.FindChild("Nob");
            var fake= mod.GetExtand();
            if(fake!=null)
            {
                unsafe
                {
                    info = *(SliderInfo*)fake.ip;
                }
            }
        }
        void Draging(EventCallBack back, UserAction action, Vector2 v)
        {
            pos += v;
            ApplyValue();
            if (OnValueChanged != null)
                OnValueChanged(this);
        }
        void Click(EventCallBack back, UserAction action)
        {
            var v = new Vector2(back.GlobalPosition.x,back.GlobalPosition.y);
            pos = action.CanPosition - v;
            pos.x /= back.GlobalScale.x;
            pos.y /= back.GlobalScale.y;
            ApplyValue();
            if (OnValueChanged != null)
                OnValueChanged(this);
        }
        void ApplyValue()
        {
            if (Background == null)
                return;
            if (Nob == null)
                return;
            var size = Background.data.sizeDelta;
            if (info.direction==Direction.Horizontal)
            {
                float rx = size.x * 0.5f;
                float lx = -rx;
                float nx = Nob.data.sizeDelta.x * 0.5f;
                Vector2 start = new Vector2(lx + info.StartOffset.x+nx, info.StartOffset.y);
                Vector2 end = new Vector2(rx - info.EndOffset.x-nx, info.EndOffset.y);
                if (pos.x < start.x)
                    pos.x = start.x;
                else if (pos.x > end.x)
                    pos.x = end.x;
                float w = end.x - start.x;
                ratio = (pos.x - start.x) / w;
                pos = (end - start) * ratio + start;
                if(Nob!=null)
                {
                    Nob.data.localPosition = pos;
                    float s = (info.MaxScale - info.MinScale) * ratio + info.MinScale;
                    Nob.data.localScale.x = s;
                    Nob.data.localScale.y = s;
                    Nob.data.localScale.z = s;
                    Nob.IsChanged = true;
                }
            }
            else
            {
                float ty = size.y * 0.5f;
                float dy = -ty;
                float ny = Nob.data.sizeDelta.y * 0.5f;
                Vector2 start = new Vector2( info.StartOffset.x,dy+ info.StartOffset.y+ny);
                Vector2 end = new Vector2(info.EndOffset.x,ty- info.EndOffset.y-ny);
                if (pos.y < start.y)
                    pos.y = start.y;
                else if (pos.y > end.y)
                    pos.y = end.y;
                float w = end.y - start.y;
                ratio = (pos.y - start.y) / w;
                pos = (end - start) * ratio + start;
                if (Nob != null)
                {
                    Nob.data.localPosition = pos;
                    float s = (info.MaxScale - info.MinScale) * ratio + info.MinScale;
                    Nob.data.localScale.x = s;
                    Nob.data.localScale.y = s;
                    Nob.data.localScale.z = s;
                    Nob.IsChanged = true;
                }
            }
        }
    }
}
