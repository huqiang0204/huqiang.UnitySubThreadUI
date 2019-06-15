using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.UI;
using huqiang.UIComposite;

public class TestPage : UIPage
{
    class View
    {
        public UIRocker Rocker;
        public ImageElement Image;
        public UIDate Date;
        public TreeView TreeView;
        public TextElement Log;
        public GridScroll Scroll;
        public ScrollXS ScrollX;
    }
    class Item
    {
        public TextElement Text;
    }
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.FindModel("baseUI", "asd");
        base.Initial(parent, dat);
      
        //model.SetParent(UIPage.Root);
        var view = model.ComponentReflection<View>();
        view.Rocker.Radius = 100;

        TreeViewNode node = new TreeViewNode();
        node.content = "root";
        for (int i = 0; i < 10; i++)
        {
            TreeViewNode son = new TreeViewNode();
            son.content = i.ToString() + "tss";
            node.child.Add(son);
            for (int j = 0; j < 6; j++)
            {
                TreeViewNode r = new TreeViewNode();
                r.content = j.ToString() + "sdfsdf";
                son.child.Add(r);
            }
        }
        view.TreeView.nodes = node;
        view.TreeView.Refresh();
        view.Log.text = Scale.LayoutWidth.ToString();
        List<int> testData = new List<int>();
        for (int i = 0; i <166; i++)
            testData.Add(i);
        //view.ScrollX.scroll.BindingData = testData;
        //view.ScrollX.scroll.ItemObject = typeof(Item);
        //view.ScrollX.scroll.ItemUpdate = (o, e, i) => {
        //    (o as Item).Text.text = i.ToString();
        //};
        //view.Scroll.Refresh();

        view.Scroll.BindingData = testData;
        view.Scroll.Column = 16;
        view.Scroll.SetItemUpdate<Item,int> ((o, e, i) => {o.Text.text = i.ToString(); });
        view.Scroll.Refresh();

    }
}
