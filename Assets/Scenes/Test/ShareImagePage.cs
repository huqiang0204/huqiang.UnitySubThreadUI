using huqiang.UI;
using huqiang.UIEvent;
using System;
using UnityEngine;

public class ShareImagePage : UIPage
{
    class View
    {
        public EventCallBack head;
        public EventCallBack body;
        public EventCallBack weapon;
        public EventCallBack Last;
        public EventCallBack Next;
    }
    View view;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "shareimg");
        base.Initial(parent, dat);
        view = model.ComponentReflection<View>();
        view.head.Click = (o, e) => { Debug.Log("head click"); };
        view.body.Click = (o, e) => { Debug.Log("body click"); };
        view.weapon.Click = (o, e) => { Debug.Log("weapon click"); };
        view.Last.Click = (o, e) => { LoadPage<ChatBoxPage>(); };
        view.Next.Click = (o, e) => { LoadPage<AniTestPage>(); };
    }
}