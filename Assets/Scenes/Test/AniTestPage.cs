using huqiang.Data;
using huqiang.Manager2D;
using huqiang.UI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System;
using UnityEngine;
using huqiang;

public class AniTestPage:UIPage
{
    class View
    {
        public RenderImageElement render;
        public UIRocker rocker;
        public EventCallBack Last;
        public EventCallBack Next;
    }
    View view;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "anitest");
        base.Initial(parent, dat);
        view = model.ComponentReflection<View>();
        view.rocker.Rocking = Rocking;
        view.rocker.Radius = 100;
        view.render.LoadAsync<RolePage>(null);
        view.Last.Click = (o, e) => { LoadPage<ScrollExTestPage>(); };
        view.Next.Click = (o, e) => { LoadPage<TestPage>(); };
    }
    void Rocking(UIRocker rocker)
    {
        view.render.InvokePage<RolePage, UIRocker.Direction>(Rock, rocker.direction);
    }
    void Rock(RolePage role,UIRocker.Direction direction)
    {
        role.Rocking(direction);
    }
    public override void Dispose()
    {
        view.render.Scene.InvokeDispose();
        base.Dispose();
    }
}
public class RolePage : ScenePage
{
    GameObject go;
    Animator animator;
    public override void Initial(Transform trans, object dat)
    {
        var ins = ElementAsset.LoadAssets<GameObject>("base.unity3d","Sample");
        if(ins!=null)
        {
            go = GameObject.Instantiate<GameObject>(ins);
            go.transform.SetParent(trans);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            animator = go.GetComponent<Animator>();
        }
    }
    public void Rocking(UIRocker.Direction direction)
    {
        if (animator == null)
            return;
        switch(direction)
        {
            case UIRocker.Direction.Up:
                animator.Play("walk");
                break;
            case UIRocker.Direction.Down:
                animator.Play("idle");
                break;
            case UIRocker.Direction.Left:
                go.transform.localEulerAngles = Vector3.zero;
                break;
            case UIRocker.Direction.Right:
                go.transform.localEulerAngles = new Vector3(0,180,0);
                break;
        }
    }
}

