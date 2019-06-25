using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.UIComposite
{
    public class UIConstructor
    {
        public static ModelElement CreateSlider()
        {
            var slider = ModelElement.CreateNew("Slider");
            slider.data.sizeDelta.x = 400;
            slider.data.sizeDelta.y = 20;
            slider.AddComponent<ImageElement>();
            return slider;
        }
    }
}
