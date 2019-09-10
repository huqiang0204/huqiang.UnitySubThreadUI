using huqiang;
using huqiang.Data;
using huqiang.UI;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShowGifPage:UIPage
{
    class View
    {
        public RawImageElement txt;
        public EventCallBack Next;
        public EventCallBack Last;
    }
    View view;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "gif");
        base.Initial(parent, dat);
        view = model.ComponentReflection<View>();
        ThreadMission.InvokeToMain(LoadGif,null);
        view.Last.Click = (o, e) => { LoadPage<AniTestPage>(); };
        view.Next.Click = (o, e) => { LoadPage<TestPage>(); };
    }
    void LoadGif(object obj)
    {
        var ta = ElementAsset.LoadAssets<TextAsset>("base.unity3d","gif");
        if(ta!=null)
        {
            GifDecoder.AsyncDecode(ta.bytes,"gif",DecodOver);
        }
    }
    void DecodOver(GifDecoder.Mission obj)
    {
        List<Texture2D> t2d = obj.texture2Ds;
        if (CurrentPage == this)
            view.txt.Play(t2d, true);
    }
}