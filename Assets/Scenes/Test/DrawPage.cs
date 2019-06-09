using huqiang.UI;
using huqiang.UIComposite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DrawPage:UIPage
{
    class View
    {
        public Paint draw;
        public UIPalette Palette;
        public RawImageElement color;
        public TextElement tip;
        public UISlider sizeS;
    }
    View view;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "drawing");
        base.Initial(parent, dat);
        view = model.ComponentReflection<View>();
        view.Palette.TemplateChanged = view.Palette.ColorChanged = (o) => {
            view.draw.BrushColor = o.SelectColor;
            view.color.color = o.SelectColor;
        };
        view.sizeS.OnValueChanged = (o) => {
            float a = o.Percentage * 78;
            a+=2;
            view.draw.BrushSize = a;
            view.tip.text = "画笔尺寸:"+a.ToString();
        };
    }
}