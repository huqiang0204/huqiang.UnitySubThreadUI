using huqiang.Other;
using huqiang.UI;
using huqiang.UIEvent;
using System;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class UIPalette : ModelInital
    {
        EventCallBack callBackR;
        EventCallBack callBackC;
        ModelElement hc;
        ModelElement NobA;
        ModelElement NobB;
        RawImageElement template;
        RawImageElement htemp;
        RawImageElement slider;
        Palette palette;
        public Color SelectColor;
        float Alpha;
        public Action<UIPalette> ColorChanged;
        public Action<UIPalette> TemplateChanged;
        UISlider uISlider;
        public override void Initial(ModelElement mod)
        {
            palette = new Palette();
            callBackR = EventCallBack.RegEvent<EventCallBack>(mod);
            callBackR.IsCircular = true;
            callBackR.Drag = callBackR.DragEnd = DragingR;
            callBackR.PointerDown = PointDownR;
            NobA = mod.Find("NobA");
            NobB = mod.Find("NobB");
            hc = mod.Find("HTemplate");
            template = hc.GetComponent<RawImageElement>();
            callBackC= EventCallBack.RegEvent<EventCallBack>(hc);
            callBackC.Drag = callBackC.DragEnd = DragingC;
            callBackC.PointerDown = PointDownC;
            htemp = mod.GetComponent<RawImageElement>();
            ThreadMission.InvokeToMain((o)=> {
                htemp.Context.texture = Palette.LoadCTemplate();
                template.Context.texture = palette.texture;
                slider.Context.texture = Palette.AlphaTemplate();
            },null);
            palette.LoadHSVT(1);
            SelectColor.a = 1;
            var son = mod.Find("Slider");
            slider = son.GetComponent<RawImageElement>();
            uISlider = new UISlider();
            uISlider.Initial(son);
            uISlider.OnValueChanged = AlphaChanged;
            uISlider.Percentage = 1;
        }
        void DragingR(EventCallBack back, UserAction action, Vector2 v)
        {
            PointDownR(back,action);
        }
        void PointDownR(EventCallBack back, UserAction action)
        {
            float x = action.CanPosition.x - back.GlobalPosition.x;
            float y = action.CanPosition.y - back.GlobalPosition.y;
            x /= back.GlobalScale.x;
            y /= back.GlobalScale.y;
            float sx = x * x + y * y;
            float r = Mathf.Sqrt(220 * 220 / sx);
            x *= r;
            y *= r;
            if(NobA!=null)
            {
                NobA.data.localPosition = new Vector3(x,y,0);
                NobA.IsChanged = true;
            }
            float al = MathH.atan(-x, -y);
            palette.LoadHSVT(al/360);
            Color col = palette.buffer[Index];
            SelectColor.r = col.r;
            SelectColor.g = col.g;
            SelectColor.b = col.b;
            if (TemplateChanged != null)
                TemplateChanged(this);
        }
        void DragingC(EventCallBack back, UserAction action, Vector2 v)
        {
            PointDownC(back, action);
        }
        int Index;
        void PointDownC(EventCallBack back, UserAction action)
        {
            float x = action.CanPosition.x - back.GlobalPosition.x;
            float y = action.CanPosition.y - back.GlobalPosition.y;
            x /= back.GlobalScale.x;
            y /= back.GlobalScale.y;
            if (x < -128)
                x = -128;
            else if (x > 128)
                x = 128;
            if (y < -128)
                y = -128;
            else if (y > 128)
                y = 128;
            if(NobB!=null)
            {
                NobB.data.localPosition = new Vector3(x,y,0);
                NobB.IsChanged = true;
            }
            int dx = (int)x+128;
            if (dx < 0)
                dx = 0;
            else if (dx > 255)
                dx = 255;
            int dy = (int)y+128;
            if (dy < 0)
                dy = 0;
            else if (dy > 255)
                dy = 255;
             Index = dy * 256 + dx;
            if (Index >= 256 * 256)
                Index = 256 * 256 - 1;
            Color col = palette.buffer[Index];
            SelectColor.r = col.r;
            SelectColor.g = col.g;
            SelectColor.b = col.b;
            if (ColorChanged != null)
                ColorChanged(this);
        }
        void AlphaChanged(UISlider slider)
        {
            SelectColor.a = slider.Percentage;
            if (ColorChanged != null)
                ColorChanged(this);
        }
    }
}
