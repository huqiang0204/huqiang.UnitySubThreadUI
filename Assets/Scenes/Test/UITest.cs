using huqiang.Data;
using huqiang.UI;
using huqiang.UIComposite;
using System.Collections.Generic;
using UnityEngine;

public class UITest : TestHelper
{
    public Transform Sphere;
    public override void LoadBundle()
    {
#if UNITY_EDITOR
        base.LoadBundle();
#else
        var dic = Application.streamingAssetsPath;
        ElementAsset.bundles.Add(AssetBundle.LoadFromFile(dic+ "/picture.unity3d"));
#endif
    }
    public override void LoadTestPage()
    {
        Application.targetFrameRate = 60;
#if UNITY_IPHONE || UNITY_ANDROID
        Scale.DpiScale = true;
#endif
        //UIPage.LoadPage<DrawPage>();
        UIPage.LoadPage<LayoutTestPage>();
        //UIPage.LoadPage<TestPage>();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}