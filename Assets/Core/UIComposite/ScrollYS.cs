using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class ScrollXS : ModelInital
    {
        public ScrollX scroll;
        public UISlider slider;
        public ModelElement model;
        public Action<ScrollX, Vector2> Scroll;
        public Action<UISlider> OnValueChanged;
        public override void Initial(ModelElement mod)
        {
            model = mod;
            var scr = mod.Find("Scroll");
            scroll = new ScrollX();
            scroll.Initial(scr);
            var sli = mod.Find("Slider");
            slider = new UISlider();
            slider.Initial(sli);
            scroll.Scroll = (o, e) => {
                slider.Percentage = scroll.Pos;
                if (Scroll != null)
                    Scroll(o, e);
            };
            slider.OnValueChanged = (o) =>
            {
                scroll.Pos = slider.Percentage;
                if (OnValueChanged != null)
                    OnValueChanged(o);
            };
        }
    }
    public class ScrollYS: ModelInital
    {
        public ScrollY scroll;
        public UISlider slider;
        public ModelElement model;
        public Action<ScrollY, Vector2> Scroll;
        public Action<UISlider> OnValueChanged;
        public override void Initial(ModelElement mod)
        {
            model = mod;
            var scr = mod.Find("Scroll");
            scroll = new ScrollY();
            scroll.Initial(scr);
            var sli = mod.Find("Slider");
            slider = new UISlider();
            slider.Initial(sli);
            scroll.Scroll = (o, e) => {
                slider.Percentage = 1 - scroll.Pos;
                if (Scroll != null)
                    Scroll(o,e);
            };
            slider.OnValueChanged = (o) => {
                scroll.Pos = 1 - slider.Percentage;
                if (OnValueChanged != null)
                    OnValueChanged(o);
            };
        }
    }
}
