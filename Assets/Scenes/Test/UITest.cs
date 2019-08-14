using huqiang.Data;
using huqiang.UI;
using huqiang.UIComposite;
using System.Collections.Generic;
using UnityEngine;

public class UITest : TestHelper
{
    public override void LoadTestPage()
    {
        //Application.targetFrameRate = 60;
#if UNITY_IPHONE || UNITY_ANDROID
        //Scale.DpiScale = true;
#endif
        //UIPage.LoadPage<DrawPage>();
        //UIPage.LoadPage<LayoutTestPage>();
        //UIPage.LoadPage<TestPage>();
        //UIPage.LoadPage<TabControlTest>();
        UIPage.LoadPage<ScrollExTestPage>();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}