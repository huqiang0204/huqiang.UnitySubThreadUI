using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.UI;
using huqiang.UIComposite;
using UnityEngine;

public class LayoutTestPage : UIPage
{
    class View
    {
        public Layout Layout;
    }
    View view;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.FindModel("baseUI", "layout");
        base.Initial(parent, dat);
        view = model.ComponentReflection<View>();
        var area = view.Layout.MainArea.AddArea(LayoutArea.Dock.Left);
        area.model.GetComponent<ImageElement>().color = Color.gray;
    }
}