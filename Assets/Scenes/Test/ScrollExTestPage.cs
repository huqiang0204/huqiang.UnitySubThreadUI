using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using huqiang.UI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using UnityEngine;

public class ScrollExTestPage : UIPage
{
    class View
    {
        public ScrollYExtand scroll;
    }
    View view;
    class TitleItem
    {
        public EventCallBack bk;
        public TextElement Text;
    }
    class SubItem
    {
        public TextElement Text;
    }
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "scrollex");
        base.Initial(parent, dat);
        view = model.ComponentReflection<View>();
        List<ScrollYExtand.DataTemplate> datas = new List<ScrollYExtand.DataTemplate>();
        ScrollYExtand.DataTemplate tmp = new ScrollYExtand.DataTemplate();
        tmp.Title = "test1";
        List<string> list = new List<string>();
        for (int i = 0; i < 22; i++)
            list.Add("tttt"+i.ToString());
        tmp.Hide = true;
        tmp.Data = list;
        datas.Add(tmp);
        view.scroll.BindingData = datas;
        view.scroll.SetTitleUpdate<TitleItem, ScrollYExtand.DataTemplate>(TitleUpdate);
        view.scroll.SetItemUpdate<SubItem, string>(ItemUpdate);
        view.scroll.Refresh();
    }
    void TitleUpdate(TitleItem title, ScrollYExtand.DataTemplate data, int index)
    {
        title.Text.text = data.Title as string;
        title.bk.DataContext = data;
        title.bk.Click = (o, e) => {
            var dt = o.DataContext as ScrollYExtand.DataTemplate;
            dt.Hide = !dt.Hide;
            view.scroll.Refresh(view.scroll.Point);
        };
    }
    void ItemUpdate(SubItem item,string data,int index)
    {
        item.Text.text = data;
    }

}
