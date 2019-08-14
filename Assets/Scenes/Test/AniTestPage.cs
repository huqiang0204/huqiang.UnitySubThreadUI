﻿using huqiang.Data;
using huqiang.Manager2D;
using huqiang.UI;
using huqiang.UIComposite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AniTestPage:UIPage
{
    class View
    {
        public RenderImageElement render;
        public UIRocker rocker;
    }
    View view;
    public override void Initial(ModelElement parent, object dat = null)
    {
        model = ModelManagerUI.CloneModel("baseUI", "anitest");
        base.Initial(parent, dat);
        view = model.ComponentReflection<View>();
        view.rocker.Rocking = Rocking;
        view.render.LoadAsync<RolePage>(null);
    }
    void Rocking(UIRocker rocker)
    {
        view.render.InvokePage<RolePage, UIRocker.Direction>(Rock, rocker.direction);
    }
    void Rock(RolePage role,UIRocker.Direction direction)
    {
        role.Rocking(direction);
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
        }
    }
}

