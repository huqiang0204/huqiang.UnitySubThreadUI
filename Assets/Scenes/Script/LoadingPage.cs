using huqiang.UI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPage : UIPage
{
    class View
    {
        public UISlider Slider;
    }
    ModelElement txt;
    System.Random ran;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.FindModel("baseUI", "Slider");
        base.Initial(parent, dat);
        model.SetParent(parent);
        var view = model.ComponentReflection<View>();
        view.Slider.SetFillSize(0.5f);
        //List<string> list = new List<string>();
        //for (int i = 0; i < 1000; i++)
        //    list.Add(i.ToString());
        //view.FriendsRanking.BindingData = list;
        //view.FriendsRanking.Refresh();
        //ran = new System.Random();
        //txt.baseEvent.Click = (o, e) =>
        //{
        //    Debug.Log("click");
        //    txt.data.localPosition = new Vector3(ran.Next(-400, 400), 0, 0);
        //    txt.IsChanged = true;
        //};
    }
}
