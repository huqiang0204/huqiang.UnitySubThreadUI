using huqiang.UI;
using huqiang.UIComposite;
using System.Collections.Generic;
using UnityEngine;

public class UITest : TestHelper
{
    class View
    {
        public UIRocker Rocker;
        public UIPalette Palette;
        public ImageElement Image;
        public UIDate Date;
        public TreeView TreeView;
        public TextElement Log;
        public ScrollX Scroll;
    }
    public Transform Sphere;
    Vector3 v;
    public override void LoadTestPage()
    {
        Application.targetFrameRate = 60;
#if UNITY_IPHONE || UNITY_ANDROID
        Scale.DpiScale = true;
#endif
        var model = ModelManagerUI.FindModel("baseUI", "asd");
        model.SetParent(UIPage.Root);
        var view = model.ComponentReflection<View>();
        view.Rocker.Radius = 100;
        view.Rocker.Rocking = (o) => {
            v.x = o.vector.x*0.0005f;
            v.z = o.vector.y*0.0005f;
        };
        view.Palette.TemplateChanged=view.Palette.ColorChanged = (o) => {
            view.Image.color = o.SelectColor;
        };
        TreeViewNode node = new TreeViewNode();
        node.content = "root";
        for (int i = 0; i < 10; i++)
        {
            TreeViewNode son = new TreeViewNode();
            son.content =i.ToString()+ "tss";
            node.child.Add(son);
            for(int j=0;j<6;j++)
            {
                TreeViewNode r = new TreeViewNode();
                r.content = j.ToString() + "sdfsdf";
                son.child.Add(r);
            }
        }
        view.TreeView.nodes = node;
        view.TreeView.Refresh();
        view.Log.text = Scale.ScreenDpi.ToString();
        List<int> testData = new List<int>();
        for (int i = 0; i < 13; i++)
            testData.Add(i);
        view.Scroll.BindingData = testData;
        view.Scroll.Refresh();
        //AnimationManage.Manage.ToDo(33,(o)=> {
        //    view.HTemplate.Context.texture = Palette.LoadCTemplate();
        //},null);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}