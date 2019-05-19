﻿using huqiang.UI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : TestHelper
{
    class View
    {
        public UIRocker Rocker;
    }
    public Transform Sphere;
    Vector3 v;
    public override void LoadTestPage()
    {
        var model = ModelManagerUI.FindModel("baseUI", "asd");
        model.SetParent(UIPage.Root);
        var view = model.ComponentReflection<View>();
        view.Rocker.Radius = 100;
        view.Rocker.Rocking = (o) => {
            v.x = o.vector.x*0.0005f;
            v.z = o.vector.y*0.0005f;
        };
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if(Sphere!=null)
        {
            Sphere.localPosition += v;
        }
    }
}