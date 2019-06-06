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
        var area = view.Layout.MainArea;
        var d = area.AddArea(LayoutArea.Dock.Down);
        d.model.GetComponent<ImageElement>().color = Color.red;
        var one = d.AddArea(LayoutArea.Dock.Right);
        one.model.GetComponent<ImageElement>().color = Color.green;
        var top= area.AddArea(LayoutArea.Dock.Top);
        top.model.GetComponent<ImageElement>().color = Color.yellow;
        var l= top.AddArea(LayoutArea.Dock.Left);
        l.model.GetComponent<ImageElement>().color = Color.blue;
        l.auxiliary.headDock = LayoutAuxiliary.HeadDock.Down;
        var context = l.auxiliary.AddContent("test");
        context.LoadPopWindow<GridTestWindow>();
    } 
}
public class GridTestWindow : PopWindow
{
    class View
    {
        public ScrollY Scroll;
    }
    class Item
    {
        public TextElement Text;
    }
    View view;
    public override void Initial(ModelElement parent, UIPage ui, object obj = null)
    {
        model = ModelManagerUI.FindModel("baseUI", "gridscroll");
        base.Initial(parent, ui, obj);
        view = model.ComponentReflection<View>();

        List<int> testData = new List<int>();
        for (int i = 0; i < 33; i++)
            testData.Add(i);
        view.Scroll.BindingData = testData;
        view.Scroll.ItemObject = typeof(Item);
        view.Scroll.ItemUpdate = (o, e, i) => {
            (o as Item).Text.text = i.ToString();
        };
        view.Scroll.Refresh();
    }
}

