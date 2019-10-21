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
    public class ScrollInputBox:ModelInital
    {
        ModelElement content;
        ModelElement mod_slider;
        UISlider slider;
        public TextInput input;
        public override void Initial(ModelElement mod)
        {
            Model = mod;
            input =  EventCallBack.RegEvent<TextInput>(mod);
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
          
        }
    }
}
