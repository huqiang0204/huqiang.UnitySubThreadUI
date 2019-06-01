using huqiang.UI;
using huqiang.UIComposite;
using System.Collections.Generic;
using UnityEngine;

public class UITest : TestHelper
{
    public Transform Sphere;
    public override void LoadTestPage()
    {
        Application.targetFrameRate = 60;
#if UNITY_IPHONE || UNITY_ANDROID
        Scale.DpiScale = true;
#endif
        UIPage.LoadPage<TestPage>();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}