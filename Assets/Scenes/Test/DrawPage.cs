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
    }
    View view;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "drawing");
        base.Initial(parent, dat);
        view = model.ComponentReflection<View>();
    }
}