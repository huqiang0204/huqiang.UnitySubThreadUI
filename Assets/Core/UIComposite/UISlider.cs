using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class UISlider : ModelInital
    {
        public enum Direction
        {
            Horizontal, Vertical
        }
        public ModelElement Background;
        ImageElement image;
        public ModelElement Nob;
        public Vector2 StartOffset;
        public Vector2 EndOffset;
        public float MinScale;
        public float MaxScale;
        public Direction direction;
        float ratio;
        EventCallBack callBack;
        Vector2 pos;
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
            var size = Background.data.sizeDelta;
            if (direction==Direction.Horizontal)
            {
                float r = pos.x / size.x+0.5f;
                if (r < 0)
                    r = 0;
                else if (r > 1)
                    r = 1;
                ratio = r;
                float rx = size.x * 0.5f;
                float lx = -rx;
                Vector2 start = new Vector2(lx+StartOffset.x,StartOffset.y);
                Vector2 end = new Vector2(rx - EndOffset.x, EndOffset.y);
                pos = (end - start) * ratio + start;
                if(Nob!=null)
                {
                    Nob.data.localPosition = pos;
                    float s = (MaxScale - MinScale) * ratio + MinScale;
                    Nob.data.localScale.x = s;
                    Nob.data.localScale.y = s;
                    Nob.data.localScale.z = s;
                    Nob.IsChanged = true;
                }
            }
            else
            {
                float r = pos.y / size.y + 0.5f;
                if (r < 0)
                    r = 0;
                else if (r > 1)
                    r = 1;
                ratio = r;
                float ry = size.y * 0.5f;
                float ly = -ry;
                Vector2 start = new Vector2(StartOffset.x, ly+StartOffset.y);
                Vector2 end = new Vector2(EndOffset.x,ly- EndOffset.y);
                pos = (end - start) * ratio + start;
                if (Nob != null)
                {
                    Nob.data.localPosition = pos;
                    float s = (MaxScale - MinScale) * ratio + MinScale;
                    Nob.data.localScale.x = s;
                    Nob.data.localScale.y = s;
                    Nob.data.localScale.z = s;
                    Nob.IsChanged = true;
                }
            }
        }
    }
}
