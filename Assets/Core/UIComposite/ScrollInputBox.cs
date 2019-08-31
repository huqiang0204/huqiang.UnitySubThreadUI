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
    public class ScrollInputBox:ModelInital,AnimatInterface
    {
        ModelElement drag;
        ModelElement content;
        ModelElement mod_slider;
        UISlider slider;
        DragContent Drag;
        public TextInput input;
        public override void Initial(ModelElement mod)
        {
            Model = mod;
            drag = mod.Find("Drag");
            Drag = new DragContent();
            Drag.Initial(drag);
            Drag.freeze = DragContent.FreezeDirection.X;
            Drag.scrollType = ScrollType.None;
            content = drag.Find("Content");
            input =  EventCallBack.RegEvent<TextInput>(content);
            mod_slider = mod.Find("Slider");
            if(mod_slider!=null)
            {
                slider = new UISlider();
                slider.Initial(mod_slider);
            }
            InitialEvent();
        }
        int State = 0;
        void InitialEvent()
        {
            input.Drag = (o, e, v) => {
                float x = v.x;
                if (x <= 0)
                    x = -x;
                float y = v.y;
                if (y < 0)
                    y = -y;
                if(x!=y)
                {
                    if (State == 0)
                    {
                        State = 1;
                        if (y > x)
                            o.RemoveFocus();
                        else Drag.eventCall.RemoveFocus();
                    }
                }
                if (State == 1)
                {
                    if (!input.entry)
                    {
                        //if (input.Forward)
                        //{
                        //    Drag.Move(new Vector2(0, input.TextCom.data.fontSize));
                        //}
                        //else
                        //{
                        //    Drag.Move(new Vector2(0, -input.TextCom.data.fontSize));
                        //}
                    }
                }
            };
            input.DragEnd = (o, e, v) => { State = 0; };
            Drag.eventCall.DragEnd = (o, e, v) => { State = 0; };
            input.OnValueChanged = (o) => {
                //TextElement text = input.TextCom;
                //TextElement.AsyncGetSize(text.fontName,new Vector2(drag.data.sizeDelta.x,0),text.data.fontSize,input.emojiString, text.data.fontStyle,TextDesignSize);
            };
        }
        void TextDesignSize(Vector2 size)
        {
            if (size.y < drag.data.sizeDelta.y)
                size.y = drag.data.sizeDelta.y;
            float os = 0;
            if (content.data.sizeDelta.y != size.y)
                os = size.y - content.data.sizeDelta.y;
            content.data.sizeDelta.y = size.y;
            os *= 0.5f;
            content.data.localPosition.y += os;
            content.IsChanged = true;
            if (os != 0)
                input.SizeChanged();
        }
        public ScrollInputBox()
        {
            UIAnimation.Manage.AddAnimat(this);
        }
        public void Dispose()
        {
            UIAnimation.Manage.ReleaseAnimat(this);
        }
        public void Update(float time)
        {
            
        }
    }
}
