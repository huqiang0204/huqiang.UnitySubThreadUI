using System;
using System.Collections.Generic;
using huqiang.UI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using UnityEngine;

public class LayoutTestPage : UIPage
{
    class View
    {
        public DesignedDockPanel Layout;
    }
    View view;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "layout");
        base.Initial(parent, dat);
        view = model.ComponentReflection<View>();
        var area = view.Layout.MainContent;
        area.AddContent("page0");
        var d = area.AddArea(DockpanelArea.Dock.Down,0.3f);
        var context = d.AddContent("page1");

        d.model.parent.GetComponent<ImageElement>().color = Color.red;
        var one = d.AddArea(DockpanelArea.Dock.Right,0.4f);
        context = one.AddContent("page2");
        context.LoadPopWindow<GridTestWindow>();
        //d.Refresh();

        one.model.parent.GetComponent<ImageElement>().color = Color.green;
        var top = area.AddArea(DockpanelArea.Dock.Top, 0.2f);
        top.AddContent("page3");

        top.model.parent.GetComponent<ImageElement>().color = Color.yellow;
        var l = top.AddArea(DockpanelArea.Dock.Left, 0.4f);
        l.model.parent.GetComponent<ImageElement>().color = Color.blue;
        l.control.headDock = TabControl.HeadDock.Down;

        context = l.AddContent("page5");
        context.LoadPopWindow<GridTestWindow2>();
        //l.Refresh();
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
        model = ModelManagerUI.CloneModel("baseUI", "gridscroll");
        base.Initial(parent, ui, obj);
        view = model.ComponentReflection<View>();

        List<int> testData = new List<int>();
        for (int i = 0; i < 33; i++)
            testData.Add(i);
        view.Scroll.BindingData = testData;
        view.Scroll.SetItemUpdate<Item,int>( (o, e,  i) => { o.Text.text = i.ToString();});
        view.Scroll.Refresh();
        view.Scroll.eventCall.Click = (o, e) => {
            if (e.IsRightButtonUp)
                UIMenu.Instance.ShowMenu<TestMenu>(this,e.CanPosition);
            Debug.Log("click");
        };
        Debug.Log("ok");
    }
    public override void Cmd(string cmd, object dat)
    {
        switch(cmd)
        {
            case "menu":
                Debug.Log(dat);
                break;
        }
    }
}
public class GridTestWindow2 : PopWindow
{
    class View
    {
        public ScrollX Scroll;
    }
    class Item
    {
        public TextElement Text;
    }
    View view;
    public override void Initial(ModelElement parent, UIPage ui, object obj = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "gridscroll");
        base.Initial(parent, ui, obj);
        view = model.ComponentReflection<View>();

        List<int> testData = new List<int>();
        for (int i = 0; i < 44; i++)
            testData.Add(i);
        view.Scroll.BindingData = testData;
        view.Scroll.SetItemUpdate<Item,int>( (o, e, i) => {o.Text.text = i.ToString();});
        view.Scroll.Refresh();
    }
}
public class TestMenu : MenuWindow
{
    class View
    {
        public EventCallBack Menu;
    }
    View view;
    public override void Initial(ModelElement parent, UIBase ui, object obj = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "Menu");
        base.Initial(parent, ui, obj);
        view = model.ComponentReflection<View>();
        view.Menu.Click = (o, e) => {
            UIMenu.Instance.HideMenu();
            Context.Cmd("menu",e);
        };
    }
}