using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.UI;
using huqiang.UIComposite;
using UnityEngine;

public class TabControlTest:UIPage
{
    class View
    {
        public TabControl col;
    }
    View view;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "tab");
        base.Initial(parent, dat);
        view = model.ComponentReflection<View>();
        view.col.AddContent(ModelElement.CreateNew("test"),"test");
        view.col.AddContent(ModelElement.CreateNew("testB"), "testB");
    }
}

