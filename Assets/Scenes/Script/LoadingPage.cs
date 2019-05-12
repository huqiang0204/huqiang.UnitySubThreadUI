using huqiang.UI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPage : UIPage
{
    class View
    {
        public TextInput Text;
    }
    ModelElement txt;
    System.Random ran;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.FindModel("baseUI", "lobby");
        base.Initial(parent, dat);
        model.SetParent(parent);
        var view =  model.ComponentReflection<View>();

        //ran = new System.Random();
        //txt.baseEvent.Click = (o, e) =>
        //{
        //    Debug.Log("click");
        //    txt.data.localPosition = new Vector3(ran.Next(-400, 400), 0, 0);
        //    txt.IsChanged = true;
        //};
    }
}
